using EyeYul.Aplicacion.Abstracciones;

namespace EyeYul.Infraestructura.Interoperabilidad;

public sealed class ControladorMultimediaWin32 : IControladorMultimedia
{
    private const byte VK_MEDIA_PLAY_PAUSE = 0xB3;

    private const uint KEYEVENTF_KEYUP = 0x0002;

    public void AlternarReproduccion()
    {
        MetodosNativos.keybd_event(VK_MEDIA_PLAY_PAUSE, 0, 0, 0);
        MetodosNativos.keybd_event(VK_MEDIA_PLAY_PAUSE, 0, KEYEVENTF_KEYUP, 0);
    }
}
