using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Media;

namespace Fractals
{
    /// <summary>
    /// Gestion de l'affichage du panneau d'informations
    /// </summary>
    public class FractalPanelManager
    {
        private readonly UserControl _view;

        public FractalPanelManager(UserControl view)
        {
            _view = view;
        }

        /// <summary>
        /// Met à jour l'affichage du type de fractale
        /// </summary>
        public void UpdateFractalTypeDisplay(FractalType currentType)
        {
            var fractalTypeText = _view.FindControl<TextBlock>("FractalTypeText");
            if (fractalTypeText != null)
            {
                string displayName = currentType switch
                {
                    FractalType.Mandelbrot => "Mandelbrot",
                    FractalType.Julia => "Julia Set",
                    FractalType.BurningShip => "Burning Ship",
                    FractalType.Tricorn => "Tricorn",
                    FractalType.Newton => "Newton Fractal",
                    _ => "Fractale"
                };
                fractalTypeText.Text = displayName;
            }

            UpdateFractalListDisplay(currentType);
        }

        /// <summary>
        /// Met à jour la liste des fractales disponibles
        /// </summary>
        private void UpdateFractalListDisplay(FractalType currentType)
        {
            var fractalList = _view.FindControl<StackPanel>("FractalList");
            if (fractalList == null) return;

            fractalList.Children.Clear();

            var fractals = new[]
            {
                (FractalType.Mandelbrot, "F1. Mandelbrot"),
                (FractalType.Julia, "F2. Julia Set"),
                (FractalType.BurningShip, "F3. Burning Ship"),
                (FractalType.Tricorn, "F4. Tricorn"),
                (FractalType.Newton, "F5. Newton Fractal")
            };

            foreach (var (type, label) in fractals)
            {
                bool isSelected = type == currentType;

                var textBlock = new TextBlock
                {
                    Text = label,
                    FontSize = 15,
                    Foreground = isSelected ? new SolidColorBrush(Color.FromRgb(100, 200, 255)) : new SolidColorBrush(Color.FromRgb(180, 180, 180)),
                    FontWeight = isSelected ? FontWeight.SemiBold : FontWeight.Normal
                };

                fractalList.Children.Add(textBlock);
            }
        }

        /// <summary>
        /// Met à jour l'affichage du zoom
        /// </summary>
        public void UpdateZoomDisplay(double viewZoom, double fractalScale)
        {
            double totalZoom = viewZoom * (3.0 / fractalScale);
            string zoomText = totalZoom < 100000 ? $"x {totalZoom:F5}" : $"x {totalZoom:E2}";

            var zoomTextBlock = _view.FindControl<TextBlock>("ZoomText");
            if (zoomTextBlock != null)
                zoomTextBlock.Text = zoomText;
        }

        /// <summary>
        /// Met à jour l'affichage du centre
        /// </summary>
        public void UpdateCenterDisplay(double centerX, double centerY)
        {
            var centerXText = _view.FindControl<TextBlock>("CenterXText");
            var centerYText = _view.FindControl<TextBlock>("CenterYText");

            if (centerXText != null)
                centerXText.Text = $"{centerX:F8}";
            if (centerYText != null)
                centerYText.Text = $"{centerY:F8}i";
        }

        /// <summary>
        /// Met à jour l'affichage du curseur
        /// </summary>
        public void UpdateCursorDisplay(double mouseX, double mouseY)
        {
            var cursorXText = _view.FindControl<TextBlock>("CursorXText");
            var cursorYText = _view.FindControl<TextBlock>("CursorYText");

            if (cursorXText != null)
                cursorXText.Text = $"{mouseX:F8}";
            if (cursorYText != null)
                cursorYText.Text = $"{mouseY:F8}i";
        }

        /// <summary>
        /// Met à jour l'affichage de la résolution
        /// </summary>
        public void UpdateQualityDisplay(List<QualityPreset> presets, int currentIndex)
        {
            var qualityList = _view.FindControl<StackPanel>("QualityList");
            if (qualityList == null) return;

            qualityList.Children.Clear();

            for (int i = 0; i < presets.Count; i++)
            {
                var preset = presets[i];
                bool isSelected = i == currentIndex;

                var textBlock = new TextBlock
                {
                    Text = string.Format("Q{0}. {1} ({2}x{3})", i + 1, preset.Name, preset.Width, preset.Height),
                    FontSize = 15,
                    Foreground = isSelected ? new SolidColorBrush(Color.FromRgb(100, 200, 255)) : new SolidColorBrush(Color.FromRgb(180, 180, 180)),
                    FontWeight = isSelected ? FontWeight.SemiBold : FontWeight.Normal
                };

                qualityList.Children.Add(textBlock);
            }
        }

