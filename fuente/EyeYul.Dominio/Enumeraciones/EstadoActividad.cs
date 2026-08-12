namespace EyeYul.Dominio.Enumeraciones;

[Flags]
public enum EstadoActividad
{
    Ninguno = 0,
    PantallaCompleta = 1,
    MicOCamaraEnUso = 2,
    ReproduciendoMedios = 4,
    Inactivo = 8,
    SesionBloqueada = 0x10,
    AsistenteConcentracion = 0x20,
    GrabandoPantalla = 0x40,
    EscribiendoActivamente = 0x80
}
