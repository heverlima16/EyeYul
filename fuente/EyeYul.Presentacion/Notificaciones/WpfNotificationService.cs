using System.Windows;
using System.Windows.Threading;
using EyeYul.Aplicacion.Abstracciones;
using EyeYul.Presentacion.Vistas;

namespace EyeYul.Presentacion.Notificaciones;

public sealed class WpfNotificationService : IServicioNotificacion
{
    private static Dispatcher Dispatcher => Application.Current.Dispatcher;

    public Task<NotificationResponse> ShowBreakDueAsync(
        string title, string body, CancellationToken ct = default)
    {
        return Dispatcher.InvokeAsync(() =>
        {
            var toast = new VentanaNotificacion(title, body, showActions: true);
            toast.Show();

            // Si el usuario no responde, se asume que acepta la pausa.
            var timeout = new DispatcherTimer { Interval = TimeSpan.FromSeconds(12) };
            timeout.Tick += (_, _) =>
            {
                timeout.Stop();
                toast.Resolve(new NotificationResponse.TakeBreakNow());
            };
            toast.Closed += (_, _) => timeout.Stop();
            timeout.Start();

            return toast.Response;
        }).Task.Unwrap();
    }

    public Task ShowInfoAsync(string title, string body, CancellationToken ct = default)
    {
        return Dispatcher.InvokeAsync(() =>
        {
            var toast = new VentanaNotificacion(title, body, showActions: false);
            toast.Show();

            var timeout = new DispatcherTimer { Interval = TimeSpan.FromSeconds(5) };
            timeout.Tick += (_, _) =>
            {
                timeout.Stop();
                toast.Close();
            };
            toast.Closed += (_, _) => timeout.Stop();
            timeout.Start();
        }).Task;
    }
}
