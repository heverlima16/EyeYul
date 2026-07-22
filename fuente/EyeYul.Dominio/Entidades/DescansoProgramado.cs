namespace EyeYul.Dominio.Entidades;

public sealed class DescansoProgramado
{
    public Guid Id { get; init; } = Guid.NewGuid();

    public string Nombre { get; set; } = "Pausa planificada";

    public TimeOnly HoraDelDia { get; set; }

    public HashSet<DayOfWeek> Dias { get; init; } = new();

    public TimeSpan Duracion { get; set; } = TimeSpan.FromMinutes(5);

    public bool Habilitado { get; set; } = true;

    public bool CuentaTiempoAusente { get; set; } = true;

    public bool OcurreEn(DayOfWeek dia) => Habilitado && Dias.Contains(dia);

    public DateTimeOffset? ProximaOcurrencia(DateTimeOffset desde)
    {
        if (!Habilitado || Dias.Count == 0)
        {
            return null;
        }

        for (int i = 0; i < 8; i++)
        {
            DateTime dia = desde.Date.AddDays(i);
            if (!Dias.Contains(dia.DayOfWeek))
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
