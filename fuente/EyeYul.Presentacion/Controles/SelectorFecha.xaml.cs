using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace EyeYul.Presentacion.Controles;

/// <summary>Boton que despliega un calendario propio (mes + grilla de dias) para elegir una fecha.</summary>
public partial class SelectorFecha : UserControl
{
    private static readonly CultureInfo Es = CultureInfo.GetCultureInfo("es-ES");

    public static readonly DependencyProperty FechaProperty = DependencyProperty.Register(
        nameof(Fecha), typeof(DateTime?), typeof(SelectorFecha),
        new FrameworkPropertyMetadata(
            null,
            FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
            (d, _) => ((SelectorFecha)d).ActualizarEtiqueta()));

    private DateTime _mesMostrado = new(DateTime.Today.Year, DateTime.Today.Month, 1);

    public DateTime? Fecha
    {
        get => (DateTime?)GetValue(FechaProperty);
        set => SetValue(FechaProperty, value);
    }

    public SelectorFecha()
    {
        InitializeComponent();
        ActualizarEtiqueta();
    }

    private void ActualizarEtiqueta()
    {
        Etiqueta.Text = Fecha?.ToString("dd MMM yyyy", Es) ?? "Elegir fecha";

        if (Fecha is { } fecha)
        {
            _mesMostrado = new DateTime(fecha.Year, fecha.Month, 1);
        }
    }

    private void OnAbrirClick(object sender, RoutedEventArgs e)
    {
        ConstruirGrilla();
        Desplegable.IsOpen = true;
    }

    private void OnMesAnteriorClick(object sender, RoutedEventArgs e)
    {
        _mesMostrado = _mesMostrado.AddMonths(-1);
        ConstruirGrilla();
    }

    private void OnMesSiguienteClick(object sender, RoutedEventArgs e)
    {
        _mesMostrado = _mesMostrado.AddMonths(1);
        ConstruirGrilla();
    }

    private void ConstruirGrilla()
    {
        TituloMes.Text = _mesMostrado.ToString("MMMM yyyy", Es);
        Dias.Children.Clear();

        // La semana en la grilla empieza en lunes: DayOfWeek.Monday=1 ... Sunday=0.
        int offset = ((int)_mesMostrado.DayOfWeek + 6) % 7;
        DateTime primerDiaGrilla = _mesMostrado.AddDays(-offset);

        for (int i = 0; i < 42; i++)
        {
            DateTime dia = primerDiaGrilla.AddDays(i);
            Dias.Children.Add(CrearBotonDia(dia));
        }
    }

    private Button CrearBotonDia(DateTime dia)
    {
        bool delMesActual = dia.Month == _mesMostrado.Month;
        bool esSeleccionado = Fecha?.Date == dia.Date;

        var boton = new Button
        {
            Content = dia.Day.ToString(),
            Margin = new Thickness(1),
            Height = 26,
            Background = Brushes.Transparent,
            BorderThickness = new Thickness(0),
            Cursor = Cursors.Hand,
            FontSize = 11,
            FontWeight = dia.Date == DateTime.Today ? FontWeights.Bold : FontWeights.Normal,
            Foreground = delMesActual
                ? (Brush)FindResource("InkBrush")
                : (Brush)FindResource("MutedBrush")
        };

        if (esSeleccionado)
        {
            boton.Background = (Brush)FindResource("AccentBrush");
            boton.Foreground = Brushes.White;
        }

        var plantilla = new ControlTemplate(typeof(Button));
        var borde = new FrameworkElementFactory(typeof(Border));
        borde.SetValue(Border.BackgroundProperty, new System.Windows.Data.Binding("Background") { RelativeSource = System.Windows.Data.RelativeSource.TemplatedParent });
        borde.SetValue(Border.CornerRadiusProperty, new CornerRadius(6));
        var contenido = new FrameworkElementFactory(typeof(ContentPresenter));
        contenido.SetValue(HorizontalAlignmentProperty, HorizontalAlignment.Center);
        contenido.SetValue(VerticalAlignmentProperty, VerticalAlignment.Center);
        borde.AppendChild(contenido);
        plantilla.VisualTree = borde;
        boton.Template = plantilla;

        boton.Click += (_, _) =>
        {
            Fecha = dia.Date;
            Desplegable.IsOpen = false;
        };

        return boton;
    }
}
