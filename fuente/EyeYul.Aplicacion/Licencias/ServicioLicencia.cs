using EyeYul.Aplicacion.Abstracciones;

namespace EyeYul.Aplicacion.Licencias;

/// <summary>
/// EyeYul no tiene servidor ni cuentas, así que la licencia se valida por completo en
/// el equipo del usuario: una clave firmada offline (ver <see cref="IValidadorClaveLicencia"/>)
/// desbloquea Premium para siempre; sin clave, Premium queda disponible solo durante el trial.
/// </summary>
public sealed class ServicioLicencia(
    IAlmacenLicencia almacen,
    IValidadorClaveLicencia validador,
    IReloj reloj)
{
    public const int DuracionTrialDias = 14;

    private RegistroLicencia _registro = new(null, null);

    public async Task CargarAsync(CancellationToken ct = default)
    {
        _registro = await almacen.LoadAsync(ct);

        // Se re-valida la firma al cargar, no solo al activar: editar licencia.json a
        // mano y poner cualquier texto en ClaveActivada no debe bastar para desbloquear
        // Premium sin una clave realmente firmada.
        if (_registro.ClaveActivada is not null && !validador.EsValida(_registro.ClaveActivada))
        {
            _registro = _registro with { ClaveActivada = null };
        }

        if (_registro.PrimerUso is null)
        {
            _registro = _registro with { PrimerUso = reloj.Now };
            await almacen.SaveAsync(_registro, ct);
        }
    }

    public bool EsPremiumActivo => _registro.ClaveActivada is not null;

    public int DiasRestantesTrial
    {
        get
        {
            DateTimeOffset inicio = _registro.PrimerUso ?? reloj.Now;
            return Math.Max(0, DuracionTrialDias - (int)(reloj.Now - inicio).TotalDays);
        }
    }

    /// <summary>Premium activo (clave válida) o todavía dentro del trial.</summary>
    public bool PremiumDisponible => EsPremiumActivo || DiasRestantesTrial > 0;

    public async Task<bool> ActivarAsync(string clave, CancellationToken ct = default)
    {
        if (!validador.EsValida(clave))
        {
            return false;
        }

        _registro = _registro with { ClaveActivada = clave };
        await almacen.SaveAsync(_registro, ct);
        return true;
    }
}
