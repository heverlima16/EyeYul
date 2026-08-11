using System.Text.Json;
using EyeYul.Aplicacion.Abstracciones;
using EyeYul.Aplicacion.Configuracion;

namespace EyeYul.Infraestructura.Ajustes;

public sealed class RespaldoAjustesArchivo : IRespaldoAjustes
{
    private static readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true
    };

    private readonly IAlmacenAjustes _store;

    public RespaldoAjustesArchivo(IAlmacenAjustes store)
    {
        _store = store;
    }

    public async Task ExportarAsync(string rutaDestino, CancellationToken ct = default)
    {
        var respaldo = new RespaldoAjustes
        {
            CreadoEn = DateTimeOffset.Now,
            Ajustes = _store.Current
        };

        await using FileStream fs = File.Create(rutaDestino);
        await JsonSerializer.SerializeAsync(fs, respaldo, Options, ct);
    }

    public async Task<AjustesEyeYul> ImportarAsync(string rutaOrigen, CancellationToken ct = default)
    {
        if (!File.Exists(rutaOrigen))
        {
            throw new FormatoRespaldoInvalidoException("No se encontró el archivo de copia de seguridad.");
        }

        RespaldoAjustes? respaldo;
        try
        {
            await using FileStream fs = File.OpenRead(rutaOrigen);
            respaldo = await JsonSerializer.DeserializeAsync<RespaldoAjustes>(fs, Options, ct);
        }
        catch (JsonException ex)
        {
            throw new FormatoRespaldoInvalidoException("El archivo no es una copia de seguridad válida de EyeYul.", ex);
        }

        if (respaldo?.Ajustes is null || respaldo.Formato != RespaldoAjustes.FormatoActual)
        {
            throw new FormatoRespaldoInvalidoException("El archivo no es una copia de seguridad válida de EyeYul.");
        }

        return respaldo.Ajustes;
    }
}
