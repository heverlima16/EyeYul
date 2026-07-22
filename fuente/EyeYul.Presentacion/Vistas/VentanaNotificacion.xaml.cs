using System.Windows;
using EyeYul.Aplicacion.Abstracciones;
using EyeYul.Dominio.ObjetosValor;

namespace EyeYul.Presentacion.Vistas;

public partial class VentanaNotificacion : Window
{
    private readonly TaskCompletionSource<NotificationResponse> _tcs = new();

    public Task<NotificationResponse> Response => _tcs.Task;

    public VentanaNotificacion(string title, string body, bool showActions)
    {
        InitializeComponent();

        TitleText.Text = title;
        BodyText.Text = body;

        if (!showActions)
        {
            Actions.Visibility = Visibility.Collapsed;
        }

        Loaded += (_, _) => PositionBottomRight();
    }

    private void PositionBottomRight()
    {
        Rect workArea = SystemParameters.WorkArea;
        Left = workArea.Right - ActualWidth - 16;
        Top = workArea.Bottom - ActualHeight - 16;
    }

    public void Resolve(NotificationResponse response)
    {
        _tcs.TrySetResult(response);
        Close();
    }

    private void OnNow(object s, RoutedEventArgs e) =>
        Resolve(new NotificationResponse.TakeBreakNow());

    private void OnSnooze1(object s, RoutedEventArgs e) =>
        Resolve(new NotificationResponse.Snooze(DuracionPausa.OneMinute));

    private void OnSnooze5(object s, RoutedEventArgs e) =>
        Resolve(new NotificationResponse.Snooze(DuracionPausa.FiveMinutes));

    private void OnSnooze15(object s, RoutedEventArgs e) =>
        Resolve(new NotificationResponse.Snooze(DuracionPausa.FifteenMinutes));

    private void OnSkip(object s, RoutedEventArgs e) =>
        Resolve(new NotificationResponse.Skip());

    protected override void OnClosed(EventArgs e)
    {
        // Cerrar sin elegir cuenta como descartada.
        _tcs.TrySetResult(new NotificationResponse.Dismissed());
        base.OnClosed(e);
    }
}
