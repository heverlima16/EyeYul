namespace EyeYul.Dominio.Enumeraciones;

[Flags]
public enum EstadoActividad
{
    None = 0,
    Fullscreen = 1,
    MicOrCameraInUse = 2,
    MediaPlaying = 4,
    Idle = 8,
    SessionLocked = 0x10,
    FocusAssist = 0x20,
    ScreenRecording = 0x40
}
