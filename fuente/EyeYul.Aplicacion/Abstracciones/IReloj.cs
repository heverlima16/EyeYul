namespace EyeYul.Aplicacion.Abstracciones;

public interface IReloj
{
    DateTimeOffset Now { get; }

    DateOnly Today => DateOnly.FromDateTime(Now.LocalDateTime);
}

public sealed class SystemClock : IReloj
{
    public DateTimeOffset Now => DateTimeOffset.Now;
}
