# build-release.ps1
$project = "LibraFlow"
$outputDir = "publish"
$runtime = "win-x64"
$innoScript = "installer.iss"

Write-Host "🔧 Publishing $project as self-contained..."
dotnet publish $project -c Release -r $runtime --self-contained true `
  /p:PublishSingleFile=true `
  /p:IncludeNativeLibrariesForSelfExtract=true `
  /p:PublishTrimmed=false `
  -o $outputDir

if (!(Test-Path $outputDir)) {
    Write-Error "❌ Publish failed. Output directory not found."
    exit 1
}

Write-Host "📦 Running Inno Setup Compiler..."
$innoPath = "C:\Program Files (x86)\Inno Setup 6\ISCC.exe"
& "$innoPath" $innoScript

Write-Host "✅ Done! Installer is ready in the Output folder."
