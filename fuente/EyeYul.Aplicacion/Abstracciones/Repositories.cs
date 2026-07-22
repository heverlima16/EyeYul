using EyeYul.Dominio.Entidades;

namespace EyeYul.Aplicacion.Abstracciones;

public interface IBreakRepository
{
    Task AddAsync(Descanso br, CancellationToken ct = default);

    Task<IReadOnlyList<Descanso>> GetByDateAsync(DateOnly date, CancellationToken ct = default);
}

public interface ISessionRepository
{
    Task<Sesion> StartAsync(DateTimeOffset at, CancellationToken ct = default);

    Task CloseAsync(Sesion session, CancellationToken ct = default);

    Task<IReadOnlyList<Sesion>> GetByDateAsync(DateOnly date, CancellationToken ct = default);
}

public interface IScreenScoreRepository
{
    Task<PuntajeVisualDiario> GetOrCreateAsync(DateOnly date, CancellationToken ct = default);

    Task SaveAsync(PuntajeVisualDiario score, CancellationToken ct = default);

    Task<IReadOnlyList<PuntajeVisualDiario>> GetRangeAsync(DateOnly from, DateOnly to, CancellationToken ct = default);
}

public interface IPlannedBreakRepository
{
    Task<IReadOnlyList<DescansoProgramado>> GetAllAsync(CancellationToken ct = default);

    Task UpsertAsync(DescansoProgramado plannedBreak, CancellationToken ct = default);

    Task DeleteAsync(Guid id, CancellationToken ct = default);
}

public interface IAppUsageRepository
{
    Task AccumulateAsync(DateOnly date, string processName, string? friendlyName, TimeSpan delta, CancellationToken ct = default);

    Task<IReadOnlyList<UsoApp>> GetByDateAsync(DateOnly date, CancellationToken ct = default);
}

public interface IWebsiteUsageRepository
{
    Task AccumulateAsync(DateOnly date, string domain, TimeSpan delta, CancellationToken ct = default);

    Task<IReadOnlyList<UsoSitioWeb>> GetByDateAsync(DateOnly date, CancellationToken ct = default);
}

public interface IAutomationRunner
{
    Task RunAsync(string? command, CancellationToken ct = default);
}
