using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using EyeYul.Presentacion.ModelosVista;

namespace EyeYul.Presentacion.Vistas;

public partial class VentanaAjustes : Window
{
    private static readonly Regex NoEsDigito = new("[^0-9]", RegexOptions.Compiled);

    public VentanaAjustes(AjustesModelo modeloVista)
    {
        InitializeComponent();
        DataContext = modeloVista;

        // Pegar texto se salta PreviewTextInput, asi que se filtra aparte.
        DataObject.AddPastingHandler(this, AlPegar);
    }

    private void SoloDigitos(object emisor, TextCompositionEventArgs e)
    {
        e.Handled = NoEsDigito.IsMatch(e.Text);
    }

    private static void AlPegar(object emisor, DataObjectPastingEventArgs e)
    {
        if (emisor is not TextBox { Tag: "numerico" })
        {
            return;
        }

        string? pegado = e.DataObject.GetData(typeof(string)) as string;

        if (pegado is null || NoEsDigito.IsMatch(pegado))
        {
            e.CancelCommand();
        }
    }
}
