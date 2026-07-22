using EyeYul.Aplicacion.Abstracciones;
using EyeYul.Dominio.Entidades;
using Microsoft.Data.Sqlite;

namespace EyeYul.Infraestructura.Persistencia;

public sealed class RepositorioSesion(BaseDatosSqlite db) : ISessionRepository
{
    public async Task<Sesion> StartAsync(DateTimeOffset at, CancellationToken ct = default)
    {
        var session = new Sesion { StartedAt = at };

        await using SqliteConnection cn = db.Open();
        using SqliteCommand cmd = cn.CreateCommand();

        cmd.CommandText = """
            INSERT INTO sessions (Id, StartedAt, EndedAt, ActiveTimeSec, BreaksTaken, BreaksSkipped, DateKey)
            VALUES ($id, $start, NULL, 0, 0, 0, $date);
            """;

        cmd.Parameters.AddWithValue("$id", session.Id.ToString());
        cmd.Parameters.AddWithValue("$start", BaseDatosSqlite.Iso(at));
        cmd.Parameters.AddWithValue(
            "$date", BaseDatosSqlite.DateKey(DateOnly.FromDateTime(at.LocalDateTime)));

        await cmd.ExecuteNonQueryAsync(ct);
        return session;
    }

    public async Task CloseAsync(Sesion session, CancellationToken ct = default)
    {
        await using SqliteConnection cn = db.Open();
        using SqliteCommand cmd = cn.CreateCommand();

        cmd.CommandText = """
            UPDATE sessions SET EndedAt=$end, ActiveTimeSec=$active, BreaksTaken=$taken, BreaksSkipped=$skipped
            WHERE Id=$id;
            """;

        cmd.Parameters.AddWithValue("$id", session.Id.ToString());
        cmd.Parameters.AddWithValue("$end", (object?)session.EndedAt?.ToString("O") ?? DBNull.Value);
        cmd.Parameters.AddWithValue("$active", session.ActiveTime.TotalSeconds);
        cmd.Parameters.AddWithValue("$taken", session.BreaksTaken);
        cmd.Parameters.AddWithValue("$skipped", session.BreaksSkipped);

        await cmd.ExecuteNonQueryAsync(ct);
    }

    public async Task<IReadOnlyList<Sesion>> GetByDateAsync(DateOnly date, CancellationToken ct = default)
    {
        await using SqliteConnection cn = db.Open();
        using SqliteCommand cmd = cn.CreateCommand();

        cmd.CommandText = """
            SELECT Id, StartedAt, EndedAt, ActiveTimeSec, BreaksTaken, BreaksSkipped
            FROM sessions WHERE DateKey=$date ORDER BY StartedAt
            """;

        cmd.Parameters.AddWithValue("$date", BaseDatosSqlite.DateKey(date));

        var list = new List<Sesion>();
        await using SqliteDataReader r = await cmd.ExecuteReaderAsync(ct);
        while (await r.ReadAsync(ct))
        {
            list.Add(new Sesion
            {
                Id = Guid.Parse(r.GetString(0)),
                StartedAt = DateTimeOffset.Parse(r.GetString(1)),
                EndedAt = BaseDatosSqlite.ParseIso(r.IsDBNull(2) ? null : r.GetString(2)),
                ActiveTime = TimeSpan.FromSeconds(r.GetDouble(3)),
                BreaksTaken = r.GetInt32(4),
                BreaksSkipped = r.GetInt32(5)
            });
        }

        return list;
    }
}
