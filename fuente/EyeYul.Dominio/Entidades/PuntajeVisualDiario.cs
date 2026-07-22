namespace EyeYul.Dominio.Entidades;

public sealed class PuntajeVisualDiario
{
    public DateOnly Fecha { get; init; }

    public int Puntaje { get; set; } = 100;

    public int DescansosTomados { get; set; }

    public int DescansosOmitidos { get; set; }

    public TimeSpan TiempoActivo { get; set; }

    public TimeSpan RachaMasLarga { get; set; }
}
