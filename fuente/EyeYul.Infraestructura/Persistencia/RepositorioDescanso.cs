using EyeYul.Aplicacion.Abstracciones;
using EyeYul.Dominio.Entidades;
using EyeYul.Dominio.Enumeraciones;
using Microsoft.Data.Sqlite;

namespace EyeYul.Infraestructura.Persistencia;

public sealed class RepositorioDescanso(BaseDatosSqlite db) : IBreakRepository
{
    public async Task AddAsync(Descanso br, CancellationToken ct = default)
    {
        await using SqliteConnection cn = db.Open();
        using SqliteCommand cmd = cn.CreateCommand();

        cmd.CommandText = """
            INSERT INTO breaks (Id, Type, ScheduledAt, StartedAt, EndedAt, PlannedDurationSec, Outcome, SnoozeCount, DateKey)
            VALUES ($id, $type, $sched, $start, $end, $dur, $outcome, $snooze, $date)
            ON CONFLICT(Id) DO UPDATE SET
                StartedAt=$start, EndedAt=$end, Outcome=$outcome, SnoozeCount=$snooze;
            """;

        cmd.Parameters.AddWithValue("$id", br.Id.ToString());
        cmd.Parameters.AddWithValue("$type", (int)br.Type);
        cmd.Parameters.AddWithValue("$sched", BaseDatosSqlite.Iso(br.ScheduledAt));
        cmd.Parameters.AddWithValue("$start", (object?)br.StartedAt?.ToString("O") ?? DBNull.Value);
        cmd.Parameters.AddWithValue("$end", (object?)br.EndedAt?.ToString("O") ?? DBNull.Value);
        cmd.Parameters.AddWithValue("$dur", br.PlannedDuration.TotalSeconds);
        cmd.Parameters.AddWithValue("$outcome", (int)br.Outcome);
        cmd.Parameters.AddWithValue("$snooze", br.SnoozeCount);
        cmd.Parameters.AddWithValue(
            "$date", BaseDatosSqlite.DateKey(DateOnly.FromDateTime(br.ScheduledAt.LocalDateTime)));

        await cmd.ExecuteNonQueryAsync(ct);
    }

    public async Task<IReadOnlyList<Descanso>> GetByDateAsync(DateOnly date, CancellationToken ct = default)
    {
        await using SqliteConnection cn = db.Open();
        using SqliteCommand cmd = cn.CreateCommand();

        cmd.CommandText = """
            SELECT Id, Type, ScheduledAt, StartedAt, EndedAt, PlannedDurationSec, Outcome, SnoozeCount
            FROM breaks WHERE DateKey=$date ORDER BY ScheduledAt
            """;

        cmd.Parameters.AddWithValue("$date", BaseDatosSqlite.DateKey(date));

        var list = new List<Descanso>();
        await using SqliteDataReader r = await cmd.ExecuteReaderAsync(ct);
        while (await r.ReadAsync(ct))
        {
            list.Add(new Descanso
            {
                Id = Guid.Parse(r.GetString(0)),
                Type = (TipoDescanso)r.GetInt32(1),
                ScheduledAt = DateTimeOffset.Parse(r.GetString(2)),
                StartedAt = BaseDatosSqlite.ParseIso(r.IsDBNull(3) ? null : r.GetString(3)),
                EndedAt = BaseDatosSqlite.ParseIso(r.IsDBNull(4) ? null : r.GetString(4)),
                PlannedDuration = TimeSpan.FromSeconds(r.GetDouble(5)),
                Outcome = (ResultadoDescanso)r.GetInt32(6),
                SnoozeCount = r.GetInt32(7)
            });
        }

        return list;
    }
}
