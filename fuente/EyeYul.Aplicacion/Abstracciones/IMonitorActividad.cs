namespace EyeYul.Aplicacion.Abstracciones;

public interface IMonitorActividad
{
    ActivitySnapshot Current { get; }

    event EventHandler<ActivityChangedEventArgs>? ActivityChanged;
}

public sealed class ActivityChangedEventArgs(ActivitySnapshot snapshot) : EventArgs
{
    public ActivitySnapshot Snapshot { get; } = snapshot;
}
