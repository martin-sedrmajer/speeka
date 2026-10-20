# Sestaví přenosnou verzi Voxu k vydání:
#  - self-contained .NET 8 (uživatel nemusí nic instalovat, stačí WebView2 Runtime z Windows 10/11),
#  - přibalený hlas Supertonic 3 ve složce voices\supertonic-3,
#  - licence převzatých částí,
#  - ZIP v dist\Vox-<verze>-win-x64.zip.
# Spouští se z kořene repozitáře:  powershell -ExecutionPolicy Bypass -File build-portable.ps1

param(
    [string]$Runtime = "win-x64"
)

$ErrorActionPreference = "Stop"
$ProgressPreference = "SilentlyContinue"   # bez ukazatele průběhu je Invoke-WebRequest výrazně rychlejší

$root = $PSScriptRoot
$project = Join-Path $root "Vox\Vox.csproj"
[xml]$csproj = Get-Content $project -Encoding UTF8
$version = $csproj.Project.PropertyGroup | ForEach-Object { $_.Version } | Where-Object { $_ } | Select-Object -First 1
if (-not $version) { $version = "0.0.0" }

$dist = Join-Path $root "dist"
$out = Join-Path $dist "Vox"
$cache = Join-Path $dist "cache"
$modelName = "sherpa-onnx-supertonic-3-tts-int8-2026-05-11"
$modelUrl = "https://github.com/k2-fsa/sherpa-onnx/releases/download/tts-models/$modelName.tar.bz2"
$modelLicenseUrl = "https://huggingface.co/Supertone/supertonic-3/raw/main/LICENSE"

# Spuštěný Vox z dist\Vox drží soubory a nejdou smazat.
$running = Get-Process Vox -ErrorAction SilentlyContinue |
    Where-Object { $_.Path -and $_.Path.StartsWith($out, [StringComparison]::OrdinalIgnoreCase) }
if ($running) { throw "Vox ze složky $out ještě běží. Zavři ho a spusť skript znovu." }

if (Test-Path $out) { Remove-Item $out -Recurse -Force }
New-Item -ItemType Directory -Force $cache | Out-Null

Write-Host "Sestavuji Vox $version ($Runtime)..."
dotnet publish $project -c Release -r $Runtime --self-contained true -p:DebugType=None -o $out
if ($LASTEXITCODE -ne 0) { throw "dotnet publish selhal." }

# Model: nejdřív vlastní složka hlasů (i pod jiným názvem), pak mezipaměť, jinak stáhnout.
function Test-Model([string]$dir) {
    (Test-Path (Join-Path $dir "tts.json")) -and (Test-Path (Join-Path $dir "unicode_indexer.bin"))
}
function Find-7Zip {
    $cmd = Get-Command 7z -ErrorAction SilentlyContinue
    if ($cmd) { return $cmd.Source }
    foreach ($p in @("$env:ProgramFiles\7-Zip\7z.exe", "${env:ProgramFiles(x86)}\7-Zip\7z.exe")) {
        if ($p -and (Test-Path $p)) { return $p }
    }
    return $null
}

$modelSrc = $null
$userVoices = Join-Path $env:APPDATA "Vox\voices"
if (Test-Path $userVoices) {
    $modelSrc = Get-ChildItem $userVoices -Directory |
        Where-Object { Test-Model $_.FullName } |
        Select-Object -First 1 -ExpandProperty FullName
}
if (-not $modelSrc) {
    $modelSrc = Join-Path $cache $modelName
    if (-not (Test-Model $modelSrc)) {
        $archive = Join-Path $cache "$modelName.tar.bz2"
        if (-not (Test-Path $archive)) {
            Write-Host "Stahuji model Supertonic: $modelUrl"
            Invoke-WebRequest $modelUrl -OutFile $archive
        }
        # tar ve Windows neumí bzip2, proto 7-Zip: .tar.bz2 -> .tar, pak tar.
        $sevenZip = Find-7Zip
        if (-not $sevenZip) {
            throw "Na rozbalení modelu je potřeba 7-Zip (https://www.7-zip.org). Případně rozbal $archive ručně do $cache a spusť skript znovu."
        }
        Write-Host "Rozbaluji model..."
        & $sevenZip x $archive "-o$cache" -y | Out-Null
        if ($LASTEXITCODE -ne 0) { throw "7-Zip nerozbalil $archive." }
        $tarFile = Join-Path $cache "$modelName.tar"
        tar -xf $tarFile -C $cache
        if ($LASTEXITCODE -ne 0) { throw "Rozbalení $tarFile selhalo." }
        Remove-Item $tarFile -Force
    }
}
if (-not (Test-Model $modelSrc)) { throw "Model Supertonic se nepodařilo připravit ($modelSrc)." }
Write-Host "Model: $modelSrc"

$voiceDir = Join-Path $out "voices\supertonic-3"
New-Item -ItemType Directory -Force $voiceDir | Out-Null
Copy-Item (Join-Path $modelSrc "*") $voiceDir -Recurse -Force

# Licence: model OpenRAIL-M (z Hugging Face), převzaté části, licence Voxu.
try {
    Invoke-WebRequest $modelLicenseUrl -OutFile (Join-Path $voiceDir "LICENSE-model-OpenRAIL-M.txt")
}
catch {
    Write-Warning "Licenci modelu se nepodařilo stáhnout ($modelLicenseUrl). Před vydáním ji přidej ručně do voices\supertonic-3."
}
Copy-Item (Join-Path $root "THIRD-PARTY-NOTICES.txt") $out
$license = Join-Path $root "LICENSE"
if (Test-Path $license) {
    Copy-Item $license (Join-Path $out "LICENSE.txt")
    Copy-Item (Join-Path $root "LICENSE-EXCEPTION.txt") $out
}
else { Write-Warning "V kořeni repozitáře chybí LICENSE (licence Voxu)." }

$zip = Join-Path $dist "Vox-$version-$Runtime.zip"
if (Test-Path $zip) { Remove-Item $zip -Force }
Write-Host "Balím $zip..."
# Compress-Archive je u velkých souborů (model) velmi pomalý; ZipFile z .NET je řádově rychlejší.
Add-Type -AssemblyName System.IO.Compression.FileSystem
[System.IO.Compression.ZipFile]::CreateFromDirectory($out, $zip, [System.IO.Compression.CompressionLevel]::Optimal, $true)

$sizeMb = [math]::Round((Get-Item $zip).Length / 1MB)
Write-Host "Hotovo: $zip ($sizeMb MB)"
