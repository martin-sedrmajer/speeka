# Sestaví přenosnou verzi Voxu k vydání:
#  - self-contained .NET 8 (uživatel nemusí nic instalovat, stačí WebView2 Runtime z Windows 10/11),
#  - přibalený hlas Supertonic 3 ve složce voices\supertonic-3,
#  - licence převzatých částí,
#  - ZIP v dist\Speeka-<verze>-win-x64.zip,
#  - s přepínačem -Msix navíc balíček pro Microsoft Store: dist\Speeka-<verze>-x64.msix
#    (potřebuje Package\StoreIdentity.json s údaji z Partner Center a makeappx.exe z Windows SDK).
# Spouští se z kořene repozitáře:  powershell -ExecutionPolicy Bypass -File build-portable.ps1 [-Msix]

param(
    [string]$Runtime = "win-x64",
    [switch]$Msix
)

$ErrorActionPreference = "Stop"
$ProgressPreference = "SilentlyContinue"   # bez ukazatele průběhu je Invoke-WebRequest výrazně rychlejší

$root = $PSScriptRoot
$project = Join-Path $root "Vox\Vox.csproj"
[xml]$csproj = Get-Content $project -Encoding UTF8
$version = $csproj.Project.PropertyGroup | ForEach-Object { $_.Version } | Where-Object { $_ } | Select-Object -First 1
if (-not $version) { $version = "0.0.0" }

$dist = Join-Path $root "dist"
$out = Join-Path $dist "Speeka"
$cache = Join-Path $dist "cache"
$modelName = "sherpa-onnx-supertonic-3-tts-int8-2026-05-11"
$modelUrl = "https://github.com/k2-fsa/sherpa-onnx/releases/download/tts-models/$modelName.tar.bz2"
$modelLicenseUrl = "https://huggingface.co/Supertone/supertonic-3/raw/main/LICENSE"

# Spuštěná Speeka z dist\Speeka drží soubory a nejdou smazat.
$running = Get-Process Speeka -ErrorAction SilentlyContinue |
    Where-Object { $_.Path -and $_.Path.StartsWith($out, [StringComparison]::OrdinalIgnoreCase) }
if ($running) { throw "Aplikace (Speeka.exe) ze složky $out ještě běží. Zavři ji a spusť skript znovu." }

if (Test-Path $out) { Remove-Item $out -Recurse -Force }
New-Item -ItemType Directory -Force $cache | Out-Null

Write-Host "Sestavuji Speeku $version ($Runtime)..."
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

$zip = Join-Path $dist "Speeka-$version-$Runtime.zip"
if (Test-Path $zip) { Remove-Item $zip -Force }
Write-Host "Balím $zip..."
# Compress-Archive je u velkých souborů (model) velmi pomalý; ZipFile z .NET je řádově rychlejší.
Add-Type -AssemblyName System.IO.Compression.FileSystem
[System.IO.Compression.ZipFile]::CreateFromDirectory($out, $zip, [System.IO.Compression.CompressionLevel]::Optimal, $true)

$sizeMb = [math]::Round((Get-Item $zip).Length / 1MB)
Write-Host "Hotovo: $zip ($sizeMb MB)"

if (-not $Msix) { return }

# ---------- balíček MSIX pro Microsoft Store (podepíše ho Store) ----------

function Find-MakeAppx {
    $cmd = Get-Command makeappx.exe -ErrorAction SilentlyContinue
    if ($cmd) { return $cmd.Source }
    $kits = Join-Path ${env:ProgramFiles(x86)} "Windows Kits\10\bin"
    if (-not (Test-Path $kits)) { return $null }
    $candidates = @(Get-ChildItem $kits -Directory | Sort-Object Name -Descending |
        ForEach-Object { Join-Path $_.FullName "x64\makeappx.exe" }) + @(Join-Path $kits "x64\makeappx.exe")
    return $candidates | Where-Object { Test-Path $_ } | Select-Object -First 1
}

$identityFile = Join-Path $root "Package\StoreIdentity.json"
$id = Get-Content $identityFile -Raw -Encoding UTF8 | ConvertFrom-Json
foreach ($key in "Name", "Publisher", "PublisherDisplayName", "DisplayName") {
    if (-not $id.$key -or $id.$key -like "DOPLNIT*") {
        throw "V Package\StoreIdentity.json chybí $key. Údaje najdeš v Partner Center: aplikace -> Product management -> Product identity."
    }
}

$makeAppx = Find-MakeAppx
if (-not $makeAppx) {
    throw "Chybí makeappx.exe z Windows SDK. Nainstaluj ho: winget install --id Microsoft.WindowsSDK.10.0.26100 --exact"
}

# Store chce čtyřmístnou verzi s nulou na konci (0.2.0 -> 0.2.0.0).
$parts = @($version.Split(".") | Select-Object -First 3)
while ($parts.Count -lt 3) { $parts += "0" }
$msixVersion = ($parts -join ".") + ".0"

$layout = Join-Path $dist "msix"
if (Test-Path $layout) { Remove-Item $layout -Recurse -Force }
Copy-Item $out $layout -Recurse
Copy-Item (Join-Path $root "Package\Assets") (Join-Path $layout "Assets") -Recurse
# obrázky jen pro stránku obchodu (nahrávají se v Partner Center), do balíčku nepatří
foreach ($storeOnly in "StoreListing-300.png", "StoreBoxArt-1080.png", "StoreBoxArt-1080-en.png", "StoreLogo-71.png") {
    Remove-Item (Join-Path $layout "Assets\$storeOnly") -ErrorAction SilentlyContinue
}

$esc = { param($v) [Security.SecurityElement]::Escape([string]$v) }
$manifest = Get-Content (Join-Path $root "Package\AppxManifest.template.xml") -Raw -Encoding UTF8
$manifest = $manifest.Replace("{{NAME}}", (& $esc $id.Name))
$manifest = $manifest.Replace("{{PUBLISHER}}", (& $esc $id.Publisher))
$manifest = $manifest.Replace("{{PUBLISHER_DISPLAY_NAME}}", (& $esc $id.PublisherDisplayName))
$manifest = $manifest.Replace("{{DISPLAY_NAME}}", (& $esc $id.DisplayName))
$manifest = $manifest.Replace("{{VERSION}}", $msixVersion)
[IO.File]::WriteAllText((Join-Path $layout "AppxManifest.xml"), $manifest, (New-Object Text.UTF8Encoding $false))

$msixFile = Join-Path $dist "Speeka-$version-x64.msix"
if (Test-Path $msixFile) { Remove-Item $msixFile -Force }
Write-Host "Balím MSIX $msixVersion..."
& $makeAppx pack /d $layout /p $msixFile /o | Out-Null
if ($LASTEXITCODE -ne 0) { throw "makeappx nevytvořil balíček (spusť ho ručně pro podrobnosti: `"$makeAppx`" pack /d `"$layout`" /p `"$msixFile`")." }

$msixMb = [math]::Round((Get-Item $msixFile).Length / 1MB)
Write-Host "Hotovo: $msixFile ($msixMb MB). Nahraj ho v Partner Center; podepíše ho Store."
Write-Host "Vyzkoušet bez podpisu (zapnutý režim pro vývojáře): Add-AppxPackage -Register `"$layout\AppxManifest.xml`""
