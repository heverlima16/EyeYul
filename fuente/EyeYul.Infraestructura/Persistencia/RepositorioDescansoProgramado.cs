using EyeYul.Aplicacion.Abstracciones;
using EyeYul.Dominio.Entidades;
using Microsoft.Data.Sqlite;

namespace EyeYul.Infraestructura.Persistencia;

public sealed class RepositorioDescansoProgramado(BaseDatosSqlite bd) : IPlannedBreakRepository
{
    public async Task<IReadOnlyList<DescansoProgramado>> GetAllAsync(CancellationToken ct = default)
    {
        await using SqliteConnection cn = bd.Abrir();
        using SqliteCommand cmd = cn.CreateCommand();

        cmd.CommandText = """
            SELECT Id, Nombre, HoraDelDia, Dias, DuracionSeg, Habilitado, CuentaTiempoAusente,
                   Recurrencia, DiaDelMes, FechaUnica
            FROM descansos_programados
            """;

        var lista = new List<DescansoProgramado>();
        await using SqliteDataReader lector = await cmd.ExecuteReaderAsync(ct);
        while (await lector.ReadAsync(ct))
        {
            var programado = new DescansoProgramado
            {
                Id = Guid.Parse(lector.GetString(0)),
                Nombre = lector.GetString(1),
                HoraDelDia = TimeOnly.Parse(lector.GetString(2)),
                Duracion = TimeSpan.FromSeconds(lector.GetDouble(4)),
                Habilitado = lector.GetInt32(5) != 0,
                CuentaTiempoAusente = lector.GetInt32(6) != 0,
                Recurrencia = (TipoRecurrencia)lector.GetInt32(7),
                DiaDelMes = lector.GetInt32(8),
                FechaUnica = lector.IsDBNull(9) ? null : DateOnly.Parse(lector.GetString(9))
            };

            foreach (DayOfWeek dia in LeerDias(lector.GetString(3)))
            {
                programado.Dias.Add(dia);
            }

            lista.Add(programado);
        }

        return lista;
    }

    public async Task UpsertAsync(DescansoProgramado programado, CancellationToken ct = default)
    {
        await using SqliteConnection cn = bd.Abrir();
        using SqliteCommand cmd = cn.CreateCommand();

        cmd.CommandText = """
            INSERT INTO descansos_programados
                (Id, Nombre, HoraDelDia, Dias, DuracionSeg, Habilitado, CuentaTiempoAusente, Recurrencia, DiaDelMes, FechaUnica)
            VALUES ($id, $nombre, $hora, $dias, $duracion, $habilitado, $ausente, $recurrencia, $diaDelMes, $fechaUnica)
            ON CONFLICT(Id) DO UPDATE SET
                Nombre=$nombre, HoraDelDia=$hora, Dias=$dias, DuracionSeg=$duracion,
                Habilitado=$habilitado, CuentaTiempoAusente=$ausente, Recurrencia=$recurrencia,
                DiaDelMes=$diaDelMes, FechaUnica=$fechaUnica;
            """;

        cmd.Parameters.AddWithValue("$id", programado.Id.ToString());
        cmd.Parameters.AddWithValue("$nombre", programado.Nombre);
        cmd.Parameters.AddWithValue("$hora", programado.HoraDelDia.ToString("HH:mm"));
        cmd.Parameters.AddWithValue("$dias", string.Join(',', programado.Dias.Select(d => (int)d)));
        cmd.Parameters.AddWithValue("$duracion", programado.Duracion.TotalSeconds);
        cmd.Parameters.AddWithValue("$habilitado", programado.Habilitado ? 1 : 0);
        cmd.Parameters.AddWithValue("$ausente", programado.CuentaTiempoAusente ? 1 : 0);
        cmd.Parameters.AddWithValue("$recurrencia", (int)programado.Recurrencia);
        cmd.Parameters.AddWithValue("$diaDelMes", programado.DiaDelMes);
        cmd.Parameters.AddWithValue(
            "$fechaUnica", (object?)programado.FechaUnica?.ToString("yyyy-MM-dd") ?? DBNull.Value);

        await cmd.ExecuteNonQueryAsync(ct);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        await using SqliteConnection cn = bd.Abrir();
        using SqliteCommand cmd = cn.CreateCommand();

        cmd.CommandText = "DELETE FROM descansos_programados WHERE Id=$id";
        cmd.Parameters.AddWithValue("$id", id.ToString());

        await cmd.ExecuteNonQueryAsync(ct);
    }

    private static IEnumerable<DayOfWeek> LeerDias(string csv) =>
        csv.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
           .Select(s => (DayOfWeek)int.Parse(s));
}
