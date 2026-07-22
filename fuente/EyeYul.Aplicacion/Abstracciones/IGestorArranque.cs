namespace EyeYul.Aplicacion.Abstracciones;

public interface IGestorArranque
{
    bool IsEnabled();

    void Enable();

    void Disable();
}
