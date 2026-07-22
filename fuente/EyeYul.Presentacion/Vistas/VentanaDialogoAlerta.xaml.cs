using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace EyeYul.Presentacion.Vistas;

public partial class VentanaDialogoAlerta : Window
{
    public VentanaDialogoAlerta(string titulo, string mensaje, string icono = "IcoBell")
    {
        InitializeComponent();

        TituloText.Text = titulo;
        MensajeText.Text = mensaje;

        if (TryFindResource(icono) is Geometry data)
        {
            Icono.Data = data;
        }
    }

    private void OnDragMove(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton == MouseButton.Left)
        {
            DragMove();
        }
    }

    private void OnAceptarClick(object sender, RoutedEventArgs e) => Close();

    public static void Mostrar(string titulo, string mensaje, string icono = "IcoBell")
    {
        var dialog = new VentanaDialogoAlerta(titulo, mensaje, icono);

        Window? owner = Application.Current.Windows.OfType<Window>().FirstOrDefault(w => w.IsActive)
                        ?? Application.Current.MainWindow;

        if (owner is not null && owner.IsVisible && owner != dialog)
        {
            dialog.Owner = owner;
            dialog.WindowStartupLocation = WindowStartupLocation.CenterOwner;
        }
        else
        {
            dialog.WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }

        dialog.ShowDialog();
    }
}
