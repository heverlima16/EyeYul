using System.Text.Json;
using EyeYul.Aplicacion.Abstracciones;
using EyeYul.Aplicacion.Licencias;
using Microsoft.Extensions.Logging;

namespace EyeYul.Infraestructura.Licencias;

public sealed class AlmacenLicenciaJson : IAlmacenLicencia
{
    private static readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true
    };

    private readonly string _path;

    private readonly ILogger<AlmacenLicenciaJson> _logger;

    private readonly SemaphoreSlim _gate = new(1, 1);

    public AlmacenLicenciaJson(ILogger<AlmacenLicenciaJson> logger)
    {
        _logger = logger;

        string folder = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "EyeYul");
        Directory.CreateDirectory(folder);
        _path = Path.Combine(folder, "licencia.json");
    }

    public async Task<RegistroLicencia> LoadAsync(CancellationToken ct = default)
    {
        await _gate.WaitAsync(ct);
        try
        {
            if (File.Exists(_path))
            {
                await using FileStream fs = File.OpenRead(_path);
                RegistroLicencia? cargado = await JsonSerializer.DeserializeAsync<RegistroLicencia>(fs, Options, ct);
                if (cargado is not null)
                {
                    return cargado;
                }
            }

            return new RegistroLicencia(null, null);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "No se pudo cargar la licencia; se asume sin activar.");
            return new RegistroLicencia(null, null);
        }
        finally
        {
            _gate.Release();
        }
    }

    public async Task SaveAsync(RegistroLicencia registro, CancellationToken ct = default)
    {
        await _gate.WaitAsync(ct);
        try
        {
            // Escritura atómica: se escribe a un temporal y se reemplaza.
            string tmp = _path + ".tmp";
            await using (FileStream fs = File.Create(tmp))
            {
                await JsonSerializer.SerializeAsync(fs, registro, Options, ct);
            }

            File.Move(tmp, _path, overwrite: true);
        }
        finally
        {
            _gate.Release();
        }
    }
}
