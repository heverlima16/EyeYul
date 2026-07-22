using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace EyeYul.Presentacion.Interoperabilidad;

internal static partial class WindowNative
{
    public struct POINT
    {
        public int X;

        public int Y;
    }

    public static readonly nint HWND_TOPMOST = new(-1);

    public const uint SWP_NOACTIVATE = 16u;

    public const uint SWP_SHOWWINDOW = 64u;

    public const int GWL_EXSTYLE = -20;

    public const int WS_EX_TRANSPARENT = 32;

    public const int WS_EX_LAYERED = 524288;

    public const int WS_EX_TOOLWINDOW = 128;

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool GetCursorPos(out POINT lpPoint);

    [LibraryImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool SetWindowPos(
        nint hWnd, nint hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

    [LibraryImport("user32.dll", SetLastError = true)]
    public static partial int GetWindowLongW(nint hWnd, int nIndex);

    [LibraryImport("user32.dll", SetLastError = true)]
    public static partial int SetWindowLongW(nint hWnd, int nIndex, int dwNewLong);

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool DestroyIcon(nint hIcon);

    /// <summary>
    /// Coloca la ventana en coordenadas fisicas de pantalla, saltandose el escalado DPI de WPF.
    /// </summary>
    public static void PlaceAtPhysical(Window window, int x, int y, int width, int height)
    {
        nint hWnd = new WindowInteropHelper(window).EnsureHandle();
        SetWindowPos(hWnd, HWND_TOPMOST, x, y, width, height, SWP_NOACTIVATE | SWP_SHOWWINDOW);
    }

    /// <summary>
    /// Hace la ventana transparente al raton (click-through) y la saca del Alt+Tab.
    /// </summary>
    public static void MakeClickThrough(Window window)
    {
        nint hWnd = new WindowInteropHelper(window).EnsureHandle();
        int exStyle = GetWindowLongW(hWnd, GWL_EXSTYLE);
        exStyle |= WS_EX_LAYERED | WS_EX_TRANSPARENT | WS_EX_TOOLWINDOW;
        SetWindowLongW(hWnd, GWL_EXSTYLE, exStyle);
    }
}
