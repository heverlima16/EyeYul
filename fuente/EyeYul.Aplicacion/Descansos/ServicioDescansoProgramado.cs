using EyeYul.Aplicacion.Abstracciones;
using EyeYul.Dominio.Entidades;

namespace EyeYul.Aplicacion.Descansos;

public sealed class ServicioDescansoProgramado(IPlannedBreakRepository repository, IReloj clock)
{
    private readonly object _sync = new();

    private IReadOnlyList<DescansoProgramado>? _cache;

    private readonly Dictionary<Guid, DateTimeOffset> _ultimoDisparoPorPausa = new();

    public async Task<IReadOnlyList<DescansoProgramado>> GetAllAsync(CancellationToken ct = default)
    {
        lock (_sync)
        {
            if (_cache is not null)
            {
                return _cache;
            }
        }

        IReadOnlyList<DescansoProgramado> desdeBaseDatos = await repository.GetAllAsync(ct);

        lock (_sync)
        {
            _cache ??= desdeBaseDatos;
            return _cache;
        }
    }

    public async Task SaveAsync(DescansoProgramado plannedBreak, CancellationToken ct = default)
    {
        await repository.UpsertAsync(plannedBreak, ct);
        InvalidarCache();
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        await repository.DeleteAsync(id, ct);
        InvalidarCache();
    }

    private void InvalidarCache()
    {
        lock (_sync)
        {
            _cache = null;
        }
    }

    public async Task<DescansoProgramado?> GetDueAsync(TimeSpan tolerance, CancellationToken ct = default)
    {
        DateTimeOffset now = clock.Now;

        foreach (DescansoProgramado pausa in await GetAllAsync(ct))
        {
            if (!pausa.OccursOn(now.DayOfWeek))
            {
                continue;
            }

            var programadaHoy = new DateTimeOffset(
                now.Year, now.Month, now.Day, pausa.TimeOfDay.Hour, pausa.TimeOfDay.Minute, 0, now.Offset);

            TimeSpan retraso = now - programadaHoy;
            if (retraso < TimeSpan.Zero || retraso > tolerance)
            {
                continue;
            }

            lock (_sync)
            {
                // No volver a disparar la misma ocurrencia dentro de la ventana de tolerancia.
                if (_ultimoDisparoPorPausa.TryGetValue(pausa.Id, out DateTimeOffset anterior)
                    && anterior == programadaHoy)
                {
                    continue;
                }

                _ultimoDisparoPorPausa[pausa.Id] = programadaHoy;
                return pausa;
            }
        }

        return null;
    }

    public async Task<DateTimeOffset?> NextOccurrenceAsync(CancellationToken ct = default)
    {
        return (await GetAllAsync(ct))
            .Select(pausa => pausa.NextOccurrence(clock.Now))
            .Where(fecha => fecha.HasValue)
            .OrderBy(fecha => fecha)
            .FirstOrDefault();
    }
}
