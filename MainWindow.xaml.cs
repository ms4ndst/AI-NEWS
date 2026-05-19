using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;
using Shapes = System.Windows.Shapes;
using Forms = System.Windows.Forms;
using WpfMessageBox = System.Windows.MessageBox;

namespace AI_NEWS
{
    public partial class MainWindow : Window
    {
        private string _currentTheme = "Mocha";
        private string _filesDirectory = "";
        private string _currentFilePath = "";
        private bool _suppressSelectionChanged;
        private const string SettingsFileName = "settings.txt";

        public MainWindow()
        {
            InitializeComponent();
            ContentHeader.Text = "Loading...";
            SourceInitialized += (_, _) => EnableDarkTitleBar();
            Loaded += MainWindow_Loaded;
        }

        // DwmSetWindowAttribute(DWMWA_USE_IMMERSIVE_DARK_MODE) — switches the
        // Win32 title bar/frame to the dark scheme on Windows 10 1809+/Windows 11.
        [DllImport("dwmapi.dll")]
        private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);

        private void EnableDarkTitleBar()
        {
            var hwnd = new WindowInteropHelper(this).Handle;
            if (hwnd == IntPtr.Zero) return;
            int useDark = 1;
            // 20 = DWMWA_USE_IMMERSIVE_DARK_MODE (Win10 20H1+/Win11).
            // 19 is the pre-20H1 value; try it as a fallback.
            if (DwmSetWindowAttribute(hwnd, 20, ref useDark, sizeof(int)) != 0)
            {
                DwmSetWindowAttribute(hwnd, 19, ref useDark, sizeof(int));
            }
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Initialize system colors from current theme
            var currentTheme = System.Windows.Application.Current.Resources.MergedDictionaries
                .FirstOrDefault(rd => rd.Source != null && rd.Source.ToString().Contains("Catppuccin"));
            if (currentTheme != null)
            {
                UpdateSystemColors(currentTheme);
            }
            
            // Resolve the news folder off the UI thread so the window paints
            // immediately and the MSIX splash screen dismisses.
            string resolved;
            try
            {
                resolved = await Task.Run(ResolveFilesDirectory);
            }
            catch (Exception ex)
            {
                ContentHeader.Text = "Could not locate AI-NEWS folder.";
                System.Diagnostics.Debug.WriteLine($"ResolveFilesDirectory failed: {ex}");
                resolved = AppDomain.CurrentDomain.BaseDirectory;
            }

            _filesDirectory = resolved;
            UpdateFolderPathDisplay();

            // Yield once so the first frame paints before we start file I/O.
            await Dispatcher.InvokeAsync(() => { }, DispatcherPriority.Background);

            LoadAvailableFiles();
        }

        private string ResolveFilesDirectory()
        {
            // 1) Check saved settings from last session
            try
            {
                var savedPath = LoadSavedFolderPath();
                if (!string.IsNullOrEmpty(savedPath) && Directory.Exists(savedPath))
                    return savedPath;
            }
            catch { /* ignore and continue */ }

            // 2) Prefer user's Documents\AI-NEWS folder if it exists with news files.
            try
            {
                var docs = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                    "AI-NEWS");
                if (Directory.Exists(docs) && Directory.EnumerateFiles(docs, "*AI_NEWS*.md").Any())
                    return docs;
            }
            catch { /* ignore and continue */ }

            // 3) Walk up from the executable looking for *AI_NEWS*.md files.
            try
            {
                var dir = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
                while (dir != null)
                {
                    if (dir.GetFiles("*AI_NEWS*.md").Length > 0)
                        return dir.FullName;
                    dir = dir.Parent;
                }
            }
            catch { /* ignore — MSIX install dir may be restricted */ }

