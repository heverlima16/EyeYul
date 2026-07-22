using System.Windows;
using System.Windows.Controls;

namespace EyeYul.Presentacion.Componentes;

public partial class ElementoClaveValor : UserControl
{
    public static readonly DependencyProperty ClaveProperty = DependencyProperty.Register(
        nameof(Clave), typeof(string), typeof(ElementoClaveValor), new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty ValorProperty = DependencyProperty.Register(
        nameof(Valor), typeof(string), typeof(ElementoClaveValor), new PropertyMetadata(string.Empty));

    public string Clave
    {
        get => (string)GetValue(ClaveProperty);
        set => SetValue(ClaveProperty, value);
    }

    public string Valor
    {
        get => (string)GetValue(ValorProperty);
        set => SetValue(ValorProperty, value);
    }

    public ElementoClaveValor()
    {
        InitializeComponent();
    }
}
