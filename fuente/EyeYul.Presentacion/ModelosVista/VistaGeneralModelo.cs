using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EyeYul.Aplicacion.Abstracciones;
using EyeYul.Aplicacion.Configuracion;
using EyeYul.Aplicacion.Descansos;
using EyeYul.Aplicacion.Estadisticas;
using EyeYul.Aplicacion.Licencias;
using EyeYul.Dominio.Entidades;
using EyeYul.Dominio.Enumeraciones;
using EyeYul.Presentacion.Temas;
using EyeYul.Presentacion.Vistas;
using Microsoft.Extensions.DependencyInjection;

namespace EyeYul.Presentacion.ModelosVista;

public sealed partial class VistaGeneralModelo : ObservableObject, IDisposable
{
    private readonly ProgramadorDescansos _scheduler;

    private readonly IAlmacenAjustes _settings;

    private readonly ServicioEstadisticas _stats;

    private readonly IReloj _clock;

    private readonly ServicioDescansoProgramado _plannedBreaks;

    private readonly ServicioLicencia _licencia;

    private static readonly (DayOfWeek Dia, string Etiqueta)[] DiasDeLaSemana =
    [
        (DayOfWeek.Monday, "L"),
        (DayOfWeek.Tuesday, "M"),
        (DayOfWeek.Wednesday, "M"),
        (DayOfWeek.Thursday, "J"),
        (DayOfWeek.Friday, "V"),
        (DayOfWeek.Saturday, "S"),
        (DayOfWeek.Sunday, "D")
    ];

    [ObservableProperty]
    private string _activeTab = "dashboard";

    [ObservableProperty]
    private string _activeSubTab = "timer";

    [ObservableProperty]
    private string _headerTitle = "";

    [ObservableProperty]
    private string _headerSubtitle = "";

    [ObservableProperty]
    private string _timerText = "20:00";

    [ObservableProperty]
    private double _percent = 100.0;

    [ObservableProperty]
    private string _statusText = "TRABAJANDO";

    [ObservableProperty]
    private bool _isPaused;

    [ObservableProperty]
    private string _pauseButtonText = "Pausar";

    [ObservableProperty]
    private string _themeMode = "light";

    [ObservableProperty]
    private string _licenciaEtiqueta = "";

    [ObservableProperty]
    private bool _licenciaEsPremium;

    [ObservableProperty]
    private int _posturaMinutos;

    [ObservableProperty]
    private int _parpadeoMinutos;

    [ObservableProperty]
    private int _hidratacionMinutos;

    [ObservableProperty]
    private int _estiramientoMinutos;

    [ObservableProperty]
    private string _streakText = "4 Días";

    [ObservableProperty]
    private string _completedText = "0";

    [ObservableProperty]
    private int _screenScore = 100;

    [ObservableProperty]
    private DateTime? _historialFecha = DateTime.Today;

    [ObservableProperty]
    private int _historialPuntaje;

    [ObservableProperty]
    private string _historialTiempoActivo = "0h 0m";

    [ObservableProperty]
    private int _historialDescansosTomados;

    [ObservableProperty]
    private string _historialRachaDias = "0 días seguidos";

    public ObservableCollection<BarraSemanal> WeeklyBars { get; } = [];

    [ObservableProperty]
    private string _newBreakName = "";

    [ObservableProperty]
    private string _newBreakTime = "13:00";

    [ObservableProperty]
    private int _newBreakDurationMinutes = 15;

    [ObservableProperty]
    private bool _newBreakMon = true;

    [ObservableProperty]
    private bool _newBreakTue = true;

    [ObservableProperty]
    private bool _newBreakWed = true;

    [ObservableProperty]
    private bool _newBreakThu = true;

    [ObservableProperty]
    private bool _newBreakFri = true;

    [ObservableProperty]
    private bool _newBreakSat;

    [ObservableProperty]
    private bool _newBreakSun;

    /// <summary>"daily" | "weekly" | "monthly" | "once".</summary>
    [ObservableProperty]
    private string _newBreakRecurrencia = "weekly";

    [ObservableProperty]
    private DateTime? _newBreakDate = DateTime.Today;

