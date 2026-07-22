using EyeYul.Aplicacion.Actividad;
using Xunit;

namespace EyeYul.UnitTests;

public class MotivoPausaTests
{
    [Theory]
    [InlineData(DecisionPausa.SuprimirPantallaCompleta)]
    [InlineData(DecisionPausa.SuprimirReunion)]
    [InlineData(DecisionPausa.SuprimirMedios)]
    [InlineData(DecisionPausa.SuprimirAsistenteConcentracion)]
    [InlineData(DecisionPausa.SuprimirGrabacionPantalla)]
    [InlineData(DecisionPausa.AplazarPorAusencia)]
    public void EverySuppressionHasItsOwnMessage(DecisionPausa decision)
    {
        string texto = MotivoPausa.Describir(decision);

        Assert.False(string.IsNullOrWhiteSpace(texto));
        Assert.NotEqual(MotivoPausa.Describir(DecisionPausa.Permitir), texto);
    }

    [Fact]
    public void MessagesAreDistinctPerReason()
    {
        // Si dos motivos comparten texto, el aviso no sirve para diagnosticar.
        DecisionPausa[] supresiones =
        [
            DecisionPausa.SuprimirPantallaCompleta,
            DecisionPausa.SuprimirReunion,
            DecisionPausa.SuprimirMedios,
            DecisionPausa.SuprimirAsistenteConcentracion,
            DecisionPausa.SuprimirGrabacionPantalla,
            DecisionPausa.AplazarPorAusencia
        ];

        var textos = supresiones.Select(MotivoPausa.Describir).ToList();

        Assert.Equal(textos.Count, textos.Distinct().Count());
    }
}
