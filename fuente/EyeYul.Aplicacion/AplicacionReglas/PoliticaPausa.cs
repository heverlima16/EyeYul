using EyeYul.Aplicacion.Configuracion;

namespace EyeYul.Aplicacion.AplicacionReglas;

public sealed class PoliticaPausa
{
    private DateOnly _diaActual;

    private int _aplazamientosHoy;

    public bool PuedeAplazar(DateOnly hoy, int aplazamientosDeEstaPausa, EnforcementSettings ajustes)
    {
        CambiarDiaSiHaceFalta(hoy);

        if (ajustes.MaxSnoozesPerDay > 0 && _aplazamientosHoy >= ajustes.MaxSnoozesPerDay)
        {
            return false;
        }

        if (ajustes.MaxSnoozesPerBreak > 0 && aplazamientosDeEstaPausa >= ajustes.MaxSnoozesPerBreak)
        {
            return false;
        }

        return true;
    }

    public void RegistrarAplazamiento(DateOnly hoy)
    {
        CambiarDiaSiHaceFalta(hoy);
        _aplazamientosHoy++;
    }

    public int AplazamientosUsadosHoy(DateOnly hoy)
    {
        CambiarDiaSiHaceFalta(hoy);
        return _aplazamientosHoy;
    }

    private void CambiarDiaSiHaceFalta(DateOnly hoy)
    {
        if (hoy != _diaActual)
        {
            _diaActual = hoy;
            _aplazamientosHoy = 0;
        }
    }
}
