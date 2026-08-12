using EyeYul.Aplicacion.Configuracion;

namespace EyeYul.Aplicacion.Abstracciones;

/// <summary>
/// Aplica (o quita) el tinte de luz azul en pantalla completa segun los ajustes actuales.
/// Idempotente: se puede llamar en cada cambio de ajuste (toggle, slider) sin acumular
/// ventanas ni parpadear.
/// </summary>
public interface IControladorFiltroLuz
{
    void Aplicar(BlueLightFilterSettings settings);

    /// <summary>Quita el tinte de todos los monitores. Se llama al cerrar la app.</summary>
    void Detener();
}
