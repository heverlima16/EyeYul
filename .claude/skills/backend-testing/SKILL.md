---
name: backend-testing
description: "Trigger: tests unitarios de EyeYul, EyeYul.UnitTests, probar una regla de dominio o un servicio de aplicacion. xUnit con dobles a mano y reloj fijo."
license: Apache-2.0
metadata:
  author: "Hever Lima <hever.lima@evolsys.pe>"
  version: "1.0"
---

## Activation Contract

Cargar al escribir o revisar tests de `EyeYul.Dominio` o `EyeYul.Aplicacion`.

## Hard Rules

- Proyecto de test: `tests/EyeYul.UnitTests` (registrado en `EyeYul.slnx`), `net10.0`.
  Referencia **solo** `EyeYul.Dominio` y `EyeYul.Aplicacion`. Nunca
  `EyeYul.Infraestructura` ni `EyeYul.Presentacion`: no hay SQLite, ni registro de
  Windows, ni WPF en los tests.
- Stack: **xUnit** (`[Fact]` / `[Theory]` + `[InlineData]`) con `Assert` de xUnit.
  No hay Moq ni FluentAssertions en el proyecto — **no agregarlos** salvo necesidad
  real; los dobles se escriben a mano porque las interfaces son pequenas.
- **Nombres de test en ingles con guion bajo**, escenario + resultado esperado,
  igual que los existentes: `PerfectDay_StaysAt100`,
  `Idle_BeyondThreshold_IsDeferred`, `Counter_ResetsOnNewDay`.
  Un archivo por tipo bajo prueba.
- **Nada de `DateTime.Now` en un test.** Fijar el tiempo con una constante
  (`private static readonly DateOnly Today = new(2026, 7, 6);`) o con un doble de
  `IReloj`. Un test que depende del reloj real es un test que falla un martes.
- Los tests de reglas puras (`Dominio/Reglas/`) no necesitan dobles: entrada →
  salida. Son los mas valiosos; escribirlos primero.
- `[Theory]` cuando lo unico que varia son los datos (bandas, umbrales).
  `[Fact]` cuando cambia el escenario.
- Cubrir explicitamente los **bordes**: el valor exacto del umbral, cero, y el caso
  "deshabilitado". Ahi es donde viven los bugs (`>=` frente a `>`).

## Dobles a mano

Para `IReloj` y repositorios, clase privada dentro del propio archivo de test:

```csharp
private sealed class RelojFijo(DateTimeOffset ahora) : IReloj
{
    public DateTimeOffset Now { get; } = ahora;
}

private sealed class RepoDescansosEnMemoria : IBreakRepository
{
    public List<Descanso> Guardados { get; } = [];

    public Task AddAsync(Descanso br, CancellationToken ct = default)
    {
        Guardados.Add(br);
        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<Descanso>> GetByDateAsync(DateOnly date, CancellationToken ct = default)
        => Task.FromResult<IReadOnlyList<Descanso>>(Guardados);
}
```

Los efectos se verifican inspeccionando el doble (`repo.Guardados`), no con
`Verify(...)`.

## Decision Gates

| Escenario | Patron de asercion |
|---|---|
| Regla pura devuelve valor | `Assert.Equal(esperado, Regla.Calculate(entrada))` |
| Valor debe quedar acotado | `Assert.InRange(actual, 0, 100)` + el extremo exacto |
| Mapeo de bandas / tabla de casos | `[Theory]` + `[InlineData]` por banda, incluyendo limites |
| Decision de un motor (enum) | `Assert.Equal(PauseDecision.SuppressMeeting, engine.Evaluate(...))` |
| Ajuste desactivado ignora la regla | Settings con la bandera en `false`, esperar `Allow` |
| Umbral | Dos tests: justo por debajo (no dispara) y en/por encima (dispara) |
| Efecto de escritura | Actuar y luego inspeccionar el doble (`Assert.Single(repo.Guardados)`) |
| Estado que se reinicia por dia | Actuar en `Today`, aseverar, repetir con `Today.AddDays(1)` |
| Fecha/hora concreta | `new DateTimeOffset(2026, 7, 6, 9, 0, 0, TimeSpan.Zero)` (6/7/2026 = lunes) |

## Execution Steps

1. Crear `tests/EyeYul.UnitTests/<Tipo>Tests.cs` con namespace `EyeYul.UnitTests`.
2. Un `[Fact]` por rama del metodo bajo prueba: camino feliz, cada guarda que corta
   el flujo, y los bordes de cada umbral.
3. Si hace falta tiempo o repositorio, agregar el doble privado en el mismo archivo.
4. Ejecutar `dotnet test EyeYul.slnx`.
5. Si un test falla, **arreglar el codigo o el test — nunca borrar la asercion**
   para que pase.

## Output Contract

- Numero de tests agregados y el resultado real (`Passed: N, Failed: 0`) copiado de
  la salida, no supuesto.
- Ramas sin cubrir y por que (p. ej. requiere Win32 o WPF).
- Si un test revelo un bug, describir el bug antes que el test.

## References

- `tests/EyeYul.UnitTests/ScreenScoreRulesTests.cs` — ejemplo de regla pura con `[Theory]`
- `tests/EyeYul.UnitTests/SmartPauseEngineTests.cs` — ejemplo de motor con enums y umbrales
- `.claude/skills/backend-feature/SKILL.md` — convenciones que estos tests deben reflejar
