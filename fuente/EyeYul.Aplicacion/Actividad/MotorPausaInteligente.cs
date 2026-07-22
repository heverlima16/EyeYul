using EyeYul.Aplicacion.Abstracciones;
using EyeYul.Aplicacion.Configuracion;
using EyeYul.Dominio.Enumeraciones;

namespace EyeYul.Aplicacion.Actividad;

public sealed class MotorPausaInteligente
{
    public PauseDecision Evaluate(ActivitySnapshot snapshot, SmartPauseSettings settings)
    {
        if (!settings.Enabled)
        {
            return PauseDecision.Allow;
        }

        EstadoActividad state = snapshot.State;

        if (state.HasFlag(EstadoActividad.Idle) && snapshot.IdleTime >= settings.IdleThreshold)
        {
            return PauseDecision.DeferIdle;
        }

        if (settings.RespectFullscreen && state.HasFlag(EstadoActividad.Fullscreen))
        {
            return PauseDecision.SuppressFullscreen;
        }

        if (settings.RespectMeetings && state.HasFlag(EstadoActividad.MicOrCameraInUse))
        {
            return PauseDecision.SuppressMeeting;
        }

        if (settings.RespectMediaPlayback && state.HasFlag(EstadoActividad.MediaPlaying))
        {
            return PauseDecision.SuppressMedia;
        }

        if (settings.RespectFocusAssist && state.HasFlag(EstadoActividad.FocusAssist))
        {
            return PauseDecision.SuppressFocusAssist;
        }

        if (state.HasFlag(EstadoActividad.ScreenRecording))
        {
            return PauseDecision.SuppressScreenRecording;
        }

        return PauseDecision.Allow;
    }

    public bool CanBreakNow(ActivitySnapshot snapshot, SmartPauseSettings settings) =>
        Evaluate(snapshot, settings) == PauseDecision.Allow;
}

public enum PauseDecision
{
    Allow,
    SuppressFullscreen,
    SuppressMeeting,
    SuppressMedia,
    SuppressFocusAssist,
    SuppressScreenRecording,
    DeferIdle
}

/// <summary>
/// Texto para el usuario de por que no se mostro una pausa. Sin esto la pausa
/// se suprime en silencio y el temporizador parece reiniciarse solo.
/// </summary>
public static class MotivoPausa
{
    public static string Describir(PauseDecision decision) => decision switch
    {
        PauseDecision.SuppressFullscreen => "Pantalla completa detectada",
        PauseDecision.SuppressMeeting => "Reunion o llamada en curso",
        PauseDecision.SuppressMedia => "Reproduccion de video activa",
        PauseDecision.SuppressFocusAssist => "Modo No molestar activo",
        PauseDecision.SuppressScreenRecording => "Grabacion de pantalla activa",
        PauseDecision.DeferIdle => "Estabas ausente",
        _ => "Pausa disponible"
    };
}
