# EyeYul — Stack Tecnológico y Arquitectura del Proyecto

App de escritorio nativa para Windows 10/11 que replica la funcionalidad de LookAway (pausas inteligentes, recordatorios, Screen Score, estadísticas, bandeja del sistema). Documento de referencia para iniciar el desarrollo.

---

## 1. Stack recomendado (resumen)

| Capa | Tecnología | Por qué |
|---|---|---|
| Lenguaje / Runtime | **C# 14 + .NET 10 (LTS)** | LTS con soporte hasta nov 2028; acceso completo a Win32 e integración profunda con el SO. |
| UI | **WinUI 3 (Windows App SDK 1.x)** | Fluent Design nativo, materiales **Mica/Acrylic**, tema claro/oscuro automático. Es el equivalente al "Liquid Glass" de LookAway en Windows. |
| Patrón UI | **MVVM** con `CommunityToolkit.Mvvm` | Separación vista/lógica, `[ObservableProperty]`, `[RelayCommand]`. |
| Host / background | **.NET Generic Host** (`Microsoft.Extensions.Hosting`) | Servicios en segundo plano (schedulers, monitores) + inyección de dependencias. |
| Persistencia | **SQLite** (`Microsoft.Data.Sqlite` o **EF Core 10**) | Historial de sesiones, stats por app y por dominio. Local por defecto (privacidad). |
| Ajustes | JSON en `%LOCALAPPDATA%` / `ApplicationData.LocalSettings` | Configuración de pausas, sonidos, límites. |
| Interop con Windows | **P/Invoke + CsWin32** (`Microsoft.Windows.CsWin32`) | Genera bindings tipados de Win32 en tiempo de compilación. |
| Ícono de bandeja | **H.NotifyIcon.WinUI** | Tray icon moderno para WinUI 3, con menú, estados y soporte de área de iconos ocultos. |
| Notificaciones | **`Microsoft.Windows.AppNotifications`** (Windows App SDK) | Toasts con botones de acción (snooze +1/+5/+15). |
| Gráficos de stats | **LiveChartsCore** o **ScottPlot** | Dashboard de estadísticas y Screen Score. |
| Empaquetado | **MSIX** (Store/winget) + build **unpackaged** (standalone) | Dos SKUs, como LookAway (Store vs standalone con más permisos). |
| IDE | **Visual Studio 2026** | Soporte de primera para WinUI 3 / Windows App SDK. |
| Tests | **xUnit** + **WinAppDriver/Appium** (UI) | Unitarias de lógica + E2E de flujos de pausa. |

**Alternativa válida:** si se prioriza máxima compatibilidad con equipos antiguos y menos fricción de empaquetado, usar **WPF + .NET 10** (también soporta Mica vía `DesktopAcrylicController`/`MicaController`). WinUI 3 es la opción moderna preferida; WPF es el "plan B" robusto.

---

## 2. La parte crítica: cómo lograr que funcione IGUAL que LookAway

El corazón de una app así no es la UI, sino la **detección de actividad del sistema** para que las pausas "respeten el flujo". Este es el mapeo funcionalidad → API/librería concreta:

