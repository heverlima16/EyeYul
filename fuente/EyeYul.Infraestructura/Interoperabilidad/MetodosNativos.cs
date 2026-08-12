using System.Runtime.InteropServices;
using System.Text;

namespace EyeYul.Infraestructura.Interoperabilidad;

internal static partial class MetodosNativos
{
    internal struct RECT
    {
        public int Left;

        public int Top;

        public int Right;

        public int Bottom;

        public readonly int Width => Right - Left;

        public readonly int Height => Bottom - Top;
    }

    internal struct MONITORINFO
    {
        public int cbSize;

        public RECT rcMonitor;

        public RECT rcWork;

        public uint dwFlags;
    }

    internal struct LASTINPUTINFO
    {
        public uint cbSize;

        public uint dwTime;
    }

    internal enum QUERY_USER_NOTIFICATION_STATE
    {
        QUNS_NOT_PRESENT = 1,
        QUNS_BUSY,
        QUNS_RUNNING_D3D_FULL_SCREEN,
        QUNS_PRESENTATION_MODE,
        QUNS_ACCEPTS_NOTIFICATIONS,
        QUNS_QUIET_TIME,
        QUNS_APP
    }

    internal const uint MONITOR_DEFAULTTONEAREST = 2u;

    internal const uint PROCESS_QUERY_LIMITED_INFORMATION = 4096u;

    [LibraryImport("user32.dll")]
    internal static partial nint GetForegroundWindow();

    [LibraryImport("user32.dll", SetLastError = true)]
    internal static partial uint GetWindowThreadProcessId(nint hWnd, out uint lpdwProcessId);

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool GetWindowRect(nint hWnd, out RECT lpRect);

    [LibraryImport("user32.dll")]
    internal static partial nint MonitorFromWindow(nint hwnd, uint dwFlags);

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool GetMonitorInfoW(nint hMonitor, ref MONITORINFO lpmi);

    [LibraryImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool GetLastInputInfo(ref LASTINPUTINFO plii);

    [LibraryImport("kernel32.dll")]
    internal static partial uint GetTickCount();

    [LibraryImport("shell32.dll")]
    internal static partial int SHQueryUserNotificationState(out QUERY_USER_NOTIFICATION_STATE pquns);

    [LibraryImport("user32.dll")]
    internal static partial short GetAsyncKeyState(int vKey);

    [LibraryImport("user32.dll")]
    internal static partial void keybd_event(byte bVk, byte bScan, uint dwFlags, nuint dwExtraInfo);

    // Las siguientes usan StringBuilder, que LibraryImport no admite: quedan como DllImport.

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    internal static extern int GetWindowText(nint hWnd, StringBuilder lpString, int nMaxCount);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    internal static extern int GetWindowTextLength(nint hWnd);

    [DllImport("kernel32.dll", SetLastError = true)]
    internal static extern nint OpenProcess(uint dwDesiredAccess, bool bInheritHandle, uint dwProcessId);

    [DllImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool CloseHandle(nint hObject);

    [DllImport("kernel32.dll", SetLastError = true)]
    internal static extern bool QueryFullProcessImageName(
        nint hProcess, uint dwFlags, StringBuilder lpExeName, ref uint lpdwSize);
}
