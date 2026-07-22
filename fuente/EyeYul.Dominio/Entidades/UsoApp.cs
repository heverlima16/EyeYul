namespace EyeYul.Dominio.Entidades;

public sealed class UsoApp
{
    public Guid Id { get; init; } = Guid.NewGuid();

    public DateOnly Fecha { get; init; }

    public string NombreProceso { get; init; } = string.Empty;

    public string? NombreAmigable { get; set; }

    public TimeSpan PrimerPlano { get; set; }

    public void Agregar(TimeSpan delta)
    {
        PrimerPlano += delta;
    }
}
