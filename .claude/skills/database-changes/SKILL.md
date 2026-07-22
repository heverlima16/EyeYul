---
name: database-changes
description: Trabajar con la base de datos SQLite local de EyeYul — agregar o modificar tablas y repositorios en fuente/EyeYul.Infraestructura/Persistencia. Usar cuando se pida guardar un dato nuevo, cambiar el esquema, o tocar cualquier archivo bajo Persistencia.
license: Apache-2.0
metadata:
  author: "Hever Lima <hever.lima@evolsys.pe>"
  version: "1.0"
user-invocable: true
---

# Base de datos de EyeYul

EyeYul guarda todo **en local, por privacidad**: no hay servidor ni cuenta.

- Base de datos: `%LOCALAPPDATA%\EyeYul\eyeyul.db` (SQLite, modo WAL)
- Ajustes: `%LOCALAPPDATA%\EyeYul\settings.json`

No hay ORM ni migraciones. El esquema completo vive en un unico `CREATE TABLE IF
NOT EXISTS` dentro de `BaseDatosSqlite.Initialize()`, que se ejecuta en cada
arranque. El acceso es SQL crudo con `Microsoft.Data.Sqlite`.

## Agregar o modificar una tabla

1. Editar el bloque SQL de `Persistencia/BaseDatosSqlite.cs` (`Initialize()`),
   dentro del raw string literal (`"""`). Toda sentencia es
   `CREATE TABLE IF NOT EXISTS` / `CREATE INDEX IF NOT EXISTS`.
2. **`Initialize()` corre en cada arranque contra bases ya existentes.** Agregar
   una tabla nueva es seguro; agregar una **columna** a una tabla existente no lo
   es: `CREATE TABLE IF NOT EXISTS` no la anade si la tabla ya existe. Para una
   columna nueva hace falta un `ALTER TABLE ... ADD COLUMN` idempotente
   (comprobando `PRAGMA table_info`) — nunca asumir que basta con editar el
   `CREATE TABLE`, o los usuarios existentes rompen en tiempo de ejecucion.
3. Crear/actualizar el repositorio en `Persistencia/Repositorio<Entidad>.cs`.
4. Registrarlo en `Infraestructura/DependencyInjection.cs`
   (`AddSingleton<I<Entidad>Repository, Repositorio<Entidad>>()`).

## Convenciones de esquema

- Nombres de tabla en **ingles y snake_case plural**: `breaks`, `sessions`,
  `app_usage`, `website_usage`, `screen_score`, `planned_breaks`.
- Columnas en **PascalCase**, igual que la propiedad C# (`ScheduledAt`,
  `PlannedDurationSec`) — el mapeo se hace a mano por indice, asi que el nombre es
  documentacion, no magia.
- `Guid` → `TEXT` (`id.ToString()`).
- `DateTimeOffset` → `TEXT` en formato ISO redondo (`BaseDatosSqlite.Iso`, formato
  `"O"`); se lee con `ParseIso`.
- `TimeSpan` → `REAL` en segundos, con sufijo `Sec` en el nombre
  (`PlannedDurationSec`, `ActiveTimeSec`).
- `DateOnly` → `TEXT` `yyyy-MM-dd` via `BaseDatosSqlite.DateKey`, en una columna
  `DateKey` **indexada**: es la clave de casi toda consulta de estadisticas.
- `bool` → `INTEGER` 0/1.
- Coleccion corta (dias de la semana) → CSV de enteros en `TEXT`. No crear tabla
  hija para algo que siempre se lee entero con su padre.

## Convenciones de repositorio

- SQL siempre en **raw string literal** (`"""`), indentado, legible. Nunca
  concatenar cadenas.
- Parametros siempre con `$nombre` y `AddWithValue`. **Nunca interpolar valores en
  el SQL** — ni siquiera "porque es un entero de mi codigo".
- `NULL` se pasa como `(object?)valor ?? DBNull.Value`.
- Escritura idempotente con `ON CONFLICT(...) DO UPDATE SET`, no `SELECT` previo +
  `INSERT`/`UPDATE`.
- Acumuladores con `ON CONFLICT ... DO UPDATE SET Col = Col + $delta`, no leer,
  sumar en C# y reescribir (se pierden actualizaciones concurrentes).
- Conexion por operacion: `await using SqliteConnection cn = db.Open();`. No
  compartir ni cachear conexiones; SQLite en WAL lo maneja bien.
- Lecturas: `await using SqliteDataReader r = await cmd.ExecuteReaderAsync(ct);`
  y mapeo posicional (`r.GetString(0)`). Si se agrega una columna al `SELECT`,
  **revisar todos los indices** del mapeo.
- Nunca `SELECT *`: listar las columnas explicitamente y en el mismo orden que el
  mapeo posterior.
- Las lecturas devuelven `Task<IReadOnlyList<T>>` (nunca `null`; lista vacia si no
  hay filas).

## Ajustes (`settings.json`)

- Toda propiedad nueva va en `Aplicacion/Configuracion/AjustesEyeYul.cs` con un
  valor por defecto sensato: la app tiene que arrancar sin fichero.
- `AlmacenAjustesJson` escribe de forma **atomica** (temporal + `File.Move`
  con `overwrite: true`). Mantener ese patron.
- Si el JSON esta corrupto, se cae a los valores por defecto y se loguea; nunca se
  propaga la excepcion al arranque.

## Anti-patrones

- Traer todas las filas y filtrar/agregar en C#: agregar en SQL (`SUM`, `GROUP BY`).
- Un `INSERT` por elemento en un bucle: usar una sola sentencia con `ON CONFLICT`.
- Guardar `DateTime` local sin offset: siempre `DateTimeOffset` en ISO.
- Anadir una columna editando solo el `CREATE TABLE` (ver punto 2).

## Verificacion

- `dotnet build EyeYul.slnx`.
- Ejecutar la app y comprobar que la tabla se crea: la base esta en
  `%LOCALAPPDATA%\EyeYul\eyeyul.db`.
- Probar tambien **contra una base ya existente**, no solo borrandola: es donde
  fallan los cambios de esquema.
