using EyeYul.Aplicacion.Abstracciones;
using EyeYul.Aplicacion.Configuracion;
using EyeYul.Dominio.Enumeraciones;

namespace EyeYul.Aplicacion.Actividad;

public sealed class MotorPausaInteligente
{
    public DecisionPausa Evaluar(ActivitySnapshot muestra, SmartPauseSettings ajustes)
    {
        if (!ajustes.Enabled)
        {
            return DecisionPausa.Permitir;
        }

        EstadoActividad estado = muestra.State;

        if (estado.HasFlag(EstadoActividad.Inactivo) && muestra.IdleTime >= ajustes.IdleThreshold)
        {
            return DecisionPausa.AplazarPorAusencia;
        }

        if (ajustes.RespectFullscreen && estado.HasFlag(EstadoActividad.PantallaCompleta))
        {
            return DecisionPausa.SuprimirPantallaCompleta;
        }

        if (ajustes.RespectMeetings && estado.HasFlag(EstadoActividad.MicOCamaraEnUso))
        {
            return DecisionPausa.SuprimirReunion;
        }

        if (ajustes.RespectMediaPlayback && estado.HasFlag(EstadoActividad.ReproduciendoMedios))
        {
            return DecisionPausa.SuprimirMedios;
        }

        if (ajustes.RespectFocusAssist && estado.HasFlag(EstadoActividad.AsistenteConcentracion))
        {
            return DecisionPausa.SuprimirAsistenteConcentracion;
        }

        if (ajustes.DetectScreenSharing && estado.HasFlag(EstadoActividad.GrabandoPantalla))
        {
            return DecisionPausa.SuprimirGrabacionPantalla;
        }

        if (ajustes.PauseOnActiveTyping && estado.HasFlag(EstadoActividad.EscribiendoActivamente))
        {
            return DecisionPausa.AplazarPorEscritura;
        }

        return DecisionPausa.Permitir;
    }

    public bool PuedePausarAhora(ActivitySnapshot muestra, SmartPauseSettings ajustes) =>
        Evaluar(muestra, ajustes) == DecisionPausa.Permitir;
}

public enum DecisionPausa
{
    Permitir,
    SuprimirPantallaCompleta,
    SuprimirReunion,
    SuprimirMedios,
    SuprimirAsistenteConcentracion,
    SuprimirGrabacionPantalla,
    AplazarPorAusencia,
    AplazarPorEscritura
}

/// <summary>
/// Texto para el usuario de por que no se mostro una pausa. Sin esto la pausa
/// se suprime en silencio y el temporizador parece reiniciarse solo.
/// </summary>
public static class MotivoPausa
{
    public static string Describir(DecisionPausa decision) => decision switch
    {
        DecisionPausa.SuprimirPantallaCompleta => "Pantalla completa detectada",
        DecisionPausa.SuprimirReunion => "Reunion o llamada en curso",
        DecisionPausa.SuprimirMedios => "Reproduccion de video activa",
        DecisionPausa.SuprimirAsistenteConcentracion => "Modo No molestar activo",
        DecisionPausa.SuprimirGrabacionPantalla => "Grabacion de pantalla activa",
        DecisionPausa.AplazarPorAusencia => "Estabas ausente",
        DecisionPausa.AplazarPorEscritura => "Escritura activa detectada",
        _ => "Pausa disponible"
    };
}
