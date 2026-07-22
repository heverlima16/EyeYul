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
        Assert.Equal(PauseDecision.Allow, _engine.Evaluate(Snap(EstadoActividad.None), Defaults));
    }

    [Fact]
    public void Disabled_AlwaysAllows()
    {
        var settings = new SmartPauseSettings { Enabled = false };

        Assert.Equal(PauseDecision.Allow, _engine.Evaluate(Snap(EstadoActividad.Fullscreen), settings));
    }

    [Fact]
    public void Fullscreen_IsSuppressed_WhenRespected()
    {
        Assert.Equal(
            PauseDecision.SuppressFullscreen,
            _engine.Evaluate(Snap(EstadoActividad.Fullscreen), Defaults));
    }

    [Fact]
    public void Meeting_IsSuppressed_WhenRespected()
    {
        Assert.Equal(
            PauseDecision.SuppressMeeting,
            _engine.Evaluate(Snap(EstadoActividad.MicOrCameraInUse), Defaults));
    }

    [Fact]
    public void Idle_BeyondThreshold_IsDeferred()
    {
        ActivitySnapshot snapshot = Snap(EstadoActividad.Idle, TimeSpan.FromMinutes(5));

        Assert.Equal(PauseDecision.DeferIdle, _engine.Evaluate(snapshot, Defaults));
    }

    [Fact]
    public void Idle_BelowThreshold_DoesNotDefer()
    {
        ActivitySnapshot snapshot = Snap(EstadoActividad.Idle, TimeSpan.FromSeconds(30));

        Assert.Equal(PauseDecision.Allow, _engine.Evaluate(snapshot, Defaults));
    }

    [Fact]
    public void Meeting_NotSuppressed_WhenDisabledInSettings()
    {
        var settings = new SmartPauseSettings { RespectMeetings = false };

        Assert.Equal(
            PauseDecision.Allow,
            _engine.Evaluate(Snap(EstadoActividad.MicOrCameraInUse), settings));
    }

    [Fact]
    public void ScreenRecording_AlwaysSuppressed()
    {
        Assert.Equal(
            PauseDecision.SuppressScreenRecording,
            _engine.Evaluate(Snap(EstadoActividad.ScreenRecording), Defaults));
    }
}
