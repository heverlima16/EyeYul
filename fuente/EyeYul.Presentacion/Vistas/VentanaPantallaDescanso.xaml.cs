using System.Windows;
using EyeYul.Presentacion.Interoperabilidad;

namespace EyeYul.Presentacion.Vistas;

public partial class VentanaPantallaDescanso : Window
{
    public event EventHandler? Skipped;

    public VentanaPantallaDescanso()
    {
        InitializeComponent();
        Loaded += (_, _) => DesenfoqueVentana.Habilitar(this, 0x59000000);
    }

    public void SetMessage(string message) => MessageText.Text = message;

    public void SetCountdown(TimeSpan remaining) =>
        CountdownText.Text = $"{(int)remaining.TotalMinutes:00}:{remaining.Seconds:00}";

    /// <summary>En monitores secundarios solo se muestra el fondo y el mensaje.</summary>
    public void ConfigureAsSecondary()
    {
        CountdownText.Visibility = Visibility.Collapsed;
        SkipButton.Visibility = Visibility.Collapsed;
    }

    public void SetSkipAllowed(bool allowed) =>
        SkipButton.Visibility = allowed ? Visibility.Visible : Visibility.Collapsed;

    private void OnSkip(object sender, RoutedEventArgs e) => Skipped?.Invoke(this, EventArgs.Empty);
}
