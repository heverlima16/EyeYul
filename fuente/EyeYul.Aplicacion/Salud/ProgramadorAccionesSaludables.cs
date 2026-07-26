using EyeYul.Aplicacion.Abstracciones;
using EyeYul.Aplicacion.Actividad;
using EyeYul.Aplicacion.Configuracion;
using EyeYul.Dominio.Enumeraciones;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace EyeYul.Aplicacion.Salud;

/// <summary>
/// Recordatorios cortos e independientes del ciclo de pausas 20-20-20 (postura, parpadeo,
/// hidratación, estiramiento). Respeta Smart Pause: no interrumpe en reuniones, pantalla
/// completa, video o mientras el usuario está ausente.
/// </summary>
public sealed class ProgramadorAccionesSaludables(
    IMonitorActividad monitorActividad,
    MotorPausaInteligente pausaInteligente,
    IControladorAccionSaludable controlador,
    IAlmacenAjustes almacenAjustes,
    IReloj reloj,
    ILogger<ProgramadorAccionesSaludables> registro) : BackgroundService
{
    private static readonly TimeSpan Intervalo = TimeSpan.FromSeconds(10);

    private readonly Dictionary<TipoAccionSaludable, DateTimeOffset> _ultimoDisparo = new();

    protected override async Task ExecuteAsync(CancellationToken tokenDetencion)
    {
        registro.LogInformation("ProgramadorAccionesSaludables iniciado.");

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
                registro.LogError(ex, "Error en el tick de ProgramadorAccionesSaludables.");
            }
        }
    }

    private async Task TicAsync(CancellationToken ct)
    {
        AjustesEyeYul ajustes = almacenAjustes.Current;
        AccionesSaludablesSettings acciones = ajustes.AccionesSaludables;

        (TipoAccionSaludable Tipo, TimeSpan Intervalo)[] configuradas =
        [
            (TipoAccionSaludable.Postura, acciones.PosturaIntervalo),
            (TipoAccionSaludable.Parpadeo, acciones.ParpadeoIntervalo),
            (TipoAccionSaludable.Hidratacion, acciones.HidratacionIntervalo),
            (TipoAccionSaludable.Estiramiento, acciones.EstiramientoIntervalo)
        ];

        DateTimeOffset ahora = reloj.Now;

        foreach ((TipoAccionSaludable tipo, TimeSpan intervalo) in configuradas)
        {
            if (intervalo <= TimeSpan.Zero)
            {
                continue;
            }

            DateTimeOffset ultimo = _ultimoDisparo.GetValueOrDefault(tipo, ahora);
            if (ahora - ultimo < intervalo)
            {
                continue;
            }

            if (!pausaInteligente.PuedePausarAhora(monitorActividad.Current, ajustes.SmartPause))
            {
                continue;
            }

            _ultimoDisparo[tipo] = ahora;
            await controlador.MostrarAsync(tipo, ct);
        }
    }
}
