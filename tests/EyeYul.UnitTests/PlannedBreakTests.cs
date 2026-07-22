using EyeYul.Dominio.Entidades;
using Xunit;

namespace EyeYul.UnitTests;

public class PlannedBreakTests
{
    [Fact]
    public void NextOccurrence_SameDay_LaterTime()
    {
        var pb = new DescansoProgramado { HoraDelDia = new TimeOnly(15, 0), Habilitado = true };
        pb.Dias.Add(DayOfWeek.Monday);

        // 2026-07-06 es lunes.
        var from = new DateTimeOffset(2026, 7, 6, 9, 0, 0, TimeSpan.Zero);
        DateTimeOffset? next = pb.ProximaOcurrencia(from);

        Assert.NotNull(next);
        Assert.Equal(15, next.Value.Hour);
        Assert.Equal(DayOfWeek.Monday, next.Value.DayOfWeek);
    }

    [Fact]
    public void NextOccurrence_RollsToNextMatchingDay()
    {
        var pb = new DescansoProgramado { HoraDelDia = new TimeOnly(8, 0), Habilitado = true };
        pb.Dias.Add(DayOfWeek.Wednesday);

        var from = new DateTimeOffset(2026, 7, 6, 9, 0, 0, TimeSpan.Zero);
        DateTimeOffset? next = pb.ProximaOcurrencia(from);

        Assert.NotNull(next);
        Assert.Equal(DayOfWeek.Wednesday, next.Value.DayOfWeek);
    }

    [Fact]
    public void NextOccurrence_Disabled_ReturnsNull()
    {
        var pb = new DescansoProgramado { HoraDelDia = new TimeOnly(8, 0), Habilitado = false };
        pb.Dias.Add(DayOfWeek.Monday);

        Assert.Null(pb.ProximaOcurrencia(DateTimeOffset.Now));
    }

    [Fact]
    public void OccursOn_RespectsEnabledAndDays()
    {
        var pb = new DescansoProgramado { Habilitado = true };
        pb.Dias.Add(DayOfWeek.Friday);

        Assert.True(pb.OcurreEn(DayOfWeek.Friday));
        Assert.False(pb.OcurreEn(DayOfWeek.Saturday));
    }
}