    [ObservableProperty]
    private string _newBreakDayOfMonth = "1";

    [ObservableProperty]
    private string _newMessageText = "";

    [ObservableProperty]
    private string _newMessageIcon = "IcoHeart";

    [ObservableProperty]
    private Guid? _editingFraseId;

    public bool EstaEditandoFrase => EditingFraseId is not null;

    partial void OnEditingFraseIdChanged(Guid? value) => OnPropertyChanged(nameof(EstaEditandoFrase));

    public static IReadOnlyList<string> IntervalosDisponibles { get; } =
        new[] { 5, 10, 15, 20, 25, 30, 45, 60, 90, 120 }.Select(m => m.ToString()).ToList();

    /// <summary>Espejo en texto del intervalo entre pausas, editable desde la tarjeta "Monitor Inteligente".</summary>
    public string IntervaloMinutosText
    {
        get => ((int)_settings.Current.Descansos.Interval.TotalMinutes).ToString();
        set
        {
            if (!int.TryParse(value, out int minutos) || minutos <= 0)
            {
                return;
            }

            _settings.Current.Descansos.Interval = TimeSpan.FromMinutes(Math.Clamp(minutos, 1, 180));
            _ = _settings.SaveAsync(_settings.Current);
            Reset();
            OnPropertyChanged(nameof(ActiveRoutineMode));
        }
    }

    public static IReadOnlyList<string> HorasDisponibles { get; } = ConstruirHorasDisponibles();

    public static IReadOnlyList<string> DiasDelMesDisponibles { get; } =
        Enumerable.Range(1, 31).Select(d => d.ToString()).ToList();

    private static IReadOnlyList<string> ConstruirHorasDisponibles()
    {
        var horas = new List<string>();
        for (TimeOnly t = new(0, 0); ; t = t.AddMinutes(15))
        {
            horas.Add(t.ToString("HH:mm"));
            if (t.Hour == 23 && t.Minute == 45)
            {
                break;
            }
        }

        return horas;
    }

    public ObservableCollection<PlannedBreakItem> PlannedBreaks { get; } = [];

    public ObservableCollection<FraseItem> Frases { get; } = [];

    public static IReadOnlyList<string> IconosDisponibles { get; } =
    [
        "IcoHeart", "IcoEye", "IcoDroplet", "IcoActivity", "IcoChevronsUp", "IcoUserCheck",
        "IcoMoon", "IcoSun", "IcoZap", "IcoCoffee", "IcoRotate", "IcoKeyboard", "IcoSparkles", "IcoAward"
    ];

    /// <summary>Se apaga cuando la ventana no esta visible para no gastar ciclos de UI.</summary>
    public bool ActualizacionesEnVivo { get; set; } = true;

    public Action? OpenSettingsAction { get; set; }

    public string RoutineSelection => ActiveTab == "routine" ? ActiveSubTab : "";

    public string ActiveRoutineMode => _settings.Current.Descansos.Interval.TotalMinutes switch
    {
        20.0 => "balanced",
        45.0 => "deep",
        15.0 => "eye",
        30.0 => "wellness",
        _ => "custom"
    };

    public string ActiveRestrictionMode
    {
        get
        {
            EnforcementSettings reglas = _settings.Current.AplicacionReglas;
            bool allowSkip = _settings.Current.Descansos.AllowSkip;

            if (!allowSkip && reglas.StrictMode)
            {
                return "strict";
            }

            if (allowSkip && !reglas.StrictMode
                && reglas.MaxSnoozesPerDay > 0 && reglas.MaxSnoozesPerDay <= 10)
            {
                return "moderate";
            }

            if (allowSkip && !reglas.StrictMode && reglas.MaxSnoozesPerDay == 0)
            {
                return "free";
            }

            return "custom";
        }
    }

