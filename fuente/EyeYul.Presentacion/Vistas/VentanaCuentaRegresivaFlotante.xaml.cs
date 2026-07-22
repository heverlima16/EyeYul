using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using EyeYul.Presentacion.Interoperabilidad;

namespace EyeYul.Presentacion.Vistas;

/// <summary>
/// Contador flotante click-through que sigue al cursor.
/// </summary>
public partial class VentanaCuentaRegresivaFlotante : Window
{
    private readonly DispatcherTimer _follow = new() { Interval = TimeSpan.FromMilliseconds(60) };

    private DpiScale? _dpi;

    private WindowNative.POINT _ultimaPosicionCursor = new() { X = int.MinValue, Y = int.MinValue };

    public VentanaCuentaRegresivaFlotante()
    {
        InitializeComponent();

        SourceInitialized += (_, _) => WindowNative.MakeClickThrough(this);
        DpiChanged += (_, e) => _dpi = e.NewDpi;

        _follow.Tick += FollowCursor;
        Loaded += (_, _) => _follow.Start();
        Closed += (_, _) => _follow.Stop();
    }

    public void SetRemaining(TimeSpan remaining) =>
        TimeText.Text = $"{(int)remaining.TotalMinutes:00}:{remaining.Seconds:00}";

    private void FollowCursor(object? sender, EventArgs e)
    {
        if (!WindowNative.GetCursorPos(out WindowNative.POINT cursor))
        {
            return;
        }

        if (cursor.X == _ultimaPosicionCursor.X && cursor.Y == _ultimaPosicionCursor.Y)
        {
            return;
        }

        _ultimaPosicionCursor = cursor;

        // GetCursorPos devuelve pixeles fisicos; Left/Top son unidades independientes de dispositivo.
        DpiScale dpi = _dpi ??= VisualTreeHelper.GetDpi(this);

        Left = (cursor.X + 18) / dpi.DpiScaleX;
        Top = (cursor.Y + 18) / dpi.DpiScaleY;
    }
}
