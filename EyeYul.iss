; Genera el instalador de EyeYul con Inno Setup (https://jrsoftware.org/isinfo.php).
;
; Pasos:
;   1) .\publicar.ps1 -Autocontenido    (deja EyeYul.exe listo en .\publicacion)
;   2) Compilar este script: ISCC EyeYul.iss   (o "Compile" desde el IDE de Inno Setup)
;
; El resultado es un unico EyeYulSetup.exe en .\instalador. Instala por-usuario (sin
; pedir admin) en %LOCALAPPDATA%\Programs\EyeYul y ahi si crea los accesos directos
; de escritorio y menu inicio. Mientras la app corre desde el repo (dotnet run, F5,
; bin\Debug o bin\Release) el auto-inicio sigue sin activarse: ver
; GestorArranqueRegistro.EsBuildDeDesarrollo en EyeYul.Infraestructura.

#define MyAppName "EyeYul"
#define MyAppVersion "1.0"
#define MyAppPublisher "EyeYul"
#define MyAppExeName "EyeYul.exe"

[Setup]
AppId={{4583FA6D-03F3-4672-940D-12D48FA2E6BE}}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppPublisher={#MyAppPublisher}
DefaultDirName={localappdata}\Programs\{#MyAppName}
DefaultGroupName={#MyAppName}
PrivilegesRequired=lowest
DisableProgramGroupPage=yes
OutputDir=instalador
OutputBaseFilename=EyeYulSetup
SetupIconFile=fuente\EyeYul.Presentacion\app.ico
UninstallDisplayIcon={app}\{#MyAppExeName}
Compression=lzma2/max
SolidCompression=yes
WizardStyle=modern

[Languages]
Name: "spanish"; MessagesFile: "compiler:Languages\Spanish.isl"

[Tasks]
Name: "desktopicon"; Description: "Crear un icono en el escritorio"; GroupDescription: "Iconos adicionales:"; Flags: unchecked

[Files]
Source: "publicacion\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{group}\Desinstalar {#MyAppName}"; Filename: "{uninstallexe}"
Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "Abrir EyeYul"; Flags: nowait postinstall skipifsilent
