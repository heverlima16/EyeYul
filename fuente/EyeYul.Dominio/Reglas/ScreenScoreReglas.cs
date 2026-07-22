using EyeYul.Dominio.Entidades;

namespace EyeYul.Dominio.Reglas;

public static class ScreenScoreRules
{
    public const int MaxScore = 100;

    public const int MinScore = 0;

    public const int PenaltyPerSkip = 8;

    public const int PenaltyPerLongStretch = 6;

    public const int BonusPerBreakTaken = 2;

    public static readonly TimeSpan HealthyStretch = TimeSpan.FromMinutes(50);

    public static readonly TimeSpan LongStretchWindow = TimeSpan.FromMinutes(30);

    public static int Calculate(PuntajeVisualDiario day)
    {
        int score = MaxScore;
        score -= day.BreaksSkipped * PenaltyPerSkip;
        score += Math.Min(day.BreaksTaken * BonusPerBreakTaken, 10);
        score -= LongStretchPenalty(day.LongestStretch);
        return Math.Clamp(score, MinScore, MaxScore);
    }

    public static int LongStretchPenalty(TimeSpan longestStretch)
    {
        if (longestStretch <= HealthyStretch)
        {
            return 0;
        }

        TimeSpan excess = longestStretch - HealthyStretch;
        int windows = (int)Math.Ceiling(excess / LongStretchWindow);
        return windows * PenaltyPerLongStretch;
    }

    public static string Grade(int score) => score switch
    {
        >= 90 => "Excelente",
        >= 75 => "Bien",
        >= 55 => "Regular",
        >= 35 => "Cuidado",
        _ => "En riesgo"
    };
}
