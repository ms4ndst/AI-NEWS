# Create a self-signed certificate for MSIX package signing
# This is for testing/development. For production, use a trusted certificate from a CA.

$certName = "CN=AI-NEWS Development Certificate"
$certStorePath = "Cert:\CurrentUser\My"
$pfxPassword = ConvertTo-SecureString -String "AI-NEWS2026" -Force -AsPlainText
$outputPath = "$PSScriptRoot\AI-NEWS_TemporaryKey.pfx"

Write-Host "Creating self-signed certificate for MSIX signing..." -ForegroundColor Cyan

# Create the certificate
$cert = New-SelfSignedCertificate `
    -Type CodeSigningCert `
    -Subject $certName `
    -KeyUsage DigitalSignature `
    -FriendlyName "AI-NEWS Development Certificate" `
    -CertStoreLocation $certStorePath `
    -TextExtension @("2.5.29.37={text}1.3.6.1.5.5.7.3.3", "2.5.29.19={text}") `
    -NotAfter (Get-Date).AddYears(2)

Write-Host "✓ Certificate created" -ForegroundColor Green
Write-Host "  Thumbprint: $($cert.Thumbprint)" -ForegroundColor Yellow

# Export to PFX file
Export-PfxCertificate `
    -Cert $cert `
    -FilePath $outputPath `
    -Password $pfxPassword | Out-Null

Write-Host "✓ Certificate exported to: AI-NEWS_TemporaryKey.pfx" -ForegroundColor Green
Write-Host "  Password: AI-NEWS2026" -ForegroundColor Yellow

# Also export as CER for installation
$cerPath = "$PSScriptRoot\AI-NEWS_TemporaryKey.cer"
Export-Certificate -Cert $cert -FilePath $cerPath | Out-Null
Write-Host "✓ Public certificate exported to: AI-NEWS_TemporaryKey.cer" -ForegroundColor Green

Write-Host "`nIMPORTANT NOTES:" -ForegroundColor Magenta
Write-Host "1. The certificate has been installed in your user certificate store" -ForegroundColor White
Write-Host "2. To install the MSIX on other machines, they need to trust this certificate" -ForegroundColor White
Write-Host "3. Install AI-NEWS_TemporaryKey.cer to 'Trusted Root Certification Authorities'" -ForegroundColor White
Write-Host "4. For production, replace with a certificate from a trusted CA" -ForegroundColor White
Write-Host "`nCertificate Thumbprint (copy this): $($cert.Thumbprint)" -ForegroundColor Cyan
