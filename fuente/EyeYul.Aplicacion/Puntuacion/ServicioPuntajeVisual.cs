using EyeYul.Aplicacion.Abstracciones;
using EyeYul.Dominio.Entidades;
using EyeYul.Dominio.Reglas;
using Microsoft.Extensions.Logging;

namespace EyeYul.Aplicacion.Puntuacion;

public sealed class ServicioPuntajeVisual(
    IScreenScoreRepository repository,
    IReloj clock,
    ILogger<ServicioPuntajeVisual> logger)
{
    public async Task<int> RegisterBreakTakenAsync(CancellationToken ct = default)
    {
        PuntajeVisualDiario day = await repository.GetOrCreateAsync(clock.Today, ct);
        day.BreaksTaken++;
        return await RecalcAndSaveAsync(day, ct);
    }

    public async Task<int> RegisterBreakSkippedAsync(CancellationToken ct = default)
    {
        PuntajeVisualDiario day = await repository.GetOrCreateAsync(clock.Today, ct);
        day.BreaksSkipped++;
        return await RecalcAndSaveAsync(day, ct);
    }

    public async Task<int> UpdateStretchAsync(TimeSpan stretch, CancellationToken ct = default)
    {
        PuntajeVisualDiario day = await repository.GetOrCreateAsync(clock.Today, ct);
        if (stretch > day.LongestStretch)
        {
            day.LongestStretch = stretch;
        }

        return await RecalcAndSaveAsync(day, ct);
    }

    public async Task<int> GetTodayScoreAsync(CancellationToken ct = default) =>
        ScreenScoreRules.Calculate(await repository.GetOrCreateAsync(clock.Today, ct));

    private async Task<int> RecalcAndSaveAsync(PuntajeVisualDiario day, CancellationToken ct)
    {
        day.Score = ScreenScoreRules.Calculate(day);
        await repository.SaveAsync(day, ct);
        logger.LogDebug(
            "Screen Score de {Date} = {Score} ({Grade})",
            day.Date, day.Score, ScreenScoreRules.Grade(day.Score));
        return day.Score;
    }
}
