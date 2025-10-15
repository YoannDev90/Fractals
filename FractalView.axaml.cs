using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.Media.Imaging;
using Avalonia.Input;
using Avalonia.Platform.Storage;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Fractals
{
    public partial class FractalView : UserControl
    {
        // Composants refactorisés
        private readonly FractalLogger _logger;
        private readonly FractalHistory _history;
        private readonly FractalPresetManager _presetManager;
        private readonly FractalPanelManager _panelManager;

        // État actuel de la fractale
        private FractalType _currentFractalType = FractalType.Mandelbrot;
        private double _fractalCenterX = -0.5;
        private double _fractalCenterY;
        private double _fractalScale = 3.0;
        
        // Paramètres Julia
        private double _juliaConstantReal = -0.7;
        private double _juliaConstantImag = 0.27015;

        // Vue et rendu
        private double _viewZoom = 1.0;
        private double _viewOffsetX;
        private double _viewOffsetY;
        private WriteableBitmap? _bitmap;
        private bool _isGenerating = true;
        private bool _isPaused;
        
        // Dimensions actuelles
        private int _currentImageWidth;
        private int _currentImageHeight;
        private int _maxIter = 300;

        // État précédent pour annulation rapide (touche X)
        private WriteableBitmap? _previousBitmap;
        private FractalState? _previousState;

        // Position de la souris
        private Point _lastMousePosition;

        // Statistiques de génération
        private DateTime _generationStartTime;
        private double _generationSpeed;
        private TimeSpan _lastGenerationTime;
        private int _currentLine;

        // État des touches Q et I
        private bool _qKeyPressed;
        private bool _iKeyPressed;

        public FractalView()
        {
            InitializeComponent();
            Focusable = true;

            // Initialiser les composants
            _logger = new FractalLogger();
            _logger.Log("Application démarrée");

            string historyPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "Fractals",
                "History"
            );
            _history = new FractalHistory(historyPath, maxStates: 50);

            // Initialiser avec une résolution par défaut
            int screenWidth = 1920;
            int screenHeight = 1080;
            _presetManager = new FractalPresetManager(screenWidth, screenHeight);
            _panelManager = new FractalPanelManager(this);

            _currentImageWidth = screenWidth;
            _currentImageHeight = screenHeight;
            _maxIter = _presetManager.CurrentIteration.MaxIterations;

            this.AttachedToVisualTree += async (_, _) =>
            {
                this.Focus();

                // Obtenir la résolution de l'écran
                var window = this.VisualRoot as Window;
                if (window != null)
                {
                    var screen = window.Screens.ScreenFromWindow(window);
                    if (screen != null)
                    {
                        screenWidth = screen.Bounds.Width;
                        screenHeight = screen.Bounds.Height;
                        
                        _logger.Log($"Résolution d'écran détectée: {screenWidth}x{screenHeight}");
                        
                        _presetManager.UpdateScreenResolution(screenWidth, screenHeight);
                        _currentImageWidth = screenWidth;
                        _currentImageHeight = screenHeight;
                    }
                }

                _logger.Log("Initialisation de la vue");
                UpdateAllDisplays();
                await GenerateFractalAsync(true);
            };

            this.PointerMoved += OnPointerMoved;
            this.PointerPressed += OnPointerPressed;
        }

        private void OnPointerMoved(object? sender, PointerEventArgs e)
        {
            _lastMousePosition = e.GetPosition(this);
            var (mouseX, mouseY) = GetComplexCoordinatesAtMouse();
            _panelManager.UpdateCursorDisplay(mouseX, mouseY);
        }

        private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
        {
            // Gestion future des clics
        }

        private void UpdateAllDisplays()
        {
            _panelManager.UpdateFractalTypeDisplay(_currentFractalType);
            _panelManager.UpdateZoomDisplay(_viewZoom, _fractalScale);
            _panelManager.UpdateCenterDisplay(_fractalCenterX, _fractalCenterY);
            _panelManager.UpdateQualityDisplay(_presetManager.QualityPresets, _presetManager.CurrentQualityIndex);
            _panelManager.UpdateIterationDisplay(_presetManager.IterationPresets, _presetManager.CurrentIterationIndex);
            UpdateGenerationDisplay();
        }

        private void UpdateGenerationDisplay()
        {
            var quality = _presetManager.CurrentQuality;
            var iteration = _presetManager.CurrentIteration;
            
            _panelManager.UpdateGenerationDisplay(
                _isGenerating, _isPaused, _generationSpeed,
                _currentLine, _currentImageHeight, _lastGenerationTime,
                quality.Name, iteration.Name
            );
        }

        private async Task GenerateFractalAsync(bool showProgress)
        {
            _logger.Log($"Génération - Résolution: {_currentImageWidth}x{_currentImageHeight}, " +
                       $"Itérations: {_maxIter}, Centre: ({_fractalCenterX:F8}, {_fractalCenterY:F8}), " +
                       $"Échelle: {_fractalScale:E2}");

            _isGenerating = true;
            _isPaused = false;
            _generationStartTime = DateTime.Now;
            _currentLine = 0;

            UpdateAllDisplays();

            // Sauvegarder l'état actuel AVANT de générer
            if (_bitmap != null)
            {
                SaveCurrentState();
            }

            _bitmap = new WriteableBitmap(
                new PixelSize(_currentImageWidth, _currentImageHeight),
                new Vector(96, 96),
                Avalonia.Platform.PixelFormat.Bgra8888,
                Avalonia.Platform.AlphaFormat.Opaque
            );

            // Réinitialiser la vue
            _viewZoom = 1.0;
            _viewOffsetX = 0.0;
            _viewOffsetY = 0.0;

            double scale = _fractalScale / Math.Min(_currentImageWidth, _currentImageHeight);
            double offsetX = _fractalCenterX - (_fractalScale * _currentImageWidth / _currentImageHeight) / 2;
            double offsetY = _fractalCenterY - _fractalScale / 2;

            // Génération ligne par ligne
            for (int py = 0; py < _currentImageHeight; py++)
            {
                // Gestion de la pause
                while (_isPaused)
                {
                    UpdateGenerationDisplay();
                    await Task.Delay(100);
                }

                // Vérification d'annulation
                if (!_isGenerating)
                {
                    _logger.Log("Génération annulée par l'utilisateur");
                    return;
                }

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

                            // Utiliser FractalCalculator
                            int iter = FractalCalculator.Calculate(
                                _currentFractalType, x0, y0, _maxIter,
                                _juliaConstantReal, _juliaConstantImag,
                                out double lastX, out double lastY
                            );

                            // Utiliser FractalColorizer
                            Color color = FractalColorizer.GetColor(iter, _maxIter, lastX, lastY);
                            ptr[py * stride + px] = (uint)((color.A << 24) | (color.R << 16) | (color.G << 8) | color.B);
                        }
                    }
                }

                // Calculer la vitesse
                var elapsed = DateTime.Now - _generationStartTime;
                if (elapsed.TotalSeconds > 0)
                {
                    _generationSpeed = py / elapsed.TotalSeconds;
                }

                // Rafraîchir l'affichage
                if (showProgress && py % 5 == 0)
                {
                    UpdateGenerationDisplay();
                    await Dispatcher.UIThread.InvokeAsync(() => InvalidateVisual());
                    await Task.Delay(1);
                }
            }

            _lastGenerationTime = DateTime.Now - _generationStartTime;
            _isGenerating = false;
            _isPaused = false;
            UpdateGenerationDisplay();

            // Sauvegarder dans l'historique
            SaveStateToHistory();

            _logger.Log($"Génération terminée - Durée: {_lastGenerationTime.TotalSeconds:F2}s, " +
                       $"Vitesse: {_currentImageHeight / _lastGenerationTime.TotalSeconds:F1} lignes/s");
        }

        protected override async void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            // Suivre l'état des touches Q et I
            if (e.Key == Key.Q)
            {
                _qKeyPressed = true;
                return;
            }
            if (e.Key == Key.I)
            {
                _iKeyPressed = true;
                return;
            }

            // Gérer C et X même pendant la génération
            if (e.Key == Key.C && _isGenerating)
            {
                _isPaused = !_isPaused;
                _logger.Log(_isPaused ? "Pause" : "Reprise");
                UpdateGenerationDisplay();
                return;
            }

            if (e.Key == Key.X && _isGenerating)
            {
                CancelAndRestorePrevious();
                return;
            }

            if (_isGenerating) return;

            switch (e.Key)
            {
                // Historique
                case Key.Z:
                    await PerformUndo();
                    return;
                case Key.Y:
                    await PerformRedo();
                    return;

                // Fractales F1-F5
                case Key.F1:
                    ChangeFractal(FractalType.Mandelbrot, -0.5, 0.0);
                    return;
                case Key.F2:
                    ChangeFractal(FractalType.Julia, 0.0, 0.0);
                    return;
                case Key.F3:
                    ChangeFractal(FractalType.BurningShip, -0.5, -0.5);
                    return;
                case Key.F4:
                    ChangeFractal(FractalType.Tricorn, 0.0, 0.0);
                    return;
                case Key.F5:
                    ChangeFractal(FractalType.Newton, 0.0, 0.0);
                    return;

                // Résolution Q + 1-5
                case Key.D1:
                case Key.D2:
                case Key.D3:
                case Key.D4:
                case Key.D5:
                case Key.NumPad1:
                case Key.NumPad2:
                case Key.NumPad3:
                case Key.NumPad4:
                case Key.NumPad5:
                    if (_qKeyPressed)
                    {
                        // Q + chiffre = changement de résolution
                        int qIndex = e.Key >= Key.D1 && e.Key <= Key.D5 ? e.Key - Key.D1 : e.Key - Key.NumPad1;
                        if (_presetManager.SetQualityIndex(qIndex))
                        {
                            var preset = _presetManager.CurrentQuality;
                            _currentImageWidth = preset.Width;
                            _currentImageHeight = preset.Height;
                            _logger.Log(string.Format("Résolution: {0} ({1}x{2})", preset.Name, preset.Width, preset.Height));
                            _panelManager.UpdateQualityDisplay(_presetManager.QualityPresets, qIndex);
                            _ = GenerateFractalAsync(true);
                        }
                    }
                    else if (_iKeyPressed)
                    {
                        // I + chiffre = changement d'itérations
                        int iIndex = e.Key >= Key.D1 && e.Key <= Key.D5 ? e.Key - Key.D1 : e.Key - Key.NumPad1;
                        if (_presetManager.SetIterationIndex(iIndex))
                        {
                            var preset = _presetManager.CurrentIteration;
                            _maxIter = preset.MaxIterations;
                            _logger.Log($"Itérations: {preset.Name} ({preset.MaxIterations})");
                            _panelManager.UpdateIterationDisplay(_presetManager.IterationPresets, iIndex);
                            _ = GenerateFractalAsync(true);
                        }
                    }
                    return;

                // Autres commandes
                case Key.F11:
                    var window = this.VisualRoot as Window;
                    if (window != null)
                    {
                        var newState = window.WindowState == WindowState.FullScreen 
                            ? WindowState.Normal 
                            : WindowState.FullScreen;
                        _logger.Log($"Plein écran: {newState}");
                        window.WindowState = newState;
                    }
                    return;

                case Key.S:
                    if (_bitmap != null)
                    {
                        await SaveBitmapWithDialog(_bitmap);
                    }
                    return;

                case Key.R:
                    _logger.Log("Réinitialisation");
                    _fractalCenterX = -0.5;
                    _fractalCenterY = 0.0;
                    _fractalScale = 3.0;
                    _ = GenerateFractalAsync(true);
                    return;

                case Key.N:
                    _logger.Log("Régénération manuelle");
                    _ = GenerateFractalAsync(true);
                    return;

                case Key.Add:
                case Key.OemPlus:
                    _logger.Log("Zoom in (x3)");
                    RegenerateAtMousePosition(true);
                    return;

                case Key.Subtract:
                case Key.OemMinus:
                    _logger.Log("Zoom out (/3)");
                    RegenerateAtMousePosition(false);
                    return;

                case Key.H:
                    bool visible = _panelManager.TogglePanelVisibility();
                    _logger.Log($"Panneau {(visible ? "affiché" : "masqué")}");
                    return;
            }
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);

            // Réinitialiser l'état des touches Q et I
            if (e.Key == Key.Q)
            {
                _qKeyPressed = false;
            }
            if (e.Key == Key.I)
            {
                _iKeyPressed = false;
            }
        }

        private void ChangeFractal(FractalType type, double centerX, double centerY)
        {
            _currentFractalType = type;
            _fractalCenterX = centerX;
            _fractalCenterY = centerY;
            _fractalScale = 3.0;
            _logger.Log($"Fractale: {type}");
            _ = GenerateFractalAsync(true);
        }

        private void CancelAndRestorePrevious()
        {
            _isGenerating = false;
            _isPaused = false;

            if (_previousBitmap != null && _previousState != null)
            {
                _logger.Log("Annulation et restauration");
                
                _bitmap = _previousBitmap;
                
                // Restaurer l'état complet
                _fractalCenterX = _previousState.FractalCenterX;
                _fractalCenterY = _previousState.FractalCenterY;
                _fractalScale = _previousState.FractalScale;
                _maxIter = _previousState.MaxIterations;
                _currentImageWidth = _previousState.ImageWidth;
                _currentImageHeight = _previousState.ImageHeight;
                _currentFractalType = _previousState.FractalType;
                _juliaConstantReal = _previousState.JuliaConstantReal;
                _juliaConstantImag = _previousState.JuliaConstantImag;
                _lastGenerationTime = _previousState.GenerationTime;
                _viewZoom = _previousState.ViewZoom;
                _viewOffsetX = _previousState.ViewOffsetX;
                _viewOffsetY = _previousState.ViewOffsetY;
                
                _presetManager.SetQualityIndex(_previousState.QualityIndex);
                _presetManager.SetIterationIndex(_previousState.IterationIndex);

                UpdateAllDisplays();
                InvalidateVisual();
            }
            else
            {
                // Première génération annulée - afficher un fond noir
                _logger.Log("Annulation de la première génération - affichage fond noir");
                
                _bitmap = new WriteableBitmap(
                    new PixelSize(_currentImageWidth, _currentImageHeight),
                    new Vector(96, 96),
                    Avalonia.Platform.PixelFormat.Bgra8888,
                    Avalonia.Platform.AlphaFormat.Opaque
                );

                // Remplir avec du noir
                using (var lockedBitmap = _bitmap.Lock())
                {
                    unsafe
                    {
                        var ptr = (uint*)lockedBitmap.Address.ToPointer();
                        int totalPixels = _currentImageWidth * _currentImageHeight;
                        for (int i = 0; i < totalPixels; i++)
                        {
                            ptr[i] = 0xFF000000; // Noir opaque (ARGB)
                        }
                    }
                }

                UpdateAllDisplays();
                InvalidateVisual();
            }
        }

        private void SaveCurrentState()
        {
            if (_bitmap == null) return;

            _previousBitmap = _bitmap;
            _previousState = new FractalState
            {
                FractalCenterX = _fractalCenterX,
                FractalCenterY = _fractalCenterY,
                FractalScale = _fractalScale,
                MaxIterations = _maxIter,
                ImageWidth = _currentImageWidth,
                ImageHeight = _currentImageHeight,
                QualityIndex = _presetManager.CurrentQualityIndex,
                IterationIndex = _presetManager.CurrentIterationIndex,
                ViewZoom = _viewZoom,
                ViewOffsetX = _viewOffsetX,
                ViewOffsetY = _viewOffsetY,
                FractalType = _currentFractalType,
                JuliaConstantReal = _juliaConstantReal,
                JuliaConstantImag = _juliaConstantImag,
                GenerationTime = _lastGenerationTime,
                Timestamp = DateTime.Now
            };
        }

        private void SaveStateToHistory()
        {
            if (_bitmap == null) return;

            var state = new FractalState
            {
                FractalCenterX = _fractalCenterX,
                FractalCenterY = _fractalCenterY,
                FractalScale = _fractalScale,
                MaxIterations = _maxIter,
                ImageWidth = _currentImageWidth,
                ImageHeight = _currentImageHeight,
                QualityIndex = _presetManager.CurrentQualityIndex,
                IterationIndex = _presetManager.CurrentIterationIndex,
                ViewZoom = _viewZoom,
                ViewOffsetX = _viewOffsetX,
                ViewOffsetY = _viewOffsetY,
                FractalType = _currentFractalType,
                JuliaConstantReal = _juliaConstantReal,
                JuliaConstantImag = _juliaConstantImag,
                GenerationTime = _lastGenerationTime,
                Timestamp = DateTime.Now
            };

            _history.AddState(state, _bitmap);
            _logger.Log($"État sauvegardé (historique: {_history.Count})");
        }

        private async Task PerformUndo()
        {
            var state = _history.Undo();
            if (state != null)
            {
                await LoadState(state);
                _logger.Log($"Undo: {_history.CurrentIndex + 1}/{_history.Count}");
            }
        }

        private async Task PerformRedo()
        {
            var state = _history.Redo();
            if (state != null)
            {
                await LoadState(state);
                _logger.Log($"Redo: {_history.CurrentIndex + 1}/{_history.Count}");
            }
        }

        private async Task LoadState(FractalState state)
        {
            var bitmap = _history.LoadImage(state);
            if (bitmap == null) return;

            _bitmap = bitmap;
            _fractalCenterX = state.FractalCenterX;
            _fractalCenterY = state.FractalCenterY;
            _fractalScale = state.FractalScale;
            _maxIter = state.MaxIterations;
            _currentImageWidth = state.ImageWidth;
            _currentImageHeight = state.ImageHeight;
            _currentFractalType = state.FractalType;
            _juliaConstantReal = state.JuliaConstantReal;
            _juliaConstantImag = state.JuliaConstantImag;
            _lastGenerationTime = state.GenerationTime;
            _viewZoom = state.ViewZoom;
            _viewOffsetX = state.ViewOffsetX;
            _viewOffsetY = state.ViewOffsetY;

            _presetManager.SetQualityIndex(state.QualityIndex);
            _presetManager.SetIterationIndex(state.IterationIndex);

            SaveCurrentState();
            UpdateAllDisplays();
            InvalidateVisual();

            await Task.CompletedTask;
        }

        private void RegenerateAtMousePosition(bool zoomIn)
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

            _fractalCenterX = complexX;
            _fractalCenterY = complexY;

            if (zoomIn)
                _fractalScale /= 3.0;
            else
                _fractalScale *= 3.0;

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

            return (imgX * scale + offsetX, imgY * scale + offsetY);
        }

        public override void Render(DrawingContext context)
        {
            base.Render(context);

            if (_bitmap != null)
            {
                double baseScaleX = Bounds.Width / _currentImageWidth;
                double baseScaleY = Bounds.Height / _currentImageHeight;
                double baseScale = Math.Min(baseScaleX, baseScaleY);
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
                    _logger.Log($"Export: {file.Name}");
                    await using var stream = await file.OpenWriteAsync();
                    bitmap.Save(stream);
                    _logger.Log("Export réussi");
                }
                catch (Exception ex)
                {
                    _logger.LogError("Erreur export", ex);
                }
            }
        }
    }
}

