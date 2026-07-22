using EyeYul.Aplicacion.Abstracciones;
using EyeYul.Dominio.Entidades;
using Microsoft.Data.Sqlite;

namespace EyeYul.Infraestructura.Persistencia;

public sealed class RepositorioDescansoProgramado(BaseDatosSqlite db) : IPlannedBreakRepository
{
    public async Task<IReadOnlyList<DescansoProgramado>> GetAllAsync(CancellationToken ct = default)
    {
        await using SqliteConnection cn = db.Open();
        using SqliteCommand cmd = cn.CreateCommand();

        cmd.CommandText = """
            SELECT Id, Name, TimeOfDay, Days, DurationSec, Enabled, CountsAwayTime FROM planned_breaks
            """;

        var list = new List<DescansoProgramado>();
        await using SqliteDataReader r = await cmd.ExecuteReaderAsync(ct);
        while (await r.ReadAsync(ct))
        {
            var pb = new DescansoProgramado
            {
                Id = Guid.Parse(r.GetString(0)),
                Name = r.GetString(1),
                TimeOfDay = TimeOnly.Parse(r.GetString(2)),
                Duration = TimeSpan.FromSeconds(r.GetDouble(4)),
                Enabled = r.GetInt32(5) != 0,
                CountsAwayTime = r.GetInt32(6) != 0
            };

            foreach (DayOfWeek d in ParseDays(r.GetString(3)))
            {
                pb.Days.Add(d);
            }

            list.Add(pb);
        }

        return list;
    }

    public async Task UpsertAsync(DescansoProgramado pb, CancellationToken ct = default)
    {
        await using SqliteConnection cn = db.Open();
        using SqliteCommand cmd = cn.CreateCommand();

        cmd.CommandText = """
            INSERT INTO planned_breaks (Id, Name, TimeOfDay, Days, DurationSec, Enabled, CountsAwayTime)
            VALUES ($id, $name, $time, $days, $dur, $enabled, $away)
            ON CONFLICT(Id) DO UPDATE SET
                Name=$name, TimeOfDay=$time, Days=$days, DurationSec=$dur,
                Enabled=$enabled, CountsAwayTime=$away;
            """;

        cmd.Parameters.AddWithValue("$id", pb.Id.ToString());
        cmd.Parameters.AddWithValue("$name", pb.Name);
        cmd.Parameters.AddWithValue("$time", pb.TimeOfDay.ToString("HH:mm"));
        cmd.Parameters.AddWithValue("$days", string.Join(',', pb.Days.Select(d => (int)d)));
        cmd.Parameters.AddWithValue("$dur", pb.Duration.TotalSeconds);
        cmd.Parameters.AddWithValue("$enabled", pb.Enabled ? 1 : 0);
        cmd.Parameters.AddWithValue("$away", pb.CountsAwayTime ? 1 : 0);

        await cmd.ExecuteNonQueryAsync(ct);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        await using SqliteConnection cn = db.Open();
        using SqliteCommand cmd = cn.CreateCommand();

        cmd.CommandText = "DELETE FROM planned_breaks WHERE Id=$id";
        cmd.Parameters.AddWithValue("$id", id.ToString());

        await cmd.ExecuteNonQueryAsync(ct);
    }

    private static IEnumerable<DayOfWeek> ParseDays(string csv) =>
        csv.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
           .Select(s => (DayOfWeek)int.Parse(s));
}
