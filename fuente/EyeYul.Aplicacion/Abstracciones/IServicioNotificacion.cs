using EyeYul.Dominio.ObjetosValor;

namespace EyeYul.Aplicacion.Abstracciones;

public interface IServicioNotificacion
{
    Task<NotificationResponse> ShowBreakDueAsync(string title, string body, CancellationToken ct = default);

    Task ShowInfoAsync(string title, string body, CancellationToken ct = default);
}

public abstract record NotificationResponse
{
    public sealed record TakeBreakNow : NotificationResponse;

    public sealed record Snooze(DuracionPausa Duration) : NotificationResponse;

    public sealed record Skip : NotificationResponse;

    public sealed record Dismissed : NotificationResponse;
}
