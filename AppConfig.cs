using System;
using System.IO;
using System.Collections.Generic;
using Tomlyn;
using Tomlyn.Model;

namespace Fractals
{
    public class AppConfig
    {
        // Singleton
        private static AppConfig? _instance;
        public static AppConfig Instance => _instance ??= Load();

        // General
        public int DefaultQuality { get; set; } = 1;
        public int MaxHistoryStates { get; set; } = 50;
        public bool ShowPanelOnStartup { get; set; } = true;

        // Fractale
        public double InitialCenterX { get; set; } = -0.5;
        public double InitialCenterY { get; set; } = 0.0;
        public double InitialScale { get; set; } = 3.0;
        public double ZoomFactor { get; set; } = 3.0;

        // Quality Presets
        public List<QualityPreset> QualityPresets { get; set; } = new();

        // Rendering
        public int RefreshEveryNLines { get; set; } = 5;
        public bool ShowProgress { get; set; } = true;

        // Colors
        public double ColorSaturation { get; set; } = 0.8;
        public double ColorValue { get; set; } = 1.0;
        public double HueMultiplier { get; set; } = 3.0;

        // Paths
        public string LogDirectory { get; set; } = "";
        public string HistoryDirectory { get; set; } = "";
        public string ExportDirectory { get; set; } = "";

        // Logging
        public bool LoggingEnabled { get; set; } = true;
        public string LogLevel { get; set; } = "INFO";
        public int LogRetentionDays { get; set; } = 30;

        // UI
        public string FontFamily { get; set; } = "Inter";
        public int FontSize { get; set; } = 12;
        public string PanelBackground { get; set; } = "#E6000000";
        public string HighlightColor { get; set; } = "#64C8FF";

        private static string ConfigPath => Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "config.toml"
        );

