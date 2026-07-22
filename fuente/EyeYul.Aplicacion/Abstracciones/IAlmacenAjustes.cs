using EyeYul.Aplicacion.Configuracion;

namespace EyeYul.Aplicacion.Abstracciones;

public interface IAlmacenAjustes
{
    AjustesEyeYul Current { get; }

    event EventHandler<AjustesEyeYul>? Changed;

    Task LoadAsync(CancellationToken ct = default);

    Task SaveAsync(AjustesEyeYul settings, CancellationToken ct = default);
}
