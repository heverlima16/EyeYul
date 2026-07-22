using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using EyeYul.Aplicacion.Abstracciones;
using EyeYul.Aplicacion.Actividad;
using EyeYul.Aplicacion.Descansos;
using EyeYul.Presentacion.ModelosVista;
using EyeYul.Presentacion.Vistas;
using H.NotifyIcon;
using Microsoft.Extensions.DependencyInjection;
using Application = System.Windows.Application;

namespace EyeYul.Presentacion.Bandeja;

public sealed class ControladorIconoBandeja(
    ProgramadorDescansos scheduler,
    IAlmacenAjustes settingsStore,
    IServicioNotificacion notifications,
    IServiceProvider services)
{
    private TaskbarIcon? _tray;

    private VentanaCuentaRegresivaFlotante? _floating;

    private VentanaAjustes? _settingsWindow;

    private VentanaEstadisticas? _statsWindow;

    private VentanaPrincipal? _mainWindow;

    private int _ultimoMinutoEnTooltip = -1;

    public void Initialize()
    {
        _tray = new TaskbarIcon
        {
            ToolTipText = "EyeYul",
            Icon = CreateLogoIcon(32),
            ContextMenu = BuildMenu(),
            Visibility = Visibility.Visible
        };

        _tray.ForceCreate(enablesEfficiencyMode: false);
        _tray.TrayLeftMouseUp += (_, _) => ShowMainWindow();

        scheduler.Tic += OnTick;
        scheduler.PausaSuprimida += OnPausaSuprimida;
        settingsStore.Changed += (_, _) => ApplyFloatingPreference();
        ApplyFloatingPreference();
    }

    /// <summary>
    /// Carga el icono de la app desde el recurso incrustado, para que la bandeja use
    /// exactamente el mismo que el ejecutable y la barra de tareas.
    /// </summary>
    private static Icon CreateLogoIcon(int size)
    {
        var uri = new Uri("pack://application:,,,/EyeYul;component/app.ico");
        using Stream stream = Application.GetResourceStream(uri)!.Stream;

        // app.ico es multi-resolucion: se pide el tamano exacto que quiere la bandeja.
        return new Icon(stream, new System.Drawing.Size(size, size));
    }

    private ContextMenu BuildMenu()
    {
        var menu = new ContextMenu();
        menu.Items.Add(MenuItem("Vista General", ShowMainWindow));
        menu.Items.Add(MenuItem("Tomar pausa ahora", () => scheduler.SolicitarPausaInmediata()));
        menu.Items.Add(MenuItem("Estadísticas…", ShowStats));
        menu.Items.Add(MenuItem("Ajustes…", ShowSettings));
        menu.Items.Add(new Separator());
        menu.Items.Add(MenuItem("Salir", () => Application.Current.Shutdown()));
        return menu;
    }

    private static MenuItem MenuItem(string header, Action onClick)
    {
        var item = new MenuItem { Header = header };
        item.Click += (_, _) => onClick();
        return item;
    }

    private void OnTick(object? sender, TimeSpan remaining)
    {
        int minutosRestantes = (int)remaining.TotalMinutes;
        bool tooltipDebeActualizarse = minutosRestantes != _ultimoMinutoEnTooltip;

        // El scheduler late cada segundo; solo se toca la UI si hay algo que cambiar.
        if (!tooltipDebeActualizarse && _floating is null)
        {
            return;
        }

        _ultimoMinutoEnTooltip = minutosRestantes;

        Application.Current?.Dispatcher.BeginInvoke(() =>
        {
            if (tooltipDebeActualizarse && _tray is not null)
            {
                _tray.ToolTipText = minutosRestantes > 0
                    ? $"EyeYul — próxima pausa en {minutosRestantes} min"
                    : "EyeYul — próxima pausa en menos de 1 min";
            }

            _floating?.SetRemaining(remaining);
        });
    }

    /// <summary>
    /// Avisa de que la pausa no se mostro y por que. Sin esto el temporizador
    /// parece reiniciarse solo y no hay forma de saber que la bloqueo.
    /// </summary>
    private void OnPausaSuprimida(object? sender, DecisionPausa decision)
    {
        Application.Current?.Dispatcher.BeginInvoke(() =>
        {
            string motivo = MotivoPausa.Describir(decision);

            _ = notifications.ShowInfoAsync(
                "Pausa pospuesta",
                $"{motivo}. Se reintentara en 2 minutos.");
        });
    }

    private void ApplyFloatingPreference()
    {
        Application.Current?.Dispatcher.BeginInvoke(() =>
        {
            bool show = settingsStore.Current.Appearance.ShowFloatingCountdown;

            if (show && _floating is null)
            {
                _floating = new VentanaCuentaRegresivaFlotante();
                _floating.Show();
            }
            else if (!show && _floating is not null)
            {
                _floating.Close();
                _floating = null;
            }
        });
    }

    private void ShowSettings()
    {
        if (_settingsWindow is { IsVisible: true })
        {
            _settingsWindow.Activate();
            return;
        }

        _settingsWindow = new VentanaAjustes(services.GetRequiredService<AjustesModelo>());
        _settingsWindow.Closed += (_, _) => _settingsWindow = null;
        _settingsWindow.Show();
        _settingsWindow.Activate();
    }

    private void ShowStats()
    {
        if (_statsWindow is { IsVisible: true })
        {
            _statsWindow.Activate();
            return;
        }

        _statsWindow = new VentanaEstadisticas(services.GetRequiredService<EstadisticasModelo>());
        _statsWindow.Closed += (_, _) => _statsWindow = null;
        _statsWindow.Show();
        _statsWindow.Activate();
    }

    public void ShowMainWindow()
    {
        if (_mainWindow is not null)
        {
            _mainWindow.Show();
            _mainWindow.Activate();
            return;
        }

        VistaGeneralModelo vm = services.GetRequiredService<VistaGeneralModelo>();
        vm.OpenSettingsAction = ShowSettings;

        _mainWindow = new VentanaPrincipal(vm);
        _mainWindow.Closed += (_, _) =>
        {
            vm.Dispose();
            _mainWindow = null;
        };
        _mainWindow.Show();
        _mainWindow.Activate();
    }
}
