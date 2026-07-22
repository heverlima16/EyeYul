# CLAUDE.md

Este archivo brinda guía a Claude Code (claude.ai/code) al trabajar con código en este repositorio.

## Qué es EyeYul

App de escritorio de **bienestar digital para Windows 10/11**: pausas visuales
inteligentes (regla 20-20-20), recordatorios de postura y parpadeo, pausas
planificadas, Screen Score y estadísticas de uso.

El objetivo de producto es que las pausas **respeten el flujo** del usuario: no
interrumpir durante juegos a pantalla completa, reuniones, video o mientras está
ausente. Todo en español, con tono cálido y de cuidado personal, nunca corporativo.

Todos los datos son **locales por privacidad**: no hay servidor ni cuenta.

- Base de datos: `%LOCALAPPDATA%\EyeYul\eyeyul.db`
- Ajustes: `%LOCALAPPDATA%\EyeYul\settings.json`

## Stack

C# 14 / .NET 10 (LTS), WPF para la presentación, SQLite vía `Microsoft.Data.Sqlite`,
`CommunityToolkit.Mvvm` para MVVM, `H.NotifyIcon.Wpf` para la bandeja, Generic Host
(`Microsoft.Extensions.Hosting`) para DI y servicios en segundo plano.

## Compilar y ejecutar

```powershell
dotnet build EyeYul.slnx
dotnet test                                    # pruebas de dominio y aplicación
dotnet run --project fuente/EyeYul.Presentacion
```

El repo incluye un `NuGet.config` propio que deja **solo `nuget.org`** como fuente.
Sin él, un feed privado configurado a nivel de máquina hace fallar el restore con
`NU1301`. No hace falta tocar la configuración global.

Al ejecutarse aparece el **icono en la bandeja**; la ventana principal se abre desde
ahí. Cerrar la ventana solo la **oculta** (`ShutdownMode="OnExplicitShutdown"`): se
sale por el menú de la bandeja.

## Arquitectura

Arquitectura limpia por capas. La dirección de dependencias es estricta:

```
Presentacion  →  Infraestructura  →  Aplicacion  →  Dominio
```

```
fuente/
  EyeYul.Dominio          net10.0          Entidades, enums, objetos valor, reglas puras. Sin dependencias.
  EyeYul.Aplicacion       net10.0          Casos de uso + interfaces en Abstracciones/. No conoce WPF ni SQLite.
  EyeYul.Infraestructura  net10.0-windows  P/Invoke Win32, repositorios SQLite, ajustes JSON, auto-inicio.
  EyeYul.Presentacion     net10.0-windows  WPF. AssemblyName = EyeYul (los pack:// URIs dependen de esto).
  EyeYul.NativeHost       net10.0-windows  Native messaging host para estadísticas web por dominio.
tests/
  EyeYul.UnitTests        net10.0          xUnit. Solo referencia Dominio y Aplicacion.
```

`Aplicacion` define las interfaces (`IBreakRepository`, `IProveedorActividadSistema`,
`IControladorPantallaDescanso`…) e `Infraestructura`/`Presentacion` las implementan.
El registro de DI está en `AddEyeYulApplication` y `AddEyeYulInfrastructure`.

### Servicios clave

- `ProgramadorDescansos` (hosted service) — late cada segundo, acumula tiempo activo
  y dispara las pausas. Es el corazón de la app.
- `MonitorActividad` (hosted service) — muestrea el estado del sistema cada 2 s.
- `MotorPausaInteligente` — decide si una pausa puede mostrarse (`PauseDecision`).
- `ServicioPuntajeVisual` / `ServicioEstadisticas` — Screen Score y agregados diarios.
- `PoliticaPausa` — límites de aplazamiento por día y por pausa.
- `ControladorPantallaDescansoWpf` — overlay a pantalla completa, uno por monitor.

Un `BackgroundService` se registra **dos veces**: como singleton concreto (para que
la UI se suscriba a sus eventos) y como hosted service.

## Convenciones

- **Nombres en español** para tipos y archivos de dominio y servicios (`Descanso`,
  `ProgramadorDescansos`), y también para **tablas, columnas y propiedades de entidad**
  (`descansos.ProgramadoEn`, `Descanso.DuracionPlanificada`). Las interfaces de
  repositorio conservan nombres en inglés (`IBreakRepository`), igual que las clases de
  `Configuracion/AjustesEyeYul.cs`: **sus propiedades son las claves de `settings.json`**
  y renombrarlas rompería los ajustes ya guardados en las máquinas de los usuarios.
  Los tests usan nombres en inglés con guion bajo (`PerfectDay_StaysAt100`).
- El esquema SQLite nació en inglés. `BaseDatosSqlite.MigrarEsquemaIngles` renombra
  tablas y columnas en caliente al arrancar; es idempotente y **debe correr antes** de
  los `CREATE TABLE IF NOT EXISTS`, o la tabla nueva se crearía vacía y el historial
  quedaría huérfano.
- Los enums persistidos (`ResultadoDescanso`, `TipoDescanso`) guardan su valor numérico
  en la BD: se pueden renombrar los miembros, **nunca reordenarlos**.
- **`IReloj` en vez de `DateTime.Now`** en Dominio y Aplicacion: es lo que hace
  testeable la lógica de tiempo.
- **Toda regla pura vive en `Dominio/Reglas/`** como función estática, sin I/O ni
  reloj. Es lo que se cubre con tests.
- **En XAML, todo color va por `{DynamicResource}`**, nunca `StaticResource` ni un
  literal. El cambio de tema muta los brushes en caliente.
- Un **control nativo de WPF sin estilo se pinta claro siempre**. `TextBox`,
  `CheckBox` y `Slider` ya tienen estilo implícito en `Temas/Controles.xaml`; al
  introducir otro hay que agregarlo ahí o se verá blanco en tema oscuro.
- Los `.csproj` mantienen `<DebugType>portable</DebugType>` y
  `<EmbedAllSources>true</EmbedAllSources>`. **No quitarlos**: el código fuente se
  perdió una vez y se pudo recuperar íntegro gracias a los PDB portables.
- `Screen.AllScreens` **no garantiza** que el monitor primario sea el primero:
  seleccionarlo con `screen.Primary`, nunca por índice.

## Verificar

**Compilar no es verificar.** Casi todos los bugs reales de esta app (temas,
overlay, multi-monitor, bandeja) compilan perfectamente y solo se ven ejecutándola.
Tras un cambio de UI, ejecutar la app y comprobarlo **en tema claro y oscuro**.

Si una pausa no aparece al llegar el temporizador a cero, casi siempre es Smart Pause
suprimiéndola. Revisar `ProgramadorDescansos.UltimaSupresion` y la tabla `descansos`,
cuya columna `Resultado` es `0=Pendiente 1=Completado 2=Aplazado 3=Omitido 4=Suprimido`.

## Skills

- `.claude/skills/backend-feature` — features en Dominio/Aplicacion/Infraestructura
- `.claude/skills/frontend-feature` — ventanas, controles y ViewModels WPF
- `.claude/skills/database-changes` — esquema SQLite y repositorios
- `.claude/skills/backend-testing` — convenciones de tests xUnit
- `.claude/skills/verify` — checklist de verificación antes de dar algo por terminado
