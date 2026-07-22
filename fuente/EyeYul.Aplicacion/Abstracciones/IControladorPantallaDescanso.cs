using EyeYul.Dominio.Entidades;

namespace EyeYul.Aplicacion.Abstracciones;

public interface IControladorPantallaDescanso
{
    Task<BreakOverlayResult> ShowAsync(BreakOverlayRequest request, CancellationToken ct = default);

    Task ShowPreWarningAsync(TimeSpan countdown, CancellationToken ct = default);
}

public sealed record BreakOverlayRequest(
    Descanso Descanso,
    string Message,
    TimeSpan Duration,
    bool AllowSkip,
    string? BackgroundPath);

public enum BreakOverlayResult
{
    Completed,
    Skipped,
    Snoozed
}
