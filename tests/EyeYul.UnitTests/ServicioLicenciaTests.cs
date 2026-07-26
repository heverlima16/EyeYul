using EyeYul.Aplicacion.Abstracciones;
using EyeYul.Aplicacion.Licencias;
using Xunit;

namespace EyeYul.UnitTests;

public class ServicioLicenciaTests
{
    private static readonly DateTimeOffset Ahora = new(2026, 7, 25, 12, 0, 0, TimeSpan.Zero);

    private sealed class RelojFijo(DateTimeOffset ahora) : IReloj
    {
        public DateTimeOffset Now { get; } = ahora;
    }

    private sealed class AlmacenLicenciaEnMemoria(RegistroLicencia? inicial = null) : IAlmacenLicencia
    {
        public RegistroLicencia? Guardado { get; private set; } = inicial;

        public Task<RegistroLicencia> LoadAsync(CancellationToken ct = default) =>
            Task.FromResult(Guardado ?? new RegistroLicencia(null, null));

        public Task SaveAsync(RegistroLicencia registro, CancellationToken ct = default)
        {
            Guardado = registro;
            return Task.CompletedTask;
        }
    }

    private sealed class ValidadorFijo(bool esValida) : IValidadorClaveLicencia
    {
        public bool EsValida(string clave) => esValida;
    }

    [Fact]
    public async Task PrimerArranque_EmpiezaTrialDeCatorceDias()
    {
        var servicio = new ServicioLicencia(new AlmacenLicenciaEnMemoria(), new ValidadorFijo(true), new RelojFijo(Ahora));

        await servicio.CargarAsync();

        Assert.Equal(14, servicio.DiasRestantesTrial);
        Assert.True(servicio.PremiumDisponible);
        Assert.False(servicio.EsPremiumActivo);
    }

    [Fact]
    public async Task TrialVencido_NoDejaPremiumDisponible()
    {
        var almacen = new AlmacenLicenciaEnMemoria(new RegistroLicencia(null, Ahora.AddDays(-15)));
        var servicio = new ServicioLicencia(almacen, new ValidadorFijo(true), new RelojFijo(Ahora));

        await servicio.CargarAsync();

        Assert.Equal(0, servicio.DiasRestantesTrial);
        Assert.False(servicio.PremiumDisponible);
    }

    [Fact]
    public async Task ActivarAsync_ConClaveValida_ActivaPremiumParaSiempre()
    {
        var almacen = new AlmacenLicenciaEnMemoria(new RegistroLicencia(null, Ahora.AddDays(-100)));
        var servicio = new ServicioLicencia(almacen, new ValidadorFijo(true), new RelojFijo(Ahora));
        await servicio.CargarAsync();

        bool ok = await servicio.ActivarAsync("clave-cualquiera");

        Assert.True(ok);
        Assert.True(servicio.EsPremiumActivo);
        Assert.True(servicio.PremiumDisponible);
    }

    [Fact]
    public async Task ActivarAsync_ConClaveInvalida_NoActiva()
    {
        var servicio = new ServicioLicencia(new AlmacenLicenciaEnMemoria(), new ValidadorFijo(false), new RelojFijo(Ahora));
        await servicio.CargarAsync();

        bool ok = await servicio.ActivarAsync("clave-falsa");

        Assert.False(ok);
        Assert.False(servicio.EsPremiumActivo);
    }

    [Fact]
    public async Task CargarAsync_ClaveManipuladaEnDisco_SeIgnoraAlNoValidar()
    {
        // Simula editar licencia.json a mano poniendo cualquier texto en ClaveActivada:
        // sin la firma real, CargarAsync debe descartarla en vez de confiar ciegamente.
        var almacen = new AlmacenLicenciaEnMemoria(new RegistroLicencia("texto-inventado", Ahora.AddDays(-100)));
        var servicio = new ServicioLicencia(almacen, new ValidadorFijo(false), new RelojFijo(Ahora));

        await servicio.CargarAsync();

        Assert.False(servicio.EsPremiumActivo);
        Assert.False(servicio.PremiumDisponible);
    }
}