| Funcionalidad (requerimiento) | Cómo implementarlo en Windows |
|---|---|
| **Ícono pequeño en bandeja, ocultable** (RQ-UI-01x) | `H.NotifyIcon.WinUI`. El estado de "oculto/visible" lo controla el propio usuario en el overflow de la barra de tareas; la app solo registra el icono. |
| **Detección de app en primer plano** (para stats por app) | `GetForegroundWindow` → `GetWindowThreadProcessId` → `Process.GetProcessById`. |
| **Detección de juego a pantalla completa** (RQ-SMART-04) | `SHQueryUserNotificationState` → estados `QUNS_RUNNING_D3D_FULL_SCREEN` / `QUNS_PRESENTATION_MODE`; complementar comparando el rect de la ventana con los bounds del monitor. |
| **Detección de reunión / llamada** (RQ-SMART-04) | (a) Uso de **micrófono/cámara**: leer el consent store del registro (`CapabilityAccessManager`, `LastUsedTimeStop == 0` ⇒ en uso). (b) Sesiones de audio activas vía **WASAPI** (`IAudioSessionManager2`). (c) Fallback por lista de procesos (Teams, Zoom, Meet, Slack, Discord). |
| **Detección de reproducción de video** (RQ-SMART-04) | `GlobalSystemMediaTransportControlsSessionManager` (`Windows.Media.Control`) para ver si hay media en `Playing`; combinar con sesión de audio de render activa. |
| **Detección de grabación de pantalla** (RQ-SMART-04) | Difícil de forma directa; usar `SHQueryUserNotificationState` (presentation mode) + lista de grabadores conocidos (OBS, ShareX, Game Bar, Snagit). |
| **Detección de inactividad / AFK** (Planned Breaks cuentan tiempo ausente, RQ-PLAN-05) | `GetLastInputInfo` para tiempo desde el último input. |
| **Bloqueo/desbloqueo de sesión** (contar como pausa) | `SystemEvents.SessionSwitch` o `WTSRegisterSessionNotification` (lock/unlock). |
| **Pantalla de pausa a pantalla completa por monitor** | Una ventana WinUI **borderless + topmost** por cada display (enumerar con `DisplayArea.FindAll()`), con fondo animado/imagen/gradiente. |
| **Countdown flotante que sigue al cursor** (RQ-UI-06) | Ventana pequeña, transparente, `WS_EX_LAYERED | WS_EX_TRANSPARENT` (click-through), topmost, reposicionada con `GetCursorPos` en un timer. |
| **Notificaciones con snooze +1/+5/+15** (RQ-SMART-03) | `AppNotificationBuilder` con `AddButton` y argumentos; manejar la activación en `AppNotificationManager.NotificationInvoked`. |
| **Integración con Asistente de concentración** (RQ-UI-09) | `SHQueryUserNotificationState` (`QUNS_QUIET_TIME`) y/o el estado WNF de Focus Assist para suprimir/adaptar avisos. |
| **Estadísticas de sitios web por dominio** (RQ-STAT-04/06) | **Recomendado:** extensión de navegador (Manifest V3) + **Native Messaging host** que reporta el dominio de la pestaña activa. Cubre Chromium **y** Firefox. **Alternativa frágil:** leer la barra de direcciones vía **UI Automation** (se rompe en el sandbox de la versión Store → de ahí la limitación de Firefox en Store). |
| **Atajos de teclado globales** (RQ-UI-08) | `RegisterHotKey` (P/Invoke) o hook de bajo nivel `SetWindowsHookEx`. |
| **Automatizaciones al iniciar/terminar pausa** (RQ-AUTO-01) | `Process.Start` lanzando `pwsh`/`powershell` con el script, un ejecutable, o una tarea del Programador de tareas. |
| **Inicio automático con Windows** (RQ-PLAT-05) | MSIX: `StartupTask`. Unpackaged: clave `HKCU\...\Run` del registro. |
| **Screen Score** (RQ-STAT-02/03) | Servicio de dominio que parte de 100 y penaliza por pausas omitidas y por sesiones largas sin descanso; recalculado en tiempo real y persistido por día. |

> **Nota sobre Store vs standalone:** la versión de **Microsoft Store** corre en sandbox MSIX, lo que limita hooks globales, UI Automation entre procesos y lectura de ciertos registros. Por eso, igual que LookAway distingue App Store vs standalone, EyeYul debe tener una **build standalone (unpackaged)** con capacidades completas y una **build Store** con el subconjunto permitido.

---

## 3. Arquitectura de la solución (capas)

Arquitectura **limpia por capas**, con el Generic Host orquestando servicios en segundo plano y la UI (WinUI) como una capa delgada sobre los ViewModels.

