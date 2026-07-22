using EyeYul.Aplicacion.Abstracciones;
using EyeYul.Dominio.Entidades;
using EyeYul.Dominio.Enumeraciones;
using Microsoft.Data.Sqlite;

namespace EyeYul.Infraestructura.Persistencia;

public sealed class RepositorioDescanso(BaseDatosSqlite bd) : IBreakRepository
{
    public async Task AddAsync(Descanso descanso, CancellationToken ct = default)
    {
        await using SqliteConnection cn = bd.Abrir();
        using SqliteCommand cmd = cn.CreateCommand();

        cmd.CommandText = """
            INSERT INTO descansos (Id, Tipo, ProgramadoEn, IniciadoEn, FinalizadoEn, DuracionPlanificadaSeg, Resultado, ConteoAplazamientos, ClaveFecha)
            VALUES ($id, $tipo, $programado, $inicio, $fin, $duracion, $resultado, $aplazamientos, $fecha)
            ON CONFLICT(Id) DO UPDATE SET
                IniciadoEn=$inicio, FinalizadoEn=$fin, Resultado=$resultado, ConteoAplazamientos=$aplazamientos;
            """;

        cmd.Parameters.AddWithValue("$id", descanso.Id.ToString());
        cmd.Parameters.AddWithValue("$tipo", (int)descanso.Tipo);
        cmd.Parameters.AddWithValue("$programado", BaseDatosSqlite.Iso(descanso.ProgramadoEn));
        cmd.Parameters.AddWithValue("$inicio", (object?)descanso.IniciadoEn?.ToString("O") ?? DBNull.Value);
        cmd.Parameters.AddWithValue("$fin", (object?)descanso.FinalizadoEn?.ToString("O") ?? DBNull.Value);
        cmd.Parameters.AddWithValue("$duracion", descanso.DuracionPlanificada.TotalSeconds);
        cmd.Parameters.AddWithValue("$resultado", (int)descanso.Resultado);
        cmd.Parameters.AddWithValue("$aplazamientos", descanso.ConteoAplazamientos);
        cmd.Parameters.AddWithValue(
            "$fecha", BaseDatosSqlite.ClaveFecha(DateOnly.FromDateTime(descanso.ProgramadoEn.LocalDateTime)));

        await cmd.ExecuteNonQueryAsync(ct);
    }

    public async Task<IReadOnlyList<Descanso>> GetByDateAsync(DateOnly fecha, CancellationToken ct = default)
    {
        await using SqliteConnection cn = bd.Abrir();
        using SqliteCommand cmd = cn.CreateCommand();

        cmd.CommandText = """
            SELECT Id, Tipo, ProgramadoEn, IniciadoEn, FinalizadoEn, DuracionPlanificadaSeg, Resultado, ConteoAplazamientos
            FROM descansos WHERE ClaveFecha=$fecha ORDER BY ProgramadoEn
            """;

        cmd.Parameters.AddWithValue("$fecha", BaseDatosSqlite.ClaveFecha(fecha));

        var lista = new List<Descanso>();
        await using SqliteDataReader lector = await cmd.ExecuteReaderAsync(ct);
        while (await lector.ReadAsync(ct))
        {
            lista.Add(new Descanso
            {
                Id = Guid.Parse(lector.GetString(0)),
                Tipo = (TipoDescanso)lector.GetInt32(1),
                ProgramadoEn = DateTimeOffset.Parse(lector.GetString(2)),
                IniciadoEn = BaseDatosSqlite.LeerIso(lector.IsDBNull(3) ? null : lector.GetString(3)),
                FinalizadoEn = BaseDatosSqlite.LeerIso(lector.IsDBNull(4) ? null : lector.GetString(4)),
                DuracionPlanificada = TimeSpan.FromSeconds(lector.GetDouble(5)),
                Resultado = (ResultadoDescanso)lector.GetInt32(6),
                ConteoAplazamientos = lector.GetInt32(7)
            });
        }

        return lista;
    }
}
