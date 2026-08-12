using System.Windows;
using System.Windows.Media;

namespace EyeYul.Presentacion.Vistas;

public partial class VentanaFiltroLuz : Window
{
    public VentanaFiltroLuz()
    {
        InitializeComponent();
    }

    public void Colorear(Color tinte, byte alfaTinte, byte alfaAtenuacion)
    {
        Tinte.Background = new SolidColorBrush(Color.FromArgb(alfaTinte, tinte.R, tinte.G, tinte.B));
        Atenuacion.Background = new SolidColorBrush(Color.FromArgb(alfaAtenuacion, 0, 0, 0));
    }
}
