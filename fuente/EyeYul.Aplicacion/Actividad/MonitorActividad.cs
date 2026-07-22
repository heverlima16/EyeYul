using EyeYul.Aplicacion.Abstracciones;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace EyeYul.Aplicacion.Actividad;

public sealed class MonitorActividad(
    IProveedorActividadSistema provider,
    ILogger<MonitorActividad> logger) : BackgroundService, IMonitorActividad
{
    private static readonly TimeSpan PollInterval = TimeSpan.FromSeconds(2);

    public ActivitySnapshot Current { get; private set; }

    public event EventHandler<ActivityChangedEventArgs>? ActivityChanged;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("MonitorActividad iniciado.");
        Current = SafeSample();

        using var timer = new PeriodicTimer(PollInterval);
        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            ActivitySnapshot sample = SafeSample();
            if (sample != Current)
            {
                Current = sample;
                ActivityChanged?.Invoke(this, new ActivityChangedEventArgs(sample));
            }
        }
    }

    private ActivitySnapshot SafeSample()
    {
        try
        {
            return provider.Sample();
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Fallo al muestrear actividad del sistema.");
            return Current;
        }
    }
}
