using EyeYul.Dominio.Entidades;

namespace EyeYul.Dominio.Reglas;

public static class ReglasPuntajeVisual
{
    public const int PuntajeMaximo = 100;

    public const int PuntajeMinimo = 0;

    public const int PenalizacionPorOmision = 8;

    public const int PenalizacionPorRachaLarga = 6;

    public const int BonoPorDescansoTomado = 2;

    public static readonly TimeSpan RachaSaludable = TimeSpan.FromMinutes(50);

    public static readonly TimeSpan VentanaRachaLarga = TimeSpan.FromMinutes(30);

    public static int Calcular(PuntajeVisualDiario dia)
    {
        int puntaje = PuntajeMaximo;
        puntaje -= dia.DescansosOmitidos * PenalizacionPorOmision;
        puntaje += Math.Min(dia.DescansosTomados * BonoPorDescansoTomado, 10);
        puntaje -= PenalizacionRachaLarga(dia.RachaMasLarga);
        return Math.Clamp(puntaje, PuntajeMinimo, PuntajeMaximo);
    }

    public static int PenalizacionRachaLarga(TimeSpan rachaMasLarga)
    {
        if (rachaMasLarga <= RachaSaludable)
        {
            return 0;
        }

        TimeSpan exceso = rachaMasLarga - RachaSaludable;
        int ventanas = (int)Math.Ceiling(exceso / VentanaRachaLarga);
        return ventanas * PenalizacionPorRachaLarga;
    }

    public static string Calificacion(int puntaje) => puntaje switch
    {
        >= 90 => "Excelente",
        >= 75 => "Bien",
        >= 55 => "Regular",
        >= 35 => "Cuidado",
        _ => "En riesgo"
    };
}
