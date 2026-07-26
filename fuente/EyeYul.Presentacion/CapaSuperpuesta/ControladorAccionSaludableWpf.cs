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
        });

        if (ventana is not null)
        {
            await ventana.EsperarCierreAsync();
        }
    }
}
