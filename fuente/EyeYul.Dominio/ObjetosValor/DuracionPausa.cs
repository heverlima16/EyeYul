namespace EyeYul.Dominio.ObjetosValor;

public readonly record struct DuracionPausa
{
    public TimeSpan Value { get; }

    public static readonly DuracionPausa OneMinute = new(TimeSpan.FromMinutes(1));

    public static readonly DuracionPausa FiveMinutes = new(TimeSpan.FromMinutes(5));

    public static readonly DuracionPausa FifteenMinutes = new(TimeSpan.FromMinutes(15));

    private DuracionPausa(TimeSpan value)
    {
        Value = value;
    }

    public static DuracionPausa FromMinutes(int minutes) => new(TimeSpan.FromMinutes(minutes));

    public override string ToString() => $"+{(int)Value.TotalMinutes}";
}
