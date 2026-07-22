using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace EyeYul.Presentacion.Componentes;

public partial class BotonRelleno : UserControl
{
    public static readonly DependencyProperty TextoProperty = DependencyProperty.Register(
        nameof(Texto), typeof(string), typeof(BotonRelleno), new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty ComandoProperty = DependencyProperty.Register(
        nameof(Comando), typeof(ICommand), typeof(BotonRelleno), new PropertyMetadata(null));

    public static readonly DependencyProperty ParametroComandoProperty = DependencyProperty.Register(
        nameof(ParametroComando), typeof(object), typeof(BotonRelleno), new PropertyMetadata(null));

    public string Texto
    {
        get => (string)GetValue(TextoProperty);
        set => SetValue(TextoProperty, value);
    }

    public ICommand Comando
    {
        get => (ICommand)GetValue(ComandoProperty);
        set => SetValue(ComandoProperty, value);
    }

    public object ParametroComando
    {
        get => GetValue(ParametroComandoProperty);
        set => SetValue(ParametroComandoProperty, value);
    }

    public BotonRelleno()
    {
        InitializeComponent();
    }
}
