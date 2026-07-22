---
name: backend-feature
description: "Trigger: nueva funcionalidad, entidad o servicio en las capas no-UI de EyeYul (Dominio, Aplicacion, Infraestructura). Arquitectura limpia por capas, DI con Generic Host, persistencia SQLite."
license: Apache-2.0
metadata:
  author: "Hever Lima <hever.lima@evolsys.pe>"
  version: "1.0"
---

## Activation Contract

Cargar al agregar una entidad de dominio, un servicio de aplicacion, un hosted
service o un repositorio a EyeYul. **No** cargar para trabajo de UI (ver
`frontend-feature`) ni para cambios de esquema SQL (ver `database-changes`).

## Estructura real de la solucion

```
fuente/
  EyeYul.Dominio          net10.0            sin dependencias
  EyeYul.Aplicacion       net10.0            solo abstracciones + Hosting/DI/Logging
  EyeYul.Infraestructura  net10.0-windows    Win32, SQLite, JSON, registro
  EyeYul.Presentacion     net10.0-windows    WPF (AssemblyName = EyeYul)
  EyeYul.NativeHost       net10.0-windows    native messaging del navegador
tests/
  EyeYul.UnitTests        net10.0            xUnit
```

## Hard Rules

- **Direccion de dependencias**: `Presentacion -> Infraestructura -> Aplicacion -> Dominio`.
  `Dominio` no referencia nada. `Aplicacion` nunca referencia `Infraestructura`
  ni WPF: solo define interfaces en `Abstracciones/` que Infraestructura implementa.
- **Nombres en espanol** para tipos y archivos de dominio/servicios
  (`Descanso`, `ProgramadorDescansos`, `MotorPausaInteligente`). Las interfaces de
  repositorio conservan nombres en ingles por consistencia con las existentes
  (`IBreakRepository`, `ISessionRepository`) — no mezclar criterios dentro de un
  mismo archivo.
- **`IReloj` en vez de `DateTime.Now`** en Aplicacion y Dominio. Inyectarlo siempre;
  es lo que hace testeable la logica de tiempo. `SystemClock` es la implementacion real.
- Los repositorios devuelven `Task<IReadOnlyList<T>>` para lecturas y `Task` para
  escrituras, y aceptan `CancellationToken ct = default` como ultimo parametro.
- Un servicio en segundo plano hereda de `BackgroundService` y se registra **dos veces**:
  como singleton concreto (para que la UI pueda suscribirse a sus eventos) y como
  hosted service. Ver `AddEyeYulApplication`:
  ```csharp
  services.AddSingleton<MiServicio>();
  services.AddHostedService(sp => sp.GetRequiredService<MiServicio>());
  ```
- **Toda regla pura va en `Dominio`**, no en el servicio. Si la regla se puede
  expresar sin I/O ni reloj, es una funcion estatica en `Dominio/Reglas/`
  (ver `ScreenScoreRules`). Eso es lo que se cubre con tests.
- Los `.csproj` mantienen `<DebugType>portable</DebugType>` y
  `<EmbedAllSources>true</EmbedAllSources>`. **No quitarlos**: son la red de
  seguridad que permitio recuperar el codigo cuando se perdio.
- Nada de `async void` salvo manejadores de eventos de WPF.
- Un hosted service atrapa y loguea dentro del tick; nunca deja que una excepcion
  mate el bucle (ver `ProgramadorDescansos.ExecuteAsync`).
- Cuando una decision automatica **oculta** algo al usuario (p. ej. suprimir una
  pausa), exponer el motivo con un evento o propiedad publica. Una decision
  silenciosa es un bug de diagnostico.

## Decision Gates

| Situacion | Donde va |
|---|---|
| Regla sin I/O ni tiempo (calculo, validacion) | `Dominio/Reglas/` estatica pura |
| Estado de una entidad + sus transiciones | `Dominio/Entidades/` con metodos `MarkXxx()` |
| Valor inmutable con semantica propia | `Dominio/ObjetosValor/` como `readonly record struct` |
| Orquestacion que necesita repositorios o reloj | `Aplicacion/<Area>/` servicio inyectado |
| Trabajo periodico en segundo plano | `Aplicacion/<Area>/` heredando `BackgroundService` |
| Contrato que Infraestructura debe cumplir | `Aplicacion/Abstracciones/` interfaz |
| P/Invoke, SQLite, registro, ficheros | `Infraestructura/<Area>/` |
| Ajuste configurable por el usuario | Clase en `Aplicacion/Configuracion/AjustesEyeYul.cs` con valor por defecto |

## Execution Steps

1. Si hay entidad nueva: crearla en `Dominio/Entidades/` y sus enums en
   `Dominio/Enumeraciones/`.
2. Si hay regla pura: `Dominio/Reglas/`, con constantes nombradas (nunca numeros
   magicos dentro del calculo).
3. Definir la interfaz del repositorio en `Aplicacion/Abstracciones/Repositories.cs`
   (o archivo propio si el contrato es grande).
4. Implementar el servicio en `Aplicacion/<Area>/` usando constructor primario.
5. Implementar la persistencia en `Infraestructura/Persistencia/` siguiendo
   `database-changes`.
6. Registrar todo en `AddEyeYulApplication` / `AddEyeYulInfrastructure`.
7. Si el ajuste es configurable, agregarlo a `AjustesEyeYul` con su valor por defecto.
8. Tests de la regla pura y del servicio segun `backend-testing`.
9. `dotnet build EyeYul.slnx` y `dotnet test` en verde.

## Output Contract

Al terminar, reportar:
- Archivos creados por capa.
- Registros de DI agregados.
- Ajustes nuevos en `AjustesEyeYul` y su valor por defecto.
- Tests agregados y resultado de `dotnet test`.
- Cualquier regla que se dejo sin test, y por que.

## References

- `assets/file-checklist.md` — checklist de archivos por capa
- `.claude/skills/database-changes/SKILL.md` — esquema SQLite y repositorios
- `.claude/skills/backend-testing/SKILL.md` — convenciones de test
