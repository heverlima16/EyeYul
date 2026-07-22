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
        if (string.IsNullOrEmpty(exePath))
        {
            return;
        }

        using RegistryKey key = Registry.CurrentUser.CreateSubKey(RunKey, writable: true);
        key.SetValue(ValueName, $"\"{exePath}\" --minimized");
    }

    public void Disable()
    {
        using RegistryKey? key = Registry.CurrentUser.OpenSubKey(RunKey, writable: true);
        key?.DeleteValue(ValueName, throwOnMissingValue: false);
    }
}
