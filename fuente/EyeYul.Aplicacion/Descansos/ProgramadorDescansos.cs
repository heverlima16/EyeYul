using EyeYul.Aplicacion.Abstracciones;
using EyeYul.Aplicacion.Actividad;
using EyeYul.Aplicacion.AplicacionReglas;
using EyeYul.Aplicacion.Configuracion;
using EyeYul.Aplicacion.Puntuacion;
using EyeYul.Dominio.Entidades;
using EyeYul.Dominio.Enumeraciones;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace EyeYul.Aplicacion.Descansos;

public sealed class ProgramadorDescansos(
    IMonitorActividad activityMonitor,
    MotorPausaInteligente smartPause,
    IControladorPantallaDescanso overlay,
    IServicioNotificacion notifications,
    ServicioPuntajeVisual screenScore,
    IBreakRepository breakRepository,
    IAlmacenAjustes settingsStore,
    PoliticaPausa snoozePolicy,
    ServicioDescansoProgramado plannedBreaks,
    IAutomationRunner automation,
    IReloj clock,
    ILogger<ProgramadorDescansos> logger) : BackgroundService
{
    private static readonly TimeSpan Tick = TimeSpan.FromSeconds(1);

    private TimeSpan _accumulated;

    private TimeSpan _currentStretch;

    private DateTimeOffset _lastTick;

    private volatile bool _forceBreak;

    public TimeSpan TimeUntilNextBreak { get; private set; }

    public bool IsPaused { get; private set; }

    /// <summary>
    /// Motivo por el que se suprimio la ultima pausa, o <c>null</c> si la ultima si se mostro.
    /// Permite explicar al usuario por que el temporizador llego a cero sin pausa.
    /// </summary>
    public PauseDecision? UltimaSupresion { get; private set; }

    public event EventHandler<TimeSpan>? Ticked;

    /// <summary>Se dispara cuando una pausa no se muestra por una regla de Smart Pause.</summary>
    public event EventHandler<PauseDecision>? PausaSuprimida;

    public void RequestImmediateBreak() => _forceBreak = true;

    public void TogglePause() => IsPaused = !IsPaused;

    public void ResetCountdown() => _accumulated = TimeSpan.Zero;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _lastTick = clock.Now;
        logger.LogInformation("ProgramadorDescansos iniciado.");

        using var timer = new PeriodicTimer(Tick);
        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            try
            {
                await TickAsync(stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error en el tick del ProgramadorDescansos.");
            }
        }
    }

    private async Task TickAsync(CancellationToken ct)
    {
        AjustesEyeYul settings = settingsStore.Current;
        DateTimeOffset now = clock.Now;
        TimeSpan elapsed = now - _lastTick;
        _lastTick = now;

        AcumularTiempoActivo(elapsed, settings);

        TimeSpan interval = settings.Descansos.Interval;
        TimeUntilNextBreak = interval > _accumulated ? interval - _accumulated : TimeSpan.Zero;
        Ticked?.Invoke(this, TimeUntilNextBreak);

        if (IsPaused && !_forceBreak)
        {
            return;
        }

        DescansoProgramado? planned = await plannedBreaks.GetDueAsync(TimeSpan.FromSeconds(30), ct);
        bool intervalDue = _accumulated >= interval;

        if (_forceBreak || planned is not null || intervalDue)
        {
            TipoDescanso type = planned is not null ? TipoDescanso.Planned : TipoDescanso.Interval;
            TimeSpan duration = planned?.Duration ?? settings.Descansos.Duration;
            await TriggerBreakAsync(type, duration, settings, ct);
        }
    }

    private void AcumularTiempoActivo(TimeSpan elapsed, AjustesEyeYul settings)
    {
        ActivitySnapshot current = activityMonitor.Current;
        bool locked = current.State.HasFlag(EstadoActividad.SessionLocked);
        bool idle = current.State.HasFlag(EstadoActividad.Idle)
                    && current.IdleTime >= settings.SmartPause.IdleThreshold;

        if (IsPaused || locked || idle)
        {
            return;
        }

        _accumulated += elapsed;
        _currentStretch += elapsed;
    }

    private async Task TriggerBreakAsync(
        TipoDescanso type, TimeSpan duration, AjustesEyeYul settings, CancellationToken ct)
    {
        bool forced = _forceBreak;
        _forceBreak = false;

        // Una pausa forzada por el usuario nunca se suprime por "flujo".
        if (!forced && await FueSuprimidaPorFlujoAsync(type, duration, settings, ct))
        {
            return;
        }

        Descanso record = NewBreak(type, duration);
        await breakRepository.AddAsync(record, ct);

        bool avisoPrevio = settings.Descansos.PreBreakWarning > TimeSpan.Zero && !forced;
        if (avisoPrevio && await ResolverAvisoPrevioAsync(record, settings, ct))
        {
            return;
        }

        await EjecutarPausaAsync(record, duration, settings, ct);
    }

    private async Task<bool> FueSuprimidaPorFlujoAsync(
        TipoDescanso type, TimeSpan duration, AjustesEyeYul settings, CancellationToken ct)
    {
        ActivitySnapshot snapshot = activityMonitor.Current;
        PauseDecision decision = smartPause.Evaluate(snapshot, settings.SmartPause);

        if (decision == PauseDecision.Allow)
        {
            return false;
        }

        logger.LogInformation("Pausa {Type} suprimida: {Reason}", type, decision);

        // Estar inactivo no gasta la pausa: se reintenta al volver.
        if (decision != PauseDecision.DeferIdle)
        {
            Descanso registroSuprimido = NewBreak(type, duration);
            registroSuprimido.MarkSuppressed();
            await breakRepository.AddAsync(registroSuprimido, ct);

            UltimaSupresion = decision;

            // Reintentar dentro de ~2 min. Se recorta contra el intervalo para que
            // un intervalo corto no deje _accumulated en negativo (lo que retrasaria
            // el reintento muchisimo mas de lo previsto).
            TimeSpan reintento = TimeSpan.FromMinutes(2);
            _accumulated = settings.Descansos.Interval > reintento
                ? settings.Descansos.Interval - reintento
                : TimeSpan.Zero;

            PausaSuprimida?.Invoke(this, decision);
        }

        return true;
    }

    private async Task<bool> ResolverAvisoPrevioAsync(
        Descanso record, AjustesEyeYul settings, CancellationToken ct)
    {
        NotificationResponse respuesta = await notifications.ShowBreakDueAsync(
            "EyeYul",
            $"Pausa en {(int)settings.Descansos.PreBreakWarning.TotalSeconds} s",
            ct);

        if (respuesta is NotificationResponse.Skip && settings.Descansos.AllowSkip)
        {
            await SkipAsync(record, ct);
            return true;
        }

        if (respuesta is NotificationResponse.Snooze snooze
            && snoozePolicy.CanSnooze(clock.Today, record.SnoozeCount, settings.AplicacionReglas))
        {
            record.MarkSnoozed();
            snoozePolicy.RecordSnooze(clock.Today);
            await breakRepository.AddAsync(record, ct);
            _accumulated = settings.Descansos.Interval - snooze.Duration.Value;
            logger.LogInformation("Pausa pospuesta {Snooze}", snooze.Duration);
            return true;
        }

        return false;
    }

    private async Task EjecutarPausaAsync(
        Descanso record, TimeSpan duration, AjustesEyeYul settings, CancellationToken ct)
    {
        await automation.RunAsync(settings.General.OnBreakStartCommand, ct);
        record.MarkStarted(clock.Now);
        UltimaSupresion = null;

        var request = new BreakOverlayRequest(
            record,
            ElegirMensajeDePausa(settings),
            duration,
            settings.Descansos.AllowSkip && !settings.AplicacionReglas.StrictMode,
            settings.Appearance.OverlayBackgroundPath);

        switch (await overlay.ShowAsync(request, ct))
        {
            case BreakOverlayResult.Completed:
                record.MarkCompleted(clock.Now);
                await screenScore.RegisterBreakTakenAsync(ct);
                _currentStretch = TimeSpan.Zero;
                break;

            case BreakOverlayResult.Skipped:
                await SkipAsync(record, ct);
                break;

            case BreakOverlayResult.Snoozed:
                record.MarkSnoozed();
                snoozePolicy.RecordSnooze(clock.Today);
                break;
        }

        await breakRepository.AddAsync(record, ct);
        await automation.RunAsync(settings.General.OnBreakEndCommand, ct);
        await screenScore.UpdateStretchAsync(_currentStretch, ct);
        _accumulated = TimeSpan.Zero;
    }

    private static string ElegirMensajeDePausa(AjustesEyeYul settings)
    {
        List<string> customMessages = settings.Appearance.CustomMessages;
        return customMessages.Count == 0
            ? settings.Appearance.BreakMessage
            : customMessages[Random.Shared.Next(customMessages.Count)];
    }

    private async Task SkipAsync(Descanso record, CancellationToken ct)
    {
        record.MarkSkipped(clock.Now);
        await breakRepository.AddAsync(record, ct);
        await screenScore.RegisterBreakSkippedAsync(ct);
        _accumulated = TimeSpan.Zero;
    }

    private Descanso NewBreak(TipoDescanso type, TimeSpan duration) => new()
    {
        Type = type,
        ScheduledAt = clock.Now,
        PlannedDuration = duration
    };
}
