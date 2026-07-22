using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EyeYul.Aplicacion.Abstracciones;
using EyeYul.Aplicacion.Estadisticas;
using EyeYul.Dominio.Entidades;
using EyeYul.Dominio.Reglas;

namespace EyeYul.Presentacion.ModelosVista;

public sealed partial class EstadisticasModelo : ObservableObject
{
    private readonly ServicioEstadisticas _estadisticas;

    private readonly IReloj _reloj;

    [ObservableProperty]
    private int _puntaje;

    [ObservableProperty]
    private string _calificacion = string.Empty;

    [ObservableProperty]
    private int _descansosTomados;

    [ObservableProperty]
    private int _descansosOmitidos;

    [ObservableProperty]
    private string _tiempoActivo = "0h 0m";

    public ObservableCollection<FilaUso> AppsPrincipales { get; } = [];

    public ObservableCollection<FilaUso> SitiosPrincipales { get; } = [];

    public EstadisticasModelo(ServicioEstadisticas estadisticas, IReloj reloj)
    {
        _estadisticas = estadisticas;
        _reloj = reloj;
    }

    [RelayCommand]
    public async Task RefrescarAsync()
    {
        ResumenDiario resumen = await _estadisticas.ObtenerResumenDiarioAsync(_reloj.Today);

        Puntaje = resumen.Puntaje;
        Calificacion = ReglasPuntajeVisual.Calificacion(resumen.Puntaje);
        DescansosTomados = resumen.DescansosTomados;
        DescansosOmitidos = resumen.DescansosOmitidos;
        TiempoActivo = Formatear(resumen.TiempoActivo);

        AppsPrincipales.Clear();
        foreach (UsoApp app in resumen.AppsPrincipales)
        {
            AppsPrincipales.Add(new FilaUso(app.NombreAmigable ?? app.NombreProceso, Formatear(app.PrimerPlano)));
        }

        SitiosPrincipales.Clear();
        foreach (UsoSitioWeb sitio in resumen.SitiosPrincipales)
        {
            SitiosPrincipales.Add(new FilaUso(sitio.Dominio, Formatear(sitio.TiempoActivo)));
        }
    }

    private static string Formatear(TimeSpan t) => $"{(int)t.TotalHours}h {t.Minutes}m";
}

public sealed record FilaUso(string Nombre, string Tiempo);
