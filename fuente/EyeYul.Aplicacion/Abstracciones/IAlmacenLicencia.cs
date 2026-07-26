using EyeYul.Aplicacion.Licencias;

namespace EyeYul.Aplicacion.Abstracciones;

public interface IAlmacenLicencia
{
    Task<RegistroLicencia> LoadAsync(CancellationToken ct = default);

    Task SaveAsync(RegistroLicencia registro, CancellationToken ct = default);
}
