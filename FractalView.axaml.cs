using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.Media.Imaging;
using Avalonia.Input;
using Avalonia.Platform;
using Avalonia.Platform.Storage;
using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Fractals
{
    public partial class FractalView : UserControl
    {
        private double _viewZoom = 1.0;
        private double _viewOffsetX;
        private double _viewOffsetY;
        private int _maxIter = 300;
        private WriteableBitmap? _bitmap;
        private bool _isGenerating = true;
        
        // Paramètres de la fractale
        private double _fractalCenterX = -0.5;
        private double _fractalCenterY;
        private double _fractalScale = 3.0;
        
        // Position actuelle de la souris
        private Point _lastMousePosition;
        
        // Statistiques de génération
        private DateTime _generationStartTime;
        private double _generationSpeed;
        private TimeSpan _lastGenerationTime;
        private int _currentLine;
        
        // Qualités prédéfinies
        private (string Name, int Width, int Height, int MaxIter)[] _qualityPresets;
        private int _currentQualityIndex = 1; // Normal par défaut
        private int _currentImageWidth;
        private int _currentImageHeight;
        
        // Logging
        private static readonly string LogFilePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "Fractals",
            $"fractals_{DateTime.Now:yyyyMMdd}.log"
        );
        private static StreamWriter? _logWriter;
        
        // Visibilité du panneau
        private bool _isPanelVisible = true;

        public FractalView()
        {
            InitializeComponent();
            Focusable = true;
            
            // Initialiser le logging
            InitializeLogging();
            Log("Application démarrée");
            
            // Obtenir la résolution de l'écran - sera initialisé après attachement
            int screenWidth = 1920;
            int screenHeight = 1080;
            
            // Définir les presets avec la résolution de l'écran pour "Normal"
            _qualityPresets = new[]
            {
                ("Rapide", 1280, 720, 150),
                ("Normal", screenWidth, screenHeight, 300),
                ("Haute", 2560, 1440, 500),
                ("Ultra", 3840, 2160, 1000),
                ("Extrême", 7680, 4320, 2000)
            };
            
            _currentImageWidth = screenWidth;
            _currentImageHeight = screenHeight;
            
            this.AttachedToVisualTree += async (_, _) =>
            {
                this.Focus();
                
                // Obtenir la résolution de l'écran maintenant que le contrôle est attaché
                var window = this.VisualRoot as Window;
                if (window != null)
                {
                    var screen = window.Screens.ScreenFromWindow(window);
                    if (screen != null)
                    {
                        screenWidth = screen.Bounds.Width;
                        screenHeight = screen.Bounds.Height;
                        
                        Log($"Résolution d'écran détectée: {screenWidth}x{screenHeight}");
                        
                        // Mettre à jour le preset Normal avec la résolution détectée
                        _qualityPresets = new[]
                        {
                            ("Rapide", 1280, 720, 150),
                            ("Normal", screenWidth, screenHeight, 300),
                            ("Haute", 2560, 1440, 500),
                            ("Ultra", 3840, 2160, 1000),
                            ("Extrême", 7680, 4320, 2000)
                        };
                        
                        _currentImageWidth = screenWidth;
                        _currentImageHeight = screenHeight;
                    }
                }
                
                Log("Initialisation de la vue");
                UpdateQualityDisplay();
                await GenerateFractalAsync(true);
            };
            
            this.PointerMoved += OnPointerMoved;
            this.PointerPressed += OnPointerPressed;
        }
        
        private void OnPointerMoved(object? sender, PointerEventArgs e)
        {
            _lastMousePosition = e.GetPosition(this);
            UpdateCursorDisplay();
        }
        
        private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
        {
            // Gestion des clics
        }
        
        private void UpdateCursorDisplay()
        {
            var (mouseX, mouseY) = GetComplexCoordinatesAtMouse();
            
            var cursorXText = this.FindControl<TextBlock>("CursorXText");
            var cursorYText = this.FindControl<TextBlock>("CursorYText");
            
            if (cursorXText != null)
                cursorXText.Text = $"{mouseX:F8}";
            if (cursorYText != null)
                cursorYText.Text = $"{mouseY:F8}i";
        }
        
        private void UpdateZoomDisplay()
        {
            double totalZoom = _viewZoom * (3.0 / _fractalScale);
            string zoomText = totalZoom < 100000 ? $"x {totalZoom:F5}" : $"x {totalZoom:E2}";
            
            var zoomTextBlock = this.FindControl<TextBlock>("ZoomText");
            if (zoomTextBlock != null)
                zoomTextBlock.Text = zoomText;
        }
        
        private void UpdateCenterDisplay()
        {
            var centerXText = this.FindControl<TextBlock>("CenterXText");
            var centerYText = this.FindControl<TextBlock>("CenterYText");
            
            if (centerXText != null)
                centerXText.Text = $"{_fractalCenterX:F8}";
            if (centerYText != null)
                centerYText.Text = $"{_fractalCenterY:F8}i";
        }
        
        private void UpdateGenerationDisplay()
        {
            var statusText = this.FindControl<TextBlock>("GenerationStatusText");
            var speedText = this.FindControl<TextBlock>("GenerationSpeedText");
            var resolutionText = this.FindControl<TextBlock>("GenerationResolutionText");
            var timeRemainingText = this.FindControl<TextBlock>("GenerationTimeRemainingText");
            
            if (_isGenerating)
            {
                if (statusText != null)
                    statusText.Text = "En cours...";
                if (speedText != null)
                    speedText.Text = $"{_generationSpeed:F1} lignes/s";
                
                // Calcul du temps restant estimé
                if (timeRemainingText != null && _generationSpeed > 0 && _currentLine > 0)
                {
                    int remainingLines = _currentImageHeight - _currentLine;
                    double secondsRemaining = remainingLines / _generationSpeed;
                    
                    if (secondsRemaining < 60)
                        timeRemainingText.Text = $"Temps restant: {secondsRemaining:F0}s";
                    else if (secondsRemaining < 3600)
                        timeRemainingText.Text = $"Temps restant: {secondsRemaining / 60:F1}min";
                    else
                        timeRemainingText.Text = $"Temps restant: {secondsRemaining / 3600:F1}h";
                }
                else if (timeRemainingText != null)
                {
                    timeRemainingText.Text = "Temps restant: calcul...";
                }
            }
            else
            {
                if (statusText != null)
                    statusText.Text = $"{_lastGenerationTime.TotalSeconds:F2}s";
                if (speedText != null)
                    speedText.Text = _qualityPresets[_currentQualityIndex].Name;
                if (timeRemainingText != null)
                    timeRemainingText.Text = "";
            }
            
            if (resolutionText != null)
                resolutionText.Text = $"{_currentImageWidth}x{_currentImageHeight}";
        }
        
        private void UpdateQualityDisplay()
        {
            var qualityList = this.FindControl<StackPanel>("QualityList");
            if (qualityList == null) return;
            
            qualityList.Children.Clear();
            
            for (int i = 0; i < _qualityPresets.Length; i++)
            {
                var preset = _qualityPresets[i];
                bool isSelected = i == _currentQualityIndex;
                
                var textBlock = new TextBlock
                {
                    Text = $"{i + 1}. {preset.Name} ({preset.Width}x{preset.Height})",
                    FontSize = isSelected ? 12 : 11,
                    Foreground = isSelected ? new SolidColorBrush(Color.FromRgb(100, 200, 255)) : new SolidColorBrush(Color.FromRgb(180, 180, 180)),
                    FontWeight = isSelected ? FontWeight.SemiBold : FontWeight.Normal
                };
                
                qualityList.Children.Add(textBlock);
            }
        }
        
        private async Task GenerateFractalAsync(bool showProgress)
        {
            Log($"Début génération fractale - Résolution: {_currentImageWidth}x{_currentImageHeight}, MaxIter: {_maxIter}, Centre: ({_fractalCenterX:F8}, {_fractalCenterY:F8}), Échelle: {_fractalScale:E2}");
            
            _isGenerating = true;
            _generationStartTime = DateTime.Now;
            _currentLine = 0;
            UpdateGenerationDisplay();
            UpdateQualityDisplay();
            UpdateZoomDisplay();
            UpdateCenterDisplay();
            
            _bitmap = new WriteableBitmap(new PixelSize(_currentImageWidth, _currentImageHeight), new Vector(96, 96), PixelFormat.Bgra8888, AlphaFormat.Opaque);
            
            double scale = _fractalScale / Math.Min(_currentImageWidth, _currentImageHeight);
            double offsetX = _fractalCenterX - (_fractalScale * _currentImageWidth / _currentImageHeight) / 2;
            double offsetY = _fractalCenterY - _fractalScale / 2;
            int maxIter = _maxIter;

            // Génération ligne par ligne
            for (int py = 0; py < _currentImageHeight; py++)
            {
                _currentLine = py;
                
                using (var lockedBitmap = _bitmap.Lock())
                {
                    unsafe
                    {
                        var ptr = (uint*)lockedBitmap.Address.ToPointer();
                        int stride = lockedBitmap.RowBytes / 4;

                        for (int px = 0; px < _currentImageWidth; px++)
                        {
                            double x0 = px * scale + offsetX;
                            double y0 = py * scale + offsetY;
                            double x = 0, y = 0;
                            int iter = 0;
                            while (x * x + y * y <= 4 && iter < maxIter)
                            {
                                double xtemp = x * x - y * y + x0;
                                y = 2 * x * y + y0;
                                x = xtemp;
                                iter++;
                            }
                            Color color = GetColor(iter, maxIter, x, y);
                            ptr[py * stride + px] = (uint)((color.A << 24) | (color.R << 16) | (color.G << 8) | color.B);
                        }
                    }
                }
                
                // Calculer la vitesse de génération
                var elapsed = DateTime.Now - _generationStartTime;
                if (elapsed.TotalSeconds > 0)
                {
                    _generationSpeed = py / elapsed.TotalSeconds;
                }
                
                // Rafraîchir l'affichage seulement si on veut montrer la progression
                if (showProgress && py % 5 == 0)
                {
                    UpdateGenerationDisplay();
                    await Dispatcher.UIThread.InvokeAsync(() => InvalidateVisual());
                    await Task.Delay(1);
                }
            }

            _lastGenerationTime = DateTime.Now - _generationStartTime;
            _isGenerating = false;
            UpdateGenerationDisplay();
            UpdateZoomDisplay();
            UpdateCenterDisplay();
            
            Log($"Génération terminée - Durée: {_lastGenerationTime.TotalSeconds:F2}s, Vitesse moyenne: {_currentImageHeight / _lastGenerationTime.TotalSeconds:F1} lignes/s");
        }
        
        protected override async void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (_isGenerating) return;
            
            switch (e.Key)
            {
                case Key.D1:
                case Key.D2:
                case Key.D3:
                case Key.D4:
                case Key.D5:
                    int index = e.Key - Key.D1;
                    if (index < _qualityPresets.Length)
                    {
                        _currentQualityIndex = index;
                        var preset = _qualityPresets[index];
                        _currentImageWidth = preset.Width;
                        _currentImageHeight = preset.Height;
                        _maxIter = preset.MaxIter;
                        Log($"Changement de qualité: {preset.Name} ({preset.Width}x{preset.Height}, {preset.MaxIter} iterations)");
                        _ = GenerateFractalAsync(true);
                    }
                    return;
                case Key.F11:
                    // Plein écran
                    var window = this.VisualRoot as Window;
                    if (window != null)
                    {
                        var newState = window.WindowState == WindowState.FullScreen ? WindowState.Normal : WindowState.FullScreen;
                        Log($"Basculement mode plein écran: {newState}");
                        window.WindowState = newState;
                    }
                    return;
                case Key.S:
                    // Export
                    if (_bitmap != null)
                    {
                        Log("Ouverture de la boîte de dialogue d'export");
                        await SaveBitmapWithDialog(_bitmap);
                    }
                    return;
                case Key.R:
                    // Réinitialiser complètement la vue ET la fractale
                    Log("Réinitialisation de la vue");
                    _fractalCenterX = -0.5;
                    _fractalCenterY = 0.0;
                    _fractalScale = 3.0;
                    _ = GenerateFractalAsync(true);
                    return;
                case Key.N:
                    Log("Régénération manuelle demandée");
                    _ = GenerateFractalAsync(true);
                    return;
                case Key.Add:
                case Key.OemPlus:
                    // Zoom sur la position actuelle de la souris
                    Log("Zoom in (facteur x3)");
                    RegenerateAtMousePosition(true);
                    return;
                case Key.Subtract:
                case Key.OemMinus:
                    // Dézoom sur la position actuelle de la souris
                    Log("Zoom out (facteur /3)");
                    RegenerateAtMousePosition(false);
                    return;
                case Key.H:
                    // Basculer la visibilité du panneau
                    TogglePanelVisibility();
                    return;
            }
        }
        
        private void TogglePanelVisibility()
        {
            _isPanelVisible = !_isPanelVisible;
            var panel = this.FindControl<Border>("InfoPanel");
            if (panel != null)
            {
                panel.IsVisible = _isPanelVisible;
                Log($"Panneau {(_isPanelVisible ? "affiché" : "masqué")}");
            }
        }
        
        private void RegenerateAtMousePosition(bool zoomIn)
        {
            // Convertir la position de l'écran en coordonnées de la fractale
            double baseScaleX = Bounds.Width / _currentImageWidth;
            double baseScaleY = Bounds.Height / _currentImageHeight;
            double baseScale = Math.Min(baseScaleX, baseScaleY);
            
            // Position relative dans l'image affichée
            double imgX = (_lastMousePosition.X - (Bounds.Width - _currentImageWidth * baseScale * _viewZoom) / 2 - _viewOffsetX) / (baseScale * _viewZoom);
            double imgY = (_lastMousePosition.Y - (Bounds.Height - _currentImageHeight * baseScale * _viewZoom) / 2 - _viewOffsetY) / (baseScale * _viewZoom);
            
            // Convertir en coordonnées du plan complexe
            double scale = _fractalScale / Math.Min(_currentImageWidth, _currentImageHeight);
            double offsetX = _fractalCenterX - (_fractalScale * _currentImageWidth / _currentImageHeight) / 2;
            double offsetY = _fractalCenterY - _fractalScale / 2;
            
            double complexX = imgX * scale + offsetX;
            double complexY = imgY * scale + offsetY;
            
            // Nouveau centre et zoom
            _fractalCenterX = complexX;
            _fractalCenterY = complexY;
            
            if (zoomIn)
            {
                _fractalScale /= 3.0; // Zoom x3
            }
            else
            {
                _fractalScale *= 3.0; // Dézoom /3
            }
            
            // Régénérer
            _ = GenerateFractalAsync(true);
        }
        
        private (double, double) GetComplexCoordinatesAtMouse()
        {
            double baseScaleX = Bounds.Width / _currentImageWidth;
            double baseScaleY = Bounds.Height / _currentImageHeight;
            double baseScale = Math.Min(baseScaleX, baseScaleY);
            
            double imgX = (_lastMousePosition.X - (Bounds.Width - _currentImageWidth * baseScale * _viewZoom) / 2 - _viewOffsetX) / (baseScale * _viewZoom);
            double imgY = (_lastMousePosition.Y - (Bounds.Height - _currentImageHeight * baseScale * _viewZoom) / 2 - _viewOffsetY) / (baseScale * _viewZoom);
            
            double scale = _fractalScale / Math.Min(_currentImageWidth, _currentImageHeight);
            double offsetX = _fractalCenterX - (_fractalScale * _currentImageWidth / _currentImageHeight) / 2;
            double offsetY = _fractalCenterY - _fractalScale / 2;
            
            double complexX = imgX * scale + offsetX;
            double complexY = imgY * scale + offsetY;
            
            return (complexX, complexY);
        }

        public override void Render(DrawingContext context)
        {
            base.Render(context);
            
            // Dessiner la fractale en plein écran
            if (_bitmap != null)
            {
                double baseScaleX = Bounds.Width / _currentImageWidth;
                double baseScaleY = Bounds.Height / _currentImageHeight;
                double baseScale = Math.Max(baseScaleX, baseScaleY); // Utiliser Max pour remplir l'écran
                double finalScale = baseScale * _viewZoom;
                double displayWidth = _currentImageWidth * finalScale;
                double displayHeight = _currentImageHeight * finalScale;
                double offsetX = (Bounds.Width - displayWidth) / 2 + _viewOffsetX;
                double offsetY = (Bounds.Height - displayHeight) / 2 + _viewOffsetY;
                context.DrawImage(_bitmap, 
                    new Rect(0, 0, _currentImageWidth, _currentImageHeight),
                    new Rect(offsetX, offsetY, displayWidth, displayHeight));
            }
        }
        
        private Color GetColor(int iter, int maxIter, double lastX, double lastY)
        {
            if (iter == maxIter) return Colors.Black;
            
            // Smooth coloring avec les dernières valeurs de x et y
            double zn = Math.Sqrt(lastX * lastX + lastY * lastY);
            double smooth = iter + 1 - Math.Log(Math.Log(zn)) / Math.Log(2.0);
            double hue = 360.0 * (smooth / maxIter * 3.0) % 360.0;
            return HsvToColor(hue, 0.8, 1.0);
        }

        private Color HsvToColor(double h, double s, double v)
        {
            h = h % 360;
            int hi = (int)(h / 60) % 6;
            double f = h / 60 - Math.Floor(h / 60);
            v = v * 255;
            byte p = (byte)(v * (1 - s));
            byte q = (byte)(v * (1 - f * s));
            byte t = (byte)(v * (1 - (1 - f) * s));
            byte vb = (byte)v;
            switch (hi)
            {
                case 0: return Color.FromArgb(255, vb, t, p);
                case 1: return Color.FromArgb(255, q, vb, p);
                case 2: return Color.FromArgb(255, p, vb, t);
                case 3: return Color.FromArgb(255, p, q, vb);
                case 4: return Color.FromArgb(255, t, p, vb);
                default: return Color.FromArgb(255, vb, p, q);
            }
        }

        private void SaveBitmap(WriteableBitmap bitmap, string filename)
        {
            using (var fs = File.Open(filename, FileMode.Create))
            {
                bitmap.Save(fs);
            }
        }

        private async Task SaveBitmapWithDialog(WriteableBitmap bitmap)
        {
            var topLevel = TopLevel.GetTopLevel(this);
            if (topLevel == null) return;

            var file = await topLevel.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
            {
                Title = "Enregistrer l'image",
                SuggestedFileName = $"fractal_{DateTime.Now:yyyyMMdd_HHmmss}_{_currentImageWidth}x{_currentImageHeight}.png",
                FileTypeChoices = new[]
                {
                    new FilePickerFileType("Images PNG") { Patterns = new[] { "*.png" } },
                    new FilePickerFileType("Tous les fichiers") { Patterns = new[] { "*.*" } }
                },
                DefaultExtension = "png"
            });

            if (file != null)
            {
                try
                {
                    Log($"Export de l'image vers: {file.Name}");
                    // Sauvegarder le bitmap au chemin spécifié
                    await using var stream = await file.OpenWriteAsync();
                    bitmap.Save(stream);
                    Log($"Image exportée avec succès: {file.Name}");
                }
                catch (Exception ex)
                {
                    LogError("Erreur lors de l'export de l'image", ex);
                }
            }
            else
            {
                Log("Export annulé par l'utilisateur");
            }
        }
        
        private static void InitializeLogging()
        {
            try
            {
                var logDir = Path.GetDirectoryName(LogFilePath);
                if (logDir != null && !Directory.Exists(logDir))
                {
                    Directory.CreateDirectory(logDir);
                }
                
                _logWriter = new StreamWriter(LogFilePath, append: true)
                {
                    AutoFlush = true
                };
                
                _logWriter.WriteLine($"\n========== Session démarrée : {DateTime.Now:yyyy-MM-dd HH:mm:ss} ==========");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Erreur lors de l'initialisation du logging : {ex.Message}");
            }
        }
        
        private static void Log(string message, string level = "INFO")
        {
            var timestamp = DateTime.Now.ToString("HH:mm:ss.fff");
            var logMessage = $"[{timestamp}] [{level}] {message}";
            
            // Écrire dans la console (visible uniquement dans l'IDE)
            Debug.WriteLine(logMessage);
            Console.WriteLine(logMessage);
            
            // Écrire dans le fichier
            try
            {
                _logWriter?.WriteLine(logMessage);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Erreur d'écriture dans le log : {ex.Message}");
            }
        }
        
        private static void LogError(string message, Exception? ex = null)
        {
            var errorMessage = ex != null ? $"{message} : {ex.Message}" : message;
            Log(errorMessage, "ERROR");
            if (ex != null)
            {
                Log($"StackTrace: {ex.StackTrace}", "ERROR");
            }
        }
    }
}
