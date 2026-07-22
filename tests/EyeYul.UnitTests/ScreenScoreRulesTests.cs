using EyeYul.Dominio.Entidades;
using EyeYul.Dominio.Reglas;
using Xunit;

namespace EyeYul.UnitTests;

public class ScreenScoreRulesTests
{
    [Fact]
    public void PerfectDay_StaysAt100()
    {
        var day = new PuntajeVisualDiario
        {
            BreaksTaken = 0,
            BreaksSkipped = 0,
            LongestStretch = TimeSpan.FromMinutes(30)
        };

        Assert.Equal(100, ScreenScoreRules.Calculate(day));
    }

    [Fact]
    public void SkippedBreaks_ApplyPenalty()
    {
        var day = new PuntajeVisualDiario { BreaksSkipped = 3 };

        // 100 - (3 * 8) = 76
        Assert.Equal(76, ScreenScoreRules.Calculate(day));
    }

    [Fact]
    public void TakenBreaks_GiveBonus_CappedAt10()
    {
        var day = new PuntajeVisualDiario { BreaksSkipped = 2, BreaksTaken = 100 };

        // 100 - 16 + min(200, 10) = 94
        Assert.Equal(94, ScreenScoreRules.Calculate(day));
    }

    [Fact]
    public void LongStretch_PenalizesPerWindow()
    {
        Assert.Equal(0, ScreenScoreRules.LongStretchPenalty(TimeSpan.FromMinutes(50)));
        Assert.Equal(6, ScreenScoreRules.LongStretchPenalty(TimeSpan.FromMinutes(70)));
        Assert.Equal(12, ScreenScoreRules.LongStretchPenalty(TimeSpan.FromMinutes(110)));
    }

    [Fact]
    public void Score_IsClampedToRange()
    {
        var day = new PuntajeVisualDiario
        {
            BreaksSkipped = 100,
            LongestStretch = TimeSpan.FromHours(10)
        };

        int actual = ScreenScoreRules.Calculate(day);

        Assert.InRange(actual, 0, 100);
        Assert.Equal(0, actual);
    }

    [Theory]
    [InlineData(95, "Excelente")]
    [InlineData(80, "Bien")]
    [InlineData(60, "Regular")]
    [InlineData(40, "Cuidado")]
    [InlineData(10, "En riesgo")]
    public void Grade_MapsScoreBands(int score, string expected)
    {
        Assert.Equal(expected, ScreenScoreRules.Grade(score));
    }
}
