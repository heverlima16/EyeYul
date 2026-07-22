namespace EyeYul.Dominio.ObjetosValor;

public readonly record struct DuracionPausa
{
    public TimeSpan Valor { get; }

    public static readonly DuracionPausa UnMinuto = new(TimeSpan.FromMinutes(1));

    public static readonly DuracionPausa CincoMinutos = new(TimeSpan.FromMinutes(5));

    public static readonly DuracionPausa QuinceMinutos = new(TimeSpan.FromMinutes(15));

    private DuracionPausa(TimeSpan valor)
    {
        Valor = valor;
    }

    public static DuracionPausa DesdeMinutos(int minutos) => new(TimeSpan.FromMinutes(minutos));

    public override string ToString() => $"+{(int)Valor.TotalMinutes}";
}
