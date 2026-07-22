using EyeYul.Aplicacion.Abstracciones;
using EyeYul.Dominio.Entidades;
using Microsoft.Data.Sqlite;

namespace EyeYul.Infraestructura.Persistencia;

public sealed class RepositorioPuntajeVisual(BaseDatosSqlite bd) : IScreenScoreRepository
{
    public async Task<PuntajeVisualDiario> GetOrCreateAsync(DateOnly fecha, CancellationToken ct = default)
    {
        await using SqliteConnection cn = bd.Abrir();
        using SqliteCommand cmd = cn.CreateCommand();

        cmd.CommandText = """
            SELECT Puntaje, DescansosTomados, DescansosOmitidos, TiempoActivoSeg, RachaMasLargaSeg
            FROM puntaje_visual WHERE ClaveFecha=$fecha
            """;

        cmd.Parameters.AddWithValue("$fecha", BaseDatosSqlite.ClaveFecha(fecha));

        await using SqliteDataReader lector = await cmd.ExecuteReaderAsync(ct);

        if (!await lector.ReadAsync(ct))
        {
            return new PuntajeVisualDiario { Fecha = fecha };
        }

        return new PuntajeVisualDiario
        {
            Fecha = fecha,
            Puntaje = lector.GetInt32(0),
            DescansosTomados = lector.GetInt32(1),
            DescansosOmitidos = lector.GetInt32(2),
            TiempoActivo = TimeSpan.FromSeconds(lector.GetDouble(3)),
            RachaMasLarga = TimeSpan.FromSeconds(lector.GetDouble(4))
        };
    }

    public async Task SaveAsync(PuntajeVisualDiario puntaje, CancellationToken ct = default)
    {
        await using SqliteConnection cn = bd.Abrir();
        using SqliteCommand cmd = cn.CreateCommand();

        cmd.CommandText = """
            INSERT INTO puntaje_visual (ClaveFecha, Puntaje, DescansosTomados, DescansosOmitidos, TiempoActivoSeg, RachaMasLargaSeg)
            VALUES ($fecha, $puntaje, $tomados, $omitidos, $activo, $racha)
            ON CONFLICT(ClaveFecha) DO UPDATE SET
                Puntaje=$puntaje, DescansosTomados=$tomados, DescansosOmitidos=$omitidos,
                TiempoActivoSeg=$activo, RachaMasLargaSeg=$racha;
            """;

        cmd.Parameters.AddWithValue("$fecha", BaseDatosSqlite.ClaveFecha(puntaje.Fecha));
        cmd.Parameters.AddWithValue("$puntaje", puntaje.Puntaje);
        cmd.Parameters.AddWithValue("$tomados", puntaje.DescansosTomados);
        cmd.Parameters.AddWithValue("$omitidos", puntaje.DescansosOmitidos);
        cmd.Parameters.AddWithValue("$activo", puntaje.TiempoActivo.TotalSeconds);
        cmd.Parameters.AddWithValue("$racha", puntaje.RachaMasLarga.TotalSeconds);

        await cmd.ExecuteNonQueryAsync(ct);
    }

    public async Task<IReadOnlyList<PuntajeVisualDiario>> GetRangeAsync(
        DateOnly desde, DateOnly hasta, CancellationToken ct = default)
    {
        await using SqliteConnection cn = bd.Abrir();
        using SqliteCommand cmd = cn.CreateCommand();

        cmd.CommandText = """
            SELECT ClaveFecha, Puntaje, DescansosTomados, DescansosOmitidos, TiempoActivoSeg, RachaMasLargaSeg
            FROM puntaje_visual WHERE ClaveFecha BETWEEN $desde AND $hasta ORDER BY ClaveFecha
            """;

        cmd.Parameters.AddWithValue("$desde", BaseDatosSqlite.ClaveFecha(desde));
        cmd.Parameters.AddWithValue("$hasta", BaseDatosSqlite.ClaveFecha(hasta));

        var lista = new List<PuntajeVisualDiario>();
        await using SqliteDataReader lector = await cmd.ExecuteReaderAsync(ct);
        while (await lector.ReadAsync(ct))
        {
            lista.Add(new PuntajeVisualDiario
            {
                Fecha = DateOnly.ParseExact(lector.GetString(0), "yyyy-MM-dd"),
                Puntaje = lector.GetInt32(1),
                DescansosTomados = lector.GetInt32(2),
                DescansosOmitidos = lector.GetInt32(3),
                TiempoActivo = TimeSpan.FromSeconds(lector.GetDouble(4)),
                RachaMasLarga = TimeSpan.FromSeconds(lector.GetDouble(5))
            });
        }

        return lista;
    }
}
