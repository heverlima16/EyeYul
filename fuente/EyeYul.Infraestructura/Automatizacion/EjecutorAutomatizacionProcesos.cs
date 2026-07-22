using System.Diagnostics;
using EyeYul.Aplicacion.Abstracciones;
using Microsoft.Extensions.Logging;

namespace EyeYul.Infraestructura.Automatizacion;

public sealed class EjecutorAutomatizacionProcesos(
    ILogger<EjecutorAutomatizacionProcesos> logger) : IAutomationRunner
{
    public Task RunAsync(string? command, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(command))
        {
            return Task.CompletedTask;
        }

        try
        {
            ProcessStartInfo startInfo = command.EndsWith(".ps1", StringComparison.OrdinalIgnoreCase)
                ? new ProcessStartInfo(
                    "powershell.exe",
                    $"-NoProfile -ExecutionPolicy Bypass -File \"{command}\"")
                : new ProcessStartInfo(command);

            startInfo.UseShellExecute = true;
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;

            using (Process.Start(startInfo))
            {
                logger.LogInformation("Automatización lanzada: {Command}", command);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "No se pudo ejecutar la automatización {Command}", command);
        }

        return Task.CompletedTask;
    }
}