```
┌──────────────────────────────────────────────────────────────┐
│  EyeYul.App (WinUI 3)  ── Presentación                        │
│  Views (XAML) · ViewModels (MVVM) · TrayIcon · BreakOverlay   │
│  Toasts · Settings UI · Stats Dashboard                       │
└───────────────▲──────────────────────────────────────────────┘
                │ (DI · MVVM binding)
┌───────────────┴──────────────────────────────────────────────┐
│  EyeYul.Application  ── Casos de uso / servicios              │
│  BreakScheduler · PlannedBreakService · SmartPauseEngine      │
│  ActivityMonitor · ScreenScoreService · StatsService          │
│  SnoozePolicy · AutomationRunner                              │
└───────────────▲──────────────────────────────────────────────┘
                │ (interfaces)
┌───────────────┴───────────────┐   ┌──────────────────────────┐
│  EyeYul.Domain                │   │  EyeYul.Infrastructure   │
│  Modelos: Break, Session,     │   │  Win32 interop (CsWin32) │
│  ScreenScore, WebsiteUsage,   │   │  SQLite/EF · Settings    │
│  Reglas puras (sin deps)      │   │  Notificaciones · Startup│
└───────────────────────────────┘   │  NativeMessaging host    │
                                     └──────────────────────────┘
```

**Responsabilidades clave por servicio:**
- `ActivityMonitor` (hosted service): sondea/escucha estados del sistema (foreground app, fullscreen, mic/cam, media, idle, lock) y publica eventos.
- `SmartPauseEngine`: decide *cuándo* puede dispararse una pausa según los eventos del `ActivityMonitor` (respeta el flujo).
- `BreakScheduler` (hosted service): temporiza pausas por intervalo; coordina con `PlannedBreakService` para no duplicar.
- `PlannedBreakService`: pausas a hora fija, días de repetición, "iniciar/posponer/omitir".
- `SnoozePolicy`: límites por día y por sesión (RQ-ENF).
- `ScreenScoreService` + `StatsService`: agregan datos de sesión, apps y dominios; persisten en SQLite.
- `AutomationRunner`: ejecuta scripts al iniciar/terminar pausa.

---

## 4. Estructura de carpetas del proyecto (solución)

```
EyeYul.sln
│
├─ src/
│  ├─ EyeYul.App/                     # WinUI 3 — capa de presentación
│  │  ├─ App.xaml(.cs)                # bootstrap del Generic Host + DI
│  │  ├─ Views/
│  │  │  ├─ SettingsWindow.xaml
│  │  │  ├─ StatsPage.xaml
│  │  │  ├─ BreakOverlayWindow.xaml   # pantalla de pausa (una por monitor)
│  │  │  └─ FloatingCountdown.xaml    # countdown que sigue al cursor
│  │  ├─ ViewModels/
│  │  ├─ Tray/                        # H.NotifyIcon: icono + Quick Look
│  │  ├─ Assets/                      # iconos (16/32/…), sonidos, fondos
│  │  └─ Notifications/               # AppNotification builders/handlers
│  │
│  ├─ EyeYul.Application/             # lógica de negocio (sin UI)
│  │  ├─ Breaks/                      # BreakScheduler, PlannedBreakService…
│  │  ├─ Activity/                    # SmartPauseEngine, ActivityMonitor
│  │  ├─ Scoring/                     # ScreenScoreService
│  │  ├─ Stats/                       # StatsService
│  │  ├─ Enforcement/                 # SnoozePolicy
│  │  └─ Automation/                  # AutomationRunner
│  │
│  ├─ EyeYul.Domain/                  # modelos y reglas puras
│  │  ├─ Entities/                    # Break, Session, ScreenScore…
│  │  └─ ValueObjects/
│  │
│  ├─ EyeYul.Infrastructure/          # detalles técnicos
│  │  ├─ Interop/                     # CsWin32: monitores, mic/cam, idle…
│  │  ├─ Persistence/                 # DbContext, repositorios SQLite
│  │  ├─ Settings/                    # lectura/escritura JSON
│  │  └─ Startup/                     # auto-inicio (StartupTask/registro)
│  │
│  └─ EyeYul.NativeHost/              # native messaging host (stats web)
│
├─ browser-extension/                 # extensión MV3 (Chromium + Firefox)
│  ├─ manifest.json
│  └─ background.js                   # reporta dominio de pestaña activa
│
├─ packaging/
│  ├─ EyeYul.Package/                 # proyecto MSIX (Store/winget)
│  └─ installer/                      # instalador standalone (.msi/.exe, ej. WiX)
│
└─ tests/
   ├─ EyeYul.UnitTests/               # xUnit — lógica de dominio/servicios
   └─ EyeYul.UiTests/                 # WinAppDriver/Appium — flujos E2E
```

