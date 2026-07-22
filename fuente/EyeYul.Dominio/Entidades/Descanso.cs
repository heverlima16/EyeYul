using EyeYul.Dominio.Enumeraciones;

namespace EyeYul.Dominio.Entidades;

public sealed class Descanso
{
    public Guid Id { get; init; } = Guid.NewGuid();

    public TipoDescanso Tipo { get; init; }

    public DateTimeOffset ProgramadoEn { get; init; }

    public DateTimeOffset? IniciadoEn { get; set; }

    public DateTimeOffset? FinalizadoEn { get; set; }

    public TimeSpan DuracionPlanificada { get; init; }

    public ResultadoDescanso Resultado { get; set; } = ResultadoDescanso.Pendiente;

    public int ConteoAplazamientos { get; set; }

    public TimeSpan? DuracionReal =>
        IniciadoEn is { } inicio && FinalizadoEn is { } fin ? fin - inicio : null;

    public bool FueRespetado => Resultado == ResultadoDescanso.Completado;

    public void MarcarIniciado(DateTimeOffset cuando)
    {
        IniciadoEn = cuando;
        Resultado = ResultadoDescanso.Pendiente;
    }

    public void MarcarCompletado(DateTimeOffset cuando)
    {
        FinalizadoEn = cuando;
        Resultado = ResultadoDescanso.Completado;
    }

    public void MarcarOmitido(DateTimeOffset cuando)
    {
        FinalizadoEn = cuando;
        Resultado = ResultadoDescanso.Omitido;
    }

    public void MarcarSuprimido()
    {
        Resultado = ResultadoDescanso.Suprimido;
    }

    public void MarcarAplazado()
    {
        ConteoAplazamientos++;
        Resultado = ResultadoDescanso.Aplazado;
    }
}
