using Microsoft.Win32;

namespace EyeYul.Infraestructura.Interoperabilidad;

/// <summary>
/// Lee el consent store de Windows para saber si el micrófono o la cámara están en uso.
/// Un <c>LastUsedTimeStop</c> de 0 significa "en uso ahora mismo".
/// </summary>
internal static class LectorAccesoCapacidades
{
    private const string MicPath =
        @"Software\Microsoft\Windows\CurrentVersion\CapabilityAccessManager\ConsentStore\microphone";

    private const string CamPath =
        @"Software\Microsoft\Windows\CurrentVersion\CapabilityAccessManager\ConsentStore\webcam";

    public static bool IsMicrophoneInUse() => AnyDeviceInUse(MicPath);

    public static bool IsCameraInUse() => AnyDeviceInUse(CamPath);

    private static bool AnyDeviceInUse(string path)
    {
        try
        {
            using RegistryKey? root = Registry.CurrentUser.OpenSubKey(path);
            if (root is null)
            {
                return false;
            }

            foreach (string subKeyName in root.GetSubKeyNames())
            {
                using RegistryKey? subKey = root.OpenSubKey(subKeyName);
                if (subKey is null)
                {
                    continue;
                }

                if (InUse(subKey))
                {
                    return true;
                }

                // Las apps de escritorio clásicas cuelgan de NonPackaged.
                if (!subKeyName.Equals("NonPackaged", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                foreach (string name in subKey.GetSubKeyNames())
                {
                    using RegistryKey? nonPackaged = subKey.OpenSubKey(name);
                    if (nonPackaged is not null && InUse(nonPackaged))
                    {
                        return true;
                    }
                }
            }
        }
        catch
        {
            // El consent store puede no existir o estar restringido: se asume "no en uso".
        }

        return false;
    }

    private static bool InUse(RegistryKey key) =>
        key.GetValue("LastUsedTimeStop") is long stop && stop == 0;
}
