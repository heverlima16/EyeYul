namespace EyeYul.Aplicacion.Licencias;

/// <summary>Lo único que se persiste sobre la licencia: la clave activada (si hay) y desde cuándo corre el trial.</summary>
public sealed record RegistroLicencia(string? ClaveActivada, DateTimeOffset? PrimerUso);
