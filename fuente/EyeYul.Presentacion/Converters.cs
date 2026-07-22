using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace EyeYul.Presentacion;

/// <summary>Fondo suave cuando el valor coincide con el parametro (item activo).</summary>
public sealed class ActiveBackgroundConverter : IValueConverter
{
    public object Convert(object? value, Type t, object? parameter, CultureInfo c) =>
        string.Equals(value as string, parameter as string, StringComparison.Ordinal)
            ? (Brush)Application.Current.Resources["AccentSoftBrush"]
            : Brushes.Transparent;

    public object ConvertBack(object? value, Type t, object? parameter, CultureInfo c) =>
        throw new NotSupportedException();
}

public sealed class ActiveSolidBackgroundConverter : IValueConverter
{
    public object Convert(object? value, Type t, object? parameter, CultureInfo c) =>
        string.Equals(value as string, parameter as string, StringComparison.Ordinal)
            ? (Brush)Application.Current.Resources["AccentBrush"]
            : (Brush)Application.Current.Resources["SurfaceAltBrush"];

    public object ConvertBack(object? value, Type t, object? parameter, CultureInfo c) =>
        throw new NotSupportedException();
}

public sealed class ActiveForegroundConverter : IValueConverter
{
    public object Convert(object? value, Type t, object? parameter, CultureInfo c) =>
        string.Equals(value as string, parameter as string, StringComparison.Ordinal)
            ? (Brush)Application.Current.Resources["AccentBrush"]
            : (Brush)Application.Current.Resources["SubBrush"];

    public object ConvertBack(object? value, Type t, object? parameter, CultureInfo c) =>
        throw new NotSupportedException();
}

public sealed class ActiveInkForegroundConverter : IValueConverter
{
    public object Convert(object? value, Type t, object? parameter, CultureInfo c) =>
        string.Equals(value as string, parameter as string, StringComparison.Ordinal)
            ? Brushes.White
            : (Brush)Application.Current.Resources["InkBrush"];

    public object ConvertBack(object? value, Type t, object? parameter, CultureInfo c) =>
        throw new NotSupportedException();
}

public sealed class ActiveSubForegroundConverter : IValueConverter
{
    public object Convert(object? value, Type t, object? parameter, CultureInfo c) =>
        string.Equals(value as string, parameter as string, StringComparison.Ordinal)
            ? Brushes.White
            : (Brush)Application.Current.Resources["SubBrush"];

    public object ConvertBack(object? value, Type t, object? parameter, CultureInfo c) =>
        throw new NotSupportedException();
}

public sealed class ActiveChipTextConverter : IValueConverter
{
    public object Convert(object? value, Type t, object? parameter, CultureInfo c) =>
        string.Equals(value as string, parameter as string, StringComparison.Ordinal)
            ? "ACTIVO"
            : "PROBAR";

    public object ConvertBack(object? value, Type t, object? parameter, CultureInfo c) =>
        throw new NotSupportedException();
}

public sealed class StringToVisibilityConverter : IValueConverter
{
    public object Convert(object? value, Type t, object? parameter, CultureInfo c) =>
        string.Equals(value as string, parameter as string, StringComparison.Ordinal)
            ? Visibility.Visible
            : Visibility.Collapsed;

    public object ConvertBack(object? value, Type t, object? parameter, CultureInfo c) =>
        throw new NotSupportedException();
}

/// <summary>Visible solo cuando la coleccion esta vacia (estados "sin datos").</summary>
public sealed class CountToVisibilityInverseConverter : IValueConverter
{
    public object Convert(object? value, Type t, object? parameter, CultureInfo c) =>
        value is int count && count == 0 ? Visibility.Visible : Visibility.Collapsed;

    public object ConvertBack(object? value, Type t, object? parameter, CultureInfo c) =>
        throw new NotSupportedException();
}

/// <summary>Resuelve una clave de recurso a la Geometry del icono correspondiente.</summary>
public sealed class IconKeyToGeometryConverter : IValueConverter
{
    public object? Convert(object? value, Type t, object? parameter, CultureInfo c) =>
        value is string key ? Application.Current.Resources[key] as Geometry : null;

    public object ConvertBack(object? value, Type t, object? parameter, CultureInfo c) =>
        throw new NotSupportedException();
}

public sealed class InverseBoolConverter : IValueConverter
{
    public object Convert(object? value, Type t, object? parameter, CultureInfo c) =>
        value is not bool b || !b;

    public object ConvertBack(object? value, Type t, object? parameter, CultureInfo c) =>
        value is not bool b || !b;
}
