using System.Runtime.InteropServices;
using System.Text;
using EyeYul.Aplicacion.Abstracciones;
using EyeYul.Dominio.Enumeraciones;
using Microsoft.Extensions.Logging;

namespace EyeYul.Infraestructura.Interoperabilidad;

public sealed class ProveedorActividadSistemaWin32(
    ILogger<ProveedorActividadSistemaWin32> logger) : IProveedorActividadSistema
{
    private static readonly TimeSpan IdleThreshold = TimeSpan.FromSeconds(30);

    private static readonly TimeSpan DeviceCheckInterval = TimeSpan.FromSeconds(10);

    private DateTime _lastDeviceCheck = DateTime.MinValue;

    private bool _micInUse;

    private bool _camInUse;

    // Caché de la última ventana en primer plano: resolver el proceso es costoso.
    private nint _hwndAnterior;

    private uint _pidAnterior;

    private string? _nombreProcesoAnterior;

    public ActivitySnapshot Sample()
    {
        EstadoActividad state = EstadoActividad.None;

        (string? proc, string? title, nint hwnd) = GetForegroundApp();
        TimeSpan idleTime = GetIdleTime();

        if (idleTime >= IdleThreshold)
        {
            state |= EstadoActividad.Idle;
        }

        // Sin ventana en primer plano se asume sesión bloqueada.
        if (hwnd == nint.Zero)
        {
            state |= EstadoActividad.SessionLocked;
        }

        state |= QueryNotificationState(hwnd);

        if (DateTime.UtcNow - _lastDeviceCheck > DeviceCheckInterval)
        {
            _micInUse = LectorAccesoCapacidades.IsMicrophoneInUse();
            _camInUse = LectorAccesoCapacidades.IsCameraInUse();
            _lastDeviceCheck = DateTime.UtcNow;
        }

        if (_micInUse || _camInUse)
        {
            state |= EstadoActividad.MicOrCameraInUse;
        }

        if (proc is not null)
        {
            if (ProcesosConocidos.Meetings.Contains(proc))
            {
                state |= EstadoActividad.MicOrCameraInUse;
            }

            if (ProcesosConocidos.ScreenRecorders.Contains(proc))
            {
                state |= EstadoActividad.ScreenRecording;
            }

            if (ProcesosConocidos.MediaPlayers.Contains(proc))
            {
                state |= EstadoActividad.MediaPlaying;
            }
        }

        return new ActivitySnapshot(state, proc, title, idleTime);
    }

    private EstadoActividad QueryNotificationState(nint foreground)
    {
        EstadoActividad state = EstadoActividad.None;

        if (MetodosNativos.SHQueryUserNotificationState(out var quns) == 0)
        {
            switch (quns)
            {
                case MetodosNativos.QUERY_USER_NOTIFICATION_STATE.QUNS_RUNNING_D3D_FULL_SCREEN:
                case MetodosNativos.QUERY_USER_NOTIFICATION_STATE.QUNS_PRESENTATION_MODE:
                    state |= EstadoActividad.Fullscreen;
                    break;

                case MetodosNativos.QUERY_USER_NOTIFICATION_STATE.QUNS_QUIET_TIME:
                    state |= EstadoActividad.FocusAssist;
                    break;
            }
        }

        // Complemento: comparar el rect de la ventana con los bounds del monitor.
        if (foreground != nint.Zero && IsWindowFullscreen(foreground))
        {
            state |= EstadoActividad.Fullscreen;
        }

        return state;
    }

    private static bool IsWindowFullscreen(nint hwnd)
    {
        if (!MetodosNativos.GetWindowRect(hwnd, out MetodosNativos.RECT windowRect))
        {
            return false;
        }

        nint hMonitor = MetodosNativos.MonitorFromWindow(hwnd, MetodosNativos.MONITOR_DEFAULTTONEAREST);

        var monitorInfo = new MetodosNativos.MONITORINFO
        {
            cbSize = Marshal.SizeOf<MetodosNativos.MONITORINFO>()
        };

        if (!MetodosNativos.GetMonitorInfoW(hMonitor, ref monitorInfo))
        {
            return false;
        }

        MetodosNativos.RECT monitor = monitorInfo.rcMonitor;
        return windowRect.Left <= monitor.Left
               && windowRect.Top <= monitor.Top
               && windowRect.Right >= monitor.Right
               && windowRect.Bottom >= monitor.Bottom;
    }

    private (string? Proc, string? Title, nint Hwnd) GetForegroundApp()
    {
        nint hwnd = MetodosNativos.GetForegroundWindow();
        if (hwnd == nint.Zero)
        {
            return (null, null, nint.Zero);
        }

        try
        {
            MetodosNativos.GetWindowThreadProcessId(hwnd, out uint pid);
            if (pid == 0)
            {
                return (null, null, hwnd);
            }

            string? title = null;
            int titleLength = MetodosNativos.GetWindowTextLength(hwnd);
            if (titleLength > 0)
            {
                var buffer = new StringBuilder(titleLength + 1);
                MetodosNativos.GetWindowText(hwnd, buffer, buffer.Capacity);
                title = buffer.ToString();
            }

            string? procName;
            if (hwnd == _hwndAnterior && pid == _pidAnterior)
            {
                procName = _nombreProcesoAnterior;
            }
            else
            {
                procName = ResolverNombreProceso(pid);
                _hwndAnterior = hwnd;
                _pidAnterior = pid;
                _nombreProcesoAnterior = procName;
            }

            return (procName, string.IsNullOrWhiteSpace(title) ? null : title, hwnd);
        }
        catch (Exception ex)
        {
            logger.LogDebug(ex, "No se pudo resolver la app en primer plano.");
            return (null, null, hwnd);
        }
    }

    private static string? ResolverNombreProceso(uint pid)
    {
        nint handle = MetodosNativos.OpenProcess(
            MetodosNativos.PROCESS_QUERY_LIMITED_INFORMATION, bInheritHandle: false, pid);

        if (handle == nint.Zero)
        {
            return null;
        }

        try
        {
            uint size = 1024u;
            var buffer = new StringBuilder((int)size);

            return MetodosNativos.QueryFullProcessImageName(handle, 0u, buffer, ref size)
                ? Path.GetFileNameWithoutExtension(buffer.ToString())
                : null;
        }
        finally
        {
            MetodosNativos.CloseHandle(handle);
        }
    }

    private static TimeSpan GetIdleTime()
    {
        var info = new MetodosNativos.LASTINPUTINFO
        {
            cbSize = (uint)Marshal.SizeOf<MetodosNativos.LASTINPUTINFO>()
        };

        if (!MetodosNativos.GetLastInputInfo(ref info))
        {
            return TimeSpan.Zero;
        }

        uint elapsed = MetodosNativos.GetTickCount() - info.dwTime;
        return TimeSpan.FromMilliseconds(elapsed);
    }
}
