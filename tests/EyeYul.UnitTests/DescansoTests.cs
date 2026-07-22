using EyeYul.Dominio.Entidades;
using EyeYul.Dominio.Enumeraciones;
using Xunit;

namespace EyeYul.UnitTests;

public class DescansoTests
{
    private static readonly DateTimeOffset Inicio = new(2026, 7, 6, 9, 0, 0, TimeSpan.Zero);

    private static Descanso NuevoDescanso() => new()
    {
        Tipo = TipoDescanso.Intervalo,
        ProgramadoEn = Inicio,
        DuracionPlanificada = TimeSpan.FromSeconds(20)
    };

    [Fact]
    public void NewBreak_StartsPending()
    {
        Descanso d = NuevoDescanso();

        Assert.Equal(ResultadoDescanso.Pendiente, d.Resultado);
        Assert.False(d.FueRespetado);
        Assert.Equal(0, d.ConteoAplazamientos);
    }

    [Fact]
    public void ActualDuration_IsNull_WhenNotStarted()
    {
        Descanso d = NuevoDescanso();

        Assert.Null(d.DuracionReal);
    }

    [Fact]
    public void ActualDuration_IsNull_WhenStartedButNotEnded()
    {
        Descanso d = NuevoDescanso();
        d.MarcarIniciado(Inicio);

        Assert.Null(d.DuracionReal);
    }

    [Fact]
    public void ActualDuration_IsElapsed_WhenStartedAndEnded()
    {
        Descanso d = NuevoDescanso();
        d.MarcarIniciado(Inicio);
        d.MarcarCompletado(Inicio.AddSeconds(25));

        Assert.Equal(TimeSpan.FromSeconds(25), d.DuracionReal);
    }

    [Fact]
    public void MarkCompleted_IsTheOnlyRespectedOutcome()
    {
        Descanso completado = NuevoDescanso();
        completado.MarcarCompletado(Inicio);

        Descanso omitido = NuevoDescanso();
        omitido.MarcarOmitido(Inicio);

        Descanso suprimido = NuevoDescanso();
        suprimido.MarcarSuprimido();

        Assert.True(completado.FueRespetado);
        Assert.False(omitido.FueRespetado);
        Assert.False(suprimido.FueRespetado);
    }

    [Fact]
    public void MarkStarted_ResetsOutcomeToPending()
    {
        Descanso d = NuevoDescanso();
        d.MarcarAplazado();

        d.MarcarIniciado(Inicio);

        Assert.Equal(ResultadoDescanso.Pendiente, d.Resultado);
    }

    [Fact]
    public void MarkSnoozed_AccumulatesCount()
    {
        Descanso d = NuevoDescanso();

        d.MarcarAplazado();
        d.MarcarAplazado();

        Assert.Equal(2, d.ConteoAplazamientos);
        Assert.Equal(ResultadoDescanso.Aplazado, d.Resultado);
    }

    [Fact]
    public void MarkSuppressed_DoesNotSetEndedAt()
    {
        // Una pausa suprimida nunca llego a mostrarse: no tiene inicio ni fin.
        Descanso d = NuevoDescanso();

        d.MarcarSuprimido();

        Assert.Equal(ResultadoDescanso.Suprimido, d.Resultado);
        Assert.Null(d.IniciadoEn);
        Assert.Null(d.FinalizadoEn);
    }
}
