# AI-NEWS Reader

A modern WPF desktop application for reading AI news with beautiful Catppuccin themes. Built for Windows with .NET 8.0 and packaged as MSIX for easy installation.

## Features

- 📰 **Markdown News Reader** - Read AI news files in a clean, distraction-free interface
- 🎨 **Four Beautiful Themes** - Catppuccin Mocha (dark), Latte (light), Frappé, and Macchiato
- 🎯 **Persistent Settings** - Remembers your preferred news folder across sessions
- 📦 **MSIX Package** - Easy installation and automatic updates
- 🖥️ **Native Windows 11** - Dark title bar and modern UI elements
- 🎯 **Responsive Design** - Clean sidebar with file list and main content area

## Screenshots

The application features a three-column layout:
- **Sidebar**: News folder path, change folder button, and file list
- **Menu Bar**: File operations, theme selection, and help
- **Content Area**: Markdown-rendered news content with syntax highlighting

### Available Themes

1. **Catppuccin Mocha** (Default) - Dark theme with warm purple accents
2. **Catppuccin Latte** - Light theme for daytime reading
3. **Catppuccin Frappé** - Medium-dark with cool tones
4. **Catppuccin Macchiato** - Dark with subtle warmth

All themes feature:
- Smooth transitions
- Custom scrollbars
- Themed menu highlights
- Consistent 8px border radius
- Official Catppuccin color palette

## Technologies

- **Framework**: .NET 8.0 WPF
- **Language**: C# 12
- **Platform**: Windows 10 19041+ (Windows 10 20H1 or later)
- **Packaging**: MSIX with self-signed certificate
- **Design System**: Catppuccin color palette
- **Content Format**: Markdown with FlowDocument rendering

## Installation

### From MSIX Package

1. Navigate to `AppPackages\AI-NEWS_1.0.0.0_x64\`
2. Double-click `AI-NEWS_1.0.0.0_x64.msix`
3. Click "Install" when prompted
4. The app will appear in your Start Menu as "AI-NEWS Reader"

**Note**: You may need to trust the self-signed certificate first:
- Right-click `AI-NEWS_1.0.0.0_x64.msix` → Properties → Digital Signatures
- Select the signature → Details → View Certificate → Install Certificate
- Choose "Local Machine" → Place in "Trusted Root Certification Authorities"

### Building from Source

```powershell
# Clone the repository
git clone <your-repo-url>
cd AI-NEWS

# Build and package
.\CreateMSIX.ps1

# The MSIX package will be in AppPackages\AI-NEWS_1.0.0.0_x64\
```

## Usage

### First Launch

1. On first launch, the app looks for an "AI-NEWS" folder in your user directory
2. If not found, you can select a folder using **File → Open Folder**
3. The selected folder path is saved and will be remembered on next launch

### Reading News

1. Select a markdown file from the sidebar list
2. Content appears in the main reading area
3. Use the scrollbar to navigate longer articles

### Changing Themes

1. Click **View → Theme** in the menu bar
2. Select one of four themes:
   - Mocha (dark, default)
   - Latte (light)
   - Frappé (medium-dark)
   - Macchiato (dark-warm)
3. Theme applies immediately, including to currently loaded content

### Menu Options

**File Menu**:
- **Open Folder** - Change the news folder location
- **Refresh Files** - Reload the file list from current folder
- **Exit** - Close the application

**View Menu**:
- **Theme** - Select between four Catppuccin themes

**Help Menu**:
- **About** - Display application information

## Project Structure

```
AI-NEWS/
├── App.xaml                    # Application resources and startup
├── App.xaml.cs                 # Application code-behind
├── MainWindow.xaml             # Main window UI layout
├── MainWindow.xaml.cs          # Main window logic and event handlers
├── Package.appxmanifest        # MSIX package manifest
├── AI-NEWS.csproj              # Project file with MSIX configuration
├── CreateMSIX.ps1              # PowerShell build script
├── AI-NEWS_TemporaryKey.pfx    # Self-signed certificate (not in git)
├── Themes/                     # Catppuccin theme files
│   ├── Catppuccin-Mocha.xaml
│   ├── Catppuccin-Latte.xaml
│   ├── Catppuccin-Frappe.xaml
│   └── Catppuccin-Macchiato.xaml
└── Assets/                     # Logo images for MSIX
    ├── LockScreenLogo.png
    ├── SplashScreen.png
    ├── Square150x150Logo.png
    ├── Square44x44Logo.png
    └── Wide310x150Logo.png
