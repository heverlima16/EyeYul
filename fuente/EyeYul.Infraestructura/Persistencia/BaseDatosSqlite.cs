using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;

namespace EyeYul.Infraestructura.Persistencia;

public sealed class BaseDatosSqlite
{
    private readonly string _cadenaConexion;

    public BaseDatosSqlite(ILogger<BaseDatosSqlite> registro)
    {
        string carpeta = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "EyeYul");
        Directory.CreateDirectory(carpeta);

        string rutaBd = Path.Combine(carpeta, "eyeyul.db");

        _cadenaConexion = new SqliteConnectionStringBuilder
        {
            DataSource = rutaBd,
            Mode = SqliteOpenMode.ReadWriteCreate
        }.ToString();

        registro.LogInformation("Base de datos en {Ruta}", rutaBd);
        Inicializar(registro);
    }

    public SqliteConnection Abrir()
    {
        var cn = new SqliteConnection(_cadenaConexion);
        cn.Open();
        return cn;
    }

    private void Inicializar(ILogger<BaseDatosSqlite> registro)
    {
        using SqliteConnection cn = Abrir();

        Ejecutar(cn, "PRAGMA journal_mode=WAL;");

        // El esquema nacio en ingles. Se renombra en caliente antes de crear nada,
        // porque un CREATE TABLE IF NOT EXISTS previo dejaria la tabla nueva vacia
        // y el historial viejo huerfano.
        MigrarEsquemaIngles(cn, registro);
        CrearEsquema(cn);
    }

    private static void CrearEsquema(SqliteConnection cn) => Ejecutar(cn, """
        CREATE TABLE IF NOT EXISTS descansos (
            Id TEXT PRIMARY KEY,
            Tipo INTEGER NOT NULL,
            ProgramadoEn TEXT NOT NULL,
            IniciadoEn TEXT NULL,
            FinalizadoEn TEXT NULL,
            DuracionPlanificadaSeg REAL NOT NULL,
            Resultado INTEGER NOT NULL,
            ConteoAplazamientos INTEGER NOT NULL,
            ClaveFecha TEXT NOT NULL
        );
        CREATE INDEX IF NOT EXISTS ix_descansos_fecha ON descansos(ClaveFecha);

        CREATE TABLE IF NOT EXISTS sesiones (
            Id TEXT PRIMARY KEY,
            IniciadaEn TEXT NOT NULL,
            FinalizadaEn TEXT NULL,
            TiempoActivoSeg REAL NOT NULL,
            DescansosTomados INTEGER NOT NULL,
            DescansosOmitidos INTEGER NOT NULL,
            ClaveFecha TEXT NOT NULL
        );
        CREATE INDEX IF NOT EXISTS ix_sesiones_fecha ON sesiones(ClaveFecha);

        CREATE TABLE IF NOT EXISTS uso_apps (
            ClaveFecha TEXT NOT NULL,
            NombreProceso TEXT NOT NULL,
            NombreAmigable TEXT NULL,
            PrimerPlanoSeg REAL NOT NULL,
            PRIMARY KEY (ClaveFecha, NombreProceso)
        );

        CREATE TABLE IF NOT EXISTS uso_sitios_web (
            ClaveFecha TEXT NOT NULL,
            Dominio TEXT NOT NULL,
            TiempoActivoSeg REAL NOT NULL,
            PRIMARY KEY (ClaveFecha, Dominio)
        );

        CREATE TABLE IF NOT EXISTS puntaje_visual (
            ClaveFecha TEXT PRIMARY KEY,
            Puntaje INTEGER NOT NULL,
            DescansosTomados INTEGER NOT NULL,
            DescansosOmitidos INTEGER NOT NULL,
            TiempoActivoSeg REAL NOT NULL,
            RachaMasLargaSeg REAL NOT NULL
        );

        CREATE TABLE IF NOT EXISTS descansos_programados (
            Id TEXT PRIMARY KEY,
            Nombre TEXT NOT NULL,
            HoraDelDia TEXT NOT NULL,
            Dias TEXT NOT NULL,
            DuracionSeg REAL NOT NULL,
            Habilitado INTEGER NOT NULL,
            CuentaTiempoAusente INTEGER NOT NULL
        );
        """);

    /// <summary>Tabla vieja, tabla nueva y los pares de columna (vieja, nueva) a renombrar.</summary>
    private static readonly (string Vieja, string Nueva, (string Vieja, string Nueva)[] Columnas)[] Renombres =
    [
        ("breaks", "descansos",
        [
            ("Type", "Tipo"),
            ("ScheduledAt", "ProgramadoEn"),
            ("StartedAt", "IniciadoEn"),
            ("EndedAt", "FinalizadoEn"),
            ("PlannedDurationSec", "DuracionPlanificadaSeg"),
            ("Outcome", "Resultado"),
            ("SnoozeCount", "ConteoAplazamientos"),
            ("DateKey", "ClaveFecha")
        ]),
        ("sessions", "sesiones",
        [
            ("StartedAt", "IniciadaEn"),
            ("EndedAt", "FinalizadaEn"),
            ("ActiveTimeSec", "TiempoActivoSeg"),
            ("BreaksTaken", "DescansosTomados"),
            ("BreaksSkipped", "DescansosOmitidos"),
            ("DateKey", "ClaveFecha")
        ]),
        ("app_usage", "uso_apps",
        [
            ("DateKey", "ClaveFecha"),
            ("ProcessName", "NombreProceso"),
            ("FriendlyName", "NombreAmigable"),
            ("ForegroundSec", "PrimerPlanoSeg")
        ]),
        ("website_usage", "uso_sitios_web",
        [
            ("DateKey", "ClaveFecha"),
            ("Domain", "Dominio"),
            ("ActiveSec", "TiempoActivoSeg")
        ]),
        ("screen_score", "puntaje_visual",
        [
            ("DateKey", "ClaveFecha"),
            ("Score", "Puntaje"),
            ("BreaksTaken", "DescansosTomados"),
            ("BreaksSkipped", "DescansosOmitidos"),
            ("ActiveTimeSec", "TiempoActivoSeg"),
            ("LongestStretchSec", "RachaMasLargaSeg")
        ]),
        ("planned_breaks", "descansos_programados",
        [
            ("Name", "Nombre"),
            ("TimeOfDay", "HoraDelDia"),
            ("Days", "Dias"),
            ("DurationSec", "DuracionSeg"),
            ("Enabled", "Habilitado"),
            ("CountsAwayTime", "CuentaTiempoAusente")
        ])
    ];

    private static void MigrarEsquemaIngles(SqliteConnection cn, ILogger<BaseDatosSqlite> registro)
    {
        foreach ((string vieja, string nueva, (string Vieja, string Nueva)[] columnas) in Renombres)
        {
            // Si la tabla nueva ya existe no hay nada que migrar; y si ambas
            // existieran, la vieja es un resto que no se toca para no perder datos.
            if (!ExisteTabla(cn, vieja) || ExisteTabla(cn, nueva))
            {
                continue;
            }

            Ejecutar(cn, $"ALTER TABLE {vieja} RENAME TO {nueva};");

            foreach ((string columnaVieja, string columnaNueva) in columnas)
            {
                if (ExisteColumna(cn, nueva, columnaVieja))
                {
                    Ejecutar(cn, $"ALTER TABLE {nueva} RENAME COLUMN {columnaVieja} TO {columnaNueva};");
                }
            }

            registro.LogInformation("Tabla {Vieja} migrada a {Nueva}", vieja, nueva);
        }

        // Los indices sobreviven al renombre de la tabla conservando su nombre viejo.
        Ejecutar(cn, "DROP INDEX IF EXISTS ix_breaks_date; DROP INDEX IF EXISTS ix_sessions_date;");
    }

    private static bool ExisteTabla(SqliteConnection cn, string nombre)
    {
        using SqliteCommand cmd = cn.CreateCommand();
        cmd.CommandText = "SELECT 1 FROM sqlite_master WHERE type='table' AND name=$nombre;";
        cmd.Parameters.AddWithValue("$nombre", nombre);
        return cmd.ExecuteScalar() is not null;
    }

    private static bool ExisteColumna(SqliteConnection cn, string tabla, string columna)
    {
        using SqliteCommand cmd = cn.CreateCommand();
        cmd.CommandText = $"SELECT 1 FROM pragma_table_info('{tabla}') WHERE name=$columna;";
        cmd.Parameters.AddWithValue("$columna", columna);
        return cmd.ExecuteScalar() is not null;
    }

    private static void Ejecutar(SqliteConnection cn, string sql)
    {
        using SqliteCommand cmd = cn.CreateCommand();
        cmd.CommandText = sql;
        cmd.ExecuteNonQuery();
    }

    internal static string ClaveFecha(DateOnly fecha) => fecha.ToString("yyyy-MM-dd");

    internal static string Iso(DateTimeOffset momento) => momento.ToString("O");

    internal static DateTimeOffset? LeerIso(object? valor) =>
        valor is string s && !string.IsNullOrEmpty(s) ? DateTimeOffset.Parse(s) : null;
}
