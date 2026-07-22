using EyeYul.Aplicacion.Abstracciones;
using EyeYul.Aplicacion.Descansos;
using EyeYul.Dominio.Entidades;
using Xunit;

namespace EyeYul.UnitTests;

public class ServicioDescansoProgramadoTests
{
    // 2026-07-06 es lunes.
    private static readonly DateTimeOffset Lunes9Am = new(2026, 7, 6, 9, 0, 0, TimeSpan.Zero);

    private sealed class RelojFijo(DateTimeOffset ahora) : IReloj
    {
        public DateTimeOffset Now { get; set; } = ahora;
    }

    private sealed class RepoPausasEnMemoria(params DescansoProgramado[] iniciales)
        : IPlannedBreakRepository
    {
        public List<DescansoProgramado> Items { get; } = [.. iniciales];

        public int LlamadasGetAll { get; private set; }

        public Task<IReadOnlyList<DescansoProgramado>> GetAllAsync(CancellationToken ct = default)
        {
            LlamadasGetAll++;
            return Task.FromResult<IReadOnlyList<DescansoProgramado>>(Items);
        }

        public Task UpsertAsync(DescansoProgramado pb, CancellationToken ct = default)
        {
            Items.RemoveAll(x => x.Id == pb.Id);
            Items.Add(pb);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Guid id, CancellationToken ct = default)
        {
            Items.RemoveAll(x => x.Id == id);
            return Task.CompletedTask;
        }
    }

    private static DescansoProgramado PausaA(int hora, int minuto, params DayOfWeek[] dias)
    {
        var pb = new DescansoProgramado
        {
            Nombre = "Almuerzo",
            HoraDelDia = new TimeOnly(hora, minuto),
            Habilitado = true
        };

        foreach (DayOfWeek d in dias)
        {
            pb.Dias.Add(d);
        }

        return pb;
    }

    [Fact]
    public async Task GetDue_ReturnsBreak_WithinTolerance()
    {
        var repo = new RepoPausasEnMemoria(PausaA(9, 0, DayOfWeek.Monday));
        var servicio = new ServicioDescansoProgramado(repo, new RelojFijo(Lunes9Am.AddSeconds(10)));

        DescansoProgramado? due = await servicio.ObtenerPendienteAsync(TimeSpan.FromSeconds(30));

        Assert.NotNull(due);
    }

    [Fact]
    public async Task GetDue_ReturnsNull_WhenPastTolerance()
    {
        var repo = new RepoPausasEnMemoria(PausaA(9, 0, DayOfWeek.Monday));
        var servicio = new ServicioDescansoProgramado(repo, new RelojFijo(Lunes9Am.AddMinutes(5)));

        Assert.Null(await servicio.ObtenerPendienteAsync(TimeSpan.FromSeconds(30)));
    }

    [Fact]
    public async Task GetDue_ReturnsNull_BeforeScheduledTime()
    {
        var repo = new RepoPausasEnMemoria(PausaA(9, 0, DayOfWeek.Monday));
        var servicio = new ServicioDescansoProgramado(repo, new RelojFijo(Lunes9Am.AddSeconds(-5)));

        Assert.Null(await servicio.ObtenerPendienteAsync(TimeSpan.FromSeconds(30)));
    }

    [Fact]
    public async Task GetDue_DoesNotFireTwiceForSameOccurrence()
    {
        // El scheduler llama cada segundo: la misma ocurrencia solo debe disparar una vez.
        var repo = new RepoPausasEnMemoria(PausaA(9, 0, DayOfWeek.Monday));
        var servicio = new ServicioDescansoProgramado(repo, new RelojFijo(Lunes9Am.AddSeconds(5)));

        Assert.NotNull(await servicio.ObtenerPendienteAsync(TimeSpan.FromSeconds(30)));
        Assert.Null(await servicio.ObtenerPendienteAsync(TimeSpan.FromSeconds(30)));
    }

    [Fact]
    public async Task GetDue_IgnoresBreakOnAnotherDay()
    {
        var repo = new RepoPausasEnMemoria(PausaA(9, 0, DayOfWeek.Wednesday));
        var servicio = new ServicioDescansoProgramado(repo, new RelojFijo(Lunes9Am.AddSeconds(5)));

        Assert.Null(await servicio.ObtenerPendienteAsync(TimeSpan.FromSeconds(30)));
    }

    [Fact]
    public async Task GetDue_IgnoresDisabledBreak()
    {
        DescansoProgramado pausa = PausaA(9, 0, DayOfWeek.Monday);
        pausa.Habilitado = false;

        var repo = new RepoPausasEnMemoria(pausa);
        var servicio = new ServicioDescansoProgramado(repo, new RelojFijo(Lunes9Am.AddSeconds(5)));

        Assert.Null(await servicio.ObtenerPendienteAsync(TimeSpan.FromSeconds(30)));
    }

    [Fact]
    public async Task GetAll_IsCachedUntilInvalidated()
    {
        var repo = new RepoPausasEnMemoria(PausaA(9, 0, DayOfWeek.Monday));
        var servicio = new ServicioDescansoProgramado(repo, new RelojFijo(Lunes9Am));

        await servicio.ObtenerTodosAsync();
        await servicio.ObtenerTodosAsync();

        Assert.Equal(1, repo.LlamadasGetAll);
    }

    [Fact]
    public async Task Save_InvalidatesCache()
    {
        var repo = new RepoPausasEnMemoria(PausaA(9, 0, DayOfWeek.Monday));
        var servicio = new ServicioDescansoProgramado(repo, new RelojFijo(Lunes9Am));

        await servicio.ObtenerTodosAsync();
        await servicio.GuardarAsync(PausaA(13, 0, DayOfWeek.Tuesday));
        IReadOnlyList<DescansoProgramado> despues = await servicio.ObtenerTodosAsync();

        Assert.Equal(2, repo.LlamadasGetAll);
        Assert.Equal(2, despues.Count);
    }

    [Fact]
    public async Task Delete_InvalidatesCache()
    {
        DescansoProgramado pausa = PausaA(9, 0, DayOfWeek.Monday);
        var repo = new RepoPausasEnMemoria(pausa);
        var servicio = new ServicioDescansoProgramado(repo, new RelojFijo(Lunes9Am));

        await servicio.ObtenerTodosAsync();
        await servicio.EliminarAsync(pausa.Id);

        Assert.Empty(await servicio.ObtenerTodosAsync());
    }

    [Fact]
    public async Task NextOccurrence_ReturnsEarliestUpcoming()
    {
        var repo = new RepoPausasEnMemoria(
            PausaA(15, 0, DayOfWeek.Monday),
            PausaA(11, 0, DayOfWeek.Monday));

        var servicio = new ServicioDescansoProgramado(repo, new RelojFijo(Lunes9Am));

        DateTimeOffset? siguiente = await servicio.ProximaOcurrenciaAsync();

        Assert.NotNull(siguiente);
        Assert.Equal(11, siguiente.Value.Hour);
    }

    [Fact]
    public async Task NextOccurrence_ReturnsNull_WhenNothingScheduled()
    {
        var servicio = new ServicioDescansoProgramado(
            new RepoPausasEnMemoria(), new RelojFijo(Lunes9Am));

        Assert.Null(await servicio.ProximaOcurrenciaAsync());
    }
}
