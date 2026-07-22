namespace EyeYul.Dominio.Entidades;

public sealed class DescansoProgramado
{
    public Guid Id { get; init; } = Guid.NewGuid();

    public string Name { get; set; } = "Pausa planificada";

    public TimeOnly TimeOfDay { get; set; }

    public HashSet<DayOfWeek> Days { get; init; } = new();

    public TimeSpan Duration { get; set; } = TimeSpan.FromMinutes(5);

    public bool Enabled { get; set; } = true;

    public bool CountsAwayTime { get; set; } = true;

    public bool OccursOn(DayOfWeek day) => Enabled && Days.Contains(day);

    public DateTimeOffset? NextOccurrence(DateTimeOffset from)
    {
        if (!Enabled || Days.Count == 0)
        {
            return null;
        }

        for (int i = 0; i < 8; i++)
        {
            DateTime day = from.Date.AddDays(i);
            if (!Days.Contains(day.DayOfWeek))
            {
                continue;
            }

            var candidate = new DateTimeOffset(
                day.Year, day.Month, day.Day, TimeOfDay.Hour, TimeOfDay.Minute, 0, from.Offset);

            if (candidate > from)
            {
                return candidate;
            }
        }

        return null;
    }
}
