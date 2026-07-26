using System.Buffers.Binary;
using System.Text.Json;
using EyeYul.Infraestructura.Persistencia;
using Microsoft.Extensions.Logging.Abstractions;

// Native messaging host: recibe del navegador el dominio de la pestana activa
// y acumula el tiempo de uso por sitio. Protocolo: int32 LE de longitud + payload JSON.

const int MaxPayloadBytes = 1024 * 1024;

var db = new BaseDatosSqlite(NullLogger<BaseDatosSqlite>.Instance);
var repo = new RepositorioUsoSitioWeb(db);

using Stream stdin = Console.OpenStandardInput();

byte[] bufferLongitud = new byte[4];

while (LeerExacto(stdin, bufferLongitud, 4))
{
    int longitud = BinaryPrimitives.ReadInt32LittleEndian(bufferLongitud);
    if (longitud <= 0 || longitud > MaxPayloadBytes)
    {
        break;
    }

    byte[] payload = new byte[longitud];
    if (!LeerExacto(stdin, payload, longitud))
    {
        break;
    }

    try
    {
        WebMessage? msg = JsonSerializer.Deserialize<WebMessage>(payload);

        if (msg is { Seconds: > 0, Domain: { Length: > 0 } domain })
        {
            await repo.AccumulateAsync(
                DateOnly.FromDateTime(DateTime.Now),
                domain,
                TimeSpan.FromSeconds(msg.Seconds));
        }
    }
    catch (JsonException)
    {
        // Mensaje malformado: se ignora y se sigue leyendo el flujo.
    }
}

static bool LeerExacto(Stream flujo, byte[] buffer, int cantidad)
{
    for (int leido = 0; leido < cantidad;)
    {
        int n = flujo.Read(buffer, leido, cantidad - leido);
        if (n == 0)
        {
            return false;
        }

        leido += n;
    }

    return true;
}

internal sealed record WebMessage(string? Domain, int Seconds);
