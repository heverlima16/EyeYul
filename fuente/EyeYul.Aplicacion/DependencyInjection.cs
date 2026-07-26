using EyeYul.Aplicacion.Abstracciones;
using EyeYul.Aplicacion.Actividad;
using EyeYul.Aplicacion.AplicacionReglas;
using EyeYul.Aplicacion.Descansos;
using EyeYul.Aplicacion.Estadisticas;
using EyeYul.Aplicacion.Licencias;
using EyeYul.Aplicacion.Puntuacion;
using EyeYul.Aplicacion.Salud;
using Microsoft.Extensions.DependencyInjection;

namespace EyeYul.Aplicacion;

public static class DependencyInjection
{
    public static IServiceCollection AddEyeYulApplication(this IServiceCollection services)
    {
        services.AddSingleton<MotorPausaInteligente>();
        services.AddSingleton<PoliticaPausa>();
        services.AddSingleton<ServicioPuntajeVisual>();
        services.AddSingleton<ServicioEstadisticas>();
        services.AddSingleton<ServicioDescansoProgramado>();
        services.AddSingleton<ServicioLicencia>();

        services.AddSingleton<MonitorActividad>();
        services.AddSingleton<IMonitorActividad>(sp => sp.GetRequiredService<MonitorActividad>());
        services.AddHostedService(sp => sp.GetRequiredService<MonitorActividad>());

        services.AddSingleton<ProgramadorDescansos>();
        services.AddHostedService(sp => sp.GetRequiredService<ProgramadorDescansos>());

        services.AddSingleton<ProgramadorAccionesSaludables>();
        services.AddHostedService(sp => sp.GetRequiredService<ProgramadorAccionesSaludables>());

        return services;
    }
}
