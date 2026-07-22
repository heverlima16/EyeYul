namespace EyeYul.Aplicacion.Configuracion;

public sealed class AjustesEyeYul
{
    public BreakSettings Descansos { get; set; } = new();

    public SmartPauseSettings SmartPause { get; set; } = new();

    public EnforcementSettings AplicacionReglas { get; set; } = new();

    public AppearanceSettings Appearance { get; set; } = new();

    public GeneralSettings General { get; set; } = new();
}

public sealed class BreakSettings
{
    public TimeSpan Interval { get; set; } = TimeSpan.FromMinutes(20);

    public TimeSpan Duration { get; set; } = TimeSpan.FromSeconds(20);

    public bool MicroBreaksEnabled { get; set; } = true;

    public TimeSpan MicroBreakInterval { get; set; } = TimeSpan.FromMinutes(20);

    public TimeSpan MicroBreakDuration { get; set; } = TimeSpan.FromSeconds(20);

    public TimeSpan PreBreakWarning { get; set; } = TimeSpan.FromSeconds(10);

    public bool AllowSkip { get; set; } = true;
}

public sealed class SmartPauseSettings
{
    public bool Enabled { get; set; } = true;

    public bool RespectFullscreen { get; set; } = true;

    public bool RespectMeetings { get; set; } = true;

    public bool RespectMediaPlayback { get; set; } = true;

    public bool RespectFocusAssist { get; set; } = true;

    public TimeSpan IdleThreshold { get; set; } = TimeSpan.FromMinutes(3);
}

public sealed class EnforcementSettings
{
    public int MaxSnoozesPerDay { get; set; } = 10;

    public int MaxSnoozesPerBreak { get; set; } = 3;

    public bool StrictMode { get; set; }
}

public sealed class AppearanceSettings
{
    public string Theme { get; set; } = "system";

    public bool PlaySound { get; set; } = true;

    public string? OverlayBackgroundPath { get; set; }

    public string BreakMessage { get; set; } = "Mira a 6 metros de distancia y relaja la vista.";

    public List<string> CustomMessages { get; set; } = new();

    public bool ShowFloatingCountdown { get; set; } = false;
}

public sealed class GeneralSettings
{
    public bool StartWithWindows { get; set; } = true;

    public bool StartMinimized { get; set; } = true;

    public string? OnBreakStartCommand { get; set; }

    public string? OnBreakEndCommand { get; set; }
}
