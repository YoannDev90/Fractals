using System;

namespace Fractals
{
    /// <summary>
    /// Calculs mathématiques pour les différentes fractales
    /// </summary>
    public static class FractalCalculator
    {
        /// <summary>
        /// Calcule la fractale selon le type sélectionné
        /// </summary>
        public static int Calculate(FractalType type, double x0, double y0, int maxIter, 
            double juliaReal, double juliaImag, out double lastX, out double lastY)
        {
            return type switch
            {
                FractalType.Mandelbrot => CalculateMandelbrot(x0, y0, maxIter, out lastX, out lastY),
                FractalType.Julia => CalculateJulia(x0, y0, maxIter, juliaReal, juliaImag, out lastX, out lastY),
                FractalType.BurningShip => CalculateBurningShip(x0, y0, maxIter, out lastX, out lastY),
                FractalType.Tricorn => CalculateTricorn(x0, y0, maxIter, out lastX, out lastY),
                FractalType.Newton => CalculateNewton(x0, y0, maxIter, out lastX, out lastY),
                _ => CalculateMandelbrot(x0, y0, maxIter, out lastX, out lastY)
            };
        }

        /// <summary>
        /// Mandelbrot: z = z² + c
        /// </summary>
        private static int CalculateMandelbrot(double x0, double y0, int maxIter, out double lastX, out double lastY)
        {
            double x = 0, y = 0;
            int iter = 0;
            while (x * x + y * y <= 4 && iter < maxIter)
            {
                double xtemp = x * x - y * y + x0;
                y = 2 * x * y + y0;
                x = xtemp;
                iter++;
            }
            lastX = x;
            lastY = y;
            return iter;
        }

        /// <summary>
        /// Julia Set: z = z² + c (avec c constant)
        /// </summary>
        private static int CalculateJulia(double x0, double y0, int maxIter, 
            double juliaReal, double juliaImag, out double lastX, out double lastY)
        {
            double x = x0, y = y0;
            int iter = 0;
            while (x * x + y * y <= 4 && iter < maxIter)
            {
                double xtemp = x * x - y * y + juliaReal;
                y = 2 * x * y + juliaImag;
                x = xtemp;
                iter++;
            }
            lastX = x;
            lastY = y;
            return iter;
        }

        /// <summary>
        /// Burning Ship: z = (|Re(z)| + i|Im(z)|)² + c
        /// </summary>
        private static int CalculateBurningShip(double x0, double y0, int maxIter, out double lastX, out double lastY)
        {
            double x = 0, y = 0;
            int iter = 0;
            while (x * x + y * y <= 4 && iter < maxIter)
            {
                double xtemp = x * x - y * y + x0;
                y = 2 * Math.Abs(x) * Math.Abs(y) + y0;
                x = xtemp;
                iter++;
            }
            lastX = x;
            lastY = y;
            return iter;
        }

        /// <summary>
        /// Tricorn (Mandelbar): z = conj(z)² + c
        /// </summary>
        private static int CalculateTricorn(double x0, double y0, int maxIter, out double lastX, out double lastY)
        {
            double x = 0, y = 0;
            int iter = 0;
            while (x * x + y * y <= 4 && iter < maxIter)
            {
                double xtemp = x * x - y * y + x0;
                y = -2 * x * y + y0; // Conjugué
                x = xtemp;
                iter++;
            }
            lastX = x;
            lastY = y;
            return iter;
        }

        /// <summary>
        /// Newton Fractal: z = z - (z³ - 1) / (3z²)
        /// </summary>
        private static int CalculateNewton(double x0, double y0, int maxIter, out double lastX, out double lastY)
        {
            double x = x0, y = y0;
            int iter = 0;
            const double tolerance = 0.0001;

            while (iter < maxIter)
            {
                // Calcul de z³
                double x2 = x * x - y * y;
                double y2 = 2 * x * y;
                double x3 = x2 * x - y2 * y;
                double y3 = x2 * y + y2 * x;

                // z³ - 1
                x3 -= 1;

                // 3z²
                double denom_x = 3 * (x * x - y * y);
                double denom_y = 3 * (2 * x * y);

                // Division complexe: (z³ - 1) / (3z²)
                double denom = denom_x * denom_x + denom_y * denom_y;
                if (denom < tolerance) break;

                double div_x = (x3 * denom_x + y3 * denom_y) / denom;
                double div_y = (y3 * denom_x - x3 * denom_y) / denom;

                // z = z - (z³ - 1) / (3z²)
                x -= div_x;
                y -= div_y;

                // Vérifier la convergence vers une racine
                if (Math.Abs(div_x) < tolerance && Math.Abs(div_y) < tolerance)
                    break;

                iter++;
            }
            lastX = x;
            lastY = y;
            return iter;
        }
    }
}

