using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace EyeYul.Presentacion.Controles;

/// <summary>Boton que despliega una grilla de iconos para elegir uno (frases de bienestar).</summary>
public partial class SelectorIcono : UserControl
{
    public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register(
        nameof(ItemsSource), typeof(IEnumerable), typeof(SelectorIcono),
        new PropertyMetadata(null, (d, e) => ((SelectorIcono)d).Lista.ItemsSource = (IEnumerable?)e.NewValue));

    public static readonly DependencyProperty SeleccionadoProperty = DependencyProperty.Register(
        nameof(Seleccionado), typeof(string), typeof(SelectorIcono),
        new FrameworkPropertyMetadata(
            string.Empty,
            FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
            (d, e) => ((SelectorIcono)d).ActualizarIcono((string)e.NewValue)));

    public IEnumerable? ItemsSource
    {
        get => (IEnumerable?)GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }

    public string Seleccionado
    {
        get => (string)GetValue(SeleccionadoProperty);
        set => SetValue(SeleccionadoProperty, value);
    }

    public SelectorIcono()
    {
        InitializeComponent();
    }

    private void ActualizarIcono(string clave)
    {
        if (!string.IsNullOrEmpty(clave) && TryFindResource(clave) is Geometry geometria)
        {
            IconoActual.Data = geometria;
        }
    }

    private void OnAbrirClick(object sender, RoutedEventArgs e) => Desplegable.IsOpen = true;

    private void OnItemClick(object sender, RoutedEventArgs e)
    {
        if (sender is Button { DataContext: string valor })
        {
            Seleccionado = valor;
        }

        Desplegable.IsOpen = false;
    }
}