    public VistaGeneralModelo(
        ProgramadorDescansos scheduler,
        IAlmacenAjustes settings,
        ServicioEstadisticas stats,
        IReloj clock,
        ServicioDescansoProgramado plannedBreaks,
        ServicioLicencia licencia)
    {
        _scheduler = scheduler;
        _settings = settings;
        _stats = stats;
        _clock = clock;
        _plannedBreaks = plannedBreaks;
        _licencia = licencia;

        ThemeMode = settings.Current.Appearance.Theme;
        TemaAplicador.Aplicar(ThemeMode);
        UpdateHeader();
        ActualizarLicencia();

        AccionesSaludablesSettings acciones = settings.Current.AccionesSaludables;
        PosturaMinutos = (int)acciones.PosturaIntervalo.TotalMinutes;
        ParpadeoMinutos = (int)acciones.ParpadeoIntervalo.TotalMinutes;
        HidratacionMinutos = (int)acciones.HidratacionIntervalo.TotalMinutes;
        EstiramientoMinutos = (int)acciones.EstiramientoIntervalo.TotalMinutes;

        _scheduler.Tic += OnTick;

        _ = RefreshStatsAsync();
        _ = RefreshPlannedBreaksAsync();

        RefreshFrases();
    }

    private void ActualizarLicencia()
    {
        LicenciaEsPremium = _licencia.EsPremiumActivo;
        LicenciaEtiqueta = _licencia.EsPremiumActivo
            ? "PREMIUM"
            : _licencia.DiasRestantesTrial > 0
                ? $"PRUEBA · {_licencia.DiasRestantesTrial}D"
                : "PRUEBA VENCIDA";
    }

    [RelayCommand]
    private void AbrirLicencia()
    {
        VentanaActivarLicencia.Mostrar(_licencia);
        ActualizarLicencia();
    }

    private void OnTick(object? sender, TimeSpan remaining)
    {
        if (!ActualizacionesEnVivo)
        {
            return;
        }

        Application.Current?.Dispatcher.BeginInvoke(DispatcherPriority.Background, () =>
        {
            TimeSpan interval = _settings.Current.Descansos.Interval;

            TimerText = $"{(int)remaining.TotalMinutes:00}:{remaining.Seconds:00}";
            Percent = interval.TotalSeconds > 0
                ? Math.Clamp(remaining.TotalSeconds / interval.TotalSeconds * 100.0, 0.0, 100.0)
                : 0.0;

            IsPaused = _scheduler.EstaEnPausa;
            PauseButtonText = IsPaused ? "Continuar" : "Pausar";
            StatusText = EstadoActual();
        });
    }

    private string EstadoActual() =>
        IsPaused ? "PAUSADO"
        : "TRABAJANDO";

    private void UpdateHeader()
    {
        (HeaderTitle, HeaderSubtitle) = ActiveTab switch
        {
            "dashboard" => ("Vista General (S.O. Cockpit)", "CONTROL EN VIVO DEL TEMPORIZADOR OCULAR"),
            "routine" => ("Rutina de Descansos Programados", "FRECUENCIA DE DESCANSOS Y HORAS DE OFICINA"),
            "wellness" => ("Acciones Saludables", "RECORDATORIOS CORTOS DE POSTURA, PARPADEO, HIDRATACIÓN Y ESTIRAMIENTO"),
            "automations" => ("Smart Pause (Reglas Automatizadas)", "REGLAS INTELIGENTES Y AUTOMATIZACIÓN"),
            "stats" => ("Historial de Productividad y Logros", "SCREEN SCORE Y RACHAS DIARIAS"),
            _ => ("EyeYul", "")
        };
    }

    partial void OnActiveTabChanged(string value)
    {
        UpdateHeader();
        OnPropertyChanged(nameof(RoutineSelection));
    }

    partial void OnHistorialFechaChanged(DateTime? value) => _ = RefreshHistorialAsync();

    partial void OnActiveSubTabChanged(string value)
    {
        OnPropertyChanged(nameof(RoutineSelection));
    }

    [RelayCommand]
    private void SelectTab(string tab) => ActiveTab = tab;

    [RelayCommand]
    private void SelectTheme(string theme)
    {
        TemaAplicador.Aplicar(theme);
        ThemeMode = theme;
        _settings.Current.Appearance.Theme = theme;
        _ = _settings.SaveAsync(_settings.Current);
    }

    [RelayCommand]
    private void SelectSubTab(string sub)
    {
        ActiveTab = "routine";
        ActiveSubTab = sub;

        if (sub == "planned")
        {
            _ = RefreshPlannedBreaksAsync();
        }
    }

