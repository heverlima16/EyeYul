using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace EyeYul.Presentacion;

/// <summary>Fondo suave cuando el valor coincide con el parametro (item activo).</summary>
public sealed class ActiveBackgroundConverter : IValueConverter
{
    public object Convert(object? valor, Type tipo, object? parametro, CultureInfo cultura) =>
        string.Equals(valor?.ToString(), parametro as string, StringComparison.Ordinal)
            ? (Brush)Application.Current.Resources["AccentSoftBrush"]
            : Brushes.Transparent;

    public object ConvertBack(object? valor, Type tipo, object? parametro, CultureInfo cultura) =>
        throw new NotSupportedException();
}

public sealed class ActiveSolidBackgroundConverter : IValueConverter
{
    public object Convert(object? valor, Type tipo, object? parametro, CultureInfo cultura) =>
        string.Equals(valor?.ToString(), parametro as string, StringComparison.Ordinal)
            ? (Brush)Application.Current.Resources["AccentBrush"]
            : (Brush)Application.Current.Resources["SurfaceAltBrush"];

    public object ConvertBack(object? valor, Type tipo, object? parametro, CultureInfo cultura) =>
        throw new NotSupportedException();
}

public sealed class ActiveForegroundConverter : IValueConverter
{
    public object Convert(object? valor, Type tipo, object? parametro, CultureInfo cultura) =>
        string.Equals(valor?.ToString(), parametro as string, StringComparison.Ordinal)
            ? (Brush)Application.Current.Resources["AccentBrush"]
            : (Brush)Application.Current.Resources["SubBrush"];

    public object ConvertBack(object? valor, Type tipo, object? parametro, CultureInfo cultura) =>
        throw new NotSupportedException();
}

public sealed class ActiveInkForegroundConverter : IValueConverter
{
    public object Convert(object? valor, Type tipo, object? parametro, CultureInfo cultura) =>
        string.Equals(valor?.ToString(), parametro as string, StringComparison.Ordinal)
            ? Brushes.White
            : (Brush)Application.Current.Resources["InkBrush"];

    public object ConvertBack(object? valor, Type tipo, object? parametro, CultureInfo cultura) =>
        throw new NotSupportedException();
}

public sealed class ActiveSubForegroundConverter : IValueConverter
{
    public object Convert(object? valor, Type tipo, object? parametro, CultureInfo cultura) =>
        string.Equals(valor?.ToString(), parametro as string, StringComparison.Ordinal)
            ? Brushes.White
            : (Brush)Application.Current.Resources["SubBrush"];

    public object ConvertBack(object? valor, Type tipo, object? parametro, CultureInfo cultura) =>
        throw new NotSupportedException();
}

public sealed class StringToVisibilityConverter : IValueConverter
{
    public object Convert(object? valor, Type tipo, object? parametro, CultureInfo cultura) =>
        string.Equals(valor?.ToString(), parametro as string, StringComparison.Ordinal)
            ? Visibility.Visible
            : Visibility.Collapsed;

    public object ConvertBack(object? valor, Type tipo, object? parametro, CultureInfo cultura) =>
        throw new NotSupportedException();
}

/// <summary>Resuelve una clave de recurso a la Geometry del icono correspondiente.</summary>
public sealed class IconKeyToGeometryConverter : IValueConverter
{
    public object? Convert(object? valor, Type tipo, object? parametro, CultureInfo cultura) =>
        valor is string clave ? Application.Current.Resources[clave] as Geometry : null;

    public object ConvertBack(object? valor, Type tipo, object? parametro, CultureInfo cultura) =>
        throw new NotSupportedException();
}

public sealed class InverseBoolConverter : IValueConverter
{
    public object Convert(object? valor, Type tipo, object? parametro, CultureInfo cultura) =>
        valor is not bool b || !b;

    public object ConvertBack(object? valor, Type tipo, object? parametro, CultureInfo cultura) =>
        valor is not bool b || !b;
}

/// <summary>Sin margen cuando la ventana está maximizada: llena el área de trabajo, sin hueco.</summary>
public sealed class EstadoVentanaAMargenConverter : IValueConverter
{
    public object Convert(object? valor, Type tipo, object? parametro, CultureInfo cultura) =>
        valor is WindowState.Maximized ? new Thickness(0) : new Thickness(12);

    public object ConvertBack(object? valor, Type tipo, object? parametro, CultureInfo cultura) =>
        throw new NotSupportedException();
}

/// <summary>Esquinas cuadradas cuando la ventana está maximizada: nada de redondeado asomando fuera de pantalla.</summary>
public sealed class EstadoVentanaARadioConverter : IValueConverter
{
    public object Convert(object? valor, Type tipo, object? parametro, CultureInfo cultura) =>
        valor is WindowState.Maximized ? new CornerRadius(0) : new CornerRadius(14);

    public object ConvertBack(object? valor, Type tipo, object? parametro, CultureInfo cultura) =>
        throw new NotSupportedException();
}

public sealed class InverseBoolToVisibilityConverter : IValueConverter
{
    public object Convert(object? valor, Type tipo, object? parametro, CultureInfo cultura) =>
        valor is true ? Visibility.Collapsed : Visibility.Visible;

    public object ConvertBack(object? valor, Type tipo, object? parametro, CultureInfo cultura) =>
        throw new NotSupportedException();
}

/// <summary>"Guardar cambios" mientras se edita una frase propia, "+ Añadir" el resto del tiempo.</summary>
public sealed class EditandoATextoConverter : IValueConverter
{
    public object Convert(object? valor, Type tipo, object? parametro, CultureInfo cultura) =>
        valor is true ? "Guardar cambios" : "+ Añadir";

    public object ConvertBack(object? valor, Type tipo, object? parametro, CultureInfo cultura) =>
        throw new NotSupportedException();
}

/// <summary>Atenúa el texto de una frase deshabilitada sin ocultarla de la lista.</summary>
public sealed class BoolToOpacityConverter : IValueConverter
{
    public object Convert(object? valor, Type tipo, object? parametro, CultureInfo cultura) =>
        valor is true ? 1.0 : 0.5;

    public object ConvertBack(object? valor, Type tipo, object? parametro, CultureInfo cultura) =>
        throw new NotSupportedException();
}
