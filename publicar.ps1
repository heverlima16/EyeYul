<#
.SYNOPSIS
    Publica EyeYul para distribuir.

.DESCRIPTION
    Genera un unico EyeYul.exe listo para copiar a otra maquina Windows.

    Por defecto usa el modo dependiente del framework (~30 MB), que necesita el
    .NET Desktop Runtime 10 instalado en el equipo destino. Con -Autocontenido
    genera un exe de ~200 MB que no necesita nada instalado.

.PARAMETER Autocontenido
    Incluye el runtime de .NET dentro del ejecutable.

.PARAMETER Destino
    Carpeta de salida. Por defecto: .\publicacion

.EXAMPLE
    .\publicar.ps1
    .\publicar.ps1 -Autocontenido
#>
[CmdletBinding()]
param(
    [switch]$Autocontenido,
    [string]$Destino = "publicacion"
)

$ErrorActionPreference = "Stop"
$proyecto = "fuente/EyeYul.Presentacion"

$modo = if ($Autocontenido) { "autocontenido (no requiere .NET instalado)" }
        else { "dependiente del framework (requiere .NET Desktop Runtime 10)" }

Write-Host "Publicando EyeYul - $modo" -ForegroundColor Cyan

if (Test-Path $Destino) { Remove-Item $Destino -Recurse -Force }

# IncludeNativeLibrariesForSelfExtract embebe e_sqlite3.dll; sin esto queda suelto
# junto al exe y la app no arranca si se copia solo el ejecutable.
#
# DebugType=embedded mete el PDB dentro del propio exe: asi no quedan .pdb sueltos
# y se conserva EmbedAllSources (la red que permitio recuperar el codigo una vez).
# No usar DebugType=none: es incompatible con EmbedAllSources y falla con CS2045.
$selfContained = $Autocontenido.IsPresent.ToString().ToLower()

dotnet publish $proyecto `
    -c Release `
    -r win-x64 `
    --self-contained $selfContained `
    -p:PublishSingleFile=true `
    -p:IncludeNativeLibrariesForSelfExtract=true `
    -p:DebugType=embedded `
    -o $Destino `
    --nologo

if ($LASTEXITCODE -ne 0) { throw "La publicacion fallo." }

$exe = Join-Path $Destino "EyeYul.exe"
if (-not (Test-Path $exe)) { throw "No se genero EyeYul.exe" }

$mb = [math]::Round((Get-Item $exe).Length / 1MB, 1)
$sueltos = (Get-ChildItem $Destino -File | Where-Object { $_.Name -ne "EyeYul.exe" }).Count

Write-Host ""
Write-Host "Listo: $exe  ($mb MB)" -ForegroundColor Green
if ($sueltos -gt 0) {
    Write-Warning "Quedaron $sueltos archivos sueltos: hay que copiar la carpeta entera, no solo el exe."
} else {
    Write-Host "Un solo archivo: basta con copiar ese .exe al equipo destino." -ForegroundColor Green
}
