using System.Windows;
using System.Windows.Media;
using Microsoft.Win32;

namespace EyeYul.Presentacion.Temas;

/// <summary>
/// Intercambia los brushes de la paleta en caliente (claro / oscuro / sistema).
/// Muta el color de los brushes existentes para que los bindings se refresquen solos.
/// </summary>
public static class TemaAplicador
{
    private static readonly string[] ClavesTemables =
    [
        "PanelBrush", "SidebarBrush", "SurfaceBrush", "SurfaceAltBrush", "CardBorderBrush",
        "BorderBrush", "CreamBrush", "InkBrush", "SubBrush", "MutedBrush", "AccentSoftBrush"
    ];

    private static string _currentTheme = "system";

    static TemaAplicador()
    {
        SystemEvents.UserPreferenceChanged += (_, e) =>
        {
            if (e.Category == UserPreferenceCategory.General && _currentTheme == "system")
            {
                Application.Current?.Dispatcher.BeginInvoke(() => Aplicar("system"));
            }
        };
    }

    public static void Aplicar(string theme)
    {
        _currentTheme = theme;

        bool dark = theme == "dark" || (theme == "system" && EsTemaOscuroDelSistema());

        var palette = new ResourceDictionary
        {
            Source = new Uri(dark
                ? "pack://application:,,,/EyeYul;component/Temas/PaletteDark.xaml"
                : "pack://application:,,,/EyeYul;component/Temas/Palette.xaml")
        };

        ResourceDictionary resources = Application.Current.Resources;

        foreach (string key in ClavesTemables)
        {
            if (palette[key] is not SolidColorBrush source)
            {
                continue;
            }

            if (resources[key] is SolidColorBrush target && !target.IsFrozen)
            {
                target.Color = source.Color;
            }
            else
            {
                resources[key] = new SolidColorBrush(source.Color);
            }
        }
    }

    public static bool EsTemaOscuroDelSistema()
    {
        try
        {
            using RegistryKey? key = Registry.CurrentUser.OpenSubKey(
                @"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize");

            return key?.GetValue("AppsUseLightTheme") is int light && light == 0;
        }
        catch
        {
            return false;
        }
    }
}
