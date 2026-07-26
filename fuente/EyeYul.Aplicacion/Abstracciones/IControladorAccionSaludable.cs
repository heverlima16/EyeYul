using EyeYul.Dominio.Enumeraciones;

namespace EyeYul.Aplicacion.Abstracciones;

public interface IControladorAccionSaludable
{
    Task MostrarAsync(TipoAccionSaludable tipo, CancellationToken ct = default);
}
