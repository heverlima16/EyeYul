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

    public static IReadOnlyList<string> IntervalosDisponibles { get; } =
        new[] { 5, 10, 15, 20, 25, 30, 45, 60, 90, 120 }.Select(m => m.ToString()).ToList();

    public static IReadOnlyList<string> DuracionesDisponibles { get; } =
        new[] { 10, 15, 20, 30, 45, 60, 90, 120 }.Select(s => s.ToString()).ToList();

    /// <summary>Espejo en texto de <see cref="IntervalMinutes"/> para el selector editable.</summary>
    public string IntervalMinutesText
    {
        get => ((int)IntervalMinutes).ToString();
        set
        {
            if (int.TryParse(value, out int minutos))
            {
                IntervalMinutes = minutos;
            }
        }
    }

    /// <summary>Espejo en texto de <see cref="DurationSeconds"/> para el selector editable.</summary>
    public string DurationSecondsText
    {
        get => ((int)DurationSeconds).ToString();
        set
        {
            if (int.TryParse(value, out int segundos))
            {
                DurationSeconds = segundos;
            }
        }
    }

    partial void OnIntervalMinutesChanged(double value) => OnPropertyChanged(nameof(IntervalMinutesText));

    partial void OnDurationSecondsChanged(double value) => OnPropertyChanged(nameof(DurationSecondsText));

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

        // Los campos se digitan, asi que se recortan al rango valido y se
        // devuelve el valor corregido a la UI en vez de guardar algo absurdo.
        IntervalMinutes = Math.Clamp(IntervalMinutes, 1.0, 120.0);
        DurationSeconds = Math.Clamp(DurationSeconds, 5.0, 600.0);
        MaxSnoozesPerDay = Math.Clamp(MaxSnoozesPerDay, 0, 30);

        s.Descansos.Interval = TimeSpan.FromMinutes(IntervalMinutes);
        s.Descansos.Duration = TimeSpan.FromSeconds(DurationSeconds);
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
