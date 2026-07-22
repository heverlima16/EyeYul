using System.Drawing;
using System.Windows.Forms;
using System.Windows.Threading;
using EyeYul.Aplicacion.Abstracciones;
using EyeYul.Presentacion.Interoperabilidad;
using EyeYul.Presentacion.Vistas;
using Application = System.Windows.Application;

namespace EyeYul.Presentacion.CapaSuperpuesta;

/// <summary>
/// Muestra la pantalla de pausa a pantalla completa en todos los monitores.
/// El monitor primario lleva el contador y el boton de omitir; el resto solo el fondo.
/// </summary>
public sealed class ControladorPantallaDescansoWpf : IControladorPantallaDescanso
{
    private static Dispatcher Dispatcher => Application.Current.Dispatcher;

    public Task<BreakOverlayResult> ShowAsync(BreakOverlayRequest request, CancellationToken ct = default)
    {
        return Dispatcher.InvokeAsync(() => RunAsync(request, ct)).Task.Unwrap();
    }

    private async Task<BreakOverlayResult> RunAsync(BreakOverlayRequest request, CancellationToken ct)
    {
        var tcs = new TaskCompletionSource<BreakOverlayResult>();
        var windows = new List<VentanaPantallaDescanso>();
        TimeSpan remaining = request.Duration;
        var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };

        void Complete(BreakOverlayResult result)
        {
            if (tcs.Task.IsCompleted)
            {
                return;
            }

            timer.Stop();
            foreach (VentanaPantallaDescanso w in windows)
            {
                w.Close();
            }

            tcs.SetResult(result);
        }

        VentanaPantallaDescanso? primary = null;

        foreach (Screen screen in Screen.AllScreens)
        {
            var w = new VentanaPantallaDescanso();
            w.SetMessage(request.Message);
            w.SetCountdown(remaining);

            if (screen.Primary)
            {
                w.SetSkipAllowed(request.AllowSkip);
                w.Skipped += (_, _) => Complete(BreakOverlayResult.Skipped);

                // Screen.AllScreens no garantiza que el monitor primario sea el
                // primero, asi que se guarda aqui: es el unico overlay que muestra
                // la cuenta atras y el que hay que ir actualizando.
                primary = w;
            }
            else
            {
                w.ConfigureAsSecondary();
            }

            w.Show();

            // Posicionado en pixeles fisicos: evita fallos con DPI mixto entre monitores.
            Rectangle b = screen.Bounds;
            WindowNative.PlaceAtPhysical(w, b.X, b.Y, b.Width, b.Height);

            windows.Add(w);
        }

        // Si Windows no reporto ningun monitor primario, se cae al primero.
        primary ??= windows.FirstOrDefault();

        timer.Tick += (_, _) =>
        {
            remaining -= TimeSpan.FromSeconds(1);
            if (remaining <= TimeSpan.Zero)
            {
                Complete(BreakOverlayResult.Completed);
            }
            else
            {
                primary?.SetCountdown(remaining);
            }
        };
        timer.Start();

        using (ct.Register(() => Dispatcher.Invoke(() => Complete(BreakOverlayResult.Completed))))
        {
            return await tcs.Task;
        }
    }

    public Task ShowPreWarningAsync(TimeSpan countdown, CancellationToken ct = default) =>
        Task.CompletedTask;
}
