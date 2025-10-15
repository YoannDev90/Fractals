using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Media;

namespace Fractals
{
    /// <summary>
    /// Gestion des préréglages de qualité et d'itérations
    /// </summary>
    public class FractalPresetManager
    {
        private readonly List<QualityPreset> _qualityPresets = new();
        private readonly List<IterationPreset> _iterationPresets = new();
        
        public int CurrentQualityIndex { get; private set; } = 1;
        public int CurrentIterationIndex { get; private set; } = 1;

        public FractalPresetManager(int screenWidth, int screenHeight)
        {
            InitializePresets(screenWidth, screenHeight);
        }

        private void InitializePresets(int screenWidth, int screenHeight)
        {
            // Préréglages de résolution
            _qualityPresets.Add(new QualityPreset("Rapide", 1280, 720));
            _qualityPresets.Add(new QualityPreset("Normal", screenWidth, screenHeight));
            _qualityPresets.Add(new QualityPreset("Haute", 2560, 1440));
            _qualityPresets.Add(new QualityPreset("Ultra", 3840, 2160));
            _qualityPresets.Add(new QualityPreset("Extrême", 7680, 4320));

            // Préréglages d'itérations
            _iterationPresets.Add(new IterationPreset("Très rapide", 100));
            _iterationPresets.Add(new IterationPreset("Rapide", 300));
            _iterationPresets.Add(new IterationPreset("Normal", 500));
            _iterationPresets.Add(new IterationPreset("Détaillé", 1000));
            _iterationPresets.Add(new IterationPreset("Très détaillé", 2000));
        }

        public void UpdateScreenResolution(int screenWidth, int screenHeight)
        {
            if (_qualityPresets.Count > 1)
            {
                _qualityPresets[1] = new QualityPreset("Normal", screenWidth, screenHeight);
            }
        }

        public bool SetQualityIndex(int index)
        {
            if (index >= 0 && index < _qualityPresets.Count)
            {
                CurrentQualityIndex = index;
                return true;
            }
            return false;
        }

        public bool SetIterationIndex(int index)
        {
            if (index >= 0 && index < _iterationPresets.Count)
            {
                CurrentIterationIndex = index;
                return true;
            }
            return false;
        }

        public QualityPreset CurrentQuality => _qualityPresets[CurrentQualityIndex];
        public IterationPreset CurrentIteration => _iterationPresets[CurrentIterationIndex];
        
        public List<QualityPreset> QualityPresets => _qualityPresets;
        public List<IterationPreset> IterationPresets => _iterationPresets;
    }
}
