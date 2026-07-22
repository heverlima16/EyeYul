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

byte[] lengthBuffer = new byte[4];

while (ReadExact(stdin, lengthBuffer, 4))
{
    int length = BinaryPrimitives.ReadInt32LittleEndian(lengthBuffer);
    if (length <= 0 || length > MaxPayloadBytes)
    {
        break;
    }

    byte[] payload = new byte[length];
    if (!ReadExact(stdin, payload, length))
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

static bool ReadExact(Stream stream, byte[] buffer, int count)
{
    for (int read = 0; read < count;)
    {
        int n = stream.Read(buffer, read, count - read);
        if (n == 0)
        {
            return false;
        }

        read += n;
    }

    return true;
}

internal sealed record WebMessage(string? Domain, int Seconds);
