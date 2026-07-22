# Convenciones de codigo — C# / WPF (EyeYul)

## Idioma

- **Tipos, archivos y miembros publicos en espanol**: `VentanaAjustes`,
  `ProgramadorDescansos`, `ElegirMensajeDePausa`.
- **Nombres de test en ingles** con guion bajo (`PerfectDay_StaysAt100`) — es la
  convencion que ya tiene el proyecto.
- **Todo el texto de UI en espanol**, tuteando, tono cercano y de cuidado personal:
  "Mira a 6 metros de distancia y relaja la vista", nunca corporativo.
- Los comentarios explican **por que**, no que. Si el codigo ya dice que hace, el
  comentario sobra.

## Orden dentro de una clase

1. Campos `static readonly` / constantes
2. Campos de instancia (`_camelCase`)
3. Campos con `[ObservableProperty]`
4. Propiedades publicas
5. Eventos
6. Constructor
7. Metodos publicos
8. Comandos (`[RelayCommand]`)
9. Metodos privados

## ViewModels

```csharp
public sealed partial class MiModelo : ObservableObject   // partial obligatorio
{
    private readonly IServicio _servicio;

    [ObservableProperty]
    private string _titulo = "";           // genera la propiedad Titulo

    public MiModelo(IServicio servicio) => _servicio = servicio;

    // Gancho del generador cuando cambia Titulo
    partial void OnTituloChanged(string value) => OnPropertyChanged(nameof(Derivado));

    public string Derivado => $"[{Titulo}]";

    [RelayCommand]
    private async Task Guardar() => await _servicio.GuardarAsync();
}
```

- El campo va en `_camelCase`; el generador crea la propiedad en `PascalCase`.
- Para reaccionar a un cambio: `partial void OnXxxChanged(T value)`.
- Un `Task` que se lanza sin esperar se marca con `_ =` y se explica por que.

## XAML

- Un atributo por linea cuando el elemento pasa de ~100 columnas.
- Orden de atributos: `x:Name`, `x:Class`, layout (`Grid.Row`, `Margin`), tamano,
  apariencia (`Background`, `Foreground`), comportamiento (`Command`, `Click`).
- Colores **siempre** `{DynamicResource Xxx}`. Nunca `StaticResource` para un brush
  de la paleta, nunca un literal.
- `x:Name` solo si el code-behind realmente lo necesita.
- Los `UserControl` no fijan `Background`: heredan del contenedor.

## Code-behind

Minimo imprescindible:

```csharp
public partial class VentanaAjustes : Window
{
    public VentanaAjustes(AjustesModelo viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
```

No declarar `InitializeComponent`, `IComponentConnector.Connect` ni los campos de
`x:Name`: los genera el compilador de markup.

## Servicios y async

- Constructor primario cuando la clase solo guarda dependencias:
  `public sealed class Servicio(IDep dep, ILogger<Servicio> logger)`.
- `CancellationToken ct = default` como ultimo parametro en todo metodo async
  publico, y propagarlo.
- Nada de `async void` salvo manejadores de eventos de WPF.
- Desde un hilo de fondo, tocar la UI solo con
  `Application.Current?.Dispatcher.BeginInvoke(...)`.

## Errores

- Un hosted service atrapa dentro del tick y loguea; nunca deja morir el bucle.
- `catch` vacio solo con un comentario que justifique por que se ignora
  (ver `LectorAccesoCapacidades`: el consent store puede no existir).
- Lo que el usuario deberia saber se le dice en la UI, no solo en el log. Una
  decision automatica silenciosa es un bug de diagnostico.
