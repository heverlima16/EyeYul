using System.Windows;
using System.Windows.Input;
using EyeYul.Presentacion.ModelosVista;

namespace EyeYul.Presentacion.Vistas;

public partial class VentanaPrincipal : Window
{
    public VentanaPrincipal(VistaGeneralModelo viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;

        // Solo refrescar en vivo mientras la ventana este visible.
        IsVisibleChanged += (_, _) => viewModel.ActualizacionesEnVivo = IsVisible;
    }

    // El WindowChrome (CaptionHeight) ya cubre la franja superior de 32px con los botones
    // de minimizar/maximizar/cerrar; esta franja arrastra el resto de encabezados (sidebar
    // y contenido) que quedan fuera de esa franja nativa.
    private void OnDragMove(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton == MouseButton.Left)
        {
            DragMove();
        }
    }

    // Cerrar solo oculta: la app sigue viva en la bandeja.
    private void OnCloseClick(object sender, RoutedEventArgs e) => Hide();

    private void OnMinimizeClick(object sender, RoutedEventArgs e) =>
        WindowState = WindowState.Minimized;

    private void OnMaximizeClick(object sender, RoutedEventArgs e) =>
        WindowState = WindowState == WindowState.Maximized
            ? WindowState.Normal
            : WindowState.Maximized;
}
