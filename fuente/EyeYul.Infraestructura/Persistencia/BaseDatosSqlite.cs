using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;

namespace EyeYul.Infraestructura.Persistencia;

public sealed class BaseDatosSqlite
{
    private readonly string _connectionString;

    public BaseDatosSqlite(ILogger<BaseDatosSqlite> logger)
    {
        string folder = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "EyeYul");
        Directory.CreateDirectory(folder);

        string dbPath = Path.Combine(folder, "eyeyul.db");

        _connectionString = new SqliteConnectionStringBuilder
        {
            DataSource = dbPath,
            Mode = SqliteOpenMode.ReadWriteCreate
        }.ToString();

        logger.LogInformation("Base de datos en {Path}", dbPath);
        Initialize();
    }

    public SqliteConnection Open()
    {
        var cn = new SqliteConnection(_connectionString);
        cn.Open();
        return cn;
    }

    private void Initialize()
    {
        using SqliteConnection cn = Open();
        using SqliteCommand cmd = cn.CreateCommand();

        cmd.CommandText = """
            PRAGMA journal_mode=WAL;

            CREATE TABLE IF NOT EXISTS breaks (
                Id TEXT PRIMARY KEY,
                Type INTEGER NOT NULL,
                ScheduledAt TEXT NOT NULL,
                StartedAt TEXT NULL,
                EndedAt TEXT NULL,
                PlannedDurationSec REAL NOT NULL,
                Outcome INTEGER NOT NULL,
                SnoozeCount INTEGER NOT NULL,
                DateKey TEXT NOT NULL
            );
            CREATE INDEX IF NOT EXISTS ix_breaks_date ON breaks(DateKey);

            CREATE TABLE IF NOT EXISTS sessions (
                Id TEXT PRIMARY KEY,
                StartedAt TEXT NOT NULL,
                EndedAt TEXT NULL,
                ActiveTimeSec REAL NOT NULL,
                BreaksTaken INTEGER NOT NULL,
                BreaksSkipped INTEGER NOT NULL,
                DateKey TEXT NOT NULL
            );
            CREATE INDEX IF NOT EXISTS ix_sessions_date ON sessions(DateKey);

            CREATE TABLE IF NOT EXISTS app_usage (
                DateKey TEXT NOT NULL,
                ProcessName TEXT NOT NULL,
                FriendlyName TEXT NULL,
                ForegroundSec REAL NOT NULL,
                PRIMARY KEY (DateKey, ProcessName)
            );

            CREATE TABLE IF NOT EXISTS website_usage (
                DateKey TEXT NOT NULL,
                Domain TEXT NOT NULL,
                ActiveSec REAL NOT NULL,
                PRIMARY KEY (DateKey, Domain)
            );

            CREATE TABLE IF NOT EXISTS screen_score (
                DateKey TEXT PRIMARY KEY,
                Score INTEGER NOT NULL,
                BreaksTaken INTEGER NOT NULL,
                BreaksSkipped INTEGER NOT NULL,
                ActiveTimeSec REAL NOT NULL,
                LongestStretchSec REAL NOT NULL
            );

            CREATE TABLE IF NOT EXISTS planned_breaks (
                Id TEXT PRIMARY KEY,
                Name TEXT NOT NULL,
                TimeOfDay TEXT NOT NULL,
                Days TEXT NOT NULL,
                DurationSec REAL NOT NULL,
                Enabled INTEGER NOT NULL,
                CountsAwayTime INTEGER NOT NULL
            );
            """;

        cmd.ExecuteNonQuery();
    }

    internal static string DateKey(DateOnly date) => date.ToString("yyyy-MM-dd");

    internal static string Iso(DateTimeOffset dto) => dto.ToString("O");

    internal static DateTimeOffset? ParseIso(object? value) =>
        value is string s && !string.IsNullOrEmpty(s) ? DateTimeOffset.Parse(s) : null;
}