        public static AppConfig Load()
        {
            var config = new AppConfig();
            
            try
            {
                if (!File.Exists(ConfigPath))
                {
                    // Créer la config par défaut
                    config.CreateDefaultConfig();
                    return config;
                }

                var tomlContent = File.ReadAllText(ConfigPath);
                var model = Toml.ToModel(tomlContent);

                // Charger General
                if (model.TryGetValue("general", out var generalObj) && generalObj is TomlTable general)
                {
                    config.DefaultQuality = GetInt(general, "default_quality", 1);
                    config.MaxHistoryStates = GetInt(general, "max_history_states", 50);
                    config.ShowPanelOnStartup = GetBool(general, "show_panel_on_startup", true);
                }

                // Charger Fractale
                if (model.TryGetValue("fractale", out var fractaleObj) && fractaleObj is TomlTable fractale)
                {
                    config.InitialCenterX = GetDouble(fractale, "initial_center_x", -0.5);
                    config.InitialCenterY = GetDouble(fractale, "initial_center_y", 0.0);
                    config.InitialScale = GetDouble(fractale, "initial_scale", 3.0);
                    config.ZoomFactor = GetDouble(fractale, "zoom_factor", 3.0);
                }

                // Charger Quality Presets
                if (model.TryGetValue("quality_presets", out var presetsObj) && presetsObj is TomlTable presets)
                {
                    if (presets.TryGetValue("preset", out var presetArray) && presetArray is TomlTableArray array)
                    {
                        foreach (var item in array)
                        {
                            if (item is TomlTable preset)
                            {
                                config.QualityPresets.Add(new QualityPreset
                                {
                                    Name = GetString(preset, "name", "Normal"),
                                    Width = GetInt(preset, "width", 1920),
                                    Height = GetInt(preset, "height", 1080),
                                    MaxIterations = GetInt(preset, "max_iterations", 300),
                                    UseScreenResolution = GetBool(preset, "use_screen_resolution", false)
                                });
                            }
                        }
                    }
                }

                // Charger Rendering
                if (model.TryGetValue("rendering", out var renderingObj) && renderingObj is TomlTable rendering)
                {
                    config.RefreshEveryNLines = GetInt(rendering, "refresh_every_n_lines", 5);
                    config.ShowProgress = GetBool(rendering, "show_progress", true);
                }

                // Charger Colors
                if (model.TryGetValue("colors", out var colorsObj) && colorsObj is TomlTable colors)
                {
                    config.ColorSaturation = GetDouble(colors, "saturation", 0.8);
                    config.ColorValue = GetDouble(colors, "value", 1.0);
                    config.HueMultiplier = GetDouble(colors, "hue_multiplier", 3.0);
                }

                // Charger Paths
                if (model.TryGetValue("paths", out var pathsObj) && pathsObj is TomlTable paths)
                {
                    config.LogDirectory = GetString(paths, "log_directory", "");
                    config.HistoryDirectory = GetString(paths, "history_directory", "");
                    config.ExportDirectory = GetString(paths, "export_directory", "");
                }

                // Charger Logging
                if (model.TryGetValue("logging", out var loggingObj) && loggingObj is TomlTable logging)
                {
                    config.LoggingEnabled = GetBool(logging, "enabled", true);
                    config.LogLevel = GetString(logging, "level", "INFO");
                    config.LogRetentionDays = GetInt(logging, "retention_days", 30);
                }

                // Charger UI
                if (model.TryGetValue("ui", out var uiObj) && uiObj is TomlTable ui)
                {
                    config.FontFamily = GetString(ui, "font_family", "Inter");
                    config.FontSize = GetInt(ui, "font_size", 12);
                    config.PanelBackground = GetString(ui, "panel_background", "#E6000000");
                    config.HighlightColor = GetString(ui, "highlight_color", "#64C8FF");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors du chargement de la configuration : {ex.Message}");
                config.CreateDefaultConfig();
            }

            return config;
        }

        private void CreateDefaultConfig()
        {
            // Créer les presets par défaut
            QualityPresets = new List<QualityPreset>
            {
                new QualityPreset { Name = "Rapide", Width = 1280, Height = 720, MaxIterations = 150 },
                new QualityPreset { Name = "Normal", Width = 1920, Height = 1080, MaxIterations = 300, UseScreenResolution = true },
                new QualityPreset { Name = "Haute", Width = 2560, Height = 1440, MaxIterations = 500 },
                new QualityPreset { Name = "Ultra", Width = 3840, Height = 2160, MaxIterations = 1000 },
                new QualityPreset { Name = "Extrême", Width = 7680, Height = 4320, MaxIterations = 2000 }
            };
        }

        // Helpers
        private static string GetString(TomlTable table, string key, string defaultValue)
        {
            return table.TryGetValue(key, out var value) && value is string str ? str : defaultValue;
        }

        private static int GetInt(TomlTable table, string key, int defaultValue)
        {
            if (table.TryGetValue(key, out var value))
            {
                if (value is long l) return (int)l;
                if (value is int i) return i;
            }
            return defaultValue;
        }

        private static double GetDouble(TomlTable table, string key, double defaultValue)
        {
            if (table.TryGetValue(key, out var value))
            {
                if (value is double d) return d;
                if (value is long l) return (double)l;
                if (value is int i) return (double)i;
            }
            return defaultValue;
        }

        private static bool GetBool(TomlTable table, string key, bool defaultValue)
        {
            return table.TryGetValue(key, out var value) && value is bool b ? b : defaultValue;
        }

        public string GetLogDirectory()
        {
            if (!string.IsNullOrEmpty(LogDirectory)) return LogDirectory;
            return Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "Fractals"
            );
        }

        public string GetHistoryDirectory()
        {
            if (!string.IsNullOrEmpty(HistoryDirectory)) return HistoryDirectory;
            return Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "Fractals",
                "History"
            );
        }

        public string GetExportDirectory()
        {
            if (!string.IsNullOrEmpty(ExportDirectory)) return ExportDirectory;
            return Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
        }
    }
}
