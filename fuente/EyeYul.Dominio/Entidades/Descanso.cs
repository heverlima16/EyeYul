using EyeYul.Dominio.Enumeraciones;

namespace EyeYul.Dominio.Entidades;

public sealed class Descanso
{
    public Guid Id { get; init; } = Guid.NewGuid();

    public TipoDescanso Type { get; init; }

    public DateTimeOffset ScheduledAt { get; init; }

    public DateTimeOffset? StartedAt { get; set; }

    public DateTimeOffset? EndedAt { get; set; }

    public TimeSpan PlannedDuration { get; init; }

    public ResultadoDescanso Outcome { get; set; } = ResultadoDescanso.Pending;

    public int SnoozeCount { get; set; }

    public TimeSpan? ActualDuration =>
        StartedAt is { } started && EndedAt is { } ended ? ended - started : null;

    public bool WasRespected => Outcome == ResultadoDescanso.Completed;

    public void MarkStarted(DateTimeOffset when)
    {
        StartedAt = when;
        Outcome = ResultadoDescanso.Pending;
    }

    public void MarkCompleted(DateTimeOffset when)
    {
        EndedAt = when;
        Outcome = ResultadoDescanso.Completed;
    }

    public void MarkSkipped(DateTimeOffset when)
    {
        EndedAt = when;
        Outcome = ResultadoDescanso.Skipped;
    }

    public void MarkSuppressed()
    {
        Outcome = ResultadoDescanso.Suppressed;
    }

    public void MarkSnoozed()
    {
        SnoozeCount++;
        Outcome = ResultadoDescanso.Snoozed;
    }
}
