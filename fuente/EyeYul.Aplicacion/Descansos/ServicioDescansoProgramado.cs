using EyeYul.Aplicacion.Abstracciones;
using EyeYul.Dominio.Entidades;

namespace EyeYul.Aplicacion.Descansos;

public sealed class ServicioDescansoProgramado(IPlannedBreakRepository repositorio, IReloj reloj)
{
    private readonly object _sync = new();

    private IReadOnlyList<DescansoProgramado>? _cache;

    private readonly Dictionary<Guid, DateTimeOffset> _ultimoDisparoPorPausa = new();

    public async Task<IReadOnlyList<DescansoProgramado>> ObtenerTodosAsync(CancellationToken ct = default)
    {
        lock (_sync)
        {
            if (_cache is not null)
            {
                return _cache;
            }
        }

        IReadOnlyList<DescansoProgramado> desdeBaseDatos = await repositorio.GetAllAsync(ct);

        lock (_sync)
        {
            _cache ??= desdeBaseDatos;
            return _cache;
        }
    }

    public async Task GuardarAsync(DescansoProgramado descansoProgramado, CancellationToken ct = default)
    {
        await repositorio.UpsertAsync(descansoProgramado, ct);
        InvalidarCache();
    }

    public async Task EliminarAsync(Guid id, CancellationToken ct = default)
    {
        await repositorio.DeleteAsync(id, ct);
        InvalidarCache();
    }

    private void InvalidarCache()
    {
        lock (_sync)
        {
            _cache = null;
        }
    }

    public async Task<DescansoProgramado?> ObtenerPendienteAsync(TimeSpan tolerancia, CancellationToken ct = default)
    {
        DateTimeOffset ahora = reloj.Now;

        foreach (DescansoProgramado pausa in await ObtenerTodosAsync(ct))
        {
            if (!pausa.OcurreEn(ahora.DayOfWeek))
            {
                continue;
            }

            var programadaHoy = new DateTimeOffset(
                ahora.Year, ahora.Month, ahora.Day, pausa.HoraDelDia.Hour, pausa.HoraDelDia.Minute, 0, ahora.Offset);

            TimeSpan retraso = ahora - programadaHoy;
            if (retraso < TimeSpan.Zero || retraso > tolerancia)
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

    public async Task<DateTimeOffset?> ProximaOcurrenciaAsync(CancellationToken ct = default)
    {
        return (await ObtenerTodosAsync(ct))
            .Select(pausa => pausa.ProximaOcurrencia(reloj.Now))
            .Where(fecha => fecha.HasValue)
            .OrderBy(fecha => fecha)
            .FirstOrDefault();
    }
}
