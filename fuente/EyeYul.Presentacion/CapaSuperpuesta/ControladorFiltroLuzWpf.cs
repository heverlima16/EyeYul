using System.Drawing;
using System.Windows.Forms;
using EyeYul.Aplicacion.Abstracciones;
using EyeYul.Aplicacion.Configuracion;
using EyeYul.Presentacion.Interoperabilidad;
using EyeYul.Presentacion.Vistas;
using Application = System.Windows.Application;
using MediaColor = System.Windows.Media.Color;

namespace EyeYul.Presentacion.CapaSuperpuesta;

/// <summary>
/// Tinte de luz azul en pantalla completa: una <see cref="VentanaFiltroLuz"/> click-through
/// por monitor, igual patron que <see cref="ControladorPantallaDescansoWpf"/>. No es gamma
/// ramp real (ver IControladorFiltroLuz): un overlay coloreado es mas simple y no deja la
/// pantalla en un estado raro si la app crashea antes de restaurarla.
/// </summary>
public sealed class ControladorFiltroLuzWpf : IControladorFiltroLuz
{
    private readonly List<VentanaFiltroLuz> _ventanas = [];

    public void Aplicar(BlueLightFilterSettings settings)
    {
        Application.Current?.Dispatcher.Invoke(() =>
        {
            if (!settings.Enabled)
            {
                CerrarTodas();
                return;
            }

            (MediaColor tinte, byte alfaTinte) = TemperaturaATinte(settings.KelvinTemp);
            byte alfaAtenuacion = (byte)Math.Clamp(settings.DimLevel * 255 / 100, 0, 255);

            AsegurarUnaVentanaPorMonitor();

            foreach (VentanaFiltroLuz v in _ventanas)
            {
                v.Colorear(tinte, alfaTinte, alfaAtenuacion);
            }
        });
    }

    public void Detener() => Application.Current?.Dispatcher.Invoke(CerrarTodas);

    private void AsegurarUnaVentanaPorMonitor()
    {
        if (_ventanas.Count > 0)
        {
            return;
        }

        foreach (Screen screen in Screen.AllScreens)
        {
            var v = new VentanaFiltroLuz();
            v.Show();

            // Posicionado en pixeles fisicos (ver ControladorPantallaDescansoWpf): evita
            // fallos con DPI mixto entre monitores.
            Rectangle b = screen.Bounds;
            WindowNative.PlaceAtPhysical(v, b.X, b.Y, b.Width, b.Height);
            WindowNative.MakeClickThrough(v);

            _ventanas.Add(v);
        }
    }

    private void CerrarTodas()
    {
        foreach (VentanaFiltroLuz v in _ventanas)
        {
            v.Close();
        }

        _ventanas.Clear();
    }

    /// <summary>Cuanto mas calida la temperatura (Kelvin bajo), mas naranja y mas opaco el
    /// tinte. Misma escala que el mockup de referencia (getFilterOverlayStyle).</summary>
    private static (MediaColor Tinte, byte Alfa) TemperaturaATinte(int kelvin) => kelvin switch
    {
        <= 2200 => (MediaColor.FromRgb(255, 120, 0), (byte)71),
        <= 3200 => (MediaColor.FromRgb(255, 160, 50), (byte)51),
        <= 4200 => (MediaColor.FromRgb(255, 190, 100), (byte)31),
        _ => (MediaColor.FromRgb(255, 220, 150), (byte)13)
    };
}
