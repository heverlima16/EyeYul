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
    /// <summary>
    /// Frases que se muestran durante la pausa cuando el usuario no ha escrito las suyas.
    /// Se rotan al azar para que la pausa no se vuelva un mensaje que se ignora de tanto verlo.
    /// </summary>
    public static readonly IReadOnlyList<string> FrasesPorDefecto =
    [
        "Mira a 6 metros de distancia y relaja la vista.",
        "Parpadea despacio unas cuantas veces. Tus ojos lo agradecen.",
        "Estira el cuello: oreja al hombro, sin prisa, a cada lado.",
        "Endereza la espalda y baja los hombros.",
        "Levantate y da unos pasos. Aunque sean pocos.",
        "Respira hondo tres veces. Suelta el aire despacio.",
        "Suelta el raton y abre y cierra las manos.",
        "Bebe un poco de agua.",
        "Mira por la ventana. Lo lejano descansa la vista.",
        "Rota los tobillos y estira las piernas.",
        "Cierra los ojos unos segundos. No pasa nada.",
        "Afloja la mandibula: solemos apretarla sin darnos cuenta."
    ];

    public string Theme { get; set; } = "system";

    public bool PlaySound { get; set; } = true;

    public string? OverlayBackgroundPath { get; set; }

    public string BreakMessage { get; set; } = "Mira a 6 metros de distancia y relaja la vista.";

    public List<string> CustomMessages { get; set; } = [.. FrasesPorDefecto];

    public bool ShowFloatingCountdown { get; set; } = false;
}

public sealed class GeneralSettings
{
    public bool StartWithWindows { get; set; } = true;

    public bool StartMinimized { get; set; } = true;

    public string? OnBreakStartCommand { get; set; }

    public string? OnBreakEndCommand { get; set; }
}
