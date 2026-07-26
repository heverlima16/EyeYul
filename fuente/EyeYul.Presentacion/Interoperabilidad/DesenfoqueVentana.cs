using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using Microsoft.Extensions.Logging;

namespace EyeYul.Presentacion.Interoperabilidad;

/// <summary>
/// Fondo con desenfoque en vivo (Acrylic), estilo Windows 11, para las ventanas de
/// Acciones Saludables. Usa <c>SetWindowCompositionAttribute</c>, una API de Windows
/// NO documentada oficialmente: puede dejar de funcionar en una actualización futura de
/// Windows sin aviso. Por eso todo va envuelto en try/catch — si falla, la ventana se
/// queda con su fondo semitransparente normal en vez de romper la app.
/// </summary>
internal static class DesenfoqueVentana
{
    [DllImport("user32.dll")]
    private static extern int SetWindowCompositionAttribute(nint hwnd, ref WindowCompositionAttributeData data);

    [StructLayout(LayoutKind.Sequential)]
    private struct WindowCompositionAttributeData
    {
        public int Attribute;
        public nint Data;
        public int SizeOfData;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct AccentPolicy
    {
        public int AccentState;
        public int AccentFlags;
        public uint GradientColor;
        public int AnimationId;
    }

    private const int WcaAccentPolicy = 19;

    private const int AccentEnableAcrylicBlurBehind = 4;

    /// <param name="tintaArgb">Color de la tinta sobre el desenfoque, formato 0xAARRGGBB.</param>
    public static void Habilitar(Window ventana, uint tintaArgb, ILogger? registro = null)
    {
        try
        {
            nint hwnd = new WindowInteropHelper(ventana).EnsureHandle();

            // La API espera el color en ABGR, no ARGB.
            uint abgr = (tintaArgb & 0xFF000000)
                        | ((tintaArgb & 0x000000FF) << 16)
                        | (tintaArgb & 0x0000FF00)
                        | ((tintaArgb & 0x00FF0000) >> 16);

            var accent = new AccentPolicy { AccentState = AccentEnableAcrylicBlurBehind, GradientColor = abgr };
            int tamano = Marshal.SizeOf<AccentPolicy>();
            nint puntero = Marshal.AllocHGlobal(tamano);
            try
            {
                Marshal.StructureToPtr(accent, puntero, false);
                var datos = new WindowCompositionAttributeData
                {
                    Attribute = WcaAccentPolicy,
                    SizeOfData = tamano,
                    Data = puntero
                };

                SetWindowCompositionAttribute(hwnd, ref datos);
            }
            finally
            {
                Marshal.FreeHGlobal(puntero);
            }
        }
        catch (Exception ex) when (ex is EntryPointNotFoundException or DllNotFoundException or InvalidOperationException)
        {
            registro?.LogDebug(ex, "No se pudo activar el blur Acrylic; se usa el fondo semitransparente normal.");
        }
    }
}
