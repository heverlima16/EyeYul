using EyeYul.Aplicacion.Abstracciones;
using EyeYul.Dominio.Entidades;
using Microsoft.Data.Sqlite;

namespace EyeYul.Infraestructura.Persistencia;

public sealed class RepositorioSesion(BaseDatosSqlite bd) : ISessionRepository
{
    public async Task<Sesion> StartAsync(DateTimeOffset momento, CancellationToken ct = default)
    {
        var sesion = new Sesion { IniciadaEn = momento };

        await using SqliteConnection cn = bd.Abrir();
        using SqliteCommand cmd = cn.CreateCommand();

        cmd.CommandText = """
            INSERT INTO sesiones (Id, IniciadaEn, FinalizadaEn, TiempoActivoSeg, DescansosTomados, DescansosOmitidos, ClaveFecha)
            VALUES ($id, $inicio, NULL, 0, 0, 0, $fecha);
            """;

        cmd.Parameters.AddWithValue("$id", sesion.Id.ToString());
        cmd.Parameters.AddWithValue("$inicio", BaseDatosSqlite.Iso(momento));
        cmd.Parameters.AddWithValue(
            "$fecha", BaseDatosSqlite.ClaveFecha(DateOnly.FromDateTime(momento.LocalDateTime)));

        await cmd.ExecuteNonQueryAsync(ct);
        return sesion;
    }

    public async Task CloseAsync(Sesion sesion, CancellationToken ct = default)
    {
        await using SqliteConnection cn = bd.Abrir();
        using SqliteCommand cmd = cn.CreateCommand();

        cmd.CommandText = """
            UPDATE sesiones SET FinalizadaEn=$fin, TiempoActivoSeg=$activo, DescansosTomados=$tomados, DescansosOmitidos=$omitidos
            WHERE Id=$id;
            """;

        cmd.Parameters.AddWithValue("$id", sesion.Id.ToString());
        cmd.Parameters.AddWithValue("$fin", (object?)sesion.FinalizadaEn?.ToString("O") ?? DBNull.Value);
        cmd.Parameters.AddWithValue("$activo", sesion.TiempoActivo.TotalSeconds);
        cmd.Parameters.AddWithValue("$tomados", sesion.DescansosTomados);
        cmd.Parameters.AddWithValue("$omitidos", sesion.DescansosOmitidos);

        await cmd.ExecuteNonQueryAsync(ct);
    }

    public async Task<IReadOnlyList<Sesion>> GetByDateAsync(DateOnly fecha, CancellationToken ct = default)
    {
        await using SqliteConnection cn = bd.Abrir();
        using SqliteCommand cmd = cn.CreateCommand();

        cmd.CommandText = """
            SELECT Id, IniciadaEn, FinalizadaEn, TiempoActivoSeg, DescansosTomados, DescansosOmitidos
            FROM sesiones WHERE ClaveFecha=$fecha ORDER BY IniciadaEn
            """;

        cmd.Parameters.AddWithValue("$fecha", BaseDatosSqlite.ClaveFecha(fecha));

        var lista = new List<Sesion>();
        await using SqliteDataReader lector = await cmd.ExecuteReaderAsync(ct);
        while (await lector.ReadAsync(ct))
        {
            lista.Add(new Sesion
            {
                Id = Guid.Parse(lector.GetString(0)),
                IniciadaEn = DateTimeOffset.Parse(lector.GetString(1)),
                FinalizadaEn = BaseDatosSqlite.LeerIso(lector.IsDBNull(2) ? null : lector.GetString(2)),
                TiempoActivo = TimeSpan.FromSeconds(lector.GetDouble(3)),
                DescansosTomados = lector.GetInt32(4),
                DescansosOmitidos = lector.GetInt32(5)
            });
        }

        return lista;
    }
}
