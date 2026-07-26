using System.Security.Cryptography;
using EyeYul.Aplicacion.Abstracciones;

namespace EyeYul.Infraestructura.Licencias;

/// <summary>
/// Verifica claves de licencia sin llamar a ningún servidor: cada clave es
/// <c>id.firma</c> (Base64Url), donde la firma es ECDSA P-256/SHA-256 sobre los bytes
/// del id. La privada correspondiente vive fuera del repositorio — con ella se generan
/// claves nuevas (ver herramientas/generar-clave-licencia.ps1). Embeber aquí la clave
/// PÚBLICA no es un riesgo: solo permite verificar firmas, nunca crearlas.
/// </summary>
public sealed class ValidadorClaveLicenciaEcdsa : IValidadorClaveLicencia
{
    private const string ClavePublicaBase64 =
        "MFkwEwYHKoZIzj0CAQYIKoZIzj0DAQcDQgAE7gu28/tkkQ4nhBl0MOSeeYTb6EBRisLjPJrH2WneFvCRx4Nj0aC4beFhqh3zWz09CkNPrstTBKL34tbL6bARrw==";

    public bool EsValida(string clave)
    {
        if (string.IsNullOrWhiteSpace(clave))
        {
            return false;
        }

        string[] partes = clave.Trim().Split('.');
        if (partes.Length != 2)
        {
            return false;
        }

        try
        {
            byte[] id = Base64UrlDecode(partes[0]);
            byte[] firma = Base64UrlDecode(partes[1]);

            using ECDsa ec = ECDsa.Create();
            ec.ImportSubjectPublicKeyInfo(Convert.FromBase64String(ClavePublicaBase64), out _);

            return ec.VerifyData(
                id, firma, HashAlgorithmName.SHA256, DSASignatureFormat.IeeeP1363FixedFieldConcatenation);
        }
        catch (Exception ex) when (ex is FormatException or CryptographicException)
        {
            return false;
        }
    }

    private static byte[] Base64UrlDecode(string valor)
    {
        string relleno = valor.Replace('-', '+').Replace('_', '/');
        relleno = relleno.PadRight(relleno.Length + ((4 - (relleno.Length % 4)) % 4), '=');
        return Convert.FromBase64String(relleno);
    }
}
