---
name: verify
description: "Trigger: verificar, comprobar, dar por terminado un cambio en EyeYul. Compilar, correr tests y ejecutar la app de verdad antes de decir que algo funciona."
license: Apache-2.0
metadata:
  author: "Hever Lima <hever.lima@evolsys.pe>"
  version: "1.0"
user-invocable: true
---

## Activation Contract

Cargar antes de dar por cerrado cualquier cambio en EyeYul, y siempre antes de
commitear o subir.

## Principio

**Compilar no es verificar.** EyeYul es una app de escritorio: casi todos sus bugs
reales (temas, overlay, multi-monitor, bandeja) compilan perfectamente y solo se
ven ejecutandola. No declarar nada "funcionando" sin haberlo visto.

## Niveles

| Nivel | Cuando | Comando |
|---|---|---|
| 1. Compilacion | Siempre | `dotnet build EyeYul.slnx -c Debug` |
| 2. Tests | Si se toco Dominio o Aplicacion | `dotnet test EyeYul.slnx` |
| 3. Ejecucion | Si se toco UI, arranque, bandeja u overlay | Ejecutar y mirar |
| 4. Persistencia | Si se toco esquema o ajustes | Revisar el fichero/BD real |

## Nivel 1 — Compilacion

```powershell
dotnet build EyeYul.slnx -c Debug
```

- Objetivo: **0 errores y 0 warnings**.
- Un warning nuevo se arregla o se suprime con justificacion escrita en el `.csproj`
  (como `WFO0003`, que es un falso positivo de WinForms en una app WPF).
- Si el restore falla con `NU1301`/401: es el feed privado de Azure. El
  `NuGet.config` del repo ya lo neutraliza; no tocar la config global de la maquina.

## Nivel 2 — Tests

```powershell
dotnet test EyeYul.slnx
```

- Copiar el conteo real (`Passed: N, Failed: 0`) al reporte. **Nunca suponerlo.**
- Un test rojo se arregla; no se borra la asercion ni se marca `Skip` para pasar.

## Nivel 3 — Ejecucion real

```powershell
dotnet run --project fuente/EyeYul.Presentacion
```

Al arrancar aparece el **icono en la bandeja** y la ventana principal.

Checklist segun lo tocado:

- **Tema** — alternar Claro / Oscuro / Sistema y recorrer las pestanas. Buscar
  cualquier superficie, texto o control que se quede claro en tema oscuro (los
  controles nativos de WPF sin estilo son el sospechoso habitual).
- **Overlay de pausa** — forzarlo con "Descansar Ya" o el menu de bandeja. Debe
  cubrir **todos** los monitores, con la cuenta atras en el primario.
- **Multi-monitor** — comprobar con 2 pantallas: el primario no es siempre
  `Screen.AllScreens[0]`.
- **Bandeja** — menu, tooltip con los minutos restantes, y que cerrar la ventana
  la oculte sin matar la app.
- **Temporizador** — si la pausa no aparece al llegar a cero, revisar si Smart
  Pause la suprimio (`ProgramadorDescansos.UltimaSupresion` y la tabla `breaks`).

Para un cambio visual, capturar la ventana y **mirarla**; no basta con que arranque.

## Nivel 4 — Persistencia

- Base de datos: `%LOCALAPPDATA%\EyeYul\eyeyul.db`
- Ajustes: `%LOCALAPPDATA%\EyeYul\settings.json`

Consultar `breaks` para ver que paso de verdad con las pausas: la columna `Outcome`
es `0=Pending 1=Completed 2=Snoozed 3=Skipped 4=Suppressed`. Un `Suppressed`
explica una pausa que "no aparecio".

Probar los cambios de esquema **contra una base ya existente**, no solo borrandola:
`CREATE TABLE IF NOT EXISTS` no agrega columnas a una tabla que ya existe.

## Antes de commitear

- [ ] `dotnet build` sin errores ni warnings nuevos
- [ ] `dotnet test` en verde, con el conteo copiado
- [ ] App ejecutada si se toco UI/arranque, y vista en **ambos temas**
- [ ] Sin ficheros de diagnostico ni codigo temporal en el commit
- [ ] `git status` revisado: que no quede fuente sin trackear
      (asi se perdio el codigo una vez)

## Output Contract

Reportar el resultado **real** de cada nivel ejecutado, y decir explicitamente
cual no se ejecuto y por que. Si algo quedo sin verificar, decirlo — no
presentarlo como terminado.
