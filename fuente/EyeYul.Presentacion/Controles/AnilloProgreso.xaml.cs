using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace EyeYul.Presentacion.Controles;

/// <summary>
/// Anillo de progreso circular dibujado con un <see cref="ArcSegment"/>.
/// </summary>
public partial class AnilloProgreso : UserControl
{
    private const double Cx = 105.0;

    private const double Cy = 105.0;

    private const double R = 91.0;

    private readonly PathFigure _figuraArco = new() { IsClosed = false };

    private readonly ArcSegment _segmentoArco = new()
    {
        Size = new Size(R, R),
        SweepDirection = SweepDirection.Clockwise
    };

    public static readonly DependencyProperty PercentProperty = DependencyProperty.Register(
        nameof(Percent), typeof(double), typeof(AnilloProgreso),
        new PropertyMetadata(100.0, (d, _) => ((AnilloProgreso)d).Redraw()));

    public static readonly DependencyProperty CenterTextProperty = DependencyProperty.Register(
        nameof(CenterText), typeof(string), typeof(AnilloProgreso),
        new PropertyMetadata("20:00", (d, e) => ((AnilloProgreso)d).CenterLabel.Text = (string)e.NewValue));

    public double Percent
    {
        get => (double)GetValue(PercentProperty);
        set => SetValue(PercentProperty, value);
    }

    public string CenterText
    {
        get => (string)GetValue(CenterTextProperty);
        set => SetValue(CenterTextProperty, value);
    }

    public Brush RingBrush
    {
        get => Arc.Stroke;
        set => Arc.Stroke = value;
    }

    public AnilloProgreso()
    {
        InitializeComponent();

        _figuraArco.StartPoint = new Point(Cx, Cy - R);
        _figuraArco.Segments.Add(_segmentoArco);
        Arc.Data = new PathGeometry { Figures = { _figuraArco } };

        Redraw();
    }

    private void Redraw()
    {
        double percent = Math.Clamp(Percent, 0.0, 100.0);
        double degrees = percent / 100.0 * 360.0;

        // Un arco de exactamente 360 grados degenera en un punto: se recorta.
        if (degrees >= 360.0)
        {
            degrees = 359.999;
        }

        double radians = degrees * Math.PI / 180.0;

        _segmentoArco.Point = new Point(Cx + R * Math.Sin(radians), Cy - R * Math.Cos(radians));
        _segmentoArco.IsLargeArc = degrees > 180.0;
    }
}