            // 4) Fallback to executable directory.
            return AppDomain.CurrentDomain.BaseDirectory;
        }

        private string GetSettingsFilePath()
        {
            var appDataFolder = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "AI-NEWS");
            
            Directory.CreateDirectory(appDataFolder);
            return Path.Combine(appDataFolder, SettingsFileName);
        }

        private string LoadSavedFolderPath()
        {
            try
            {
                var settingsPath = GetSettingsFilePath();
                if (File.Exists(settingsPath))
                {
                    return File.ReadAllText(settingsPath).Trim();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to load saved folder path: {ex.Message}");
            }
            return string.Empty;
        }

        private void SaveFolderPath(string folderPath)
        {
            try
            {
                var settingsPath = GetSettingsFilePath();
                File.WriteAllText(settingsPath, folderPath);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to save folder path: {ex.Message}");
            }
        }
        
        private void LoadAvailableFiles()
        {
            try
            {
                var files = Directory.Exists(_filesDirectory)
                    ? Directory.GetFiles(_filesDirectory, "*AI_NEWS*.md")
                        .OrderByDescending(f => ExtractDateFromFilename(Path.GetFileName(f)))
                        .ToList()
                    : new List<string>();

                FileListBox.DisplayMemberPath = "DisplayName";

                _suppressSelectionChanged = true;
                try
                {
                    FileListBox.ItemsSource = files.Select(f => new FileItem
                    {
                        FullPath = f,
                        DisplayName = Path.GetFileNameWithoutExtension(f)
                    }).ToList();

                    if (FileListBox.Items.Count > 0)
                    {
                        FileListBox.SelectedIndex = 0;
                    }
                    else
                    {
                        ContentHeader.Text = "No AI-NEWS files found. Use File → Open News Folder…";
                    }
                }
                finally
                {
                    _suppressSelectionChanged = false;
                }

                // Defer initial file render so the window finishes its first paint first.
                if (FileListBox.SelectedItem is FileItem first)
                {
                    Dispatcher.BeginInvoke(new Action(() => LoadFile(first.FullPath)),
                        DispatcherPriority.Background);
                }
            }
            catch (Exception ex)
            {
                ContentHeader.Text = $"Error loading files: {ex.Message}";
            }
        }
        
        private DateTime ExtractDateFromFilename(string filename)
        {
            // Try to parse date from filenames like "19-MAY-2026-AI_NEWS.md"
            var match = Regex.Match(filename, @"(\d{1,2})-(\w{3})-(\d{4})");
            if (match.Success && DateTime.TryParse($"{match.Groups[1].Value} {match.Groups[2].Value} {match.Groups[3].Value}", out var date))
                return date;
            return DateTime.MinValue;
        }
        
        private void FileListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_suppressSelectionChanged) return;
            if (FileListBox.SelectedItem is FileItem selectedFile)
            {
                LoadFile(selectedFile.FullPath);
            }
        }
        
        private void LoadFile(string filePath)
        {
            try
            {
                _currentFilePath = filePath;
                string content = File.ReadAllText(filePath);
                
                // Set header
                ContentHeader.Text = Path.GetFileName(filePath);
                
                // Convert markdown to flow document
                var flowDoc = MarkdownToFlowDocument(content);
                ContentRichTextBox.Document = flowDoc;
                
                // Scroll to top
                ContentRichTextBox.ScrollToHome();
            }
            catch (Exception ex)
            {
                WpfMessageBox.Show($"Error loading file: {ex.Message}");
            }
        }
        
        private FlowDocument MarkdownToFlowDocument(string markdown)
        {
            var doc = new FlowDocument();
            doc.FontFamily = new System.Windows.Media.FontFamily("Segoe UI");
            doc.FontSize = 14;
            doc.PagePadding = new Thickness(10);
            // Force a single-column flow that fills the host width. Without these
            // settings WPF can pick a narrow column width and wrap per character.
            doc.ColumnWidth = double.PositiveInfinity;
            doc.ColumnGap = 0;
            doc.IsColumnWidthFlexible = false;
            doc.TextAlignment = TextAlignment.Left;
            
            // Use transparent background so RichTextBox theme background shows through
            doc.Background = System.Windows.Media.Brushes.Transparent;
            doc.Foreground = TryFindResource("TextBrush") as System.Windows.Media.Brush 
                ?? new SolidColorBrush(System.Windows.Media.Color.FromRgb(205, 214, 244));
            
            var lines = markdown.Split('\n');
            Paragraph? currentParagraph = null;
            bool inCodeBlock = false;
            var codeLines = new List<string>();
            
            foreach (var rawLine in lines)
            {
                var line = rawLine.TrimEnd('\r');
                
                // Handle code block boundaries
                if (line.TrimStart().StartsWith("```"))
                {
                    if (!inCodeBlock)
                    {
                        FlushParagraph(doc, ref currentParagraph);
                        inCodeBlock = true;
                        codeLines.Clear();
                    }
                    else
                    {
                        // End of code block
                        var codePara = new Paragraph();
                        codePara.Background = TryFindResource("CtpSurface0Brush") as System.Windows.Media.Brush
                            ?? new SolidColorBrush(System.Windows.Media.Color.FromRgb(49, 50, 68));
                        codePara.Margin = new Thickness(10, 5, 10, 5);
                        codePara.Padding = new Thickness(10);
                        codePara.FontFamily = new System.Windows.Media.FontFamily("Cascadia Code, Consolas, Courier New");
                        codePara.FontSize = 12;
                        codePara.Inlines.Add(new Run(string.Join("\n", codeLines)));
                        doc.Blocks.Add(codePara);
                        inCodeBlock = false;
                    }
                    continue;
                }
                
                if (inCodeBlock)
                {
                    codeLines.Add(line);
                    continue;
                }
                
                // Empty line
                if (string.IsNullOrWhiteSpace(line))
                {
                    FlushParagraph(doc, ref currentParagraph);
                    continue;
                }
                
                // Headers
                if (line.StartsWith("#"))
                {
                    FlushParagraph(doc, ref currentParagraph);
                    int level = 0;
                    while (level < line.Length && line[level] == '#') level++;
                    string headerText = line.Substring(level).Trim();
                    
                    var block = new Paragraph();
                    block.FontSize = level switch
                    {
                        1 => 26,
                        2 => 22,
                        3 => 18,
                        4 => 16,
                        _ => 15
                    };
                    block.FontWeight = FontWeights.Bold;
                    block.Foreground = FindBrush("PrimaryBrush", System.Windows.Media.Color.FromRgb(203, 166, 247));
                    block.Margin = new Thickness(0, level == 1 ? 5 : 12, 0, 4);
                    AddInlineFormatted(block.Inlines, headerText);
                    doc.Blocks.Add(block);
                    continue;
                }
                
                // Horizontal rule
                if (line.Trim() == "---" || line.Trim() == "***" || line.Trim() == "___")
                {
                    FlushParagraph(doc, ref currentParagraph);
                    var rule = new BlockUIContainer(new Shapes.Rectangle
                    {
                        Height = 1,
                        Fill = FindBrush("BorderBrush", System.Windows.Media.Color.FromRgb(127, 132, 156)),
                        Margin = new Thickness(0, 8, 0, 8)
                    });
                    doc.Blocks.Add(rule);
                    continue;
                }
                
                // List items (- or *)
                if (line.StartsWith("- ") || line.StartsWith("* "))
                {
                    FlushParagraph(doc, ref currentParagraph);
                    string listText = line.Substring(2).Trim();
                    var listPara = new Paragraph();
                    listPara.Margin = new Thickness(20, 2, 0, 2);
                    listPara.Inlines.Add(new Run("• ") { FontWeight = FontWeights.Bold });
                    AddInlineFormatted(listPara.Inlines, listText);
                    doc.Blocks.Add(listPara);
                    continue;
                }
                
                // Numbered lists
                var numberedMatch = Regex.Match(line, @"^(\d+\.\s)(.*)");
                if (numberedMatch.Success)
                {
                    FlushParagraph(doc, ref currentParagraph);
                    var listPara = new Paragraph();
                    listPara.Margin = new Thickness(20, 2, 0, 2);
                    listPara.Inlines.Add(new Run(numberedMatch.Groups[1].Value) { FontWeight = FontWeights.Bold });
                    AddInlineFormatted(listPara.Inlines, numberedMatch.Groups[2].Value);
                    doc.Blocks.Add(listPara);
                    continue;
                }
                
                // Blockquotes
                if (line.StartsWith(">"))
                {
                    FlushParagraph(doc, ref currentParagraph);
                    string quoteText = line.Substring(1).Trim();
                    var quotePara = new Paragraph();
                    quotePara.FontStyle = FontStyles.Italic;
                    quotePara.Foreground = FindBrush("TextMutedBrush", System.Windows.Media.Color.FromRgb(108, 112, 134));
                    quotePara.Margin = new Thickness(20, 5, 0, 5);
                    quotePara.BorderBrush = FindBrush("AccentBrush", System.Windows.Media.Color.FromRgb(116, 199, 236));
                    quotePara.BorderThickness = new Thickness(3, 0, 0, 0);
                    quotePara.Padding = new Thickness(10, 0, 0, 0);
                    AddInlineFormatted(quotePara.Inlines, quoteText);
                    doc.Blocks.Add(quotePara);
                    continue;
                }
                
                // Italics-only line (e.g. *compiled date*)
                // Falls through to regular paragraph handling with inline formatting
                
                // Regular paragraph text
                if (currentParagraph == null)
                {
                    currentParagraph = new Paragraph();
                    currentParagraph.Margin = new Thickness(0, 0, 0, 8);
                }
                else
                {
                    currentParagraph.Inlines.Add(new Run(" "));
                }
                
                AddInlineFormatted(currentParagraph.Inlines, line);
            }
            
            FlushParagraph(doc, ref currentParagraph);
            return doc;
        }
        
        private void FlushParagraph(FlowDocument doc, ref Paragraph? para)
        {
            if (para != null)
            {
                doc.Blocks.Add(para);
                para = null;
            }
        }
        
        private System.Windows.Media.Brush FindBrush(string resourceKey, System.Windows.Media.Color fallback)
        {
            if (System.Windows.Application.Current.Resources[resourceKey] is System.Windows.Media.Brush brush)
                return brush;
            return new SolidColorBrush(fallback);
        }
        
        // Inline patterns: link [text](url), code `text`, bold **text**, italic *text*.
        // Bold uses [^*] inside to avoid swallowing adjacent italics; link URL allows one
        // level of paren nesting (handles e.g. Wikipedia URLs).
        private static readonly Regex InlinePattern = new Regex(
            @"\[([^\]]+)\]\(((?:[^()]|\([^()]*\))+)\)" +
            @"|`([^`]+)`" +
            @"|\*\*([^*]+(?:\*(?!\*)[^*]*)*)\*\*" +
            @"|\*([^*\s][^*]*?)\*",
            RegexOptions.Compiled);

        /// <summary>
        /// Parses inline markdown formatting recursively so nested constructs like
        /// **[link](url)** (bold-wrapping-a-link, common in this file) render correctly.
        /// </summary>
        private void AddInlineFormatted(InlineCollection inlines, string text)
        {
            int lastIndex = 0;
            foreach (Match m in InlinePattern.Matches(text))
            {
                if (m.Index > lastIndex)
                    inlines.Add(new Run(text.Substring(lastIndex, m.Index - lastIndex)));

                if (m.Groups[1].Success) // [text](url)
                {
                    var link = new Hyperlink();
                    AddInlineFormatted(link.Inlines, m.Groups[1].Value); // allow nested formatting in link text
                    link.Foreground = FindBrush("AccentBrush", System.Windows.Media.Color.FromRgb(116, 199, 236));
                    link.TextDecorations = System.Windows.TextDecorations.Underline;
                    link.Cursor = System.Windows.Input.Cursors.Hand;
                    try
                    {
                        link.NavigateUri = new Uri(m.Groups[2].Value);
                        link.RequestNavigate += (s, e) =>
                        {
                            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true });
                            e.Handled = true;
                        };
                    }
                    catch { /* invalid URI — leave as styled text */ }
                    inlines.Add(link);
                }
                else if (m.Groups[3].Success) // `code`
                {
                    var codeRun = new Run(m.Groups[3].Value);
                    codeRun.FontFamily = new System.Windows.Media.FontFamily("Cascadia Code, Consolas");
                    codeRun.Background = TryFindResource("CtpSurface0Brush") as System.Windows.Media.Brush
                        ?? new SolidColorBrush(System.Windows.Media.Color.FromRgb(49, 50, 68));
                    inlines.Add(codeRun);
                }
                else if (m.Groups[4].Success) // **bold**
                {
                    var bold = new Bold();
                    AddInlineFormatted(bold.Inlines, m.Groups[4].Value);
                    inlines.Add(bold);
                }
                else if (m.Groups[5].Success) // *italic*
                {
                    var italic = new Italic();
                    AddInlineFormatted(italic.Inlines, m.Groups[5].Value);
                    inlines.Add(italic);
                }

                lastIndex = m.Index + m.Length;
            }

            if (lastIndex < text.Length)
                inlines.Add(new Run(text.Substring(lastIndex)));
        }
        
        private void ApplyTheme(string themeName)
        {
            _currentTheme = themeName;
            
            // Remove existing theme resources
            var oldResources = System.Windows.Application.Current.Resources.MergedDictionaries
                .Where(rd => rd.Source != null && rd.Source.ToString().Contains("Catppuccin"))
                .ToList();
            
            foreach (var oldResource in oldResources)
            {
                System.Windows.Application.Current.Resources.MergedDictionaries.Remove(oldResource);
            }
            
            // Add new theme
            try
            {
                var themeUri = new Uri($"Themes/Catppuccin-{themeName}.xaml", UriKind.Relative);
                var themeDict = new ResourceDictionary { Source = themeUri };
                System.Windows.Application.Current.Resources.MergedDictionaries.Add(themeDict);
                
                // Update SystemColors for MenuItem to use theme colors
                UpdateSystemColors(themeDict);
                
                // Reload current file to update colors
                if (!string.IsNullOrEmpty(_currentFilePath) && File.Exists(_currentFilePath))
                {
                    LoadFile(_currentFilePath);
                }
            }
            catch (Exception ex)
            {
                WpfMessageBox.Show($"Error applying theme: {ex.Message}");
            }
        }
        
        private void UpdateSystemColors(ResourceDictionary themeDict)
        {
            try
            {
                // Get brushes from the theme (these are guaranteed to exist)
                var crustBrush = themeDict["CtpCrustBrush"] as SolidColorBrush;
                var mantleBrush = themeDict["CtpMantleBrush"] as SolidColorBrush;
                var textBrush = themeDict["TextBrush"] as SolidColorBrush;
                
                if (crustBrush != null && mantleBrush != null && textBrush != null)
                {
                    // Update system color overrides
                    System.Windows.Application.Current.Resources[System.Windows.SystemColors.HighlightBrushKey] = crustBrush;
                    System.Windows.Application.Current.Resources[System.Windows.SystemColors.HighlightTextBrushKey] = textBrush;
                    System.Windows.Application.Current.Resources[System.Windows.SystemColors.ControlBrushKey] = mantleBrush;
                    System.Windows.Application.Current.Resources[System.Windows.SystemColors.MenuBrushKey] = mantleBrush;
                    System.Windows.Application.Current.Resources[System.Windows.SystemColors.MenuTextBrushKey] = textBrush;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to update system colors: {ex.Message}");
            }
        }
        
        private void UpdateFolderPathDisplay()
        {
            try
            {
                if (FolderPathText != null)
                {
                    FolderPathText.Text = _filesDirectory;
                }
            }
            catch
            {
                // Silently fail if folder path text box doesn't exist
            }
        }
        
        // Menu Event Handlers
        private void OpenFolder_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Forms.FolderBrowserDialog
            {
                Description = "Select folder containing AI-NEWS markdown files",
                ShowNewFolderButton = true,
                SelectedPath = _filesDirectory
            };
            
            if (dialog.ShowDialog() == Forms.DialogResult.OK)
            {
                _filesDirectory = dialog.SelectedPath;
                SaveFolderPath(_filesDirectory);
                UpdateFolderPathDisplay();
                LoadAvailableFiles();
            }
        }
        
        private void RefreshFiles_Click(object sender, RoutedEventArgs e)
        {
            LoadAvailableFiles();
            WpfMessageBox.Show($"File list refreshed. Found {FileListBox.Items.Count} file(s).", 
                "Refresh Complete", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        
        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }
        
        private void ThemeMocha_Click(object sender, RoutedEventArgs e)
        {
            ApplyTheme("Mocha");
        }
        
        private void ThemeLatte_Click(object sender, RoutedEventArgs e)
        {
            ApplyTheme("Latte");
        }
        
        private void ThemeFrappe_Click(object sender, RoutedEventArgs e)
        {
            ApplyTheme("Frappe");
        }
        
        private void ThemeMacchiato_Click(object sender, RoutedEventArgs e)
        {
            ApplyTheme("Macchiato");
        }
        
        private void About_Click(object sender, RoutedEventArgs e)
        {
            WpfMessageBox.Show(
                "AI-NEWS Reader v1.0\n\n" +
                "A beautiful markdown reader with Catppuccin themes.\n\n" +
                "Features:\n" +
                "• Four Catppuccin color themes\n" +
                "• Markdown rendering\n" +
                "• File browser\n\n" +
                "Theme: " + _currentTheme,
                "About AI-NEWS",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }
        
        private class FileItem
        {
            public string FullPath { get; set; } = "";
            public string DisplayName { get; set; } = "";
            
            public override string ToString() => DisplayName;
        }
    }
}

