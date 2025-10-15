using System;

namespace Fractals
{
    /// <summary>
    /// Types de fractales disponibles
    /// </summary>
    public enum FractalType
    {
        Mandelbrot,
        Julia,
        BurningShip,
        Tricorn,
        Newton
    }
    
    /// <summary>
    /// Préréglages de qualité (résolution)
    /// </summary>
    public class QualityPreset
    {
        public string Name { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int MaxIterations { get; set; }
        public bool UseScreenResolution { get; set; }

        public QualityPreset(string name, int width, int height, int maxIterations = 300, bool useScreenResolution = false)
        {
            Name = name;
            Width = width;
            Height = height;
            MaxIterations = maxIterations;
            UseScreenResolution = useScreenResolution;
        }

        public QualityPreset()
        {
            Name = "Normal";
            Width = 1920;
            Height = 1080;
            MaxIterations = 300;
            UseScreenResolution = false;
        }
    }
    
    /// <summary>
    /// Préréglages d'itérations
    /// </summary>
    public class IterationPreset
    {
        public string Name { get; set; }
        public int MaxIterations { get; set; }

        public IterationPreset(string name, int maxIterations)
        {
            Name = name;
            MaxIterations = maxIterations;
        }
    }
    
    /// <summary>
    /// État d'une fractale pour l'historique
    /// </summary>
    public class FractalState
    {
        public string ImagePath { get; set; } = "";
        public double FractalCenterX { get; set; }
        public double FractalCenterY { get; set; }
        public double FractalScale { get; set; }
        public int MaxIterations { get; set; }
        public int ImageWidth { get; set; }
        public int ImageHeight { get; set; }
        public int QualityIndex { get; set; }
        public int IterationIndex { get; set; }
        public double ViewZoom { get; set; }
        public double ViewOffsetX { get; set; }
        public double ViewOffsetY { get; set; }
        public FractalType FractalType { get; set; }
        public double JuliaConstantReal { get; set; }
        public double JuliaConstantImag { get; set; }
        public TimeSpan GenerationTime { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
