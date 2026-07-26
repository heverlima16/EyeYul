using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Threading;
using EyeYul.Dominio.Enumeraciones;
using EyeYul.Presentacion.Interoperabilidad;

namespace EyeYul.Presentacion.Vistas;

public partial class VentanaAccionSaludable : Window
{
    private static readonly TimeSpan Duracion = TimeSpan.FromSeconds(5);

    private readonly TaskCompletionSource _cerrada = new();

    private readonly DispatcherTimer _autoCierre = new() { Interval = Duracion };

    public VentanaAccionSaludable(TipoAccionSaludable tipo)
    {
        InitializeComponent();

        (string titulo, string mensaje, Path icono, Storyboard animacion) = Configurar(tipo);
        TituloText.Text = titulo;
        MensajeText.Text = mensaje;
        icono.Visibility = Visibility.Visible;

        Loaded += (_, _) =>
        {
            // Tinte liviano: que se note el desenfoque de lo que hay detras, no una caja opaca.
            DesenfoqueVentana.Habilitar(this, 0x59101014);
            animacion.Begin(icono, isControllable: true);
        };

        _autoCierre.Tick += (_, _) =>
        {
            _autoCierre.Stop();
            Close();
        };

        Closed += (_, _) =>
        {
            _autoCierre.Stop();
            _cerrada.TrySetResult();
        };

        PosicionarCentro();
    }

    public Task EsperarCierreAsync() => _cerrada.Task;

    private void PosicionarCentro()
    {
        // SystemParameters.WorkArea ya viene en DIPs (las mismas unidades que Left/Top de
        // la ventana): usar Screen.PrimaryScreen (System.Windows.Forms, pixeles fisicos)
        // aqui posicionaba la ventana fuera de pantalla en cualquier equipo con DPI != 100%.
        Rect area = SystemParameters.WorkArea;

        Left = area.Left + (area.Width - Width) / 2;
        Top = area.Top + (area.Height - Height) / 2;
    }

    private void OnClick(object sender, MouseButtonEventArgs e) => Close();

    protected override void OnContentRendered(EventArgs e)
    {
        base.OnContentRendered(e);
        _autoCierre.Start();
    }

    private (string Titulo, string Mensaje, Path Icono, Storyboard Animacion) Configurar(TipoAccionSaludable tipo) => tipo switch
    {
        TipoAccionSaludable.Postura => ("¡Endereza la espalda!", "Siéntate erguido y baja los hombros.", IconoPostura, AnimacionPostura()),
        TipoAccionSaludable.Parpadeo => ("¡Parpadea!", "Parpadea despacio varias veces seguidas.", IconoParpadeo, AnimacionParpadeo()),
        TipoAccionSaludable.Hidratacion => ("Hidrátate", "Bebe un poco de agua ahora mismo.", IconoHidratacion, AnimacionHidratacion()),
        TipoAccionSaludable.Estiramiento => ("Hora de estirar", "Estira brazos, cuello y espalda.", IconoEstiramiento, AnimacionEstiramiento()),
        _ => throw new ArgumentOutOfRangeException(nameof(tipo))
    };

    private Storyboard AnimacionPostura()
    {
        var animacion = new DoubleAnimationUsingKeyFrames { RepeatBehavior = RepeatBehavior.Forever };
        animacion.KeyFrames.Add(new EasingDoubleKeyFrame(0, KeyTime.FromPercent(0)));
        animacion.KeyFrames.Add(new EasingDoubleKeyFrame(-8, KeyTime.FromPercent(0.5)) { EasingFunction = new SineEase { EasingMode = EasingMode.EaseOut } });
        animacion.KeyFrames.Add(new EasingDoubleKeyFrame(0, KeyTime.FromPercent(1)) { EasingFunction = new SineEase { EasingMode = EasingMode.EaseIn } });
        Storyboard.SetTarget(animacion, TransPostura);
        Storyboard.SetTargetProperty(animacion, new PropertyPath(TranslateTransform.YProperty));

        var timeline = new Storyboard { Duration = TimeSpan.FromSeconds(1.1) };
        timeline.Children.Add(animacion);
        return timeline;
    }

    private Storyboard AnimacionParpadeo()
    {
        var animacion = new DoubleAnimationUsingKeyFrames { RepeatBehavior = RepeatBehavior.Forever };
        animacion.KeyFrames.Add(new EasingDoubleKeyFrame(1, KeyTime.FromPercent(0)));
        animacion.KeyFrames.Add(new EasingDoubleKeyFrame(1, KeyTime.FromPercent(0.55)));
        animacion.KeyFrames.Add(new EasingDoubleKeyFrame(0.08, KeyTime.FromPercent(0.7)));
        animacion.KeyFrames.Add(new EasingDoubleKeyFrame(1, KeyTime.FromPercent(0.85)));
        Storyboard.SetTarget(animacion, ScalaParpadeo);
        Storyboard.SetTargetProperty(animacion, new PropertyPath(ScaleTransform.ScaleYProperty));

        var timeline = new Storyboard { Duration = TimeSpan.FromSeconds(1.6) };
        timeline.Children.Add(animacion);
        return timeline;
    }

    private Storyboard AnimacionHidratacion()
    {
        var animacion = new DoubleAnimationUsingKeyFrames { RepeatBehavior = RepeatBehavior.Forever };
        animacion.KeyFrames.Add(new EasingDoubleKeyFrame(-6, KeyTime.FromPercent(0)));
        animacion.KeyFrames.Add(new EasingDoubleKeyFrame(6, KeyTime.FromPercent(0.5)) { EasingFunction = new BounceEase { Bounces = 1, Bounciness = 2 } });
        animacion.KeyFrames.Add(new EasingDoubleKeyFrame(-6, KeyTime.FromPercent(1)));
        Storyboard.SetTarget(animacion, TransHidratacion);
        Storyboard.SetTargetProperty(animacion, new PropertyPath(TranslateTransform.YProperty));

        var timeline = new Storyboard { Duration = TimeSpan.FromSeconds(1.4) };
        timeline.Children.Add(animacion);
        return timeline;
    }

    private Storyboard AnimacionEstiramiento()
    {
        var animacion = new DoubleAnimationUsingKeyFrames { RepeatBehavior = RepeatBehavior.Forever };
        animacion.KeyFrames.Add(new EasingDoubleKeyFrame(0, KeyTime.FromPercent(0)));
        animacion.KeyFrames.Add(new EasingDoubleKeyFrame(-12, KeyTime.FromPercent(0.3)) { EasingFunction = new SineEase() });
        animacion.KeyFrames.Add(new EasingDoubleKeyFrame(12, KeyTime.FromPercent(0.7)) { EasingFunction = new SineEase() });
        animacion.KeyFrames.Add(new EasingDoubleKeyFrame(0, KeyTime.FromPercent(1)));
        Storyboard.SetTarget(animacion, RotaEstiramiento);
        Storyboard.SetTargetProperty(animacion, new PropertyPath(RotateTransform.AngleProperty));

        var timeline = new Storyboard { Duration = TimeSpan.FromSeconds(2.0) };
        timeline.Children.Add(animacion);
        return timeline;
    }
}
