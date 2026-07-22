using System.Windows;
using EyeYul.Aplicacion;
using EyeYul.Aplicacion.Abstracciones;
using EyeYul.Infraestructura;
using EyeYul.Presentacion.Bandeja;
using EyeYul.Presentacion.CapaSuperpuesta;
using EyeYul.Presentacion.ModelosVista;
using EyeYul.Presentacion.Notificaciones;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace EyeYul.Presentacion;

public partial class App : Application
{
    private IHost? _host;

    public static IServiceProvider Services { get; private set; } = null!;

    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        _host = Host.CreateDefaultBuilder()
            .ConfigureLogging(l =>
            {
                l.ClearProviders();
                l.AddDebug();
                l.SetMinimumLevel(LogLevel.Information);
            })
            .ConfigureServices(services =>
            {
                services.AddEyeYulInfrastructure();
                services.AddEyeYulApplication();

                services.AddSingleton<IControladorPantallaDescanso, ControladorPantallaDescansoWpf>();
                services.AddSingleton<IServicioNotificacion, WpfNotificationService>();
                services.AddSingleton<ControladorIconoBandeja>();

                services.AddTransient<VistaGeneralModelo>();
                services.AddTransient<AjustesModelo>();
                services.AddTransient<EstadisticasModelo>();
            })
            .Build();

        Services = _host.Services;

        IAlmacenAjustes settingsStore = Services.GetRequiredService<IAlmacenAjustes>();
        await settingsStore.LoadAsync();
        ApplyStartupPreference(settingsStore);

        await _host.StartAsync();

        ControladorIconoBandeja trayController = Services.GetRequiredService<ControladorIconoBandeja>();
        trayController.Initialize();
        trayController.ShowMainWindow();
    }

    private static void ApplyStartupPreference(IAlmacenAjustes store)
    {
        IGestorArranque startup = Services.GetRequiredService<IGestorArranque>();
        bool wanted = store.Current.General.StartWithWindows;

        if (wanted && !startup.IsEnabled())
        {
            startup.Enable();
        }
        else if (!wanted && startup.IsEnabled())
        {
            startup.Disable();
        }
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        if (_host is not null)
        {
            await _host.StopAsync(TimeSpan.FromSeconds(3));
            _host.Dispose();
        }

        base.OnExit(e);
    }
}
