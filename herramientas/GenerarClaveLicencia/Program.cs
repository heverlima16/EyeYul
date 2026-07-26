// Genera claves de licencia de EyeYul offline, sin tocar ningun servidor.
//
// Uso:
//   dotnet run --project herramientas/GenerarClaveLicencia -- <ruta-a-la-clave-privada.b64>
//
// La clave privada NUNCA vive en este repositorio (ver la nota en
// ValidadorClaveLicenciaEcdsa.cs): se guarda fuera, en un lugar privado del
// desarrollador. Este script solo la LEE para firmar; nunca la imprime ni la copia.
//
// Cada corrida genera una clave nueva e independiente (un id aleatorio de 8 bytes
// + firma ECDSA P-256/SHA-256 sobre esos bytes). Al no llevar fecha de vencimiento
// ni atarse a una maquina, la clave desbloquea Premium para siempre en quien la
// pegue — coherente con que EyeYul no tiene cuentas ni servidor.

using System.Security.Cryptography;

if (args.Length != 1)
{
    Console.Error.WriteLine("Uso: dotnet run --project herramientas/GenerarClaveLicencia -- <ruta-clave-privada.b64>");
    return 1;
}

string rutaClavePrivada = args[0];
if (!File.Exists(rutaClavePrivada))
{
    Console.Error.WriteLine($"No se encontro la clave privada en: {rutaClavePrivada}");
    return 1;
}

byte[] privada = Convert.FromBase64String(File.ReadAllText(rutaClavePrivada).Trim());

using ECDsa ec = ECDsa.Create();
ec.ImportPkcs8PrivateKey(privada, out _);

byte[] id = RandomNumberGenerator.GetBytes(8);
byte[] firma = ec.SignData(id, HashAlgorithmName.SHA256, DSASignatureFormat.IeeeP1363FixedFieldConcatenation);

string clave = $"{Base64UrlEncode(id)}.{Base64UrlEncode(firma)}";

Console.WriteLine("Clave de licencia generada:");
Console.WriteLine();
Console.WriteLine(clave);

return 0;

static string Base64UrlEncode(byte[] bytes) =>
    Convert.ToBase64String(bytes).TrimEnd('=').Replace('+', '-').Replace('/', '_');
