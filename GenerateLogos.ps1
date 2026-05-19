Add-Type -AssemblyName System.Drawing

$baseDir = "c:\Users\magnus.sandstrom\Code\Repo\AI_news\AI-NEWS"

function New-Logo {
    param(
        [string]$Path,
        [int]$Width,
        [int]$Height,
        [bool]$ShowText = $true,
        [string]$Text = "AI-NEWS"
    )
    
    # Create bitmap and graphics
    $bmp = New-Object System.Drawing.Bitmap($Width, $Height)
    $graphics = [System.Drawing.Graphics]::FromImage($bmp)
    $graphics.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
    $graphics.TextRenderingHint = [System.Drawing.Text.TextRenderingHint]::AntiAlias
    
    # Create Catppuccin Mocha gradient background (Mauve to Blue)
    $rect = New-Object System.Drawing.Rectangle(0, 0, $Width, $Height)
    $startColor = [System.Drawing.Color]::FromArgb(203, 166, 247)  # Catppuccin Mocha Mauve
    $endColor = [System.Drawing.Color]::FromArgb(137, 180, 250)    # Catppuccin Mocha Blue
    $brush = New-Object System.Drawing.Drawing2D.LinearGradientBrush(
        $rect,
        $startColor,
        $endColor,
        [System.Drawing.Drawing2D.LinearGradientMode]::Vertical
    )
    $graphics.FillRectangle($brush, $rect)
    
    # Add some geometric shapes for AI theme (neural network nodes)
    $whitePen = New-Object System.Drawing.Pen([System.Drawing.Color]::FromArgb(100, 255, 255, 255), 2)
    $nodeSize = [Math]::Max(3, $Width / 30)
    
    # Draw some connection lines and nodes
    if ($Width -ge 100) {
        $random = New-Object System.Random(42)
        for ($i = 0; $i -lt 5; $i++) {
            $x1 = $random.Next(0, $Width)
            $y1 = $random.Next(0, $Height)
            $x2 = $random.Next(0, $Width)
            $y2 = $random.Next(0, $Height)
            $graphics.DrawLine($whitePen, $x1, $y1, $x2, $y2)
            
            $nodeBrush = New-Object System.Drawing.SolidBrush([System.Drawing.Color]::FromArgb(150, 255, 255, 255))
            $graphics.FillEllipse($nodeBrush, $x1 - $nodeSize/2, $y1 - $nodeSize/2, $nodeSize, $nodeSize)
            $graphics.FillEllipse($nodeBrush, $x2 - $nodeSize/2, $y2 - $nodeSize/2, $nodeSize, $nodeSize)
            $nodeBrush.Dispose()
        }
    }
    
    # Add text if requested
    if ($ShowText) {
        # Determine font size based on image dimensions
        $fontSize = [Math]::Max(8, [Math]::Min($Width, $Height) / 8)
        
        # For small logos, just show "AI"
        if ($Width -lt 100) {
            $Text = "AI"
            $fontSize = $Width / 2.5
        }
        
        $font = New-Object System.Drawing.Font("Segoe UI", $fontSize, [System.Drawing.FontStyle]::Bold)
        $textBrush = New-Object System.Drawing.SolidBrush([System.Drawing.Color]::White)
        
        # Measure text for centering
        $textSize = $graphics.MeasureString($Text, $font)
        $x = ($Width - $textSize.Width) / 2
        $y = ($Height - $textSize.Height) / 2
        
        # Draw shadow for depth
        $shadowBrush = New-Object System.Drawing.SolidBrush([System.Drawing.Color]::FromArgb(100, 0, 0, 0))
        $graphics.DrawString($Text, $font, $shadowBrush, $x + 2, $y + 2)
        $shadowBrush.Dispose()
        
        # Draw main text
        $graphics.DrawString($Text, $font, $textBrush, $x, $y)
        
        $font.Dispose()
        $textBrush.Dispose()
    }
    
    # Clean up
    $whitePen.Dispose()
    $brush.Dispose()
    $graphics.Dispose()
    
    # Save image
    $bmp.Save($Path, [System.Drawing.Imaging.ImageFormat]::Png)
    $bmp.Dispose()
    
    Write-Host "Created: $([System.IO.Path]::GetFileName($Path))" -ForegroundColor Green
}

# Generate all required logos
Write-Host "`nGenerating AI-NEWS logos..." -ForegroundColor Cyan

New-Logo -Path "$baseDir\Square44x44Logo.png" -Width 44 -Height 44 -ShowText $true
New-Logo -Path "$baseDir\Square150x150Logo.png" -Width 150 -Height 150 -ShowText $true
New-Logo -Path "$baseDir\Wide310x150Logo.png" -Width 310 -Height 150 -ShowText $true
New-Logo -Path "$baseDir\StoreLogo.png" -Width 50 -Height 50 -ShowText $true
New-Logo -Path "$baseDir\SplashScreen.png" -Width 620 -Height 300 -ShowText $true

Write-Host "`n✓ All logos created successfully!" -ForegroundColor Green
Write-Host "Files created in: $baseDir" -ForegroundColor Yellow
