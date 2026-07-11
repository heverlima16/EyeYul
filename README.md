# EyeYul

App de escritorio para **Windows 10/11** del sistema EyeYul: pausas inteligentes,
recordatorios, **Screen Score**, estadísticas y bandeja del sistema. Su objetivo es que
las pausas **respeten el flujo** del usuario (no interrumpir en juegos, reuniones o video).

> Stack: **C# 14 / .NET 10 (LTS)**. Capa de presentación en **WPF** (materiales Fluent vía
> Mica/Acrylic). El backend es UI-agnóstico y migrable a WinUI 3.

## Arquitectura (limpia por capas)

```
EyeYul.App (WPF)        Presentación: Host+DI, bandeja, overlay, countdown, ajustes, stats
   │
EyeYul.Application      Casos de uso: BreakScheduler, SmartPauseEngine, ScreenScore, Stats, Snooze
   │            ┌───────────────────────────────┐
EyeYul.Domain  │  EyeYul.Infrastructure         │
Entidades y     │  Win32 interop, SQLite,        │
reglas puras    │  settings JSON, auto-inicio    │
                └───────────────────────────────┘
```

- **Domain** (`net10.0`): entidades (`Break`, `Session`, `DailyScreenScore`, `WebsiteUsage`…)
  y reglas puras (`ScreenScoreRules`). Sin dependencias.
- **Application** (`net10.0`): orquestación. Servicios hosted `ActivityMonitor` y
  `BreakScheduler`, motor `SmartPauseEngine`, `SnoozePolicy`, `ScreenScoreService`,
  `StatsService`, `PlannedBreakService`. Solo depende de abstracciones.
- **Infrastructure** (`net10.0-windows`): P/Invoke (foreground app, idle, fullscreen,
  mic/cámara vía consent store, media/grabación por lista de procesos), repositorios
  **SQLite** (`Microsoft.Data.Sqlite`), ajustes JSON, auto-inicio por registro.
- **App** (`net10.0-windows`, WPF): icono de bandeja (`H.NotifyIcon`), overlay de pausa
  por monitor, countdown flotante click-through, toasts con snooze, ajustes y dashboard (MVVM).

## Requisitos

- .NET SDK **10.0.3xx** (`dotnet --version`).
- Windows 10 1809+ / Windows 11.

## Compilar y ejecutar

```powershell
dotnet build EyeYul.sln
dotnet test                       # pruebas de dominio/servicios
dotnet run --project src/EyeYul.App
```

Al ejecutarse aparece el **icono en la bandeja**. Menú (clic derecho):

- **Tomar pausa ahora** — fuerza el overlay de pausa en todos los monitores.
- **Estadísticas…** — Screen Score del día + top de apps/sitios.
- **Ajustes…** — intervalo, duración, pausa inteligente, snooze, auto-inicio, etc.
- **Salir**.

Los datos se guardan localmente (privacidad por defecto):

- Base de datos: `%LOCALAPPDATA%\EyeYul\eyeyul.db`
- Ajustes: `%LOCALAPPDATA%\EyeYul\settings.json`

## Estado del proyecto

Implementado (MVP, pasos 1–6 del roadmap):

- [x] Host + DI + icono de bandeja + Quick Look
- [x] `BreakScheduler` + overlay de pausa por monitor + countdown flotante
- [x] `ActivityMonitor` + `SmartPauseEngine` (fullscreen / media / mic-cam / idle / focus assist)
- [x] Notificaciones con snooze (+1/+5/+15) + `SnoozePolicy`
- [x] Persistencia SQLite + `ScreenScoreService` + `StatsService` + dashboard
- [x] Pausas planificadas (`PlannedBreakService`) y auto-inicio con Windows

Siguiente iteración (documentado, no implementado aún):

- [ ] `browser-extension/` (MV3) + `EyeYul.NativeHost` para stats por dominio web
- [ ] Empaquetado: instalador standalone (WiX/Velopack) + **MSIX** (Store)
- [ ] Migración de la capa de presentación a **WinUI 3**
- [ ] Personalización avanzada (sonidos/fondos) y automatizaciones

## Pruebas

`dotnet test` cubre la lógica pura: `ScreenScoreRules`, `SmartPauseEngine`, `SnoozePolicy`
y el cálculo de próximas pausas planificadas.

## Créditos

Basado en la arquitectura de referencia `Arquitectura_EyeYul.md` y el UI Kit `EyeYul App.html`.
