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
            DescansosTomados = 0,
            DescansosOmitidos = 0,
            RachaMasLarga = TimeSpan.FromMinutes(30)
        };

        Assert.Equal(100, ReglasPuntajeVisual.Calcular(day));
    }

    [Fact]
    public void SkippedBreaks_ApplyPenalty()
    {
        var day = new PuntajeVisualDiario { DescansosOmitidos = 3 };

        // 100 - (3 * 8) = 76
        Assert.Equal(76, ReglasPuntajeVisual.Calcular(day));
    }

    [Fact]
    public void TakenBreaks_GiveBonus_CappedAt10()
    {
        var day = new PuntajeVisualDiario { DescansosOmitidos = 2, DescansosTomados = 100 };

        // 100 - 16 + min(200, 10) = 94
        Assert.Equal(94, ReglasPuntajeVisual.Calcular(day));
    }

    [Fact]
    public void LongStretch_PenalizesPerWindow()
    {
        Assert.Equal(0, ReglasPuntajeVisual.PenalizacionRachaLarga(TimeSpan.FromMinutes(50)));
        Assert.Equal(6, ReglasPuntajeVisual.PenalizacionRachaLarga(TimeSpan.FromMinutes(70)));
        Assert.Equal(12, ReglasPuntajeVisual.PenalizacionRachaLarga(TimeSpan.FromMinutes(110)));
    }

    [Fact]
    public void Score_IsClampedToRange()
    {
        var day = new PuntajeVisualDiario
        {
            DescansosOmitidos = 100,
            RachaMasLarga = TimeSpan.FromHours(10)
        };

        int actual = ReglasPuntajeVisual.Calcular(day);

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
        Assert.Equal(expected, ReglasPuntajeVisual.Calificacion(score));
    }
}
