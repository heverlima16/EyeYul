using EyeYul.Aplicacion.Abstracciones;
using EyeYul.Dominio.Entidades;

namespace EyeYul.Aplicacion.Estadisticas;

public sealed class ServicioEstadisticas(
    IScreenScoreRepository scoreRepo,
    IAppUsageRepository appRepo,
    IWebsiteUsageRepository webRepo,
    ISessionRepository sessionRepo)
{
    public async Task<DailyStatsSummary> GetDailySummaryAsync(
        DateOnly date, int topN = 5, CancellationToken ct = default)
    {
        PuntajeVisualDiario score = await scoreRepo.GetOrCreateAsync(date, ct);
        IReadOnlyList<UsoApp> apps = await appRepo.GetByDateAsync(date, ct);
        IReadOnlyList<UsoSitioWeb> sites = await webRepo.GetByDateAsync(date, ct);
        IReadOnlyList<Sesion> sessions = await sessionRepo.GetByDateAsync(date, ct);

        return new DailyStatsSummary(
            Date: date,
            Score: score.Score,
            BreaksTaken: score.BreaksTaken,
            BreaksSkipped: score.BreaksSkipped,
            ActiveTime: sessions.Aggregate(TimeSpan.Zero, (acc, s) => acc + s.ActiveTime),
            TopApps: apps.OrderByDescending(a => a.Foreground).Take(topN).ToList(),
            TopSites: sites.OrderByDescending(w => w.ActiveTime).Take(topN).ToList());
    }
}

public sealed record DailyStatsSummary(
    DateOnly Date,
    int Score,
    int BreaksTaken,
    int BreaksSkipped,
    TimeSpan ActiveTime,
    IReadOnlyList<UsoApp> TopApps,
    IReadOnlyList<UsoSitioWeb> TopSites);
