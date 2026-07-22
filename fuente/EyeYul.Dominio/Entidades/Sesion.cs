namespace EyeYul.Dominio.Entidades;

public sealed class Sesion
{
    public Guid Id { get; init; } = Guid.NewGuid();

    public DateTimeOffset StartedAt { get; init; }

    public DateTimeOffset? EndedAt { get; set; }

    public TimeSpan ActiveTime { get; set; }

    public int BreaksTaken { get; set; }

    public int BreaksSkipped { get; set; }

    public bool IsOpen => !EndedAt.HasValue;

    public TimeSpan Duration => (EndedAt ?? DateTimeOffset.Now) - StartedAt;

    public void Close(DateTimeOffset when)
    {
        EndedAt = when;
    }
}
