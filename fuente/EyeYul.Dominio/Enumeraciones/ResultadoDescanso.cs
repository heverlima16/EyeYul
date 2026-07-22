namespace EyeYul.Dominio.Enumeraciones;

// El orden es parte del contrato: la columna Resultado de la tabla descansos
// guarda el valor numerico (0=Pendiente ... 4=Suprimido). No reordenar.
public enum ResultadoDescanso
{
    Pendiente,
    Completado,
    Aplazado,
    Omitido,
    Suprimido
}
