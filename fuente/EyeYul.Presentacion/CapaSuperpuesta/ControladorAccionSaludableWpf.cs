using System.Windows;
using EyeYul.Aplicacion.Abstracciones;
using EyeYul.Dominio.Enumeraciones;
using EyeYul.Presentacion.Vistas;

namespace EyeYul.Presentacion.CapaSuperpuesta;

public sealed class ControladorAccionSaludableWpf : IControladorAccionSaludable
{
    public async Task MostrarAsync(TipoAccionSaludable tipo, CancellationToken ct = default)
    {
        VentanaAccionSaludable? ventana = null;

        await Application.Current.Dispatcher.InvokeAsync(() =>
        {
            ventana = new VentanaAccionSaludable(tipo);
            ventana.Show();

            // Posicionado en pixeles fisicos DESPUES de Show() (ver ControladorPantallaDescansoWpf):
            // evita fallos con DPI mixto y deja que la ventana ya tenga su superficie en capas
            // creada antes de estirarla a pantalla completa.
            // PrimaryScreen puede devolver null (p.ej. reconfiguracion de monitores en curso);
            // igual que ControladorPantallaDescansoWpf, se cae al primer monitor disponible.
            System.Windows.Forms.Screen? pantalla = System.Windows.Forms.Screen.PrimaryScreen
                ?? System.Windows.Forms.Screen.AllScreens.FirstOrDefault();

            if (pantalla is not null)
            {
                ventana.PosicionarPantallaCompleta(pantalla.Bounds);
            }
        });

        if (ventana is not null)
        {
            await ventana.EsperarCierreAsync();
        }
    }
}
