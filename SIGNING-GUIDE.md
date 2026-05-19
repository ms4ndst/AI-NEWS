# MSIX Package Signing Guide

## Certificate Setup Complete! ✓

Your project is now configured to sign MSIX packages with the certificate:
- **Certificate File**: `AI-NEWS_TemporaryKey.pfx`
- **Public Certificate**: `AI-NEWS_TemporaryKey.cer`
- **Password**: `AI-NEWS2026`
- **Publisher**: `CN=AI-NEWS`

## Building the Signed MSIX Package

Run one of these commands to create a signed MSIX package:

```powershell
# Option 1: Publish with Release configuration
dotnet publish -c Release

# Option 2: Create package using MSBuild
msbuild /t:Publish /p:Configuration=Release /p:Platform=x64
```

The signed MSIX package will be created in:
```
.\AppPackages\AI-NEWS_1.0.0.0_x64_Test\
```

## Installing the MSIX Package

### On Your Development Machine
1. Double-click the `.msix` file in the AppPackages folder
2. Windows will prompt you to install
3. Since the certificate is self-signed, you may need to install the certificate first (see below)

### Installing the Certificate (First Time Only)

Before you can install the MSIX, Windows needs to trust the certificate:

#### Method 1: PowerShell (Recommended)
```powershell
# Run as Administrator
Import-Certificate -FilePath "AI-NEWS_TemporaryKey.cer" -CertStoreLocation "Cert:\LocalMachine\TrustedPeople"
```

#### Method 2: Manual Installation
1. Right-click `AI-NEWS_TemporaryKey.cer`
2. Click "Install Certificate"
3. Choose "Local Machine" (requires admin)
4. Select "Place all certificates in the following store"
5. Click "Browse" → Select "Trusted People"
6. Click "Next" → "Finish"

## Distributing to Other Machines

To install your MSIX package on another computer:

1. **Copy these files**:
   - The `.msix` file from AppPackages folder
   - `AI-NEWS_TemporaryKey.cer` (certificate)

2. **On the target machine** (as Administrator):
   ```powershell
   Import-Certificate -FilePath "AI-NEWS_TemporaryKey.cer" -CertStoreLocation "Cert:\LocalMachine\TrustedPeople"
   ```

3. **Install the MSIX**:
   - Double-click the `.msix` file
   - Click "Install"

## For Production Distribution

⚠️ **IMPORTANT**: The self-signed certificate is only for testing and internal use.

For public distribution, you need a **trusted certificate** from:
- **Microsoft Store**: Submit through Partner Center (recommended)
- **Commercial CA**: Purchase a code signing certificate from DigiCert, Sectigo, etc.
- **Azure Code Signing**: Use Azure's cloud-based signing service

### To Use a Commercial Certificate:

1. Purchase a code signing certificate (.pfx file)
2. Update `AI-NEWS.csproj`:
   ```xml
   <PackageCertificateKeyFile>YourCertificate.pfx</PackageCertificateKeyFile>
   <PackageCertificatePassword>YourPassword</PackageCertificatePassword>
   ```
3. Update `Package.appxmanifest` Publisher to match your certificate subject

## Troubleshooting

### "This app package's publisher certificate could not be verified"
→ Install the `.cer` certificate to Trusted People store

### "The package could not be installed because it was signed with a certificate..."
→ Make sure the Publisher in Package.appxmanifest matches the certificate subject (CN=AI-NEWS)

### Build fails with signing error
→ Check that the PFX password is correct in the project file
→ Verify the PFX file exists in the project directory

## Security Notes

🔒 **Password in Project File**: For better security in production:
1. Remove `PackageCertificatePassword` from the project file
2. Use Visual Studio's certificate manager instead
3. Or use Azure Key Vault for enterprise scenarios

📌 **Certificate Storage**: The certificate is also installed in your Windows certificate store at:
```
Cert:\CurrentUser\My
```

You can view it with: `certmgr.msc` → Personal → Certificates
