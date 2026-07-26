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
        // 2026-07-10 es viernes, 2026-07-11 es sabado.
        var pb = new DescansoProgramado { Habilitado = true };
        pb.Dias.Add(DayOfWeek.Friday);

        Assert.True(pb.OcurreEn(new DateOnly(2026, 7, 10)));
        Assert.False(pb.OcurreEn(new DateOnly(2026, 7, 11)));
    }

    [Fact]
    public void Diario_OcurreCualquierDia()
    {
        var pb = new DescansoProgramado { Habilitado = true, Recurrencia = TipoRecurrencia.Diario };

        Assert.True(pb.OcurreEn(new DateOnly(2026, 7, 10)));
        Assert.True(pb.OcurreEn(new DateOnly(2026, 7, 11)));
    }

    [Fact]
    public void Mensual_OcurreSoloEnElDiaDelMes()
    {
        var pb = new DescansoProgramado { Habilitado = true, Recurrencia = TipoRecurrencia.Mensual, DiaDelMes = 15 };

        Assert.True(pb.OcurreEn(new DateOnly(2026, 7, 15)));
        Assert.False(pb.OcurreEn(new DateOnly(2026, 7, 14)));
    }

    [Fact]
    public void Mensual_DiaFueraDeRango_CaeEnElUltimoDiaDelMes()
    {
        // Febrero 2026 (no bisiesto) tiene 28 dias: dia 31 debe caer el 28.
        var pb = new DescansoProgramado { Habilitado = true, Recurrencia = TipoRecurrencia.Mensual, DiaDelMes = 31 };

        Assert.True(pb.OcurreEn(new DateOnly(2026, 2, 28)));
    }

    [Fact]
    public void UnaVez_OcurreSoloEsaFecha()
    {
        var pb = new DescansoProgramado
        {
            Habilitado = true,
            Recurrencia = TipoRecurrencia.UnaVez,
            FechaUnica = new DateOnly(2026, 8, 1)
        };

        Assert.True(pb.OcurreEn(new DateOnly(2026, 8, 1)));
        Assert.False(pb.OcurreEn(new DateOnly(2026, 8, 2)));
    }

    [Fact]
    public void UnaVez_ProximaOcurrencia_NoRepiteDespuesDePasada()
    {
        var pb = new DescansoProgramado
        {
            HoraDelDia = new TimeOnly(9, 0),
            Habilitado = true,
            Recurrencia = TipoRecurrencia.UnaVez,
            FechaUnica = new DateOnly(2026, 7, 6)
        };

        // 2026-07-06 9:00 ya paso (estamos a las 10:00 del mismo dia).
        var from = new DateTimeOffset(2026, 7, 6, 10, 0, 0, TimeSpan.Zero);

        Assert.Null(pb.ProximaOcurrencia(from));
    }
}
