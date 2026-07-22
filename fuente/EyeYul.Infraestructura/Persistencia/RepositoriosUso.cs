using EyeYul.Aplicacion.Abstracciones;
using EyeYul.Dominio.Entidades;
using Microsoft.Data.Sqlite;

namespace EyeYul.Infraestructura.Persistencia;

public sealed class RepositorioUsoApp(BaseDatosSqlite bd) : IAppUsageRepository
{
    public async Task AccumulateAsync(
        DateOnly fecha, string nombreProceso, string? nombreAmigable, TimeSpan delta, CancellationToken ct = default)
    {
        await using SqliteConnection cn = bd.Abrir();
        using SqliteCommand cmd = cn.CreateCommand();

        cmd.CommandText = """
            INSERT INTO uso_apps (ClaveFecha, NombreProceso, NombreAmigable, PrimerPlanoSeg)
            VALUES ($fecha, $proceso, $amigable, $segundos)
            ON CONFLICT(ClaveFecha, NombreProceso) DO UPDATE SET
                PrimerPlanoSeg = PrimerPlanoSeg + $segundos,
                NombreAmigable = COALESCE($amigable, NombreAmigable);
            """;

        cmd.Parameters.AddWithValue("$fecha", BaseDatosSqlite.ClaveFecha(fecha));
        cmd.Parameters.AddWithValue("$proceso", nombreProceso);
        cmd.Parameters.AddWithValue("$amigable", (object?)nombreAmigable ?? DBNull.Value);
        cmd.Parameters.AddWithValue("$segundos", delta.TotalSeconds);

        await cmd.ExecuteNonQueryAsync(ct);
    }

    public async Task<IReadOnlyList<UsoApp>> GetByDateAsync(DateOnly fecha, CancellationToken ct = default)
    {
        await using SqliteConnection cn = bd.Abrir();
        using SqliteCommand cmd = cn.CreateCommand();

        cmd.CommandText = "SELECT NombreProceso, NombreAmigable, PrimerPlanoSeg FROM uso_apps WHERE ClaveFecha=$fecha";
        cmd.Parameters.AddWithValue("$fecha", BaseDatosSqlite.ClaveFecha(fecha));

        var lista = new List<UsoApp>();
        await using SqliteDataReader lector = await cmd.ExecuteReaderAsync(ct);
        while (await lector.ReadAsync(ct))
        {
            lista.Add(new UsoApp
            {
                Fecha = fecha,
                NombreProceso = lector.GetString(0),
                NombreAmigable = lector.IsDBNull(1) ? null : lector.GetString(1),
                PrimerPlano = TimeSpan.FromSeconds(lector.GetDouble(2))
            });
        }

        return lista;
    }
}

public sealed class RepositorioUsoSitioWeb(BaseDatosSqlite bd) : IWebsiteUsageRepository
{
    public async Task AccumulateAsync(
        DateOnly fecha, string dominio, TimeSpan delta, CancellationToken ct = default)
    {
        await using SqliteConnection cn = bd.Abrir();
        using SqliteCommand cmd = cn.CreateCommand();

        cmd.CommandText = """
            INSERT INTO uso_sitios_web (ClaveFecha, Dominio, TiempoActivoSeg)
            VALUES ($fecha, $dominio, $segundos)
            ON CONFLICT(ClaveFecha, Dominio) DO UPDATE SET TiempoActivoSeg = TiempoActivoSeg + $segundos;
            """;

        cmd.Parameters.AddWithValue("$fecha", BaseDatosSqlite.ClaveFecha(fecha));
        cmd.Parameters.AddWithValue("$dominio", dominio);
        cmd.Parameters.AddWithValue("$segundos", delta.TotalSeconds);

        await cmd.ExecuteNonQueryAsync(ct);
    }

    public async Task<IReadOnlyList<UsoSitioWeb>> GetByDateAsync(DateOnly fecha, CancellationToken ct = default)
    {
        await using SqliteConnection cn = bd.Abrir();
        using SqliteCommand cmd = cn.CreateCommand();

        cmd.CommandText = "SELECT Dominio, TiempoActivoSeg FROM uso_sitios_web WHERE ClaveFecha=$fecha";
        cmd.Parameters.AddWithValue("$fecha", BaseDatosSqlite.ClaveFecha(fecha));

        var lista = new List<UsoSitioWeb>();
        await using SqliteDataReader lector = await cmd.ExecuteReaderAsync(ct);
        while (await lector.ReadAsync(ct))
        {
            lista.Add(new UsoSitioWeb
            {
                Fecha = fecha,
                Dominio = lector.GetString(0),
                TiempoActivo = TimeSpan.FromSeconds(lector.GetDouble(1))
            });
        }

        return lista;
    }
}
