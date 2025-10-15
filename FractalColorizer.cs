using Avalonia.Media;
using System;

namespace Fractals
{
    /// <summary>
    /// Gestion des couleurs pour le rendu des fractales
    /// </summary>
    public static class FractalColorizer
    {
        /// <summary>
        /// Calcule la couleur d'un pixel en fonction du nombre d'itérations
        /// </summary>
        public static Color GetColor(int iter, int maxIter, double lastX, double lastY)
        {
            if (iter == maxIter) 
                return Colors.Black;

            // Smooth coloring avec les dernières valeurs de x et y
            double zn = Math.Sqrt(lastX * lastX + lastY * lastY);
            double smooth = iter + 1 - Math.Log(Math.Log(zn)) / Math.Log(2.0);
            double hue = 360.0 * (smooth / maxIter * 3.0) % 360.0;
            
            return HsvToColor(hue, 0.8, 1.0);
        }

        /// <summary>
        /// Convertit une couleur HSV en RGB
        /// </summary>
        private static Color HsvToColor(double h, double s, double v)
        {
            h = h % 360;
            int hi = (int)(h / 60) % 6;
            double f = h / 60 - Math.Floor(h / 60);
            v = v * 255;
            byte p = (byte)(v * (1 - s));
            byte q = (byte)(v * (1 - f * s));
            byte t = (byte)(v * (1 - (1 - f) * s));
            byte vb = (byte)v;
            
            return hi switch
            {
                0 => Color.FromArgb(255, vb, t, p),
                1 => Color.FromArgb(255, q, vb, p),
                2 => Color.FromArgb(255, p, vb, t),
                3 => Color.FromArgb(255, p, q, vb),
                4 => Color.FromArgb(255, t, p, vb),
                _ => Color.FromArgb(255, vb, p, q)
            };
        }
    }
}

