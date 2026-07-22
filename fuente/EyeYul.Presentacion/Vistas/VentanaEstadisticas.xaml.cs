using System.Windows;
using EyeYul.Presentacion.ModelosVista;

namespace EyeYul.Presentacion.Vistas;

public partial class VentanaEstadisticas : Window
{
    public VentanaEstadisticas(EstadisticasModelo viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
        Loaded += async (_, _) => await viewModel.RefrescarAsync();
    }
}
