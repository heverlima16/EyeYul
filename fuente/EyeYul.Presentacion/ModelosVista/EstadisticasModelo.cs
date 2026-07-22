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
    private readonly ServicioEstadisticas _stats;

    private readonly IReloj _clock;

    [ObservableProperty]
    private int _score;

    [ObservableProperty]
    private string _grade = string.Empty;

    [ObservableProperty]
    private int _breaksTaken;

    [ObservableProperty]
    private int _breaksSkipped;

    [ObservableProperty]
    private string _activeTime = "0h 0m";

    public ObservableCollection<UsageRow> TopApps { get; } = [];

    public ObservableCollection<UsageRow> TopSites { get; } = [];

    public EstadisticasModelo(ServicioEstadisticas stats, IReloj clock)
    {
        _stats = stats;
        _clock = clock;
    }

    [RelayCommand]
    public async Task RefreshAsync()
    {
        DailyStatsSummary summary = await _stats.GetDailySummaryAsync(_clock.Today);

        Score = summary.Score;
        Grade = ScreenScoreRules.Grade(summary.Score);
        BreaksTaken = summary.BreaksTaken;
        BreaksSkipped = summary.BreaksSkipped;
        ActiveTime = Format(summary.ActiveTime);

        TopApps.Clear();
        foreach (UsoApp a in summary.TopApps)
        {
            TopApps.Add(new UsageRow(a.FriendlyName ?? a.ProcessName, Format(a.Foreground)));
        }

        TopSites.Clear();
        foreach (UsoSitioWeb w in summary.TopSites)
        {
            TopSites.Add(new UsageRow(w.Domain, Format(w.ActiveTime)));
        }
    }

    private static string Format(TimeSpan t) => $"{(int)t.TotalHours}h {t.Minutes}m";
}

public sealed record UsageRow(string Name, string Time);
