namespace EyeYul.Dominio.Entidades;

public sealed class PuntajeVisualDiario
{
    public DateOnly Date { get; init; }

    public int Score { get; set; } = 100;

    public int BreaksTaken { get; set; }

    public int BreaksSkipped { get; set; }

    public TimeSpan ActiveTime { get; set; }

    public TimeSpan LongestStretch { get; set; }
}
