using EyeYul.Aplicacion.Abstracciones;
using EyeYul.Aplicacion.Actividad;
using EyeYul.Aplicacion.AplicacionReglas;
using EyeYul.Aplicacion.Configuracion;
using EyeYul.Aplicacion.Licencias;
using EyeYul.Aplicacion.Puntuacion;
using EyeYul.Dominio.Entidades;
using EyeYul.Dominio.Enumeraciones;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace EyeYul.Aplicacion.Descansos;

public sealed class ProgramadorDescansos(
    IMonitorActividad monitorActividad,
    MotorPausaInteligente pausaInteligente,
    IControladorPantallaDescanso pantallaDescanso,
    IServicioNotificacion notificaciones,
    ServicioPuntajeVisual puntajeVisual,
    IBreakRepository repositorioDescansos,
    IAlmacenAjustes almacenAjustes,
    PoliticaPausa politicaAplazamiento,
    ServicioDescansoProgramado descansosProgramados,
    IAutomationRunner automatizacion,
    ServicioLicencia licencia,
    IReloj reloj,
    ILogger<ProgramadorDescansos> registro) : BackgroundService
{
    private static readonly TimeSpan Intervalo = TimeSpan.FromSeconds(1);

    private TimeSpan _acumulado;

    private TimeSpan _rachaActual;

    private DateTimeOffset _ultimoTic;

    private volatile bool _forzarPausa;

    public TimeSpan TiempoHastaProximaPausa { get; private set; }

    public bool EstaEnPausa { get; private set; }

    /// <summary>
    /// Motivo por el que se suprimio la ultima pausa, o <c>null</c> si la ultima si se mostro.
    /// Permite explicar al usuario por que el temporizador llego a cero sin pausa.
    /// </summary>
    public DecisionPausa? UltimaSupresion { get; private set; }

    public event EventHandler<TimeSpan>? Tic;

    /// <summary>Se dispara cuando una pausa no se muestra por una regla de Smart Pause.</summary>
    public event EventHandler<DecisionPausa>? PausaSuprimida;

    public void SolicitarPausaInmediata() => _forzarPausa = true;

    public void AlternarPausa() => EstaEnPausa = !EstaEnPausa;

    public void ReiniciarCuentaRegresiva() => _acumulado = TimeSpan.Zero;

    protected override async Task ExecuteAsync(CancellationToken tokenDetencion)
    {
        _ultimoTic = reloj.Now;
        registro.LogInformation("ProgramadorDescansos iniciado.");

        using var temporizador = new PeriodicTimer(Intervalo);
        while (await temporizador.WaitForNextTickAsync(tokenDetencion))
        {
            try
            {
                await TicAsync(tokenDetencion);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                registro.LogError(ex, "Error en el tick del ProgramadorDescansos.");
            }
        }
    }

    private async Task TicAsync(CancellationToken ct)
    {
        AjustesEyeYul ajustes = almacenAjustes.Current;
        DateTimeOffset ahora = reloj.Now;
        TimeSpan transcurrido = ahora - _ultimoTic;
        _ultimoTic = ahora;

        AcumularTiempoActivo(transcurrido, ajustes);

        TimeSpan intervalo = ajustes.Descansos.Interval;
        TiempoHastaProximaPausa = intervalo > _acumulado ? intervalo - _acumulado : TimeSpan.Zero;
        Tic?.Invoke(this, TiempoHastaProximaPausa);

        if (EstaEnPausa && !_forzarPausa)
        {
            return;
        }

        // Pausas planificadas (Agenda): función Premium.
        DescansoProgramado? programado = licencia.PremiumDisponible
            ? await descansosProgramados.ObtenerPendienteAsync(TimeSpan.FromSeconds(30), ct)
            : null;
        bool tocaPorIntervalo = _acumulado >= intervalo;

        if (_forzarPausa || programado is not null || tocaPorIntervalo)
        {
            TipoDescanso tipo = programado is not null ? TipoDescanso.Planificado : TipoDescanso.Intervalo;
            TimeSpan duracion = programado?.Duracion ?? ajustes.Descansos.Duration;
            await DispararPausaAsync(tipo, duracion, ajustes, ct);
        }
    }

    private void AcumularTiempoActivo(TimeSpan transcurrido, AjustesEyeYul ajustes)
    {
        ActivitySnapshot actual = monitorActividad.Current;
        bool bloqueada = actual.State.HasFlag(EstadoActividad.SesionBloqueada);
        bool ausente = actual.State.HasFlag(EstadoActividad.Inactivo)
                       && actual.IdleTime >= ajustes.SmartPause.IdleThreshold;

        if (EstaEnPausa || bloqueada || ausente)
        {
            return;
        }

        _acumulado += transcurrido;
        _rachaActual += transcurrido;
    }

    private async Task DispararPausaAsync(
        TipoDescanso tipo, TimeSpan duracion, AjustesEyeYul ajustes, CancellationToken ct)
    {
        bool forzada = _forzarPausa;
        _forzarPausa = false;

        // Una pausa forzada por el usuario nunca se suprime por "flujo".
        if (!forzada && await FueSuprimidaPorFlujoAsync(tipo, duracion, ajustes, ct))
        {
            return;
        }

        Descanso registroDescanso = NuevoDescanso(tipo, duracion);
        await repositorioDescansos.AddAsync(registroDescanso, ct);

        bool avisoPrevio = ajustes.Descansos.PreBreakWarning > TimeSpan.Zero && !forzada;
        if (avisoPrevio && await ResolverAvisoPrevioAsync(registroDescanso, ajustes, ct))
        {
            return;
        }

        await EjecutarPausaAsync(registroDescanso, duracion, ajustes, ct);
    }

    private async Task<bool> FueSuprimidaPorFlujoAsync(
        TipoDescanso tipo, TimeSpan duracion, AjustesEyeYul ajustes, CancellationToken ct)
    {
        ActivitySnapshot muestra = monitorActividad.Current;
        DecisionPausa decision = pausaInteligente.Evaluar(muestra, ajustes.SmartPause);

        if (decision == DecisionPausa.Permitir)
        {
            return false;
        }

        registro.LogInformation("Pausa {Tipo} suprimida: {Motivo}", tipo, decision);

        // Estar inactivo no gasta la pausa: se reintenta al volver.
        if (decision != DecisionPausa.AplazarPorAusencia)
        {
            Descanso registroSuprimido = NuevoDescanso(tipo, duracion);
            registroSuprimido.MarcarSuprimido();
            await repositorioDescansos.AddAsync(registroSuprimido, ct);

            UltimaSupresion = decision;

            // Reintentar dentro de ~2 min. Se recorta contra el intervalo para que
            // un intervalo corto no deje _acumulado en negativo (lo que retrasaria
            // el reintento muchisimo mas de lo previsto).
            TimeSpan reintento = TimeSpan.FromMinutes(2);
            _acumulado = ajustes.Descansos.Interval > reintento
                ? ajustes.Descansos.Interval - reintento
                : TimeSpan.Zero;

            PausaSuprimida?.Invoke(this, decision);
        }

        return true;
    }

    private async Task<bool> ResolverAvisoPrevioAsync(
        Descanso descanso, AjustesEyeYul ajustes, CancellationToken ct)
    {
        NotificationResponse respuesta = await notificaciones.ShowBreakDueAsync(
            "EyeYul",
            $"Pausa en {(int)ajustes.Descansos.PreBreakWarning.TotalSeconds} s",
            ct);

        if (respuesta is NotificationResponse.Skip && ajustes.Descansos.AllowSkip)
        {
            await OmitirAsync(descanso, ct);
            return true;
        }

        if (respuesta is NotificationResponse.Snooze aplazamiento
            && politicaAplazamiento.PuedeAplazar(reloj.Today, descanso.ConteoAplazamientos, ajustes.AplicacionReglas))
        {
            descanso.MarcarAplazado();
            politicaAplazamiento.RegistrarAplazamiento(reloj.Today);
            await repositorioDescansos.AddAsync(descanso, ct);
            _acumulado = ajustes.Descansos.Interval - aplazamiento.Duration.Valor;
            registro.LogInformation("Pausa pospuesta {Aplazamiento}", aplazamiento.Duration);
            return true;
        }

        return false;
    }

    private async Task EjecutarPausaAsync(
        Descanso descanso, TimeSpan duracion, AjustesEyeYul ajustes, CancellationToken ct)
    {
        bool premium = licencia.PremiumDisponible;

        // Automatizaciones (comando al iniciar/terminar pausa): función Premium.
        if (premium)
        {
            await automatizacion.RunAsync(ajustes.General.OnBreakStartCommand, ct);
        }

        descanso.MarcarIniciado(reloj.Now);
        UltimaSupresion = null;

        var solicitud = new BreakOverlayRequest(
            descanso,
            ElegirMensajeDePausa(ajustes, premium),
            duracion,
            ajustes.Descansos.AllowSkip && !ajustes.AplicacionReglas.StrictMode,
            // Fondo de overlay personalizado: función Premium.
            premium ? ajustes.Appearance.OverlayBackgroundPath : null);

        switch (await pantallaDescanso.ShowAsync(solicitud, ct))
        {
            case BreakOverlayResult.Completed:
                descanso.MarcarCompletado(reloj.Now);
                await puntajeVisual.RegistrarDescansoTomadoAsync(ct);
                _rachaActual = TimeSpan.Zero;
                break;

            case BreakOverlayResult.Skipped:
                await OmitirAsync(descanso, ct);
                break;

            case BreakOverlayResult.Snoozed:
                descanso.MarcarAplazado();
                politicaAplazamiento.RegistrarAplazamiento(reloj.Today);
                break;
        }

        await repositorioDescansos.AddAsync(descanso, ct);

        if (premium)
        {
            await automatizacion.RunAsync(ajustes.General.OnBreakEndCommand, ct);
        }

        await puntajeVisual.ActualizarRachaAsync(_rachaActual, ct);
        _acumulado = TimeSpan.Zero;
    }

    private static string ElegirMensajeDePausa(AjustesEyeYul ajustes, bool premium)
    {
        // Elegir cuáles frases de fábrica usar es gratis; crear frases propias es Premium.
        List<FraseBienestar> disponibles = ajustes.Appearance.Frases
            .Where(f => f.Habilitada && (premium || f.EsPredefinida))
            .ToList();

        return disponibles.Count == 0
            ? ajustes.Appearance.BreakMessage
            : disponibles[Random.Shared.Next(disponibles.Count)].Texto;
    }

    private async Task OmitirAsync(Descanso descanso, CancellationToken ct)
    {
        descanso.MarcarOmitido(reloj.Now);
        await repositorioDescansos.AddAsync(descanso, ct);
        await puntajeVisual.RegistrarDescansoOmitidoAsync(ct);
        _acumulado = TimeSpan.Zero;
    }

    private Descanso NuevoDescanso(TipoDescanso tipo, TimeSpan duracion) => new()
    {
        Tipo = tipo,
        ProgramadoEn = reloj.Now,
        DuracionPlanificada = duracion
    };
}
