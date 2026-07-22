using EyeYul.Aplicacion.Abstracciones;
using EyeYul.Dominio.Entidades;
using Microsoft.Data.Sqlite;

namespace EyeYul.Infraestructura.Persistencia;

public sealed class AppUsageRepository(BaseDatosSqlite db) : IAppUsageRepository
{
    public async Task AccumulateAsync(
        DateOnly date, string processName, string? friendlyName, TimeSpan delta, CancellationToken ct = default)
    {
        await using SqliteConnection cn = db.Open();
        using SqliteCommand cmd = cn.CreateCommand();

        cmd.CommandText = """
            INSERT INTO app_usage (DateKey, ProcessName, FriendlyName, ForegroundSec)
            VALUES ($date, $proc, $friendly, $sec)
            ON CONFLICT(DateKey, ProcessName) DO UPDATE SET
                ForegroundSec = ForegroundSec + $sec,
                FriendlyName = COALESCE($friendly, FriendlyName);
            """;

        cmd.Parameters.AddWithValue("$date", BaseDatosSqlite.DateKey(date));
        cmd.Parameters.AddWithValue("$proc", processName);
        cmd.Parameters.AddWithValue("$friendly", (object?)friendlyName ?? DBNull.Value);
        cmd.Parameters.AddWithValue("$sec", delta.TotalSeconds);

        await cmd.ExecuteNonQueryAsync(ct);
    }

    public async Task<IReadOnlyList<UsoApp>> GetByDateAsync(DateOnly date, CancellationToken ct = default)
    {
        await using SqliteConnection cn = db.Open();
        using SqliteCommand cmd = cn.CreateCommand();

        cmd.CommandText = "SELECT ProcessName, FriendlyName, ForegroundSec FROM app_usage WHERE DateKey=$date";
        cmd.Parameters.AddWithValue("$date", BaseDatosSqlite.DateKey(date));

        var list = new List<UsoApp>();
        await using SqliteDataReader r = await cmd.ExecuteReaderAsync(ct);
        while (await r.ReadAsync(ct))
        {
            list.Add(new UsoApp
            {
                Date = date,
                ProcessName = r.GetString(0),
                FriendlyName = r.IsDBNull(1) ? null : r.GetString(1),
                Foreground = TimeSpan.FromSeconds(r.GetDouble(2))
            });
        }

        return list;
    }
}

public sealed class WebsiteUsageRepository(BaseDatosSqlite db) : IWebsiteUsageRepository
{
    public async Task AccumulateAsync(
        DateOnly date, string domain, TimeSpan delta, CancellationToken ct = default)
    {
        await using SqliteConnection cn = db.Open();
        using SqliteCommand cmd = cn.CreateCommand();

        cmd.CommandText = """
            INSERT INTO website_usage (DateKey, Domain, ActiveSec)
            VALUES ($date, $domain, $sec)
            ON CONFLICT(DateKey, Domain) DO UPDATE SET ActiveSec = ActiveSec + $sec;
            """;

        cmd.Parameters.AddWithValue("$date", BaseDatosSqlite.DateKey(date));
        cmd.Parameters.AddWithValue("$domain", domain);
        cmd.Parameters.AddWithValue("$sec", delta.TotalSeconds);

        await cmd.ExecuteNonQueryAsync(ct);
    }

    public async Task<IReadOnlyList<UsoSitioWeb>> GetByDateAsync(DateOnly date, CancellationToken ct = default)
    {
        await using SqliteConnection cn = db.Open();
        using SqliteCommand cmd = cn.CreateCommand();

        cmd.CommandText = "SELECT Domain, ActiveSec FROM website_usage WHERE DateKey=$date";
        cmd.Parameters.AddWithValue("$date", BaseDatosSqlite.DateKey(date));

        var list = new List<UsoSitioWeb>();
        await using SqliteDataReader r = await cmd.ExecuteReaderAsync(ct);
        while (await r.ReadAsync(ct))
        {
            list.Add(new UsoSitioWeb
            {
                Date = date,
                Domain = r.GetString(0),
                ActiveTime = TimeSpan.FromSeconds(r.GetDouble(1))
            });
        }

        return list;
    }
}