        /// <summary>
        /// Met à jour l'affichage des itérations
        /// </summary>
        public void UpdateIterationDisplay(List<IterationPreset> presets, int currentIndex)
        {
            var iterationList = _view.FindControl<StackPanel>("IterationList");
            if (iterationList == null) return;

            iterationList.Children.Clear();

            for (int i = 0; i < presets.Count; i++)
            {
                var preset = presets[i];
                bool isSelected = i == currentIndex;

                var textBlock = new TextBlock
                {
                    Text = $"I{i + 1}. {preset.Name} ({preset.MaxIterations})",
                    FontSize = 15,
                    Foreground = isSelected ? new SolidColorBrush(Color.FromRgb(100, 200, 255)) : new SolidColorBrush(Color.FromRgb(180, 180, 180)),
                    FontWeight = isSelected ? FontWeight.SemiBold : FontWeight.Normal
                };

                iterationList.Children.Add(textBlock);
            }
        }

        /// <summary>
        /// Met à jour l'affichage de la génération
        /// </summary>
        public void UpdateGenerationDisplay(bool isGenerating, bool isPaused, double speed, 
            int currentLine, int totalLines, System.TimeSpan lastTime, string qualityName, string iterationName)
        {
            var statusText = _view.FindControl<TextBlock>("GenerationStatusText");
            var speedText = _view.FindControl<TextBlock>("GenerationSpeedText");
            var timeRemainingText = _view.FindControl<TextBlock>("GenerationTimeRemainingText");

            if (isGenerating)
            {
                // Statut : En cours ou En pause
                if (statusText != null)
                {
                    statusText.Text = isPaused ? "En pause" : "En cours";
                    statusText.Foreground = isPaused 
                        ? new SolidColorBrush(Color.FromRgb(255, 170, 0)) // Orange
                        : new SolidColorBrush(Color.FromRgb(100, 200, 255)); // Cyan
                }

                // Vitesse
                if (speedText != null)
                {
                    if (speed > 0 && !isPaused)
                        speedText.Text = $"{speed:F1} l/s";
                    else
                        speedText.Text = isPaused ? "En pause" : "Calcul...";
                }

                // Temps restant
                if (timeRemainingText != null && speed > 0 && currentLine > 0 && !isPaused)
                {
                    int remainingLines = totalLines - currentLine;
                    double secondsRemaining = remainingLines / speed;

                    if (secondsRemaining < 60)
                        timeRemainingText.Text = $"{secondsRemaining:F0}s";
                    else if (secondsRemaining < 3600)
                        timeRemainingText.Text = $"{secondsRemaining / 60:F1}min";
                    else
                        timeRemainingText.Text = $"{secondsRemaining / 3600:F1}h";
                }
                else if (timeRemainingText != null)
                {
                    timeRemainingText.Text = isPaused ? "Pause" : "Calcul...";
                }
            }
            else
            {
                // Génération terminée
                if (statusText != null)
                {
                    statusText.Text = "Terminé";
                    statusText.Foreground = new SolidColorBrush(Color.FromRgb(136, 255, 136)); // Vert
                }

                if (speedText != null)
                {
                    speedText.Text = $"{qualityName}";
                }

                if (timeRemainingText != null)
                {
                    if (lastTime.TotalSeconds > 0)
                        timeRemainingText.Text = $"{lastTime.TotalSeconds:F2}s";
                    else
                        timeRemainingText.Text = "--";
                }
            }
        }

        /// <summary>
        /// Bascule la visibilité du panneau
        /// </summary>
        public bool TogglePanelVisibility()
        {
            var panel = _view.FindControl<Border>("InfoPanel");
            if (panel != null)
            {
                panel.IsVisible = !panel.IsVisible;
                return panel.IsVisible;
            }
            return true;
        }
    }
}
