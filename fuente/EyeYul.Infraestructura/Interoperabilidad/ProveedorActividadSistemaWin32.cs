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

    private bool _microfonoEnUso;

    private bool _camaraEnUso;

    // Caché de la última ventana en primer plano: resolver el proceso es costoso.
    private nint _hwndAnterior;

    private uint _pidAnterior;

    private string? _nombreProcesoAnterior;

    public ActivitySnapshot Sample()
    {
        EstadoActividad state = EstadoActividad.Ninguno;

        (string? proceso, string? titulo, nint hwnd) = ObtenerAppPrimerPlano();
        TimeSpan idleTime = ObtenerTiempoInactivo();

        if (idleTime >= IdleThreshold)
        {
            state |= EstadoActividad.Inactivo;
        }

        // Sin ventana en primer plano se asume sesión bloqueada.
        if (hwnd == nint.Zero)
        {
            state |= EstadoActividad.SesionBloqueada;
        }

        state |= ConsultarEstadoNotificacion(hwnd);

        if (DateTime.UtcNow - _lastDeviceCheck > DeviceCheckInterval)
        {
            _microfonoEnUso = LectorAccesoCapacidades.EstaMicrofonoEnUso();
            _camaraEnUso = LectorAccesoCapacidades.EstaCamaraEnUso();
            _lastDeviceCheck = DateTime.UtcNow;
        }

        if (_microfonoEnUso || _camaraEnUso)
        {
            state |= EstadoActividad.MicOCamaraEnUso;
        }

        if (proceso is not null)
        {
            if (ProcesosConocidos.Meetings.Contains(proceso))
            {
                state |= EstadoActividad.MicOCamaraEnUso;
            }

            if (ProcesosConocidos.ScreenRecorders.Contains(proceso))
            {
                state |= EstadoActividad.GrabandoPantalla;
            }

            if (ProcesosConocidos.MediaPlayers.Contains(proceso))
            {
                state |= EstadoActividad.ReproduciendoMedios;
            }
        }

        return new ActivitySnapshot(state, proceso, titulo, idleTime);
    }

    private EstadoActividad ConsultarEstadoNotificacion(nint primerPlano)
    {
        EstadoActividad state = EstadoActividad.Ninguno;

        if (MetodosNativos.SHQueryUserNotificationState(out var estadoNotificacion) == 0)
        {
            switch (estadoNotificacion)
            {
                case MetodosNativos.QUERY_USER_NOTIFICATION_STATE.QUNS_RUNNING_D3D_FULL_SCREEN:
                case MetodosNativos.QUERY_USER_NOTIFICATION_STATE.QUNS_PRESENTATION_MODE:
                    state |= EstadoActividad.PantallaCompleta;
                    break;

                case MetodosNativos.QUERY_USER_NOTIFICATION_STATE.QUNS_QUIET_TIME:
                    state |= EstadoActividad.AsistenteConcentracion;
                    break;
            }
        }

        // Complemento: comparar el rect de la ventana con los bounds del monitor.
        if (primerPlano != nint.Zero && EsVentanaPantallaCompleta(primerPlano))
        {
            state |= EstadoActividad.PantallaCompleta;
        }

        return state;
    }

    private static bool EsVentanaPantallaCompleta(nint hwnd)
    {
        if (!MetodosNativos.GetWindowRect(hwnd, out MetodosNativos.RECT rectVentana))
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

        MetodosNativos.RECT rectMonitor = monitorInfo.rcMonitor;
        return rectVentana.Left <= rectMonitor.Left
               && rectVentana.Top <= rectMonitor.Top
               && rectVentana.Right >= rectMonitor.Right
               && rectVentana.Bottom >= rectMonitor.Bottom;
    }

    private (string? Proceso, string? Titulo, nint Hwnd) ObtenerAppPrimerPlano()
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

            string? titulo = null;
            int longitudTitulo = MetodosNativos.GetWindowTextLength(hwnd);
            if (longitudTitulo > 0)
            {
                var buffer = new StringBuilder(longitudTitulo + 1);
                MetodosNativos.GetWindowText(hwnd, buffer, buffer.Capacity);
                titulo = buffer.ToString();
            }

            string? nombreProceso;
            if (hwnd == _hwndAnterior && pid == _pidAnterior)
            {
                nombreProceso = _nombreProcesoAnterior;
            }
            else
            {
                nombreProceso = ResolverNombreProceso(pid);
                _hwndAnterior = hwnd;
                _pidAnterior = pid;
                _nombreProcesoAnterior = nombreProceso;
            }

            return (nombreProceso, string.IsNullOrWhiteSpace(titulo) ? null : titulo, hwnd);
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
            uint tamano = 1024u;
            var buffer = new StringBuilder((int)tamano);

            return MetodosNativos.QueryFullProcessImageName(handle, 0u, buffer, ref tamano)
                ? Path.GetFileNameWithoutExtension(buffer.ToString())
                : null;
        }
        finally
        {
            MetodosNativos.CloseHandle(handle);
        }
    }

    private static TimeSpan ObtenerTiempoInactivo()
    {
        var info = new MetodosNativos.LASTINPUTINFO
        {
            cbSize = (uint)Marshal.SizeOf<MetodosNativos.LASTINPUTINFO>()
        };

        if (!MetodosNativos.GetLastInputInfo(ref info))
        {
            return TimeSpan.Zero;
        }

        uint transcurrido = MetodosNativos.GetTickCount() - info.dwTime;
        return TimeSpan.FromMilliseconds(transcurrido);
    }
}
