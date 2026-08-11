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

    private struct RECT
    {
        public int Left;

        public int Top;

        public int Right;

        public int Bottom;
    }

    // Layout fijo de la struct nativa MINMAXINFO: ptReserved/ptMinTrackSize/ptMaxTrackSize
    // los rellena Windows via PtrToStructure y no se leen aca, pero deben existir en el
    // orden exacto o el marshaling desalinea los campos que si se usan.
    private struct MINMAXINFO
    {
        public POINT ptReserved = default;

        public POINT ptMaxSize;

        public POINT ptMaxPosition;

        public POINT ptMinTrackSize = default;

        public POINT ptMaxTrackSize = default;

        public MINMAXINFO()
        {
        }
    }

    private struct MONITORINFO
    {
        public int cbSize;

        public RECT rcMonitor;

        public RECT rcWork;

        public int dwFlags;
    }

    public static readonly nint HWND_TOPMOST = new(-1);

    public const uint SWP_NOACTIVATE = 16u;

    public const uint SWP_SHOWWINDOW = 64u;

    public const int GWL_EXSTYLE = -20;

    public const int WS_EX_TRANSPARENT = 32;

    public const int WS_EX_LAYERED = 524288;

    public const int WS_EX_TOOLWINDOW = 128;

    private const int WM_GETMINMAXINFO = 0x0024;

    private const uint MONITOR_DEFAULTTONEAREST = 2u;

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

    [LibraryImport("user32.dll")]
    private static partial nint MonitorFromWindow(nint hwnd, uint dwFlags);

    [LibraryImport("user32.dll", EntryPoint = "GetMonitorInfoW")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool GetMonitorInfo(nint hMonitor, ref MONITORINFO lpmi);

    /// <summary>
    /// Coloca la ventana en coordenadas fisicas de pantalla, saltandose el escalado DPI de WPF.
    /// </summary>
    public static void PlaceAtPhysical(Window window, int x, int y, int width, int height)
    {
        nint hWnd = new WindowInteropHelper(window).EnsureHandle();
        SetWindowPos(hWnd, HWND_TOPMOST, x, y, width, height, SWP_NOACTIVATE | SWP_SHOWWINDOW);
    }

    /// <summary>
    /// Con WindowStyle="None" + AllowsTransparency="True", maximizar hace que la ventana
    /// cubra el monitor entero (incluida la barra de tareas) en vez de respetar el area de
    /// trabajo: WindowChrome deberia evitarlo solo, pero no lo hace de forma confiable
    /// combinado con AllowsTransparency. Se intercepta WM_GETMINMAXINFO y se fuerza el
    /// tamaño/posicion maximizados al area de trabajo real del monitor mas cercano.
    /// Debe llamarse una vez el handle de la ventana ya existe (evento SourceInitialized).
    /// </summary>
    public static void CorregirMaximizadoTrasLaBarraDeTareas(Window window)
    {
        nint hWnd = new WindowInteropHelper(window).EnsureHandle();
        HwndSource.FromHwnd(hWnd)?.AddHook(WndProc);
    }

    private static nint WndProc(nint hwnd, int msg, nint wParam, nint lParam, ref bool handled)
    {
        if (msg == WM_GETMINMAXINFO)
        {
            AjustarMinMaxInfo(hwnd, lParam);
        }

        return nint.Zero;
    }

    private static void AjustarMinMaxInfo(nint hwnd, nint lParam)
    {
        nint monitor = MonitorFromWindow(hwnd, MONITOR_DEFAULTTONEAREST);
        if (monitor == nint.Zero)
        {
            return;
        }

        var monitorInfo = new MONITORINFO { cbSize = Marshal.SizeOf<MONITORINFO>() };
        if (!GetMonitorInfo(monitor, ref monitorInfo))
        {
            return;
        }

        RECT area = monitorInfo.rcMonitor;
        RECT trabajo = monitorInfo.rcWork;

        var infoActual = Marshal.PtrToStructure<MINMAXINFO>(lParam);
        infoActual.ptMaxPosition = new POINT { X = trabajo.Left - area.Left, Y = trabajo.Top - area.Top };
        infoActual.ptMaxSize = new POINT { X = trabajo.Right - trabajo.Left, Y = trabajo.Bottom - trabajo.Top };
        Marshal.StructureToPtr(infoActual, lParam, true);
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
