using EyeYul.Aplicacion.Abstracciones;
using EyeYul.Aplicacion.Actividad;
using EyeYul.Aplicacion.Configuracion;
using EyeYul.Dominio.Enumeraciones;
using Xunit;

namespace EyeYul.UnitTests;

public class SmartPauseEngineTests
{
    private readonly MotorPausaInteligente _engine = new();

    private static SmartPauseSettings Defaults => new();

    private static ActivitySnapshot Snap(EstadoActividad state, TimeSpan? idle = null) =>
        new(state, null, null, idle ?? TimeSpan.Zero);

    [Fact]
    public void CleanState_AllowsBreak()
    {
        Assert.Equal(DecisionPausa.Permitir, _engine.Evaluar(Snap(EstadoActividad.Ninguno), Defaults));
    }

    [Fact]
    public void Disabled_AlwaysAllows()
    {
        var settings = new SmartPauseSettings { Enabled = false };

        Assert.Equal(DecisionPausa.Permitir, _engine.Evaluar(Snap(EstadoActividad.PantallaCompleta), settings));
    }

    [Fact]
    public void Fullscreen_IsSuppressed_WhenRespected()
    {
        Assert.Equal(
            DecisionPausa.SuprimirPantallaCompleta,
            _engine.Evaluar(Snap(EstadoActividad.PantallaCompleta), Defaults));
    }

    [Fact]
    public void Meeting_IsSuppressed_WhenRespected()
    {
        Assert.Equal(
            DecisionPausa.SuprimirReunion,
            _engine.Evaluar(Snap(EstadoActividad.MicOCamaraEnUso), Defaults));
    }

    [Fact]
    public void Idle_BeyondThreshold_IsDeferred()
    {
        ActivitySnapshot snapshot = Snap(EstadoActividad.Inactivo, TimeSpan.FromMinutes(5));

        Assert.Equal(DecisionPausa.AplazarPorAusencia, _engine.Evaluar(snapshot, Defaults));
    }

    [Fact]
    public void Idle_BelowThreshold_DoesNotDefer()
    {
        ActivitySnapshot snapshot = Snap(EstadoActividad.Inactivo, TimeSpan.FromSeconds(30));

        Assert.Equal(DecisionPausa.Permitir, _engine.Evaluar(snapshot, Defaults));
    }

    [Fact]
    public void Meeting_NotSuppressed_WhenDisabledInSettings()
    {
        var settings = new SmartPauseSettings { RespectMeetings = false };

        Assert.Equal(
            DecisionPausa.Permitir,
            _engine.Evaluar(Snap(EstadoActividad.MicOCamaraEnUso), settings));
    }

    [Fact]
    public void ScreenRecording_IsSuppressed_WhenRespected()
    {
        Assert.Equal(
            DecisionPausa.SuprimirGrabacionPantalla,
            _engine.Evaluar(Snap(EstadoActividad.GrabandoPantalla), Defaults));
    }

    [Fact]
    public void ScreenRecording_NotSuppressed_WhenDisabledInSettings()
    {
        var settings = new SmartPauseSettings { DetectScreenSharing = false };

        Assert.Equal(
            DecisionPausa.Permitir,
            _engine.Evaluar(Snap(EstadoActividad.GrabandoPantalla), settings));
    }

    [Fact]
    public void ActiveTyping_IsDeferred_WhenRespected()
    {
        var settings = new SmartPauseSettings { PauseOnActiveTyping = true };

        Assert.Equal(
            DecisionPausa.AplazarPorEscritura,
            _engine.Evaluar(Snap(EstadoActividad.EscribiendoActivamente), settings));
    }

    [Fact]
    public void ActiveTyping_Ignored_WhenNotRespected()
    {
        Assert.Equal(
            DecisionPausa.Permitir,
            _engine.Evaluar(Snap(EstadoActividad.EscribiendoActivamente), Defaults));
    }
}
