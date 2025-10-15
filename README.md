# 🎨 Fractals Explorer - Explorateur de Fractales Interactif

Une application interactive multi-plateforme pour explorer et générer des fractales (Mandelbrot, Julia, Burning Ship, Tricorn, Newton) en haute résolution, développée avec Avalonia UI et .NET 9.

![Fractals](https://img.shields.io/badge/.NET-9.0-512BD4?style=flat-square&logo=dotnet)
![Avalonia](https://img.shields.io/badge/Avalonia-11.3.7-8B44AC?style=flat-square)
![Platform](https://img.shields.io/badge/platform-Windows%20%7C%20Linux%20%7C%20macOS-lightgrey?style=flat-square)

## 📥 Téléchargement

### Versions pré-compilées

Téléchargez la dernière version pour votre système d'exploitation :

| Plateforme | Architecture | Téléchargement |
|------------|--------------|----------------|
| 🪟 **Windows** | x64 | [Fractals-win-x64.zip](https://github.com/votre-username/Fractals/releases) |
| 🐧 **Linux** | x64 | [Fractals-linux-x64.zip](https://github.com/votre-username/Fractals/releases) |
| 🍎 **macOS** | Intel (x64) | [Fractals-osx-x64.zip](https://github.com/votre-username/Fractals/releases) |
| 🍎 **macOS** | Apple Silicon (ARM64) | [Fractals-osx-arm64.zip](https://github.com/votre-username/Fractals/releases) |

**Installation** :
1. Téléchargez l'archive correspondant à votre système
2. Extrayez le contenu
3. Lancez l'exécutable `Fractals` (Linux/macOS) ou `Fractals.exe` (Windows)

**Note pour macOS** : Au premier lancement, faites un clic droit → "Ouvrir" pour contourner la vérification Gatekeeper.

**Note pour Linux** : Rendez l'exécutable avec `chmod +x Fractals` si nécessaire.

---

## 📋 Table des matières

- [Téléchargement](#-téléchargement)
- [Fonctionnalités](#-fonctionnalités)
- [Fractales disponibles](#-fractales-disponibles)
- [Utilisation](#-utilisation)
- [Raccourcis clavier](#️-raccourcis-clavier)
- [Qualités prédéfinies](#-qualités-prédéfinies)
- [Architecture technique](#-architecture-technique)
- [Compilation](#-compilation)
- [Contribution](#-contribution)
- [Licence](#-licence)

## ✨ Fonctionnalités

### 🎯 Fractales multiples
- **Mandelbrot** : La fractale classique et iconique
- **Julia Set** : Ensemble de Julia avec paramètres personnalisables
- **Burning Ship** : Variante du Mandelbrot avec valeurs absolues
- **Tricorn** : Mandelbrot avec conjugaison complexe
- **Newton Fractal** : Basé sur la méthode de Newton-Raphson

### 🔍 Exploration interactive
- **Zoom infini** : Explorez les fractales avec un zoom quasi-illimité
- **Navigation en temps réel** : Suivez les coordonnées complexes sous votre curseur
- **Zoom centré sur le curseur** : Zoomez directement sur la position de votre souris (touches + et -)
- **Génération progressive** : Visualisez la fractale en cours de génération ligne par ligne
- **Pause/Reprise** : Mettez en pause la génération (touche C)
- **Annulation rapide** : Annulez une génération en cours (touche X)

### 🎨 Rendu de qualité
- **5 niveaux de qualité** prédéfinis (de 720p à 8K - 7680x4320)
- **5 niveaux d'itérations** (de 100 à 2000 itérations)
- **Coloration lissée** (smooth coloring) pour des dégradés fluides
- **Algorithme optimisé** utilisant du code unsafe pour des performances maximales
- **Adaptation automatique** à la résolution de votre écran

### 💾 Historique et export
- **Historique Z/Y** : Navigation Undo/Redo dans vos explorations (jusqu'à 50 états)
- **Export PNG haute résolution** avec dialogue de sauvegarde
- **Nommage automatique** incluant la date, l'heure et la résolution
- **Sauvegarde automatique** des images dans l'historique

### 🎛️ Interface moderne
- **Panneau flottant** avec design glass morphism moderne
- **Affichage en temps réel** :
  - Niveau de zoom avec précision
  - Coordonnées du centre et du curseur
  - Statistiques de génération (vitesse, temps restant)
  - Type de fractale actif
  - Qualité et itérations sélectionnées
- **Interface masquable** (touche H) pour des captures d'écran sans éléments d'UI
- **Mode plein écran** (F11) pour une immersion totale

### 📝 Logging complet
- **Fichiers journaux** automatiques dans `~/.local/share/Fractals/` (Linux/macOS) ou `%LOCALAPPDATA%\Fractals\` (Windows)
- **Horodatage précis** au milliseconde près
- **Traçabilité complète** des actions utilisateur et des générations

## 🌀 Fractales disponibles

| Touche | Fractale | Description |
|--------|----------|-------------|
| **F1** | Mandelbrot | La fractale classique, centrée sur (-0.5, 0) |
| **F2** | Julia Set | Ensemble de Julia avec paramètres c = -0.7 + 0.27015i |
| **F3** | Burning Ship | Variante avec abs() appliqué aux coordonnées |
| **F4** | Tricorn | Mandelbrot avec conjugaison complexe |
| **F5** | Newton Fractal | Basé sur z³ - 1 = 0 avec méthode de Newton |

## 🎮 Utilisation

### Premier lancement

1. **Lancez l'application** : L'application démarre automatiquement avec une génération Mandelbrot
2. **Attendez la génération** : La fractale se génère progressivement (vous pouvez voir la progression)
3. **Explorez** : Utilisez les touches + et - pour zoomer/dézoomer sur la position du curseur
4. **Changez de fractale** : Appuyez sur F1-F5 pour changer de type de fractale
5. **Ajustez la qualité** : Appuyez sur Q+1 à Q+5 pour changer la résolution
6. **Ajustez les itérations** : Appuyez sur I+1 à I+5 pour changer le niveau de détail

### Exploration avancée

- **Zoom précis** : Placez votre curseur sur une zone intéressante et appuyez sur +
- **Historique** : Utilisez Z (undo) et Y (redo) pour naviguer dans votre historique
- **Pause** : Appuyez sur C pour mettre en pause une génération longue
- **Annulation** : Appuyez sur X pour annuler et revenir à l'état précédent
- **Export** : Appuyez sur S pour sauvegarder l'image actuelle en PNG

## ⌨️ Raccourcis clavier

### Navigation et fractales
| Touche | Action |
|--------|--------|
| **F1-F5** | Changer le type de fractale (Mandelbrot, Julia, Burning Ship, Tricorn, Newton) |
| **+ / -** | Zoom in / Zoom out (centré sur le curseur) |
| **R** | Réinitialiser la vue (retour à la position initiale) |

### Qualité et rendu
| Touche | Action |
|--------|--------|
| **Q + 1-5** | Changer la qualité (résolution) |
| **I + 1-5** | Changer le nombre d'itérations |
| **N** | Régénérer manuellement la fractale |

### Contrôles de génération
| Touche | Action |
|--------|--------|
| **C** | Pause / Reprendre la génération |
| **X** | Annuler la génération et revenir à l'état précédent |

### Historique et sauvegarde
| Touche | Action |
|--------|--------|
| **Z** | Undo (revenir en arrière dans l'historique) |
| **Y** | Redo (avancer dans l'historique) |
| **S** | Sauvegarder l'image actuelle (PNG) |

### Interface
| Touche | Action |
|--------|--------|
| **H** | Masquer/Afficher le panneau d'informations |
| **F11** | Basculer en mode plein écran |

## 📐 Qualités prédéfinies

| Index | Nom | Résolution | Raccourci |
|-------|-----|------------|-----------|
| 1 | **Rapide** | 1280 x 720 | Q+1 |
| 2 | **Normal** | Résolution écran | Q+2 |
| 3 | **Haute** | 2560 x 1440 | Q+3 |
| 4 | **Ultra** | 3840 x 2160 (4K) | Q+4 |
| 5 | **Extrême** | 7680 x 4320 (8K) | Q+5 |

## 🔄 Niveaux d'itérations

| Index | Nom | Itérations | Raccourci | Temps estimé (1080p) |
|-------|-----|------------|-----------|---------------------|
| 1 | **Très rapide** | 100 | I+1 | ~0.5s |
| 2 | **Rapide** | 300 | I+2 | ~1.5s |
| 3 | **Normal** | 500 | I+3 | ~2.5s |
| 4 | **Détaillé** | 1000 | I+4 | ~5s |
| 5 | **Très détaillé** | 2000 | I+5 | ~10s |

*Les temps sont indicatifs et varient selon votre processeur.*

## 🏗️ Architecture technique

### Technologies utilisées

- **.NET 9.0** : Framework moderne et performant
- **Avalonia UI 11.3.7** : Interface utilisateur multi-plateforme
- **C# 12** : Langage avec code unsafe pour optimisation
- **SkiaSharp** : Moteur de rendu graphique

### Structure du projet

```
Fractals/
├── FractalView.axaml(.cs)        # Interface utilisateur principale
├── FractalCalculator.cs          # Algorithmes de calcul des fractales
├── FractalColorizer.cs           # Système de coloration
├── FractalHistory.cs             # Gestion de l'historique (Undo/Redo)
├── FractalLogger.cs              # Système de logging
├── FractalModels.cs              # Modèles de données
├── FractalPanelManager.cs        # Gestion de l'affichage du panneau
├── FractalPresetManager.cs       # Gestion des préréglages
└── AppConfig.cs                  # Configuration TOML (future)
```

### Optimisations

- **Code unsafe** : Accès direct à la mémoire pour manipulation rapide des pixels
- **Génération asynchrone** : L'UI reste responsive pendant les calculs
- **Rafraîchissement progressif** : Affichage tous les 5 lignes pour feedback visuel
- **Calcul optimisé** : Algorithme d'échappement optimisé pour chaque type de fractale
- **Smooth coloring** : Interpolation logarithmique pour des couleurs fluides

## 🛠️ Compilation

### Prérequis
- **.NET 9.0 SDK** : [Télécharger](https://dotnet.microsoft.com/download/dotnet/9.0)

### Depuis les sources

```bash
# Cloner le dépôt
git clone https://github.com/votre-username/Fractals.git
cd Fractals

# Restaurer les dépendances
dotnet restore

# Compiler
dotnet build

# Lancer
dotnet run
```

### Publication pour distribution

**Windows (x64)** :
```bash
dotnet publish -c Release \
  -r win-x64 \
  --self-contained true \
  /p:PublishSingleFile=true \
  /p:IncludeNativeLibrariesForSelfExtract=true \
  /p:PublishTrimmed=true \
  /p:EnableCompressionInSingleFile=true \
  /p:DebuggerSupport=false \
  /p:DebugType=None \
  -o ./publish/win-x64
```

**Linux (x64)** :
```bash
dotnet publish -c Release \
  -r linux-x64 \
  --self-contained true \
  /p:PublishSingleFile=true \
  /p:IncludeNativeLibrariesForSelfExtract=true \
  /p:PublishTrimmed=true \
  /p:EnableCompressionInSingleFile=true \
  /p:DebuggerSupport=false \
  /p:DebugType=None \
  -o ./publish/linux-x64
```

**macOS Intel (x64)** :
```bash
dotnet publish -c Release \
  -r osx-x64 \
  --self-contained true \
  /p:PublishSingleFile=true \
  /p:IncludeNativeLibrariesForSelfExtract=true \
  /p:PublishTrimmed=true \
  /p:EnableCompressionInSingleFile=true \
  /p:DebuggerSupport=false \
  /p:DebugType=None \
  -o ./publish/osx-x64
```

**macOS Apple Silicon (ARM64)** :
```bash
dotnet publish -c Release \
  -r osx-arm64 \
  --self-contained true \
  /p:PublishSingleFile=true \
  /p:IncludeNativeLibrariesForSelfExtract=true \
  /p:PublishTrimmed=true \
  /p:EnableCompressionInSingleFile=true \
  /p:DebuggerSupport=false \
  /p:DebugType=None \
  -o ./publish/osx-arm64
```

Les exécutables optimisés seront générés dans `./publish/<platform>/`

## 🤝 Contribution

Les contributions sont les bienvenues ! N'hésitez pas à :

1. **Fork** le projet
2. Créer une **branche** pour votre fonctionnalité (`git checkout -b feature/AmazingFeature`)
3. **Commit** vos changements (`git commit -m 'Add some AmazingFeature'`)
4. **Push** vers la branche (`git push origin feature/AmazingFeature`)
5. Ouvrir une **Pull Request**

### Idées de fonctionnalités futures

- [ ] Paramètres Julia personnalisables via l'interface
- [ ] Plus de schémas de couleurs
- [ ] Export en résolutions personnalisées
- [ ] Animation de zoom
- [ ] Sauvegarde/chargement de positions favorites
- [ ] Support du zoom à la molette
- [ ] Drag & drop pour déplacer la vue
- [ ] Mode "deep zoom" avec précision arbitraire

## 📄 Licence

Ce projet est sous licence MIT - voir le fichier `LICENSE` pour plus de détails.

## 🙏 Remerciements

- **Avalonia Team** pour le framework UI multiplateforme
- **Communauté .NET** pour l'écosystème riche
- **Benoit Mandelbrot** pour la découverte de ces magnifiques fractales

---

**Développé avec ❤️ en C# et Avalonia**

*Explorez l'infini, une fractale à la fois.*
