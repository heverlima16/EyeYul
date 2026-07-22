# Checklist de archivos — Feature no-UI de EyeYul

Reemplazar `<Entidad>` por el nombre en PascalCase y espanol (`Descanso`,
`DescansoProgramado`), y `<tabla>` por el nombre de tabla en snake_case
(`breaks`, `planned_breaks`).

No todo aplica siempre: una regla de dominio pura no necesita repositorio, y un
servicio de calculo no necesita entidad nueva. Marcar como N/A lo que no aplique
y decirlo en el reporte final.

## fuente/EyeYul.Dominio

- [ ] `Entidades/<Entidad>.cs` — `sealed class`, `Id { get; init; } = Guid.NewGuid()`,
      metodos de transicion (`MarkStarted`, `MarkCompleted`…) en vez de setters sueltos
- [ ] `Enumeraciones/<Nombre>.cs` — un enum por archivo; `[Flags]` si son combinables
- [ ] `ObjetosValor/<Nombre>.cs` — `readonly record struct` con constructor privado
      y factorias estaticas
- [ ] `Reglas/<Nombre>Reglas.cs` — `static class` pura, constantes nombradas
      (`PenaltyPerSkip = 8`), sin `DateTime.Now` ni I/O

## fuente/EyeYul.Aplicacion

- [ ] `Abstracciones/Repositories.cs` — agregar `I<Entidad>Repository` (lecturas
      `Task<IReadOnlyList<T>>`, escrituras `Task`, `CancellationToken ct = default`)
- [ ] `Abstracciones/I<Servicio>.cs` — si Infraestructura debe implementar algo nuevo
- [ ] `<Area>/<Servicio>.cs` — constructor primario, dependencias por interfaz
- [ ] `Configuracion/AjustesEyeYul.cs` — propiedad nueva con valor por defecto
      sensato (la app arranca sin `settings.json`)
- [ ] `DependencyInjection.cs` — `AddSingleton` (+ `AddHostedService` si es
      `BackgroundService`)

## fuente/EyeYul.Infraestructura

- [ ] `Persistencia/Repositorio<Entidad>.cs` — SQL crudo con `Microsoft.Data.Sqlite`,
      parametros `$nombre`, raw string literals (`"""`)
- [ ] `Persistencia/BaseDatosSqlite.cs` — `CREATE TABLE IF NOT EXISTS` de la tabla nueva
- [ ] `Interoperabilidad/` — solo si hace falta P/Invoke nuevo (`[LibraryImport]`)
- [ ] `DependencyInjection.cs` — `AddSingleton<I<Entidad>Repository, Repositorio<Entidad>>()`

## tests/EyeYul.UnitTests

- [ ] `<Nombre>Tests.cs` — un `[Fact]` por rama de la regla; `[Theory]` para tablas
      de casos. Ver `backend-testing`.

## Verificacion final

- [ ] `dotnet build EyeYul.slnx` sin warnings nuevos
- [ ] `dotnet test` en verde
- [ ] Si toca la UI o el arranque, ejecutar la app y comprobar el flujo real