    [RelayCommand]
    private void TogglePause()
    {
        _scheduler.AlternarPausa();
        IsPaused = _scheduler.EstaEnPausa;
        PauseButtonText = IsPaused ? "Continuar" : "Pausar";
        StatusText = IsPaused ? "PAUSADO" : "TRABAJANDO";
    }

    [RelayCommand]
    private void Reset() => _scheduler.ReiniciarCuentaRegresiva();

    [RelayCommand]
    private void StartBreak() => _scheduler.SolicitarPausaInmediata();

    [RelayCommand]
    private void SetMode(string mode)
    {
        BreakSettings descansos = _settings.Current.Descansos;

        switch (mode)
        {
            case "balanced":
                descansos.Interval = TimeSpan.FromMinutes(20);
                descansos.Duration = TimeSpan.FromSeconds(30);
                break;

            case "deep":
                descansos.Interval = TimeSpan.FromMinutes(45);
                descansos.Duration = TimeSpan.FromSeconds(60);
                break;

            case "eye":
                descansos.Interval = TimeSpan.FromMinutes(15);
                descansos.Duration = TimeSpan.FromSeconds(20);
                break;

            case "wellness":
                descansos.Interval = TimeSpan.FromMinutes(30);
                descansos.Duration = TimeSpan.FromSeconds(45);
                break;
        }

        _ = _settings.SaveAsync(_settings.Current);
        Reset();
        OnPropertyChanged(nameof(ActiveRoutineMode));
        OnPropertyChanged(nameof(IntervaloMinutosText));

        VentanaDialogoAlerta.Mostrar(
            "Modo aplicado",
            "¡Listo! Los nuevos intervalos ya están activos.",
            "IcoShieldCheck");
    }

    [RelayCommand]
    private void SetRestrictionMode(string mode)
    {
        AjustesEyeYul s = _settings.Current;

        switch (mode)
        {
            case "free":
                s.Descansos.AllowSkip = true;
                s.AplicacionReglas.MaxSnoozesPerDay = 0;
                s.AplicacionReglas.MaxSnoozesPerBreak = 0;
                s.AplicacionReglas.StrictMode = false;
                break;

            case "moderate":
                s.Descansos.AllowSkip = true;
                s.AplicacionReglas.MaxSnoozesPerDay = 10;
                s.AplicacionReglas.MaxSnoozesPerBreak = 1;
                s.AplicacionReglas.StrictMode = false;
                break;

            case "strict":
                s.Descansos.AllowSkip = false;
                s.AplicacionReglas.MaxSnoozesPerDay = 1;
                s.AplicacionReglas.MaxSnoozesPerBreak = 1;
                s.AplicacionReglas.StrictMode = true;
                break;
        }

        _ = _settings.SaveAsync(s);
        OnPropertyChanged(nameof(ActiveRestrictionMode));

        VentanaDialogoAlerta.Mostrar(
            "Límites actualizados",
            "El grado de restricción se aplicó correctamente.",
            "IcoShield");
    }

    public async Task RefreshPlannedBreaksAsync()
    {
        IReadOnlyList<DescansoProgramado> all = await _plannedBreaks.ObtenerTodosAsync();

        PlannedBreaks.Clear();
        foreach (DescansoProgramado pb in all.OrderBy(p => p.HoraDelDia))
        {
            PlannedBreaks.Add(ToItem(pb));
        }
    }

    private static PlannedBreakItem ToItem(DescansoProgramado pb) => new()
    {
        Id = pb.Id,
        Name = pb.Nombre,
        TimeText = pb.HoraDelDia.ToString("HH:mm"),
        DurationText = $"Duración: {(int)pb.Duracion.TotalMinutes} mins",
        DaysText = FormatearRecurrencia(pb),
        IconKey = ElegirIconoSegunNombre(pb.Nombre)
    };

    private static string FormatearRecurrencia(DescansoProgramado pb) => pb.Recurrencia switch
    {
        TipoRecurrencia.Diario => "Todos los días",
        TipoRecurrencia.Mensual => $"Día {pb.DiaDelMes} de cada mes",
        TipoRecurrencia.UnaVez => pb.FechaUnica is { } f ? $"Solo el {f:dd/MM/yyyy}" : "Sin fecha asignada",
        _ => FormatearDias(pb.Dias)
    };

