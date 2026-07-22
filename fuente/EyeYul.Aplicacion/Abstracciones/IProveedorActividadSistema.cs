using EyeYul.Dominio.Enumeraciones;

namespace EyeYul.Aplicacion.Abstracciones;

public interface IProveedorActividadSistema
{
    ActivitySnapshot Sample();
}

public readonly record struct ActivitySnapshot(
    EstadoActividad State,
    string? ForegroundProcessName,
    string? ForegroundTitle,
    TimeSpan IdleTime);
