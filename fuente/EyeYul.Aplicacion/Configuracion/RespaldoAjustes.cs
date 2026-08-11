namespace EyeYul.Aplicacion.Configuracion;

/// <summary>
/// Envoltorio de un archivo de copia de seguridad <c>.yul</c>. Contiene la configuración
/// completa (<see cref="AjustesEyeYul"/>) más metadatos para validar el archivo al importarlo.
/// </summary>
public sealed class RespaldoAjustes
{
    public const string FormatoActual = "EyeYul.Backup";

    public const int VersionFormatoActual = 1;

    public string Formato { get; set; } = FormatoActual;

    public int VersionFormato { get; set; } = VersionFormatoActual;

    public DateTimeOffset CreadoEn { get; set; }

    public AjustesEyeYul? Ajustes { get; set; }
}

/// <summary>Se lanza cuando un archivo .yul no es una copia de seguridad válida de EyeYul.</summary>
public sealed class FormatoRespaldoInvalidoException : Exception
{
    public FormatoRespaldoInvalidoException(string message) : base(message)
    {
    }

    public FormatoRespaldoInvalidoException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
