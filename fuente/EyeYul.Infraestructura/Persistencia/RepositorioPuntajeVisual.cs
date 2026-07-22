using EyeYul.Aplicacion.Abstracciones;
using EyeYul.Dominio.Entidades;
using Microsoft.Data.Sqlite;

namespace EyeYul.Infraestructura.Persistencia;

public sealed class RepositorioPuntajeVisual(BaseDatosSqlite db) : IScreenScoreRepository
{
    public async Task<PuntajeVisualDiario> GetOrCreateAsync(DateOnly date, CancellationToken ct = default)
    {
        await using SqliteConnection cn = db.Open();
        using SqliteCommand cmd = cn.CreateCommand();

        cmd.CommandText = """
            SELECT Score, BreaksTaken, BreaksSkipped, ActiveTimeSec, LongestStretchSec
            FROM screen_score WHERE DateKey=$date
            """;

        cmd.Parameters.AddWithValue("$date", BaseDatosSqlite.DateKey(date));

        await using SqliteDataReader r = await cmd.ExecuteReaderAsync(ct);

        if (!await r.ReadAsync(ct))
        {
            return new PuntajeVisualDiario { Date = date };
        }

        return new PuntajeVisualDiario
        {
            Date = date,
            Score = r.GetInt32(0),
            BreaksTaken = r.GetInt32(1),
            BreaksSkipped = r.GetInt32(2),
            ActiveTime = TimeSpan.FromSeconds(r.GetDouble(3)),
            LongestStretch = TimeSpan.FromSeconds(r.GetDouble(4))
        };
    }

    public async Task SaveAsync(PuntajeVisualDiario score, CancellationToken ct = default)
    {
        await using SqliteConnection cn = db.Open();
        using SqliteCommand cmd = cn.CreateCommand();

        cmd.CommandText = """
            INSERT INTO screen_score (DateKey, Score, BreaksTaken, BreaksSkipped, ActiveTimeSec, LongestStretchSec)
            VALUES ($date, $score, $taken, $skipped, $active, $stretch)
            ON CONFLICT(DateKey) DO UPDATE SET
                Score=$score, BreaksTaken=$taken, BreaksSkipped=$skipped,
                ActiveTimeSec=$active, LongestStretchSec=$stretch;
            """;

        cmd.Parameters.AddWithValue("$date", BaseDatosSqlite.DateKey(score.Date));
        cmd.Parameters.AddWithValue("$score", score.Score);
        cmd.Parameters.AddWithValue("$taken", score.BreaksTaken);
        cmd.Parameters.AddWithValue("$skipped", score.BreaksSkipped);
        cmd.Parameters.AddWithValue("$active", score.ActiveTime.TotalSeconds);
        cmd.Parameters.AddWithValue("$stretch", score.LongestStretch.TotalSeconds);

        await cmd.ExecuteNonQueryAsync(ct);
    }

    public async Task<IReadOnlyList<PuntajeVisualDiario>> GetRangeAsync(
        DateOnly from, DateOnly to, CancellationToken ct = default)
    {
        await using SqliteConnection cn = db.Open();
        using SqliteCommand cmd = cn.CreateCommand();

        cmd.CommandText = """
            SELECT DateKey, Score, BreaksTaken, BreaksSkipped, ActiveTimeSec, LongestStretchSec
            FROM screen_score WHERE DateKey BETWEEN $from AND $to ORDER BY DateKey
            """;

        cmd.Parameters.AddWithValue("$from", BaseDatosSqlite.DateKey(from));
        cmd.Parameters.AddWithValue("$to", BaseDatosSqlite.DateKey(to));

        var list = new List<PuntajeVisualDiario>();
        await using SqliteDataReader r = await cmd.ExecuteReaderAsync(ct);
        while (await r.ReadAsync(ct))
        {
            list.Add(new PuntajeVisualDiario
            {
                Date = DateOnly.ParseExact(r.GetString(0), "yyyy-MM-dd"),
                Score = r.GetInt32(1),
                BreaksTaken = r.GetInt32(2),
                BreaksSkipped = r.GetInt32(3),
                ActiveTime = TimeSpan.FromSeconds(r.GetDouble(4)),
                LongestStretch = TimeSpan.FromSeconds(r.GetDouble(5))
            });
        }

        return list;
    }
}
