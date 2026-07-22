using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EyeYul.Aplicacion.Abstracciones;
using EyeYul.Aplicacion.Configuracion;
using EyeYul.Aplicacion.Descansos;
using EyeYul.Aplicacion.Estadisticas;
using EyeYul.Dominio.Entidades;
using EyeYul.Presentacion.Temas;
using EyeYul.Presentacion.Vistas;

namespace EyeYul.Presentacion.ModelosVista;

public sealed partial class VistaGeneralModelo : ObservableObject, IDisposable
{
    private readonly ProgramadorDescansos _scheduler;

    private readonly IAlmacenAjustes _settings;

    private readonly ServicioEstadisticas _stats;

    private readonly IReloj _clock;

    private readonly ServicioDescansoProgramado _plannedBreaks;

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
    private string _simContext = "";

    [ObservableProperty]
    private string _themeMode = "light";

    [ObservableProperty]
    private string _streakText = "4 Días";

    [ObservableProperty]
    private string _completedText = "0";

    [ObservableProperty]
    private int _screenScore = 100;

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

    [ObservableProperty]
    private string _newMessageText = "";

    public ObservableCollection<PlannedBreakItem> PlannedBreaks { get; } = [];

    public ObservableCollection<string> CustomMessages { get; } = [];

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
        ServicioDescansoProgramado plannedBreaks)
    {
        _scheduler = scheduler;
        _settings = settings;
        _stats = stats;
        _clock = clock;
        _plannedBreaks = plannedBreaks;

        ThemeMode = settings.Current.Appearance.Theme;
        TemaAplicador.Aplicar(ThemeMode);
        UpdateHeader();

        _scheduler.Tic += OnTick;

        _ = RefreshStatsAsync();
        _ = RefreshPlannedBreaksAsync();

        foreach (string message in settings.Current.Appearance.CustomMessages)
        {
            CustomMessages.Add(message);
        }
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
        !string.IsNullOrEmpty(SimContext) ? "POSPUESTO"
        : IsPaused ? "PAUSADO"
        : "TRABAJANDO";

    private void UpdateHeader()
    {
        (HeaderTitle, HeaderSubtitle) = ActiveTab switch
        {
            "dashboard" => ("Vista General (S.O. Cockpit)", "CONTROL EN VIVO DEL TEMPORIZADOR OCULAR"),
            "routine" => ("Rutina de Descansos Programados", "FRECUENCIA DE DESCANSOS Y HORAS DE OFICINA"),
            "wellness" => ("Postura Erguida y Salud Ocular", "CONFIGURACIÓN DE BIOMONITOREO DE WEBCAM"),
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
        DaysText = FormatearDias(pb.Dias),
        IconKey = ElegirIconoSegunNombre(pb.Nombre)
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
    private async Task AddPlannedBreak()
    {
        if (string.IsNullOrWhiteSpace(NewBreakName) || !TimeOnly.TryParse(NewBreakTime, out TimeOnly hora))
        {
            VentanaDialogoAlerta.Mostrar(
                "Datos incompletos",
                "Escribe un nombre y una hora válida (HH:mm) para la pausa.",
                "IcoShieldAlert");
            return;
        }

        var pb = new DescansoProgramado
        {
            Nombre = NewBreakName.Trim(),
            HoraDelDia = hora,
            Duracion = TimeSpan.FromMinutes(Math.Max(1, NewBreakDurationMinutes))
        };

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

        foreach ((DayOfWeek dia, bool activo) in seleccion)
        {
            if (activo)
            {
                pb.Dias.Add(dia);
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
        string text = NewMessageText.Trim();
        if (string.IsNullOrEmpty(text))
        {
            return;
        }

        _settings.Current.Appearance.CustomMessages.Add(text);
        CustomMessages.Add(text);
        _ = _settings.SaveAsync(_settings.Current);
        NewMessageText = "";
    }

    [RelayCommand]
    private void RemoveMessage(string message)
    {
        _settings.Current.Appearance.CustomMessages.Remove(message);
        CustomMessages.Remove(message);
        _ = _settings.SaveAsync(_settings.Current);
    }

    [RelayCommand]
    private void OpenSettings() => OpenSettingsAction?.Invoke();

    [RelayCommand]
    private void TestAlert() => VentanaDialogoAlerta.Mostrar(
        "Alerta de prueba",
        "Así se verá el recordatorio de salud (postura/parpadeo) cuando se active.");

    /// <summary>
    /// Simula un contexto (juego, reunion, video). Mientras hay contexto activo
    /// el temporizador queda en pausa, y se reanuda al desactivarlo.
    /// </summary>
    [RelayCommand]
    private void ToggleSim(string id)
    {
        SimContext = SimContext == id ? "" : id;

        bool debePausar = !string.IsNullOrEmpty(SimContext);
        if (debePausar != _scheduler.EstaEnPausa)
        {
            _scheduler.AlternarPausa();
        }

        IsPaused = _scheduler.EstaEnPausa;
        PauseButtonText = IsPaused ? "Continuar" : "Pausar";
        StatusText = EstadoActual();
    }

    public async Task RefreshStatsAsync()
    {
        ResumenDiario summary = await _stats.ObtenerResumenDiarioAsync(_clock.Today);
        ScreenScore = summary.Puntaje;
        CompletedText = summary.DescansosTomados.ToString();
    }

    public void Dispose()
    {
        _scheduler.Tic -= OnTick;
    }
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
