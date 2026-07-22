using EyeYul.Aplicacion.Configuracion;

namespace EyeYul.Aplicacion.AplicacionReglas;

public sealed class PoliticaPausa
{
    private DateOnly _currentDay;

    private int _snoozesToday;

    public bool CanSnooze(DateOnly today, int snoozesOnThisBreak, EnforcementSettings settings)
    {
        RollDayIfNeeded(today);

        if (settings.MaxSnoozesPerDay > 0 && _snoozesToday >= settings.MaxSnoozesPerDay)
        {
            return false;
        }

        if (settings.MaxSnoozesPerBreak > 0 && snoozesOnThisBreak >= settings.MaxSnoozesPerBreak)
        {
            return false;
        }

        return true;
    }

    public void RecordSnooze(DateOnly today)
    {
        RollDayIfNeeded(today);
        _snoozesToday++;
    }

    public int SnoozesUsedToday(DateOnly today)
    {
        RollDayIfNeeded(today);
        return _snoozesToday;
    }

    private void RollDayIfNeeded(DateOnly today)
    {
        if (today != _currentDay)
        {
            _currentDay = today;
            _snoozesToday = 0;
        }
    }
}
