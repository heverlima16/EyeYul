using System.Collections;
using System.Windows;
using System.Windows.Controls;

namespace EyeYul.Presentacion.Controles;

/// <summary>Boton que despliega una lista de opciones para elegir (fecha, dia del mes...).</summary>
public partial class SelectorLista : UserControl
{
    public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register(
        nameof(ItemsSource), typeof(IEnumerable), typeof(SelectorLista),
        new PropertyMetadata(null, (d, e) => ((SelectorLista)d).Lista.ItemsSource = (IEnumerable?)e.NewValue));

    public static readonly DependencyProperty SeleccionadoProperty = DependencyProperty.Register(
        nameof(Seleccionado), typeof(string), typeof(SelectorLista),
        new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

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

    public SelectorLista()
    {
        InitializeComponent();
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
