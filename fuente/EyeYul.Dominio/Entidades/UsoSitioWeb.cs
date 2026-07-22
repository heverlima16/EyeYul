namespace EyeYul.Dominio.Entidades;

public sealed class UsoSitioWeb
{
    public Guid Id { get; init; } = Guid.NewGuid();

    public DateOnly Fecha { get; init; }

    public string Dominio { get; init; } = string.Empty;

    public TimeSpan TiempoActivo { get; set; }

    public void Agregar(TimeSpan delta)
    {
        TiempoActivo += delta;
    }
}
