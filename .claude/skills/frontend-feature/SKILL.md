---
name: frontend-feature
description: "Trigger: nueva pantalla, ventana, control o seccion de UI en EyeYul. WPF + MVVM con CommunityToolkit.Mvvm, paleta clara/oscura por DynamicResource, bandeja del sistema."
license: Apache-2.0
metadata:
  author: "Hever Lima <hever.lima@evolsys.pe>"
  version: "1.0"
---

## Activation Contract

Cargar al agregar o modificar una ventana, control, ViewModel o recurso visual en
`fuente/EyeYul.Presentacion`.

## Estructura

```
EyeYul.Presentacion/          AssemblyName = EyeYul  (los pack:// URIs dependen de esto)
  App.xaml(.cs)               Generic Host + DI; ShutdownMode=OnExplicitShutdown
  Vistas/                     Ventanas (VentanaPrincipal, VentanaAjustes...)
  Componentes/                UserControls reutilizables (TarjetaBase, BotonRelleno...)
  Controles/                  Controles con logica de dibujo propia (AnilloProgreso)
  ModelosVista/               ViewModels (VistaGeneralModelo, AjustesModelo...)
  Temas/                      Palette.xaml, PaletteDark.xaml, Icons.xaml, Controles.xaml
  Bandeja/                    Icono de bandeja y menu
  CapaSuperpuesta/            Overlay de pausa a pantalla completa
  Interoperabilidad/          P/Invoke de ventanas (WindowNative)
  Converters.cs               IValueConverter de la UI
```

## Hard Rules

- **MVVM con `CommunityToolkit.Mvvm`.** El ViewModel hereda `ObservableObject` y la
  clase es `partial` (lo exige el generador). Estado con `[ObservableProperty]`
  sobre campo `_camelCase`; acciones con `[RelayCommand]`. Nunca escribir a mano el
  `OnPropertyChanged` de una propiedad que puede generar el toolkit.
- **Todo color va por `{DynamicResource ...}`, nunca `StaticResource`.** El cambio
  de tema muta los brushes en caliente; un `StaticResource` se queda con el color
  del arranque. Cero excepciones para los brushes tematizados.
- **Nunca un color literal** (`Background="White"`, `#FFFFFF`) en superficies, texto
  o bordes. Solo se permite en el overlay de pausa, que es oscuro por diseno, y en
  velos semitransparentes (`#22000000`).
- **Un control nativo de WPF sin estilo se pinta claro siempre.** `TextBox`,
  `CheckBox`, `Slider`, `ComboBox`, `ScrollViewer`, `ListBox` necesitan estilo
  implicito en `Temas/Controles.xaml`. Al introducir un control nativo nuevo,
  agregar ahi su estilo o se vera blanco en tema oscuro.
- **Un brush nuevo debe existir en `Palette.xaml` y en `PaletteDark.xaml`.** Si sus
  valores difieren entre temas, agregar su clave a `ClavesTemables` en
  `Temas/TemaAplicador.cs`; si no, no cambiara al alternar el tema.
- **El code-behind (`.xaml.cs`) solo hace lo que el binding no puede**: constructor,
  `DataContext`, arrastre de ventana, colocacion nativa. Nada de logica de negocio
  ni acceso a repositorios.
- **No escribir `InitializeComponent`, `Connect` ni los campos `x:Name`**: los genera
  el compilador de markup a partir del XAML.
- Cerrar la ventana principal **oculta** (`Hide()`), no cierra la app: EyeYul vive en
  la bandeja (`ShutdownMode="OnExplicitShutdown"`).
- Los ViewModels se registran `AddTransient` en `App.OnStartup`; los controladores de
  larga vida (bandeja, overlay) `AddSingleton`.
- Multi-monitor: colocar ventanas con `WindowNative.PlaceAtPhysical` en pixeles
  fisicos. **`Screen.AllScreens` no garantiza que el primario sea el primero** —
  seleccionarlo con `screen.Primary`, nunca por indice.
- La ventana principal solo refresca en vivo si esta visible
  (`ActualizacionesEnVivo`). Respetarlo: el scheduler late cada segundo.
- Tocar la UI desde un servicio en segundo plano siempre via
  `Application.Current?.Dispatcher.BeginInvoke(...)`.

## Decision Gates

| Situacion | Accion |
|---|---|
| Dato que la vista muestra y cambia | `[ObservableProperty]` en el ViewModel |
| Valor derivado de otras propiedades | Propiedad calculada + `OnPropertyChanged(nameof(...))` desde `partial void OnXxxChanged` |
| Accion de usuario | `[RelayCommand]`; `async Task` si hace I/O |
| Bloque visual repetido 2+ veces | `UserControl` en `Componentes/` con `DependencyProperty` |
| Control con logica de dibujo propia | `Controles/` (ver `AnilloProgreso`) |
| Ventana secundaria | `Vistas/` + ViewModel propio, abierta desde `ControladorIconoBandeja` |
| Transformacion solo para mostrar | `IValueConverter` en `Converters.cs` |
| Icono nuevo | `Geometry` en `Temas/Icons.xaml`, resuelto con `IconKeyToGeometryConverter` |
| Aviso corto no bloqueante | `IServicioNotificacion.ShowInfoAsync` (toast propio) |
| Confirmacion modal | `VentanaDialogoAlerta.Mostrar(titulo, mensaje, icono)` |
| Estado solo de la vista (pestana activa) | `[ObservableProperty]`, no `AjustesEyeYul` |
| Preferencia que debe persistir | `AjustesEyeYul` + `IAlmacenAjustes.SaveAsync` |

## Execution Steps

1. ViewModel en `ModelosVista/` (`sealed partial class ... : ObservableObject`), con
   dependencias por constructor.
2. XAML en `Vistas/` o `Componentes/`, con `{DynamicResource}` para todo color.
3. Code-behind minimo: `InitializeComponent()` + `DataContext`.
4. Registrar el ViewModel en `App.OnStartup`.
5. Si hay control nativo nuevo, agregar su estilo implicito en `Temas/Controles.xaml`.
6. Si hay brush nuevo, agregarlo a **ambas** paletas (+ `ClavesTemables` si difiere).
7. `dotnet build EyeYul.slnx`.
8. **Ejecutar la app y comprobarlo en los dos temas.** Un cambio de UI no esta
   verificado hasta verlo; probar claro y oscuro.

## Output Contract

- Archivos creados (vista, ViewModel, estilos) y registros de DI.
- Brushes/iconos agregados, confirmando que estan en ambas paletas.
- Confirmacion de que se ejecuto la app y se vio en tema claro **y** oscuro.
- Controles nativos introducidos y su estilo asociado.

## References

- `assets/code-conventions.md` — convenciones de nombres y orden dentro del archivo
- `assets/component-catalog.md` — componentes y brushes ya disponibles
- `.claude/skills/backend-feature/SKILL.md` — de donde salen los datos que muestra la UI
