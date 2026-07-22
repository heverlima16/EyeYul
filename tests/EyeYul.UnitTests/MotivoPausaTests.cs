using EyeYul.Aplicacion.Actividad;
using Xunit;

namespace EyeYul.UnitTests;

public class MotivoPausaTests
{
    [Theory]
    [InlineData(PauseDecision.SuppressFullscreen)]
    [InlineData(PauseDecision.SuppressMeeting)]
    [InlineData(PauseDecision.SuppressMedia)]
    [InlineData(PauseDecision.SuppressFocusAssist)]
    [InlineData(PauseDecision.SuppressScreenRecording)]
    [InlineData(PauseDecision.DeferIdle)]
    public void EverySuppressionHasItsOwnMessage(PauseDecision decision)
    {
        string texto = MotivoPausa.Describir(decision);

        Assert.False(string.IsNullOrWhiteSpace(texto));
        Assert.NotEqual(MotivoPausa.Describir(PauseDecision.Allow), texto);
    }

    [Fact]
    public void MessagesAreDistinctPerReason()
    {
        // Si dos motivos comparten texto, el aviso no sirve para diagnosticar.
        PauseDecision[] supresiones =
        [
            PauseDecision.SuppressFullscreen,
            PauseDecision.SuppressMeeting,
            PauseDecision.SuppressMedia,
            PauseDecision.SuppressFocusAssist,
            PauseDecision.SuppressScreenRecording,
            PauseDecision.DeferIdle
        ];

        var textos = supresiones.Select(MotivoPausa.Describir).ToList();

        Assert.Equal(textos.Count, textos.Distinct().Count());
    }
}
