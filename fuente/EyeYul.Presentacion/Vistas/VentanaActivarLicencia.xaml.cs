using System.Windows;
using System.Windows.Input;
using EyeYul.Aplicacion.Licencias;

namespace EyeYul.Presentacion.Vistas;

public partial class VentanaActivarLicencia : Window
{
    private readonly ServicioLicencia _licencia;

    public VentanaActivarLicencia(ServicioLicencia licencia)
    {
        InitializeComponent();
        _licencia = licencia;
        ActualizarEstado();
    }

    private void ActualizarEstado()
    {
        EstadoText.Text = _licencia.EsPremiumActivo
            ? "Premium ya está activo en este equipo. Puedes pegar otra clave para reemplazarla."
            : _licencia.DiasRestantesTrial > 0
                ? $"Estás en el período de prueba: quedan {_licencia.DiasRestantesTrial} día(s) con todo desbloqueado. Pega tu clave para activar Premium para siempre."
                : "El período de prueba terminó. Pega tu clave de licencia para reactivar las funciones Premium.";
    }

    private void OnDragMove(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton == MouseButton.Left)
        {
            DragMove();
        }
    }

    private void OnCerrarClick(object sender, RoutedEventArgs e) => Close();

    private async void OnActivarClick(object sender, RoutedEventArgs e)
    {
        string clave = ClaveTextBox.Text.Trim();
        bool ok = clave.Length > 0 && await _licencia.ActivarAsync(clave);

        if (ok)
        {
            VentanaDialogoAlerta.Mostrar("Premium activado", "Gracias por apoyar a EyeYul. Ya tienes todas las funciones desbloqueadas.", "IcoAward");
            Close();
            return;
        }

        ErrorText.Visibility = Visibility.Visible;
    }

    public static void Mostrar(ServicioLicencia licencia)
    {
        var dialog = new VentanaActivarLicencia(licencia);

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
