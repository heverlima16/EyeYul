namespace EyeYul.Aplicacion.Configuracion;

public sealed class AjustesEyeYul
{
    public BreakSettings Descansos { get; set; } = new();

    public SmartPauseSettings SmartPause { get; set; } = new();

    public EnforcementSettings AplicacionReglas { get; set; } = new();

    public AppearanceSettings Appearance { get; set; } = new();

    public GeneralSettings General { get; set; } = new();

    public AccionesSaludablesSettings AccionesSaludables { get; set; } = new();
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

    /// <summary>
    /// Frases que se muestran durante la pausa, rotadas al azar entre las habilitadas.
    /// Las de fábrica (<see cref="FraseBienestar.EsPredefinida"/>) solo se pueden
    /// habilitar o deshabilitar; las que crea el usuario se pueden editar y eliminar.
    /// </summary>
    public List<FraseBienestar> Frases { get; set; } = CrearFrasesPorDefecto();

    public bool ShowFloatingCountdown { get; set; } = false;

    public static List<FraseBienestar> CrearFrasesPorDefecto() =>
    [
        Predefinida("Mira a 6 metros de distancia y relaja la vista.", "IcoEye"),
        Predefinida("Parpadea despacio unas cuantas veces. Tus ojos lo agradecen.", "IcoEye"),
        Predefinida("Parpadea rápido diez veces para humectar los ojos.", "IcoEye"),
        Predefinida("Enfoca la mirada en algo lejano por 20 segundos.", "IcoEye"),
        Predefinida("Mira por la ventana. Lo lejano descansa la vista.", "IcoEye"),
        Predefinida("Revisa la distancia entre tus ojos y la pantalla.", "IcoEye"),
        Predefinida("Estira el cuello: oreja al hombro, sin prisa, a cada lado.", "IcoUserCheck"),
        Predefinida("Afloja la mandíbula: solemos apretarla sin darnos cuenta.", "IcoUserCheck"),
        Predefinida("Relaja la mandíbula y los hombros.", "IcoUserCheck"),
        Predefinida("Endereza la espalda y baja los hombros.", "IcoChevronsUp"),
        Predefinida("Ajusta tu postura: pies apoyados, espalda recta.", "IcoChevronsUp"),
        Predefinida("Ponte de pie y estira la espalda hacia atrás con cuidado.", "IcoChevronsUp"),
        Predefinida("Levántate y da unos pasos. Aunque sean pocos.", "IcoActivity"),
        Predefinida("Rota los tobillos y estira las piernas.", "IcoActivity"),
        Predefinida("Camina hasta la cocina y estírate un poco.", "IcoActivity"),
        Predefinida("Estira los brazos hacia arriba y respira profundo.", "IcoActivity"),
        Predefinida("Respira hondo tres veces. Suelta el aire despacio.", "IcoHeart"),
        Predefinida("Cierra los ojos unos segundos. No pasa nada.", "IcoMoon"),
        Predefinida("Baja el volumen mental: cierra los ojos y respira.", "IcoMoon"),
        Predefinida("Suelta el ratón y abre y cierra las manos.", "IcoZap"),
        Predefinida("Sacude las manos y mueve las muñecas en círculos.", "IcoZap"),
        Predefinida("Bebe un poco de agua.", "IcoDroplet"),
        Predefinida("Toma un vaso de agua fresca antes de seguir.", "IcoDroplet"),
        Predefinida("Bebe agua: tu cuerpo (y tu concentración) lo necesitan.", "IcoDroplet"),
        Predefinida("Aléjate un momento del teclado y estira los dedos.", "IcoKeyboard"),
        Predefinida("Gira los hombros hacia atrás, despacio, unas cuantas veces.", "IcoRotate"),
        Predefinida("Haz una pausa mental: piensa en algo que te haga sonreír.", "IcoSparkles"),
        Predefinida("Estírate como si acabaras de despertar.", "IcoSun"),
        Predefinida("Dale un descanso a tu mente: mira el cielo un momento.", "IcoSun"),
        Predefinida("Aprovecha para tomar un café o té con calma.", "IcoCoffee")
    ];

    private static FraseBienestar Predefinida(string texto, string icono) => new()
    {
        Texto = texto,
        IconKey = icono,
        Habilitada = true,
        EsPredefinida = true
    };
}

/// <summary>Una frase de bienestar con su icono. Ver <see cref="AppearanceSettings.Frases"/>.</summary>
public sealed class FraseBienestar
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Texto { get; set; } = "";

    public string IconKey { get; set; } = "IcoHeart";

    public bool Habilitada { get; set; } = true;

    /// <summary>De fábrica: solo se puede habilitar/deshabilitar, nunca editar ni eliminar.</summary>
    public bool EsPredefinida { get; set; }
}

public sealed class GeneralSettings
{
    public bool StartWithWindows { get; set; } = true;

    public bool StartMinimized { get; set; } = true;

    public string? OnBreakStartCommand { get; set; }

    public string? OnBreakEndCommand { get; set; }
}

/// <summary>
/// Recordatorios cortos e independientes del ciclo de pausas 20-20-20: postura, parpadeo,
/// hidratación y estiramiento. Un intervalo en <see cref="TimeSpan.Zero"/> significa "Off".
/// </summary>
public sealed class AccionesSaludablesSettings
{
    public TimeSpan PosturaIntervalo { get; set; } = TimeSpan.FromMinutes(10);

    public TimeSpan ParpadeoIntervalo { get; set; } = TimeSpan.FromMinutes(5);

    public TimeSpan HidratacionIntervalo { get; set; } = TimeSpan.Zero;

    public TimeSpan EstiramientoIntervalo { get; set; } = TimeSpan.Zero;
}
