namespace EyeYul.Aplicacion.Abstracciones;

/// <summary>Controles de reproduccion multimedia del sistema operativo (tecla Play/Pause).</summary>
public interface IControladorMultimedia
{
    /// <summary>Envia la tecla multimedia Play/Pause: pausa lo que este sonando (Spotify,
    /// YouTube, etc.) o lo reanuda si ya estaba pausado. El SO no distingue las dos cosas,
    /// asi que si no habia nada sonando esto no tiene efecto visible.</summary>
    void AlternarReproduccion();
}
