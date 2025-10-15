using System;
using System.Collections.Generic;
using System.IO;
using Avalonia.Media.Imaging;

namespace Fractals
{
    /// <summary>
    /// Gestion de l'historique des fractales avec cache disque
    /// </summary>
    public class FractalHistory
    {
        private readonly List<FractalState> _history = new();
        private int _currentIndex = -1;
        private readonly string _historyPath;
        private readonly int _maxStates;

        public FractalHistory(string historyPath, int maxStates = 50)
        {
            _historyPath = historyPath;
            _maxStates = maxStates;
            
            if (!Directory.Exists(_historyPath))
            {
                Directory.CreateDirectory(_historyPath);
            }
        }

        /// <summary>
        /// Ajoute un état à l'historique
        /// </summary>
        public void AddState(FractalState state, WriteableBitmap bitmap)
        {
            // Supprimer tous les états après l'index actuel (si on a fait des undo)
            if (_currentIndex < _history.Count - 1)
            {
                for (int i = _history.Count - 1; i > _currentIndex; i--)
                {
                    if (File.Exists(_history[i].ImagePath))
                    {
                        File.Delete(_history[i].ImagePath);
                    }
                    _history.RemoveAt(i);
                }
            }

            // Créer un nouveau nom de fichier unique
            var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss_fff");
            var filename = $"fractal_{timestamp}_{state.ImageWidth}x{state.ImageHeight}.png";
            state.ImagePath = Path.Combine(_historyPath, filename);

            // Sauvegarder l'image sur le disque
            using (var fs = File.Open(state.ImagePath, FileMode.Create))
            {
                bitmap.Save(fs);
            }

            _history.Add(state);
            _currentIndex = _history.Count - 1;

            // Nettoyer l'historique si trop d'éléments
            CleanupOldStates();
        }

        /// <summary>
        /// Revenir à l'état précédent (Undo)
        /// </summary>
        public FractalState? Undo()
        {
            if (_currentIndex > 0)
            {
                _currentIndex--;
                return _history[_currentIndex];
            }
            return null;
        }

        /// <summary>
        /// Avancer à l'état suivant (Redo)
        /// </summary>
        public FractalState? Redo()
        {
            if (_currentIndex < _history.Count - 1)
            {
                _currentIndex++;
                return _history[_currentIndex];
            }
            return null;
        }

        /// <summary>
        /// Charge l'image d'un état depuis le disque
        /// </summary>
        public WriteableBitmap? LoadImage(FractalState state)
        {
            if (!File.Exists(state.ImagePath))
                return null;

            using (var stream = File.OpenRead(state.ImagePath))
            {
                return WriteableBitmap.Decode(stream);
            }
        }

        /// <summary>
        /// Nettoie les anciens états de l'historique
        /// </summary>
        private void CleanupOldStates()
        {
            while (_history.Count > _maxStates)
            {
                var oldestState = _history[0];
                if (File.Exists(oldestState.ImagePath))
                {
                    File.Delete(oldestState.ImagePath);
                }
                _history.RemoveAt(0);
                _currentIndex--;
            }
        }

        public int CurrentIndex => _currentIndex;
        public int Count => _history.Count;
        public bool CanUndo => _currentIndex > 0;
        public bool CanRedo => _currentIndex < _history.Count - 1;
    }
}

