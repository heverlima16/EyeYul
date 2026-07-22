using EyeYul.Aplicacion.Abstracciones;
using EyeYul.Dominio.Entidades;

namespace EyeYul.Aplicacion.Estadisticas;

public sealed class ServicioEstadisticas(
    IScreenScoreRepository repoPuntaje,
    IAppUsageRepository repoApps,
    IWebsiteUsageRepository repoSitios,
    ISessionRepository repoSesiones)
{
    public async Task<ResumenDiario> ObtenerResumenDiarioAsync(
        DateOnly fecha, int primeros = 5, CancellationToken ct = default)
    {
        PuntajeVisualDiario puntaje = await repoPuntaje.GetOrCreateAsync(fecha, ct);
        IReadOnlyList<UsoApp> apps = await repoApps.GetByDateAsync(fecha, ct);
        IReadOnlyList<UsoSitioWeb> sitios = await repoSitios.GetByDateAsync(fecha, ct);
        IReadOnlyList<Sesion> sesiones = await repoSesiones.GetByDateAsync(fecha, ct);

        return new ResumenDiario(
            Fecha: fecha,
            Puntaje: puntaje.Puntaje,
            DescansosTomados: puntaje.DescansosTomados,
            DescansosOmitidos: puntaje.DescansosOmitidos,
            TiempoActivo: sesiones.Aggregate(TimeSpan.Zero, (acumulado, s) => acumulado + s.TiempoActivo),
            AppsPrincipales: apps.OrderByDescending(a => a.PrimerPlano).Take(primeros).ToList(),
            SitiosPrincipales: sitios.OrderByDescending(s => s.TiempoActivo).Take(primeros).ToList());
    }
}

public sealed record ResumenDiario(
    DateOnly Fecha,
    int Puntaje,
    int DescansosTomados,
    int DescansosOmitidos,
    TimeSpan TiempoActivo,
    IReadOnlyList<UsoApp> AppsPrincipales,
    IReadOnlyList<UsoSitioWeb> SitiosPrincipales);
