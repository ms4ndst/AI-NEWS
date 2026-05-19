# Install the AI-NEWS certificate to trust the MSIX package
# Run this script as Administrator

$certFile = "$PSScriptRoot\AI-NEWS_TemporaryKey.cer"

if (-not (Test-Path $certFile)) {
    Write-Host "ERROR: Certificate file not found: $certFile" -ForegroundColor Red
    exit 1
}

# Check if running as administrator
$isAdmin = ([Security.Principal.WindowsPrincipal] [Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)

if (-not $isAdmin) {
    Write-Host "ERROR: This script must be run as Administrator" -ForegroundColor Red
    Write-Host "`nRight-click PowerShell and select 'Run as Administrator', then run this script again" -ForegroundColor Yellow
    exit 1
}

Write-Host "Installing AI-NEWS certificate..." -ForegroundColor Cyan

try {
    # Import to Trusted People store
    Import-Certificate -FilePath $certFile -CertStoreLocation "Cert:\LocalMachine\TrustedPeople" -ErrorAction Stop | Out-Null
    
    Write-Host "✓ Certificate installed successfully!" -ForegroundColor Green
    Write-Host "`nYou can now install the AI-NEWS MSIX package" -ForegroundColor White
    Write-Host "The certificate has been added to:" -ForegroundColor Gray
    Write-Host "  Cert:\LocalMachine\TrustedPeople" -ForegroundColor Gray
    
} catch {
    Write-Host "✗ Failed to install certificate" -ForegroundColor Red
    Write-Host "Error: $($_.Exception.Message)" -ForegroundColor Yellow
    exit 1
}
