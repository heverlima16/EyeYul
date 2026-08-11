using System.Windows;
using EyeYul.Presentacion.ModelosVista;

namespace EyeYul.Presentacion.Vistas;

public partial class VentanaAjustes : Window
{
    public VentanaAjustes(AjustesModelo modeloVista)
    {
        InitializeComponent();
        DataContext = modeloVista;
    }
}
