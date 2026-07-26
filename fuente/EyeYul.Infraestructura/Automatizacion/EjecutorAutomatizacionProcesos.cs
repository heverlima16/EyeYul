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
            bool esScriptPowerShell = command.EndsWith(".ps1", StringComparison.OrdinalIgnoreCase);

            ProcessStartInfo startInfo = esScriptPowerShell
                ? new ProcessStartInfo("powershell.exe")
                : new ProcessStartInfo(command);

            if (esScriptPowerShell)
            {
                // ArgumentList escapa cada elemento por separado (vía las reglas de
                // Win32 para command lines): a diferencia de interpolar el comando en
                // un string, una comilla dentro de "command" no puede escapar la
                // ruta del -File e inyectar argumentos extra a powershell.exe.
                startInfo.ArgumentList.Add("-NoProfile");
                startInfo.ArgumentList.Add("-ExecutionPolicy");
                startInfo.ArgumentList.Add("Bypass");
                startInfo.ArgumentList.Add("-File");
                startInfo.ArgumentList.Add(command);
            }

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