```

## Development

### Prerequisites

- Visual Studio 2022 (or later) with:
  - .NET 8.0 SDK
  - Windows App SDK
  - WPF workload
- Windows 10 SDK (10.0.19041.0 or later)
- PowerShell 5.1 or later

### Build Configuration

The project uses:
- **TargetFramework**: net8.0-windows10.0.19041.0
- **TargetPlatformMinVersion**: 10.0.17763.0 (Windows 10 1809)
- **UseWPF**: true
- **UseWindowsForms**: true (for folder browser dialog)
- **WindowsPackageType**: MSIX

### Key Features Implementation

**Theme Switching**:
- Uses `DynamicResource` bindings for runtime theme updates
- Custom `ControlTemplate` for MenuItem and ScrollBar
- Tracks current file to reload content with new theme colors

**Persistent Settings**:
- Stored in `%LOCALAPPDATA%\AI-NEWS\settings.txt`
- Contains last selected folder path
- Loaded on startup, saved on folder change

**Dark Title Bar**:
- Uses Windows DWM API (`DwmSetWindowAttribute`)
- Applies dark mode to native title bar and frame
- Fallback for Windows 10 pre-20H1 versions

**Markdown Rendering**:
- Converts markdown to WPF `FlowDocument`
- Basic support for headings, bold, italic, links
- Transparent background to show theme colors

### Building MSIX Package

The `CreateMSIX.ps1` script:
1. Cleans previous builds
2. Runs `dotnet publish` with release configuration
3. Copies theme files to publish directory
4. Prepares package file structure
5. Creates `AppxManifest.xml` from template
6. Packs using `makeappx.exe`
7. Signs with certificate using `signtool.exe`

Certificate password: `AI-NEWS2026`

### Adding New Themes

1. Create new XAML file in `Themes/` folder
2. Define all 26 Catppuccin colors (Rosewater through Crust)
3. Create semantic color mappings (Background, Primary, Text, etc.)
4. Define brush resources (CtpSurface0Brush, CtpMantleBrush, etc.)
5. Copy all control styles from existing theme
6. Add to project file as `Content` with `CopyToOutputDirectory`
7. Add menu item in `MainWindow.xaml`
8. Add handler in `MainWindow.xaml.cs`

## Customization

### Changing Colors

Edit the theme files in `Themes/` to modify colors. Each theme defines:
- **26 base colors**: Official Catppuccin palette
- **Semantic mappings**: Background, Surface, Primary, Text, etc.
- **Brush resources**: Used in control templates

### Adding Menu Items

1. Edit `MainWindow.xaml` to add new `<MenuItem>`
2. Create Click handler in `MainWindow.xaml.cs`
3. Use fully qualified types for namespace conflicts (e.g., `WpfMessageBox.Show()`)

### Modifying Layout

The main layout is a 2-row Grid:
- Row 0: Menu bar
- Row 1: 3-column grid (sidebar, separator, content)

Edit `MainWindow.xaml` to adjust column widths, spacing, or add new sections.

## Known Issues

- Windows Forms integration causes namespace conflicts (Color, FontFamily, MessageBox)
  - Workaround: Fully qualified type names throughout codebase
- ScrollBar theming requires full ControlTemplate (simple Style.Triggers don't work)
- MenuItem highlight requires custom ControlTemplate to override system colors

## Credits

- **Design System**: [Catppuccin](https://github.com/catppuccin/catppuccin) color palette
- **Framework**: .NET WPF Team
- **Packaging**: Windows App SDK

## License

This project uses the Catppuccin color palette which is licensed under the MIT License.

## Version History

### 1.0.0 (May 19, 2026)
- Initial release
- Four Catppuccin themes (Mocha, Latte, Frappé, Macchiato)
- Markdown news reader
- Persistent folder settings
- MSIX packaging with self-signed certificate
- Dark title bar support
- Custom themed scrollbars and menus