---

## 5. Paquetes NuGet principales

- `Microsoft.WindowsAppSDK` — WinUI 3, notificaciones, DisplayArea.
- `Microsoft.Windows.CsWin32` — bindings Win32 tipados (source generator).
- `CommunityToolkit.Mvvm` — MVVM.
- `Microsoft.Extensions.Hosting` + `Microsoft.Extensions.DependencyInjection` — host y DI.
- `H.NotifyIcon.WinUI` — ícono de bandeja.
- `Microsoft.Data.Sqlite` o `Microsoft.EntityFrameworkCore.Sqlite` — persistencia.
- `LiveChartsCore.SkiaSharpView.WinUI` (o `ScottPlot`) — gráficos.
- `Serilog` — logging.
- `NAudio` o WASAPI vía CsWin32 — enumerar sesiones de audio (video/llamadas).

---

## 6. Empaquetado y distribución (2 SKUs)

- **Standalone (unpackaged):** instalador `.msi`/`.exe` (WiX o Velopack). Capacidades completas: hooks globales, UI Automation, seguimiento de Firefox. Distribuido por descarga directa y **winget**.
- **Microsoft Store (MSIX):** empaquetado con sandbox. Subconjunto de capacidades (sin lo que el sandbox bloquea). Auto-inicio vía `StartupTask`.
- Actualizaciones: **Velopack** o **Squirrel** para el canal standalone; la Store gestiona las suyas.

---

## 7. Riesgos técnicos y decisiones a cerrar

1. **Detección de grabación de pantalla**: no hay API pública fiable; se resuelve con heurísticas (presentation mode + lista de procesos). Definir la lista y hacerla actualizable.
2. **Stats por dominio en Store**: el sandbox impide UI Automation entre procesos → decisión de producto: la extensión de navegador es la vía robusta para ambos canales; documentar la limitación como en LookAway.
3. **Multi-monitor y DPI mixto**: probar overlay y countdown en configuraciones de varios monitores con distinto escalado.
4. **Consumo en segundo plano**: el `ActivityMonitor` debe usar eventos donde existan y sondeo de baja frecuencia donde no, para no penalizar batería/CPU.
5. **Falsos positivos de "reunión"**: micrófono activo no siempre es llamada (p. ej. dictado); combinar señales antes de suprimir una pausa.

---

## 8. Orden sugerido de implementación (MVP → paridad)

1. Host + DI + ícono de bandeja + Quick Look (esqueleto que corre en segundo plano).
2. `BreakScheduler` + overlay de pausa por monitor + countdown flotante (pausas por intervalo básicas).
3. `ActivityMonitor` + `SmartPauseEngine` (respetar el flujo: fullscreen, media, mic/cam, idle).
4. Notificaciones con snooze + `SnoozePolicy` (límites por día/sesión).
5. `PlannedBreakService` (pausas planificadas).
6. Persistencia + `StatsService` + `ScreenScoreService` + dashboard.
7. Extensión de navegador + native host (stats por dominio).
8. Personalización (sonidos/fondos/mensajes) + automatizaciones + Focus Assist.
9. Empaquetado (standalone + MSIX) y auto-actualización.

---

*Stack verificado a julio de 2026: .NET 10 LTS (soporte hasta nov 2028), Windows App SDK / WinUI 3, Visual Studio 2026, C# 14.*
