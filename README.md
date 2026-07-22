# EyeYul

App de escritorio para **Windows 10/11** del sistema EyeYul: pausas inteligentes,
recordatorios, **Screen Score**, estadísticas y bandeja del sistema. Su objetivo es que
las pausas **respeten el flujo** del usuario (no interrumpir en juegos, reuniones o video).

> Stack: **C# 14 / .NET 10 (LTS)**. Capa de presentación en **WPF**.
> El resto de capas son UI-agnósticas y migrables a WinUI 3.

## Arquitectura (limpia por capas)

La dirección de dependencias es estricta:

```
Presentacion  →  Infraestructura  →  Aplicacion  →  Dominio
```

```
fuente/
  EyeYul.Dominio          net10.0          Entidades, enums, objetos valor y reglas puras. Sin dependencias.
  EyeYul.Aplicacion       net10.0          Casos de uso + interfaces en Abstracciones/. No conoce WPF ni SQLite.
  EyeYul.Infraestructura  net10.0-windows  Interop Win32, repositorios SQLite, ajustes JSON, auto-inicio.
  EyeYul.Presentacion     net10.0-windows  WPF. AssemblyName = EyeYul.
  EyeYul.NativeHost       net10.0-windows  Native messaging host para estadísticas web por dominio.
tests/
  EyeYul.UnitTests        net10.0          xUnit. Solo referencia Dominio y Aplicacion.
```

- **Dominio**: entidades (`Descanso`, `Sesion`, `PuntajeVisualDiario`, `UsoSitioWeb`…) y
  reglas puras (`ScreenScoreRules`). Sin dependencias, y es lo que cubren los tests.
- **Aplicacion**: orquestación. Servicios hosted `MonitorActividad` y
  `ProgramadorDescansos`, motor `MotorPausaInteligente`, `PoliticaPausa`,
  `ServicioPuntajeVisual`, `ServicioEstadisticas`, `ServicioDescansoProgramado`.
- **Infraestructura**: P/Invoke (app en primer plano, inactividad, pantalla completa,
  micrófono/cámara vía consent store, grabación por lista de procesos), repositorios
  **SQLite** (`Microsoft.Data.Sqlite`), ajustes JSON y auto-inicio por registro.
- **Presentacion**: icono de bandeja (`H.NotifyIcon`), overlay de pausa por monitor,
  countdown flotante click-through, toasts con snooze, ajustes y dashboard (MVVM con
  `CommunityToolkit.Mvvm`).

## Requisitos

- .NET SDK **10.0.3xx** (`dotnet --version`).
- Windows 10 1809+ / Windows 11.

## Compilar y ejecutar

```powershell
dotnet build EyeYul.slnx
dotnet test                                    # pruebas de dominio y aplicación
dotnet run --project fuente/EyeYul.Presentacion
```

> El repo incluye un `NuGet.config` propio que deja solo `nuget.org` como fuente.
> Sin él, un feed privado configurado a nivel de máquina puede hacer fallar el restore.

Al ejecutarse aparece el **icono en la bandeja**. Menú (clic derecho):

- **Vista General** — abre la ventana principal.
- **Tomar pausa ahora** — fuerza el overlay de pausa en todos los monitores.
- **Estadísticas…** — Screen Score del día + top de apps/sitios.
- **Ajustes…** — intervalo, duración, pausa inteligente, snooze, auto-inicio, etc.
- **Salir**.

Cerrar la ventana principal solo la **oculta**: la app sigue viva en la bandeja.

Los datos se guardan localmente (privacidad por defecto) y **no se versionan**:

- Base de datos: `%LOCALAPPDATA%\EyeYul\eyeyul.db`
- Ajustes: `%LOCALAPPDATA%\EyeYul\settings.json`

Ambos se recrean solos al arrancar, así que un clon limpio funciona sin más. Para
conservar tu historial y preferencias entre máquinas, copia esos dos ficheros a mano.

## Estado del proyecto

Implementado (MVP, pasos 1–6 del roadmap):

- [x] Host + DI + icono de bandeja + Quick Look
- [x] `ProgramadorDescansos` + overlay de pausa por monitor + countdown flotante
- [x] `MonitorActividad` + `MotorPausaInteligente` (fullscreen / media / mic-cam / idle / focus assist)
- [x] Notificaciones con snooze (+1/+5/+15) + `PoliticaPausa`
- [x] Persistencia SQLite + `ServicioPuntajeVisual` + `ServicioEstadisticas` + dashboard
- [x] Pausas planificadas (`ServicioDescansoProgramado`) y auto-inicio con Windows
- [x] Tema claro/oscuro/sistema aplicado en caliente

Siguiente iteración (documentado, no implementado aún):

- [ ] Extensión de navegador (MV3) que alimente `EyeYul.NativeHost` para stats por dominio
- [ ] Empaquetado: instalador standalone (WiX/Velopack) + **MSIX** (Store)
- [ ] Personalización avanzada (sonidos/fondos) y automatizaciones

## Pruebas

```powershell
dotnet test
```

58 pruebas sobre la lógica pura y los servicios sin I/O: `ScreenScoreRules`,
`MotorPausaInteligente`, `PoliticaPausa`, transiciones de `Descanso`, `DuracionPausa`,
motivos de supresión y `ServicioDescansoProgramado`.

`EyeYul.UnitTests` solo referencia `Dominio` y `Aplicacion`: no hay SQLite, ni registro
de Windows, ni WPF en las pruebas.

## Si una pausa no aparece

Casi siempre es **Smart Pause** suprimiéndola (reunión, pantalla completa, video o
modo No molestar). La app avisa del motivo con un toast. Para confirmarlo, la tabla
`breaks` de la base de datos registra cada intento; su columna `Outcome` es
`0=Pending 1=Completed 2=Snoozed 3=Skipped 4=Suppressed`.

Los filtros se desactivan en **Ajustes → Pausa inteligente**.

## Documentación

- [`docs/Arquitectura_EyeYul.md`](docs/Arquitectura_EyeYul.md) — arquitectura de
  referencia y mapeo de cada requisito a su API de Windows.
- [`docs/design-system/`](docs/design-system/) — sistema de diseño: tokens, guías de
  color y tipografía, catálogo de componentes y el UI kit interactivo
  (`EyeYul App.html`).
- [`CLAUDE.md`](CLAUDE.md) — convenciones del repo y guía para trabajar con el código.
