using EyeYul.Aplicacion.Abstracciones;
using EyeYul.Infraestructura.Ajustes;
using EyeYul.Infraestructura.Arranque;
using EyeYul.Infraestructura.Automatizacion;
using EyeYul.Infraestructura.Interoperabilidad;
using EyeYul.Infraestructura.Persistencia;
using Microsoft.Extensions.DependencyInjection;

namespace EyeYul.Infraestructura;

public static class DependencyInjection
{
    public static IServiceCollection AddEyeYulInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<IReloj, SystemClock>();
        services.AddSingleton<IProveedorActividadSistema, ProveedorActividadSistemaWin32>();

        services.AddSingleton<BaseDatosSqlite>();
        services.AddSingleton<IBreakRepository, RepositorioDescanso>();
        services.AddSingleton<ISessionRepository, RepositorioSesion>();
        services.AddSingleton<IAppUsageRepository, RepositorioUsoApp>();
        services.AddSingleton<IWebsiteUsageRepository, RepositorioUsoSitioWeb>();
        services.AddSingleton<IScreenScoreRepository, RepositorioPuntajeVisual>();
        services.AddSingleton<IPlannedBreakRepository, RepositorioDescansoProgramado>();

        services.AddSingleton<IAlmacenAjustes, AlmacenAjustesJson>();
        services.AddSingleton<IGestorArranque, GestorArranqueRegistro>();
        services.AddSingleton<IAutomationRunner, EjecutorAutomatizacionProcesos>();

        return services;
    }
}
