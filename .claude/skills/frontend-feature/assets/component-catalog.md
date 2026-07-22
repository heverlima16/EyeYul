# Catalogo visual de EyeYul

Lo que ya existe. Antes de crear un componente o un color, buscarlo aqui.

## Componentes (`Componentes/`)

| Componente | Para que | Propiedades |
|---|---|---|
| `TarjetaBase` | Tarjeta con fondo, borde y esquinas redondeadas. Contenedor por defecto de cualquier bloque | `Content` |
| `BotonRelleno` | Boton de accion relleno en color de acento | `Texto`, `Comando`, `ParametroComando` |
| `EtiquetaTitulo` | Titulo de seccion en mayusculas, tipografia mono | `Texto` |
| `ElementoClaveValor` | Fila etiqueta / valor para paneles de datos | `Clave`, `Valor` |

## Controles (`Controles/`)

| Control | Para que | Propiedades |
|---|---|---|
| `AnilloProgreso` | Anillo circular de progreso con texto central. Es el reloj de la vista general | `Percent` (0-100), `CenterText`, `RingBrush` |

## Ventanas (`Vistas/`)

| Ventana | Rol |
|---|---|
| `VentanaPrincipal` | Cockpit con sidebar y pestanas. Cerrar solo oculta |
| `VentanaAjustes` | Preferencias, enlazada a `AjustesModelo` |
| `VentanaEstadisticas` | Screen Score del dia y top de apps/sitios |
| `VentanaPantallaDescanso` | Overlay de pausa a pantalla completa, uno por monitor |
| `VentanaNotificacion` | Toast propio abajo a la derecha, con acciones de posponer |
| `VentanaDialogoAlerta` | Modal simple. Usar `VentanaDialogoAlerta.Mostrar(...)` |
| `VentanaCuentaRegresivaFlotante` | Contador flotante click-through que sigue al cursor |

## Brushes tematizados

Cambian entre claro y oscuro. Sus claves estan en `ClavesTemables` de `TemaAplicador`.

| Clave | Uso | Claro | Oscuro |
|---|---|---|---|
| `PanelBrush` | Fondo de la ventana | `#FAF9F6` | `#121212` |
| `SidebarBrush` | Fondo de la barra lateral | `#F3E6D3` | `#1E1E1E` |
| `SurfaceBrush` | Fondo de tarjeta | `#FFFFFF` | `#242424` |
| `SurfaceAltBrush` | Superficie secundaria | `#F9FAFB` | `#2D2D2D` |
| `CardBorderBrush` | Borde de tarjeta | `#E5E7EB` | `#333333` |
| `BorderBrush` | Borde general | `#E5E7EB` | `#333333` |
| `CreamBrush` | Fondo de ventanas secundarias | `#FAF9F6` | `#121212` |
| `InkBrush` | Texto principal | `#111827` | `#F9FAFB` |
| `SubBrush` | Texto secundario | `#6B7280` | `#9CA3AF` |
| `MutedBrush` | Texto apagado | `#9CA3AF` | `#6B7280` |
| `AccentSoftBrush` | Fondo suave del item activo | `#FBEBE5` | `#3B2A23` |

## Brushes de acento (identicos en ambos temas)

No estan en `ClavesTemables` **a proposito**: la identidad de marca no cambia con el
tema. No agregarlos ahi.

| Clave | Valor | Uso |
|---|---|---|
| `AccentBrush` | `#DE7356` | Terracota de marca. Acciones primarias, item activo |
| `AccentStrongBrush` | `#CB5B3E` | Terracota oscuro. Fondo del overlay de pausa |
| `TerracottaBrush` | `#DE7356` | Alias de acento |
| `TerracottaDarkBrush` | `#CB5B3E` | Alias de acento oscuro |
| `GreenBrush` | `#10B981` | Estado correcto |
| `AmberBrush` | `#F59E0B` | Advertencia |
| `BlueBrush` | `#3B82F6` | Informacion |

## Estilos implicitos (`Temas/Controles.xaml`)

`TextBox`, `CheckBox` y `Slider` ya tienen estilo implicito que sigue la paleta.
Cualquier otro control nativo que se use por primera vez hay que agregarlo ahi,
o se vera claro en tema oscuro.

## Converters (`Converters.cs`)

| Converter | Devuelve |
|---|---|
| `ActiveBackgroundConverter` | `AccentSoftBrush` si el valor coincide con el parametro, si no transparente |
| `ActiveSolidBackgroundConverter` | `AccentBrush` / `SurfaceAltBrush` |
| `ActiveForegroundConverter` | `AccentBrush` / `SubBrush` |
| `ActiveInkForegroundConverter` | Blanco / `InkBrush` |
| `ActiveSubForegroundConverter` | Blanco / `SubBrush` |
| `ActiveChipTextConverter` | `"ACTIVO"` / `"PROBAR"` |
| `StringToVisibilityConverter` | `Visible` si el valor coincide con el parametro |
| `CountToVisibilityInverseConverter` | `Visible` solo si la coleccion esta vacia (estado sin datos) |
| `IconKeyToGeometryConverter` | La `Geometry` del icono a partir de su clave |
| `InverseBoolConverter` | Booleano negado |

## Iconos (`Temas/Icons.xaml`)

Claves por nombre: `IcoBell`, `IcoCoffee`, `IcoUtensils`, `IcoCalendar`, `IcoShield`,
`IcoShieldCheck`, `IcoShieldAlert`, y `EyeLogo` (la marca de la zanahoria).
Para uno nuevo: agregar la `Geometry` con clave `Ico<Nombre>` y resolverla con
`IconKeyToGeometryConverter`.
