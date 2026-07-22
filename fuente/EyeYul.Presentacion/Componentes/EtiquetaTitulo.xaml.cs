using System.Windows;
using System.Windows.Controls;

namespace EyeYul.Presentacion.Componentes;

public partial class EtiquetaTitulo : UserControl
{
    public static readonly DependencyProperty TextoProperty = DependencyProperty.Register(
        nameof(Texto), typeof(string), typeof(EtiquetaTitulo), new PropertyMetadata(string.Empty));

    public string Texto
    {
        get => (string)GetValue(TextoProperty);
        set => SetValue(TextoProperty, value);
    }

    public EtiquetaTitulo()
    {
        InitializeComponent();
    }
}
