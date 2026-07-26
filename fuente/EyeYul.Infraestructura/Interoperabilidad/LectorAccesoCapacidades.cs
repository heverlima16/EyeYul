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

    public static bool EstaMicrofonoEnUso() => AlgunDispositivoEnUso(MicPath);

    public static bool EstaCamaraEnUso() => AlgunDispositivoEnUso(CamPath);

    private static bool AlgunDispositivoEnUso(string ruta)
    {
        try
        {
            using RegistryKey? raiz = Registry.CurrentUser.OpenSubKey(ruta);
            if (raiz is null)
            {
                return false;
            }

            foreach (string nombreSubclave in raiz.GetSubKeyNames())
            {
                using RegistryKey? subclave = raiz.OpenSubKey(nombreSubclave);
                if (subclave is null)
                {
                    continue;
                }

                if (EstaEnUso(subclave))
                {
                    return true;
                }

                // Las apps de escritorio clásicas cuelgan de NonPackaged.
                if (!nombreSubclave.Equals("NonPackaged", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                foreach (string nombre in subclave.GetSubKeyNames())
                {
                    using RegistryKey? sinEmpaquetar = subclave.OpenSubKey(nombre);
                    if (sinEmpaquetar is not null && EstaEnUso(sinEmpaquetar))
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

    private static bool EstaEnUso(RegistryKey clave) =>
        clave.GetValue("LastUsedTimeStop") is long detenido && detenido == 0;
}