    private static string FormatearDias(IReadOnlySet<DayOfWeek> diasActivos)
    {
        List<string> etiquetas = [];

        foreach ((DayOfWeek dia, string etiqueta) in DiasDeLaSemana)
        {
            if (diasActivos.Contains(dia))
            {
                etiquetas.Add(etiqueta);
            }
        }

        return etiquetas.Count == 0
            ? "Sin días asignados"
            : "Días: " + string.Join(", ", etiquetas);
    }

    private static string ElegirIconoSegunNombre(string nombre)
    {
        string n = nombre.ToLowerInvariant();

        if (n.Contains("café") || n.Contains("cafe"))
        {
            return "IcoCoffee";
        }

        if (n.Contains("almuerzo") || n.Contains("comida"))
        {
            return "IcoUtensils";
        }

        return "IcoCalendar";
    }

    [RelayCommand]
    private void SetNewBreakRecurrencia(string valor) => NewBreakRecurrencia = valor;

    [RelayCommand]
    private async Task AddPlannedBreak()
    {
        if (string.IsNullOrWhiteSpace(NewBreakName) || !TimeOnly.TryParse(NewBreakTime, out TimeOnly hora))
        {
            VentanaDialogoAlerta.Mostrar(
                "Datos incompletos",
                "Escribe un nombre y una hora válida para la pausa.",
                "IcoShieldAlert");
            return;
        }

        TipoRecurrencia recurrencia = NewBreakRecurrencia switch
        {
            "daily" => TipoRecurrencia.Diario,
            "monthly" => TipoRecurrencia.Mensual,
            "once" => TipoRecurrencia.UnaVez,
            _ => TipoRecurrencia.Semanal
        };

        if (recurrencia == TipoRecurrencia.UnaVez && NewBreakDate is null)
        {
            VentanaDialogoAlerta.Mostrar("Datos incompletos", "Elige una fecha para la pausa.", "IcoShieldAlert");
            return;
        }

        var pb = new DescansoProgramado
        {
            Nombre = NewBreakName.Trim(),
            HoraDelDia = hora,
            Recurrencia = recurrencia,
            DiaDelMes = int.TryParse(NewBreakDayOfMonth, out int dia) ? Math.Clamp(dia, 1, 31) : 1,
            FechaUnica = NewBreakDate is { } f ? DateOnly.FromDateTime(f) : null,
            Duracion = TimeSpan.FromMinutes(Math.Max(1, NewBreakDurationMinutes))
        };

        if (recurrencia == TipoRecurrencia.Semanal)
        {
            (DayOfWeek Dia, bool Activo)[] seleccion =
            [
                (DayOfWeek.Monday, NewBreakMon),
                (DayOfWeek.Tuesday, NewBreakTue),
                (DayOfWeek.Wednesday, NewBreakWed),
                (DayOfWeek.Thursday, NewBreakThu),
                (DayOfWeek.Friday, NewBreakFri),
                (DayOfWeek.Saturday, NewBreakSat),
                (DayOfWeek.Sunday, NewBreakSun)
            ];

            foreach ((DayOfWeek diaSemana, bool activo) in seleccion)
            {
                if (activo)
                {
                    pb.Dias.Add(diaSemana);
                }
            }

            if (pb.Dias.Count == 0)
            {
                VentanaDialogoAlerta.Mostrar("Datos incompletos", "Elige al menos un día de la semana.", "IcoShieldAlert");
                return;
            }
        }

        await _plannedBreaks.GuardarAsync(pb);
        NewBreakName = "";
        await RefreshPlannedBreaksAsync();
    }

    [RelayCommand]
    private async Task RemovePlannedBreak(Guid id)
    {
        await _plannedBreaks.EliminarAsync(id);
        await RefreshPlannedBreaksAsync();
    }

