using System.Text.Json;
using EyeYul.Aplicacion.Abstracciones;
using EyeYul.Aplicacion.Configuracion;
using Microsoft.Extensions.Logging;

namespace EyeYul.Infraestructura.Ajustes;

public sealed class AlmacenAjustesJson : IAlmacenAjustes
{
    private static readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true
    };

    private readonly string _path;

    private readonly ILogger<AlmacenAjustesJson> _logger;

    private readonly SemaphoreSlim _gate = new(1, 1);

    public AjustesEyeYul Current { get; private set; } = new();

    public event EventHandler<AjustesEyeYul>? Changed;

    public AlmacenAjustesJson(ILogger<AlmacenAjustesJson> logger)
    {
        _logger = logger;

        string folder = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "EyeYul");
        Directory.CreateDirectory(folder);
        _path = Path.Combine(folder, "settings.json");
    }

    public async Task LoadAsync(CancellationToken ct = default)
    {
        await _gate.WaitAsync(ct);
        try
        {
            if (File.Exists(_path))
            {
                await using FileStream fs = File.OpenRead(_path);
                AjustesEyeYul? loaded = await JsonSerializer.DeserializeAsync<AjustesEyeYul>(fs, Options, ct);
                if (loaded is not null)
                {
                    Current = loaded;
                    _logger.LogInformation("Ajustes cargados de {Path}", _path);
                    return;
                }
            }

            await WriteAsync(Current, ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "No se pudieron cargar los ajustes; se usan valores por defecto.");
            Current = new AjustesEyeYul();
        }
        finally
        {
            _gate.Release();
        }
    }

    public async Task SaveAsync(AjustesEyeYul settings, CancellationToken ct = default)
    {
        await _gate.WaitAsync(ct);
        try
        {
            await WriteAsync(settings, ct);
            Current = settings;
        }
        finally
        {
            _gate.Release();
        }

        Changed?.Invoke(this, settings);
    }

    private async Task WriteAsync(AjustesEyeYul settings, CancellationToken ct)
    {
        // Escritura atómica: se escribe a un temporal y se reemplaza.
        string tmp = _path + ".tmp";
        await using (FileStream fs = File.Create(tmp))
        {
            await JsonSerializer.SerializeAsync(fs, settings, Options, ct);
        }

        File.Move(tmp, _path, overwrite: true);
    }
}
