using EyeYul.Dominio.ObjetosValor;
using Xunit;

namespace EyeYul.UnitTests;

public class DuracionPausaTests
{
    [Fact]
    public void PresetsHaveExpectedValues()
    {
        Assert.Equal(TimeSpan.FromMinutes(1), DuracionPausa.OneMinute.Value);
        Assert.Equal(TimeSpan.FromMinutes(5), DuracionPausa.FiveMinutes.Value);
        Assert.Equal(TimeSpan.FromMinutes(15), DuracionPausa.FifteenMinutes.Value);
    }

    [Theory]
    [InlineData(1, "+1")]
    [InlineData(5, "+5")]
    [InlineData(15, "+15")]
    public void ToString_RendersSnoozeLabel(int minutos, string esperado)
    {
        Assert.Equal(esperado, DuracionPausa.FromMinutes(minutos).ToString());
    }

    [Fact]
    public void FromMinutes_EqualsMatchingPreset()
    {
        // Es un record struct: la igualdad es por valor.
        Assert.Equal(DuracionPausa.FiveMinutes, DuracionPausa.FromMinutes(5));
        Assert.NotEqual(DuracionPausa.FiveMinutes, DuracionPausa.FromMinutes(6));
    }
}
