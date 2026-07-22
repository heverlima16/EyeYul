using EyeYul.Aplicacion.Abstracciones;
using EyeYul.Dominio.Entidades;
using EyeYul.Dominio.Reglas;
using Microsoft.Extensions.Logging;

namespace EyeYul.Aplicacion.Puntuacion;

public sealed class ServicioPuntajeVisual(
    IScreenScoreRepository repositorio,
    IReloj reloj,
    ILogger<ServicioPuntajeVisual> registro)
{
    public async Task<int> RegistrarDescansoTomadoAsync(CancellationToken ct = default)
    {
        PuntajeVisualDiario dia = await repositorio.GetOrCreateAsync(reloj.Today, ct);
        dia.DescansosTomados++;
        return await RecalcularYGuardarAsync(dia, ct);
    }

    public async Task<int> RegistrarDescansoOmitidoAsync(CancellationToken ct = default)
    {
        PuntajeVisualDiario dia = await repositorio.GetOrCreateAsync(reloj.Today, ct);
        dia.DescansosOmitidos++;
        return await RecalcularYGuardarAsync(dia, ct);
    }

    public async Task<int> ActualizarRachaAsync(TimeSpan racha, CancellationToken ct = default)
    {
        PuntajeVisualDiario dia = await repositorio.GetOrCreateAsync(reloj.Today, ct);
        if (racha > dia.RachaMasLarga)
        {
            dia.RachaMasLarga = racha;
        }

        return await RecalcularYGuardarAsync(dia, ct);
    }

    public async Task<int> ObtenerPuntajeDeHoyAsync(CancellationToken ct = default) =>
        ReglasPuntajeVisual.Calcular(await repositorio.GetOrCreateAsync(reloj.Today, ct));

    private async Task<int> RecalcularYGuardarAsync(PuntajeVisualDiario dia, CancellationToken ct)
    {
        dia.Puntaje = ReglasPuntajeVisual.Calcular(dia);
        await repositorio.SaveAsync(dia, ct);
        registro.LogDebug(
            "Screen Score de {Fecha} = {Puntaje} ({Calificacion})",
            dia.Fecha, dia.Puntaje, ReglasPuntajeVisual.Calificacion(dia.Puntaje));
        return dia.Puntaje;
    }
}