    [RelayCommand]
    private void AddMessage()
    {
        string texto = NewMessageText.Trim();
        if (string.IsNullOrEmpty(texto))
        {
            return;
        }

        List<FraseBienestar> frases = _settings.Current.Appearance.Frases;

        if (EditingFraseId is { } id)
        {
            FraseBienestar? existente = frases.FirstOrDefault(f => f.Id == id && !f.EsPredefinida);
            if (existente is not null)
            {
                existente.Texto = texto;
                existente.IconKey = NewMessageIcon;
            }

            EditingFraseId = null;
        }
        else
        {
            frases.Add(new FraseBienestar
            {
                Texto = texto,
                IconKey = NewMessageIcon,
                Habilitada = true,
                EsPredefinida = false
            });
        }

        _ = _settings.SaveAsync(_settings.Current);
        NewMessageText = "";
        NewMessageIcon = "IcoHeart";
        RefreshFrases();
    }

    [RelayCommand]
    private void CancelEditMessage()
    {
        EditingFraseId = null;
        NewMessageText = "";
        NewMessageIcon = "IcoHeart";
    }

    [RelayCommand]
    private void EditMessage(Guid id)
    {
        FraseBienestar? frase = _settings.Current.Appearance.Frases.FirstOrDefault(f => f.Id == id);
        if (frase is null || frase.EsPredefinida)
        {
            return;
        }

        EditingFraseId = id;
        NewMessageText = frase.Texto;
        NewMessageIcon = frase.IconKey;
    }

    [RelayCommand]
    private void RemoveMessage(Guid id)
    {
        List<FraseBienestar> frases = _settings.Current.Appearance.Frases;
        FraseBienestar? frase = frases.FirstOrDefault(f => f.Id == id);
        if (frase is null || frase.EsPredefinida)
        {
            return;
        }

        frases.Remove(frase);
        _ = _settings.SaveAsync(_settings.Current);

        if (EditingFraseId == id)
        {
            EditingFraseId = null;
            NewMessageText = "";
            NewMessageIcon = "IcoHeart";
        }

        RefreshFrases();
    }

    private void RefreshFrases()
    {
        Frases.Clear();
        foreach (FraseBienestar f in _settings.Current.Appearance.Frases)
        {
            Frases.Add(new FraseItem
            {
                Id = f.Id,
                Texto = f.Texto,
                IconKey = f.IconKey,
                EsPredefinida = f.EsPredefinida,
                Habilitada = f.Habilitada,
                AlCambiarHabilitada = OnFraseHabilitadaCambiada
            });
        }
    }

    private void OnFraseHabilitadaCambiada(FraseItem item)
    {
        FraseBienestar? frase = _settings.Current.Appearance.Frases.FirstOrDefault(f => f.Id == item.Id);
        if (frase is null)
        {
            return;
        }

        frase.Habilitada = item.Habilitada;
        _ = _settings.SaveAsync(_settings.Current);
    }

    [RelayCommand]
    private void OpenSettings() => OpenSettingsAction?.Invoke();

    /// <summary>"postura:20" -&gt; guarda 20 minutos de intervalo para la accion de postura. "0" es Off.</summary>
    [RelayCommand]
    private void SetAccionMinutos(string clave)
    {
        string[] partes = clave.Split(':');
        if (partes.Length != 2 || !int.TryParse(partes[1], out int minutos))
        {
            return;
        }

        TimeSpan intervalo = TimeSpan.FromMinutes(minutos);
        AccionesSaludablesSettings acciones = _settings.Current.AccionesSaludables;

        switch (partes[0])
        {
            case "postura":
                acciones.PosturaIntervalo = intervalo;
                PosturaMinutos = minutos;
                break;

            case "parpadeo":
                acciones.ParpadeoIntervalo = intervalo;
                ParpadeoMinutos = minutos;
                break;

            case "hidratacion":
                acciones.HidratacionIntervalo = intervalo;
                HidratacionMinutos = minutos;
                break;

            case "estiramiento":
                acciones.EstiramientoIntervalo = intervalo;
                EstiramientoMinutos = minutos;
                break;
        }

        _ = _settings.SaveAsync(_settings.Current);
    }

