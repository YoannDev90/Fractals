using System;
using System.Diagnostics;
using System.IO;

namespace Fractals
{
    /// <summary>
    /// Système de logging pour l'application
    /// </summary>
    public class FractalLogger : IDisposable
    {
        private readonly StreamWriter? _logWriter;
        private readonly string _logFilePath;

        public FractalLogger()
        {
            _logFilePath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "Fractals",
                $"fractals_{DateTime.Now:yyyyMMdd}.log"
            );

            try
            {
                var logDir = Path.GetDirectoryName(_logFilePath);
                if (logDir != null && !Directory.Exists(logDir))
                {
                    Directory.CreateDirectory(logDir);
                }

                _logWriter = new StreamWriter(_logFilePath, append: true)
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

        public void Log(string message, string level = "INFO")
        {
            var timestamp = DateTime.Now.ToString("HH:mm:ss.fff");
            var logMessage = $"[{timestamp}] [{level}] {message}";

            Debug.WriteLine(logMessage);
            Console.WriteLine(logMessage);

            try
            {
                _logWriter?.WriteLine(logMessage);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Erreur d'écriture dans le log : {ex.Message}");
            }
        }

        public void LogError(string message, Exception? ex = null)
        {
            var errorMessage = ex != null ? $"{message} : {ex.Message}" : message;
            Log(errorMessage, "ERROR");
            if (ex != null)
            {
                Log($"StackTrace: {ex.StackTrace}", "ERROR");
            }
        }

        public void Dispose()
        {
            _logWriter?.Dispose();
        }
    }
}

