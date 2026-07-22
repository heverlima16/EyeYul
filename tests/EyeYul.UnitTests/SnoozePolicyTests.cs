using EyeYul.Aplicacion.AplicacionReglas;
using EyeYul.Aplicacion.Configuracion;
using Xunit;

namespace EyeYul.UnitTests;

public class PoliticaPausaTests
{
    private static readonly DateOnly Today = new(2026, 7, 6);

    [Fact]
    public void AllowsSnooze_WithinLimits()
    {
        var policy = new PoliticaPausa();
        var settings = new EnforcementSettings { MaxSnoozesPerDay = 3, MaxSnoozesPerBreak = 2 };

        Assert.True(policy.CanSnooze(Today, 0, settings));
    }

    [Fact]
    public void BlocksSnooze_WhenPerBreakLimitReached()
    {
        var policy = new PoliticaPausa();
        var settings = new EnforcementSettings { MaxSnoozesPerBreak = 2 };

        Assert.False(policy.CanSnooze(Today, 2, settings));
    }

    [Fact]
    public void BlocksSnooze_WhenDailyLimitReached()
    {
        var policy = new PoliticaPausa();
        var settings = new EnforcementSettings { MaxSnoozesPerDay = 2, MaxSnoozesPerBreak = 0 };

        policy.RecordSnooze(Today);
        policy.RecordSnooze(Today);

        Assert.False(policy.CanSnooze(Today, 0, settings));
    }

    [Fact]
    public void Zero_MeansUnlimited()
    {
        var policy = new PoliticaPausa();
        var settings = new EnforcementSettings { MaxSnoozesPerDay = 0, MaxSnoozesPerBreak = 0 };

        for (int i = 0; i < 50; i++)
        {
            policy.RecordSnooze(Today);
        }

        Assert.True(policy.CanSnooze(Today, 100, settings));
    }

    [Fact]
    public void Counter_ResetsOnNewDay()
    {
        var policy = new PoliticaPausa();
        var settings = new EnforcementSettings { MaxSnoozesPerDay = 1 };

        policy.RecordSnooze(Today);
        Assert.False(policy.CanSnooze(Today, 0, settings));

        DateOnly tomorrow = Today.AddDays(1);
        Assert.True(policy.CanSnooze(tomorrow, 0, settings));
        Assert.Equal(0, policy.SnoozesUsedToday(tomorrow));
    }
}
