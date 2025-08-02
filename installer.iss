; installer.iss
[Setup]
AppName=LibraFlow
AppVersion=1.0.0
DefaultDirName={pf}\LibraFlow
DefaultGroupName=LibraFlow
UninstallDisplayIcon={app}\LibraFlow.exe
OutputDir=Output
OutputBaseFilename=LibraFlowSetup
Compression=lzma
SolidCompression=yes

[Files]
Source: "publish\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{group}\LibraFlow"; Filename: "{app}\LibraFlow.exe"
Name: "{group}\Uninstall LibraFlow"; Filename: "{uninstallexe}"

[Run]
Filename: "{app}\LibraFlow.exe"; Description: "Launch LibraFlow"; Flags: nowait postinstall skipifsilent
