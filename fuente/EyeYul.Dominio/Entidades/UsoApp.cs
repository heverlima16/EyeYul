namespace EyeYul.Dominio.Entidades;

public sealed class UsoApp
{
    public Guid Id { get; init; } = Guid.NewGuid();

    public DateOnly Date { get; init; }

    public string ProcessName { get; init; } = string.Empty;

    public string? FriendlyName { get; set; }

    public TimeSpan Foreground { get; set; }

    public void Add(TimeSpan delta)
    {
        Foreground += delta;
    }
}
