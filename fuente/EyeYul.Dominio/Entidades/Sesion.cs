namespace EyeYul.Dominio.Entidades;

public sealed class Sesion
{
    public Guid Id { get; init; } = Guid.NewGuid();

    public DateTimeOffset IniciadaEn { get; init; }

    public DateTimeOffset? FinalizadaEn { get; set; }

    public TimeSpan TiempoActivo { get; set; }

    public int DescansosTomados { get; set; }

    public int DescansosOmitidos { get; set; }

    public bool EstaAbierta => !FinalizadaEn.HasValue;

    public TimeSpan Duracion => (FinalizadaEn ?? DateTimeOffset.Now) - IniciadaEn;

    public void Cerrar(DateTimeOffset cuando)
    {
        FinalizadaEn = cuando;
    }
}
