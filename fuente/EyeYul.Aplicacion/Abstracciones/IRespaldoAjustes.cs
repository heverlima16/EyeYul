using EyeYul.Aplicacion.Configuracion;

namespace EyeYul.Aplicacion.Abstracciones;

/// <summary>Exporta e importa la configuración de EyeYul como archivo .yul.</summary>
public interface IRespaldoAjustes
{
    /// <summary>Escribe la configuración actual en <paramref name="rutaDestino"/>.</summary>
    Task ExportarAsync(string rutaDestino, CancellationToken ct = default);

    /// <summary>
    /// Lee y valida un archivo .yul. No aplica los ajustes: el llamador decide cuándo
    /// guardarlos (p. ej. con <see cref="IAlmacenAjustes.SaveAsync"/>).
    /// </summary>
    /// <exception cref="FormatoRespaldoInvalidoException">
    /// El archivo no existe, no es JSON válido o no es una copia de seguridad de EyeYul.
    /// </exception>
    Task<AjustesEyeYul> ImportarAsync(string rutaOrigen, CancellationToken ct = default);
}
