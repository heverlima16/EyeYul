namespace EyeYul.Dominio.Entidades;

/// <summary>Valor persistido en `descansos_programados.Recurrencia`: nunca reordenar, solo se pueden renombrar los miembros.</summary>
public enum TipoRecurrencia
{
    Diario = 0,
    Semanal = 1,
    Mensual = 2,
    UnaVez = 3
}

public sealed class DescansoProgramado
{
    public Guid Id { get; init; } = Guid.NewGuid();

    public string Nombre { get; set; } = "Pausa planificada";

    public TipoRecurrencia Recurrencia { get; set; } = TipoRecurrencia.Semanal;

    public TimeOnly HoraDelDia { get; set; }

    /// <summary>Usado solo cuando <see cref="Recurrencia"/> es <see cref="TipoRecurrencia.Semanal"/>.</summary>
    public HashSet<DayOfWeek> Dias { get; init; } = new();

    /// <summary>Usado solo cuando <see cref="Recurrencia"/> es <see cref="TipoRecurrencia.Mensual"/> (1-31).</summary>
    public int DiaDelMes { get; set; } = 1;

    /// <summary>Usado solo cuando <see cref="Recurrencia"/> es <see cref="TipoRecurrencia.UnaVez"/>.</summary>
    public DateOnly? FechaUnica { get; set; }

    public TimeSpan Duracion { get; set; } = TimeSpan.FromMinutes(5);

    public bool Habilitado { get; set; } = true;

    public bool CuentaTiempoAusente { get; set; } = true;

    public bool OcurreEn(DateOnly fecha)
    {
        if (!Habilitado)
        {
            return false;
        }

        return Recurrencia switch
        {
            TipoRecurrencia.Diario => true,
            TipoRecurrencia.Semanal => Dias.Contains(fecha.DayOfWeek),
            // Clamp al ultimo dia del mes: un "31" en febrero cae el 28 (o 29).
            TipoRecurrencia.Mensual => fecha.Day == Math.Min(DiaDelMes, DateTime.DaysInMonth(fecha.Year, fecha.Month)),
            TipoRecurrencia.UnaVez => FechaUnica == fecha,
            _ => false
        };
    }

    public DateTimeOffset? ProximaOcurrencia(DateTimeOffset desde)
    {
        if (!Habilitado)
        {
            return null;
        }

        if (Recurrencia == TipoRecurrencia.Semanal && Dias.Count == 0)
        {
            return null;
        }

        // El mensual necesita mirar hasta ~5 semanas adelante para encontrar el dia del mes.
        int horizonteDias = Recurrencia == TipoRecurrencia.Mensual ? 35 : 8;

        for (int i = 0; i < horizonteDias; i++)
        {
            DateTime dia = desde.Date.AddDays(i);
            var fecha = DateOnly.FromDateTime(dia);

            if (!OcurreEn(fecha))
            {
                continue;
            }

            var candidata = new DateTimeOffset(
                dia.Year, dia.Month, dia.Day, HoraDelDia.Hour, HoraDelDia.Minute, 0, desde.Offset);

            if (candidata > desde)
            {
                return candidata;
            }
        }

        return null;
    }
}
