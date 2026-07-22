using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EyeYul.Aplicacion.Abstracciones;
using EyeYul.Aplicacion.Configuracion;

namespace EyeYul.Presentacion.ModelosVista;

public sealed partial class AjustesModelo : ObservableObject
{
    private readonly IAlmacenAjustes _store;

    [ObservableProperty]
    private double _intervalMinutes;

    [ObservableProperty]
    private double _durationSeconds;

    [ObservableProperty]
    private bool _smartPauseEnabled;

    [ObservableProperty]
    private bool _respectFullscreen;

    [ObservableProperty]
    private bool _respectMeetings;

    [ObservableProperty]
    private bool _respectMedia;

    [ObservableProperty]
    private bool _startWithWindows;

    [ObservableProperty]
    private bool _playSound;

    [ObservableProperty]
    private bool _showFloatingCountdown;

    [ObservableProperty]
    private int _maxSnoozesPerDay;

    [ObservableProperty]
    private string _breakMessage = string.Empty;

    [ObservableProperty]
    private string _statusMessage = string.Empty;

    public AjustesModelo(IAlmacenAjustes store)
    {
        _store = store;
        LoadFrom(store.Current);
    }

    private void LoadFrom(AjustesEyeYul s)
    {
        IntervalMinutes = s.Descansos.Interval.TotalMinutes;
        DurationSeconds = s.Descansos.Duration.TotalSeconds;
        SmartPauseEnabled = s.SmartPause.Enabled;
        RespectFullscreen = s.SmartPause.RespectFullscreen;
        RespectMeetings = s.SmartPause.RespectMeetings;
        RespectMedia = s.SmartPause.RespectMediaPlayback;
        StartWithWindows = s.General.StartWithWindows;
        PlaySound = s.Appearance.PlaySound;
        ShowFloatingCountdown = s.Appearance.ShowFloatingCountdown;
        MaxSnoozesPerDay = s.AplicacionReglas.MaxSnoozesPerDay;
        BreakMessage = s.Appearance.BreakMessage;
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        AjustesEyeYul s = _store.Current;

        s.Descansos.Interval = TimeSpan.FromMinutes(Math.Max(1.0, IntervalMinutes));
        s.Descansos.Duration = TimeSpan.FromSeconds(Math.Max(5.0, DurationSeconds));
        s.SmartPause.Enabled = SmartPauseEnabled;
        s.SmartPause.RespectFullscreen = RespectFullscreen;
        s.SmartPause.RespectMeetings = RespectMeetings;
        s.SmartPause.RespectMediaPlayback = RespectMedia;
        s.General.StartWithWindows = StartWithWindows;
        s.Appearance.PlaySound = PlaySound;
        s.Appearance.ShowFloatingCountdown = ShowFloatingCountdown;
        s.AplicacionReglas.MaxSnoozesPerDay = MaxSnoozesPerDay;
        s.Appearance.BreakMessage = BreakMessage;

        await _store.SaveAsync(s);

        StatusMessage = $"Guardado {DateTime.Now:HH:mm:ss}";
    }
}