    [RelayCommand]
    private async Task ProbarAccionAsync(string accion)
    {
        TipoAccionSaludable tipo = accion switch
        {
            "postura" => TipoAccionSaludable.Postura,
            "parpadeo" => TipoAccionSaludable.Parpadeo,
            "hidratacion" => TipoAccionSaludable.Hidratacion,
            "estiramiento" => TipoAccionSaludable.Estiramiento,
            _ => TipoAccionSaludable.Postura
        };

        IControladorAccionSaludable controlador = App.Services.GetRequiredService<IControladorAccionSaludable>();
        await controlador.MostrarAsync(tipo);
    }

    public async Task RefreshStatsAsync()
    {
        ResumenDiario summary = await _stats.ObtenerResumenDiarioAsync(_clock.Today);
        ScreenScore = summary.Puntaje;
        CompletedText = summary.DescansosTomados.ToString();

        await RefreshHistorialAsync();
    }

    public async Task RefreshHistorialAsync()
    {
        DateOnly fecha = DateOnly.FromDateTime(HistorialFecha ?? DateTime.Today);

        // Una sola consulta de rango cubre tanto las barras de la semana como la racha:
        // no hace falta una pausa por dia para saber si hoy tocaba pausa (eso ya se resuelve
        // en tiempo real via ProgramadorDescansos); esto es solo el resumen historico.
        DateOnly desde = fecha.AddDays(-59);
        IReadOnlyList<PuntajeVisualDiario> rango = await _stats.ObtenerRangoPuntajeAsync(desde, fecha);
        Dictionary<DateOnly, PuntajeVisualDiario> porFecha = rango.ToDictionary(p => p.Fecha);

        PuntajeVisualDiario DiaOrDefault(DateOnly f) =>
            porFecha.TryGetValue(f, out PuntajeVisualDiario? p) ? p : new PuntajeVisualDiario { Fecha = f };

        PuntajeVisualDiario diaSeleccionado = DiaOrDefault(fecha);
        HistorialPuntaje = diaSeleccionado.Puntaje;
        HistorialTiempoActivo = Formatear(diaSeleccionado.TiempoActivo);
        HistorialDescansosTomados = diaSeleccionado.DescansosTomados;

        int racha = 0;
        for (DateOnly f = fecha; f >= desde; f = f.AddDays(-1))
        {
            if (DiaOrDefault(f).DescansosTomados <= 0)
            {
                break;
            }

            racha++;
        }

        HistorialRachaDias = $"{racha} día{(racha == 1 ? "" : "s")} seguidos";

        WeeklyBars.Clear();
        int maxPuntaje = Math.Max(1, Enumerable.Range(0, 7).Select(i => DiaOrDefault(fecha.AddDays(-6 + i)).Puntaje).Max());
        for (int i = 0; i < 7; i++)
        {
            DateOnly f = fecha.AddDays(-6 + i);
            PuntajeVisualDiario dia = DiaOrDefault(f);
            string etiqueta = DiasDeLaSemana.First(d => d.Dia == f.DayOfWeek).Etiqueta;
            WeeklyBars.Add(new BarraSemanal(etiqueta, dia.Puntaje.ToString(), Math.Max(4.0, dia.Puntaje / (double)maxPuntaje * 100.0)));
        }
    }

    private static string Formatear(TimeSpan t) => $"{(int)t.TotalHours}h {t.Minutes}m";

    public void Dispose()
    {
        _scheduler.Tic -= OnTick;
    }
}

public sealed record BarraSemanal(string Label, string Value, double Height);

/// <summary>Fila de la lista de Frases. Predefinida: solo habilitar/deshabilitar. Del usuario: todo.</summary>
public sealed partial class FraseItem : ObservableObject
{
    public required Guid Id { get; init; }

    public required string Texto { get; init; }

    public required string IconKey { get; init; }

    public required bool EsPredefinida { get; init; }

    public Action<FraseItem>? AlCambiarHabilitada { get; init; }

    [ObservableProperty]
    private bool _habilitada;

    partial void OnHabilitadaChanged(bool value) => AlCambiarHabilitada?.Invoke(this);
}

public sealed class PlannedBreakItem
{
    public required Guid Id { get; init; }

    public required string Name { get; init; }

    public required string TimeText { get; init; }

    public required string DurationText { get; init; }

    public required string DaysText { get; init; }

    public required string IconKey { get; init; }
}
