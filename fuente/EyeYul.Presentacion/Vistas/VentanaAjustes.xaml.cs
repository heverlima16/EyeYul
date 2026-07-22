using System.Windows;
using EyeYul.Presentacion.ModelosVista;

namespace EyeYul.Presentacion.Vistas;

public partial class VentanaAjustes : Window
{
    public VentanaAjustes(AjustesModelo viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
