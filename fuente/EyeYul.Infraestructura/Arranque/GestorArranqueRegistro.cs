using System.Diagnostics;
using EyeYul.Aplicacion.Abstracciones;
using Microsoft.Win32;

namespace EyeYul.Infraestructura.Arranque;

public sealed class GestorArranqueRegistro : IGestorArranque
{
    private const string RunKey = @"Software\Microsoft\Windows\CurrentVersion\Run";

    private const string ValueName = "EyeYul";

    public bool IsEnabled()
    {
        using RegistryKey? key = Registry.CurrentUser.OpenSubKey(RunKey);
        return key?.GetValue(ValueName) is not null;
    }

    public void Enable()
    {
        string? exePath = Process.GetCurrentProcess().MainModule?.FileName;
        if (string.IsNullOrEmpty(exePath) || EsBuildDeDesarrollo(exePath))
        {
            return;
        }

        using RegistryKey key = Registry.CurrentUser.CreateSubKey(RunKey, writable: true);
        key.SetValue(ValueName, $"\"{exePath}\" --minimized");
    }

    // Evita que una ejecucion desde el repo (dotnet run / F5 en Visual Studio) pise
    // el registro con la ruta de la carpeta bin: el auto-inicio solo debe apuntar a
    // una copia instalada (el instalador la deja fuera de cualquier carpeta "bin").
    private static bool EsBuildDeDesarrollo(string exePath)
    {
        string separador = Path.DirectorySeparatorChar.ToString();
        return exePath.Contains(separador + "bin" + separador, StringComparison.OrdinalIgnoreCase);
    }

    public void Disable()
    {
        using RegistryKey? key = Registry.CurrentUser.OpenSubKey(RunKey, writable: true);
        key?.DeleteValue(ValueName, throwOnMissingValue: false);
    }
}
