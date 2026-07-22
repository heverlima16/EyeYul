using EyeYul.Dominio.Entidades;
using Xunit;

namespace EyeYul.UnitTests;

public class PlannedBreakTests
{
    [Fact]
    public void NextOccurrence_SameDay_LaterTime()
    {
        var pb = new DescansoProgramado { TimeOfDay = new TimeOnly(15, 0), Enabled = true };
        pb.Days.Add(DayOfWeek.Monday);

        // 2026-07-06 es lunes.
        var from = new DateTimeOffset(2026, 7, 6, 9, 0, 0, TimeSpan.Zero);
        DateTimeOffset? next = pb.NextOccurrence(from);

        Assert.NotNull(next);
        Assert.Equal(15, next.Value.Hour);
        Assert.Equal(DayOfWeek.Monday, next.Value.DayOfWeek);
    }

    [Fact]
    public void NextOccurrence_RollsToNextMatchingDay()
    {
        var pb = new DescansoProgramado { TimeOfDay = new TimeOnly(8, 0), Enabled = true };
        pb.Days.Add(DayOfWeek.Wednesday);

        var from = new DateTimeOffset(2026, 7, 6, 9, 0, 0, TimeSpan.Zero);
        DateTimeOffset? next = pb.NextOccurrence(from);

        Assert.NotNull(next);
        Assert.Equal(DayOfWeek.Wednesday, next.Value.DayOfWeek);
    }

    [Fact]
    public void NextOccurrence_Disabled_ReturnsNull()
    {
        var pb = new DescansoProgramado { TimeOfDay = new TimeOnly(8, 0), Enabled = false };
        pb.Days.Add(DayOfWeek.Monday);

        Assert.Null(pb.NextOccurrence(DateTimeOffset.Now));
    }

    [Fact]
    public void OccursOn_RespectsEnabledAndDays()
    {
        var pb = new DescansoProgramado { Enabled = true };
        pb.Days.Add(DayOfWeek.Friday);

        Assert.True(pb.OccursOn(DayOfWeek.Friday));
        Assert.False(pb.OccursOn(DayOfWeek.Saturday));
    }
}
