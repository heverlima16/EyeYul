namespace EyeYul.Dominio.Entidades;

public sealed class UsoSitioWeb
{
    public Guid Id { get; init; } = Guid.NewGuid();

    public DateOnly Date { get; init; }

    public string Domain { get; init; } = string.Empty;

    public TimeSpan ActiveTime { get; set; }

    public void Add(TimeSpan delta)
    {
        ActiveTime += delta;
    }
}
