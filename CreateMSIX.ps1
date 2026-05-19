# Manual MSIX Package Creator for WPF Apps
# This script packages the published app into an MSIX and signs it

Write-Host "`n=== AI-NEWS Manual MSIX Packager ===" -ForegroundColor Cyan
Write-Host "Creating MSIX package from published output`n" -ForegroundColor White

# Step 1: Build the app
Write-Host "[1/4] Building application..." -ForegroundColor Yellow
dotnet publish -c Release -r win-x64 --self-contained true

if ($LASTEXITCODE -ne 0) {
    Write-Host "✗ Build failed!" -ForegroundColor Red
    exit 1
}
Write-Host "✓ Build complete`n" -ForegroundColor Green

# Step 2: Prepare packaging folder
Write-Host "[2/4] Preparing package files..." -ForegroundColor Yellow

$publishDir = "bin\Release\net8.0-windows10.0.19041.0\win-x64\publish"
$packageDir = "PackageFiles"

# Clean and create directories
Remove-Item $packageDir -Recurse -Force -ErrorAction SilentlyContinue
New-Item -ItemType Directory -Path $packageDir -Force | Out-Null

# Copy all published files to package root
Copy-Item -Path "$publishDir\*" -Destination $packageDir -Recurse -Force

# Parse identity from the source manifest so output paths track Version/Arch automatically.
[xml]$manifestXml = Get-Content "Package.appxmanifest" -Raw
$pkgName    = $manifestXml.Package.Identity.Name
$pkgVersion = $manifestXml.Package.Identity.Version
$pkgArch    = $manifestXml.Package.Identity.ProcessorArchitecture
if (-not $pkgArch) { $pkgArch = 'neutral' }

# Copy manifest to package root and replace MSBuild tokens.
# NOTE: do NOT substitute $targetentrypoint$ here. For sideloaded full-trust WPF apps
# the EntryPoint must be the literal "Windows.FullTrustApplication" (set directly in
# Package.appxmanifest). Substituting the WPF App class name (e.g. "AI_NEWS.App")
# causes the MSIX activator to treat the app as a UWP component, which results in a
# permanent splash-screen hang.
$manifestContent = Get-Content "Package.appxmanifest" -Raw
$manifestContent = $manifestContent -replace '\$targetnametoken\$', $pkgName
$manifestContent | Set-Content "$packageDir\AppxManifest.xml" -Force

# Copy logo assets to package root
Copy-Item "*.png" -Destination $packageDir -Force -ErrorAction SilentlyContinue

Write-Host "✓ Files prepared`n" -ForegroundColor Green

# Step 3: Find Windows SDK tools
Write-Host "[3/4] Creating MSIX package..." -ForegroundColor Yellow

$sdkPath = Get-ChildItem "C:\Program Files (x86)\Windows Kits\10\bin" -Directory | 
    Where-Object { $_.Name -match '^\d+\.\d+\.\d+\.\d+$' } | 
    Sort-Object Name -Descending | 
    Select-Object -First 1

if (-not $sdkPath) {
    Write-Host "✗ Windows SDK not found!" -ForegroundColor Red
    Write-Host "Please install Windows SDK from: https://developer.microsoft.com/windows/downloads/windows-sdk/" -ForegroundColor Yellow
    Write-Host "`nAlternative: Open the project in Visual Studio and use:" -ForegroundColor Yellow
    Write-Host "  Project → Publish → Create App Packages`n" -ForegroundColor White
    exit 1
}

$makeappx = Join-Path $sdkPath.FullName "x64\makeappx.exe"
$signtool = Join-Path $sdkPath.FullName "x64\signtool.exe"

# Create output directory — name tracks Version/Arch from Package.appxmanifest
$pkgStem   = "${pkgName}_${pkgVersion}_${pkgArch}"
$outputDir = "AppPackages\$pkgStem"
New-Item -ItemType Directory -Path $outputDir -Force | Out-Null

$msixPath = "$outputDir\$pkgStem.msix"

# Create MSIX package
& $makeappx pack /d $packageDir /p $msixPath /o

if ($LASTEXITCODE -ne 0) {
    Write-Host "✗ Package creation failed!" -ForegroundColor Red
    exit 1
}
Write-Host "✓ MSIX package created`n" -ForegroundColor Green

# Step 4: Sign the package
Write-Host "[4/4] Signing package..." -ForegroundColor Yellow

& $signtool sign /fd SHA256 /a /f "AI-NEWS_TemporaryKey.pfx" /p "AI-NEWS2026" $msixPath

if ($LASTEXITCODE -eq 0) {
    Write-Host "✓ Package signed successfully!`n" -ForegroundColor Green
    
    $msixFile = Get-Item $msixPath
    Write-Host "========================================" -ForegroundColor Cyan
    Write-Host "✓ MSIX Package Ready!" -ForegroundColor Green
    Write-Host "========================================" -ForegroundColor Cyan
    Write-Host "Location: $($msixFile.FullName)" -ForegroundColor White
    Write-Host "Size: $([math]::Round($msixFile.Length/1MB,2)) MB`n" -ForegroundColor Gray
    
    Write-Host "Next steps:" -ForegroundColor Yellow
    Write-Host "1. Install certificate (run as Admin, first time):" -ForegroundColor White
    Write-Host "   .\InstallCertificate.ps1" -ForegroundColor Cyan
    Write-Host "2. Install the app:" -ForegroundColor White
    Write-Host "   Double-click: $msixPath`n" -ForegroundColor Cyan
    
    # Open the folder
    explorer $outputDir
} else {
    Write-Host "✗ Signing failed!" -ForegroundColor Red
    exit 1
}
