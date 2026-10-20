# Sestaví Vox pro Android (.NET MAUI).
#  - Připraví přibalený hlas Supertonic do Vox.Mobile\Resources\Raw\voices\supertonic-3
#    (model se bere z dist\Vox po build-portable.ps1, z %APPDATA%\Vox\voices nebo z dist\cache).
#  - Bez přepínače: sestaví ladicí verzi a nainstaluje ji do běžícího emulátoru nebo připojeného telefonu.
#  - S -Release: vytvoří balíček .aab pro Google Play (dist\android).
# Spouští se z kořene repozitáře:  powershell -ExecutionPolicy Bypass -File build-android.ps1 [-Release]

param(
    [switch]$Release
)

$ErrorActionPreference = "Stop"

$root = $PSScriptRoot
$project = Join-Path $root "Vox.Mobile\Vox.Mobile.csproj"
$target = Join-Path $root "Vox.Mobile\Resources\Raw\voices\supertonic-3"
$modelName = "sherpa-onnx-supertonic-3-tts-int8-2026-05-11"

function Test-Model([string]$dir) {
    $dir -and (Test-Path (Join-Path $dir "tts.json")) -and (Test-Path (Join-Path $dir "unicode_indexer.bin"))
}

# ---------- model ----------
$candidates = @(
    (Join-Path $root "dist\Vox\voices\supertonic-3"),
    (Join-Path $root "dist\cache\$modelName")
)
$userVoices = Join-Path $env:APPDATA "Vox\voices"
if (Test-Path $userVoices) {
    $candidates += @(Get-ChildItem $userVoices -Directory | ForEach-Object { $_.FullName })
}
$source = $candidates | Where-Object { Test-Model $_ } | Select-Object -First 1
if (-not $source) {
    throw "Nenašel jsem model Supertonic. Nejdřív spusť build-portable.ps1, ten ho stáhne a připraví."
}
Write-Host "Model: $source"

if (Test-Path $target) { Remove-Item $target -Recurse -Force }
New-Item -ItemType Directory -Force $target | Out-Null
$files = Get-ChildItem $source -File | Where-Object { $_.Name -ne "files.txt" }
foreach ($f in $files) { Copy-Item $f.FullName (Join-Path $target $f.Name) }
# seznam souborů: aplikace podle něj model při prvním spuštění rozbalí (a pozná změnu)
$list = ($files | ForEach-Object { "$($_.Name)|$($_.Length)" }) -join "`n"
[IO.File]::WriteAllText((Join-Path $target "files.txt"), $list, (New-Object Text.UTF8Encoding $false))

# ---------- sestavení ----------
if ($Release) {
    $out = Join-Path $root "dist\android"
    Write-Host "Sestavuji balíček pro Google Play..."
    dotnet publish $project -f net10.0-android -c Release -o $out
    if ($LASTEXITCODE -ne 0) { throw "Sestavení selhalo." }
    Write-Host "Hotovo: $out (soubor .aab nahraj do Google Play Console)"
}
else {
    Write-Host "Sestavuji a instaluji do emulátoru nebo telefonu (musí běžet / být připojený)..."
    dotnet build $project -f net10.0-android -c Debug -t:Run
    if ($LASTEXITCODE -ne 0) { throw "Sestavení nebo instalace selhaly." }
}
