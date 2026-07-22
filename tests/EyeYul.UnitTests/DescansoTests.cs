using EyeYul.Dominio.Entidades;
using EyeYul.Dominio.Enumeraciones;
using Xunit;

namespace EyeYul.UnitTests;

public class DescansoTests
{
    private static readonly DateTimeOffset Inicio = new(2026, 7, 6, 9, 0, 0, TimeSpan.Zero);

    private static Descanso NuevoDescanso() => new()
    {
        Type = TipoDescanso.Interval,
        ScheduledAt = Inicio,
        PlannedDuration = TimeSpan.FromSeconds(20)
    };

    [Fact]
    public void NewBreak_StartsPending()
    {
        Descanso d = NuevoDescanso();

        Assert.Equal(ResultadoDescanso.Pending, d.Outcome);
        Assert.False(d.WasRespected);
        Assert.Equal(0, d.SnoozeCount);
    }

    [Fact]
    public void ActualDuration_IsNull_WhenNotStarted()
    {
        Descanso d = NuevoDescanso();

        Assert.Null(d.ActualDuration);
    }

    [Fact]
    public void ActualDuration_IsNull_WhenStartedButNotEnded()
    {
        Descanso d = NuevoDescanso();
        d.MarkStarted(Inicio);

        Assert.Null(d.ActualDuration);
    }

    [Fact]
    public void ActualDuration_IsElapsed_WhenStartedAndEnded()
    {
        Descanso d = NuevoDescanso();
        d.MarkStarted(Inicio);
        d.MarkCompleted(Inicio.AddSeconds(25));

        Assert.Equal(TimeSpan.FromSeconds(25), d.ActualDuration);
    }

    [Fact]
    public void MarkCompleted_IsTheOnlyRespectedOutcome()
    {
        Descanso completado = NuevoDescanso();
        completado.MarkCompleted(Inicio);

        Descanso omitido = NuevoDescanso();
        omitido.MarkSkipped(Inicio);

        Descanso suprimido = NuevoDescanso();
        suprimido.MarkSuppressed();

        Assert.True(completado.WasRespected);
        Assert.False(omitido.WasRespected);
        Assert.False(suprimido.WasRespected);
    }

    [Fact]
    public void MarkStarted_ResetsOutcomeToPending()
    {
        Descanso d = NuevoDescanso();
        d.MarkSnoozed();

        d.MarkStarted(Inicio);

        Assert.Equal(ResultadoDescanso.Pending, d.Outcome);
    }

    [Fact]
    public void MarkSnoozed_AccumulatesCount()
    {
        Descanso d = NuevoDescanso();

        d.MarkSnoozed();
        d.MarkSnoozed();

        Assert.Equal(2, d.SnoozeCount);
        Assert.Equal(ResultadoDescanso.Snoozed, d.Outcome);
    }

    [Fact]
    public void MarkSuppressed_DoesNotSetEndedAt()
    {
        // Una pausa suprimida nunca llego a mostrarse: no tiene inicio ni fin.
        Descanso d = NuevoDescanso();

        d.MarkSuppressed();

        Assert.Equal(ResultadoDescanso.Suppressed, d.Outcome);
        Assert.Null(d.StartedAt);
        Assert.Null(d.EndedAt);
    }
}
