Add-Type -AssemblyName System.Drawing

$imagesPath = "c:\Users\magnus.sandstrom\Code\Repo\AI_news\AI-NEWS\Images"

function Create-PlaceholderImage {
    param([string]$Path, [int]$Width, [int]$Height)
    $bmp = New-Object System.Drawing.Bitmap($Width, $Height)
    $graphics = [System.Drawing.Graphics]::FromImage($bmp)
    $graphics.Clear([System.Drawing.Color]::FromArgb(0, 120, 215))
    $graphics.Dispose()
    $bmp.Save($Path, [System.Drawing.Imaging.ImageFormat]::Png)
    $bmp.Dispose()
}

Create-PlaceholderImage -Path "$imagesPath\Square44x44Logo.png" -Width 44 -Height 44
Create-PlaceholderImage -Path "$imagesPath\Square150x150Logo.png" -Width 150 -Height 150
Create-PlaceholderImage -Path "$imagesPath\Wide310x150Logo.png" -Width 310 -Height 150
Create-PlaceholderImage -Path "$imagesPath\StoreLogo.png" -Width 50 -Height 50
Create-PlaceholderImage -Path "$imagesPath\SplashScreen.png" -Width 620 -Height 300

Write-Host "Package asset images created successfully" -ForegroundColor Green
