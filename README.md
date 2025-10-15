# 🎨 Fractals - Explorateur de l'ensemble de Mandelbrot

Une application interactive multi-plateforme pour explorer et générer l'ensemble de Mandelbrot en haute résolution, développée avec Avalonia UI et .NET 9.

![Fractals](https://img.shields.io/badge/.NET-9.0-512BD4?style=flat-square&logo=dotnet)
![Avalonia](https://img.shields.io/badge/Avalonia-11.3.7-8B44AC?style=flat-square)
![Platform](https://img.shields.io/badge/platform-Windows%20%7C%20Linux%20%7C%20macOS-lightgrey?style=flat-square)

## 📋 Table des matières

- [Fonctionnalités](#-fonctionnalités)
- [Captures d'écran](#-captures-décran)
- [Prérequis](#-prérequis)
- [Installation](#-installation)
- [Utilisation](#-utilisation)
- [Raccourcis clavier](#-raccourcis-clavier)
- [Qualités prédéfinies](#-qualités-prédéfinies)
- [Architecture technique](#-architecture-technique)
- [Compilation](#-compilation)
- [Contribution](#-contribution)
- [Licence](#-licence)

## ✨ Fonctionnalités

### 🔍 Exploration interactive
- **Zoom infini** : Explorez l'ensemble de Mandelbrot avec un zoom quasi-illimité
- **Navigation en temps réel** : Suivez les coordonnées complexes sous votre curseur
- **Zoom centré sur le curseur** : Zoomez directement sur la position de votre souris
- **Génération progressive** : Visualisez la fractale en cours de génération ligne par ligne

### 🎨 Rendu de qualité
- **5 niveaux de qualité** prédéfinis (de 720p à 8K)
- **Coloration lissée** (smooth coloring) pour des dégradés fluides
- **Algorithme optimisé** utilisant du code unsafe pour des performances maximales
- **Adaptation automatique** à la résolution de votre écran

### 💾 Export et personnalisation
- **Export PNG haute résolution** avec dialogue de sauvegarde
- **Nommage automatique** incluant la date, l'heure et la résolution
- **Interface masquable** pour des captures d'écran sans éléments d'UI
- **Mode plein écran** pour une immersion totale

### 📊 Informations en temps réel
- **Niveau de zoom** affiché avec précision
- **Coordonnées du centre** de la vue actuelle
- **Coordonnées sous le curseur** en temps réel
- **Statistiques de génération** :
  - Vitesse de génération (lignes/seconde)
  - Temps de génération total
  - Temps restant estimé
  - Résolution actuelle

### 📝 Logging complet
- **Fichiers journaux** automatiques dans `~/.local/share/Fractals/` (Linux) ou `%LOCALAPPDATA%\Fractals\` (Windows)
- **Horodatage précis** au milliseconde près
- **Traçabilité complète** des actions utilisateur et des générations

## 📸 Captures d'écran

*L'application affiche un panneau d'informations élégant avec fond semi-transparent sur fond noir, offrant toutes les informations nécessaires pour l'exploration.*

## 🔧 Prérequis

- **.NET 9.0 SDK** ou ultérieur
- **Système d'exploitation** : Windows, Linux ou macOS
- **Résolution recommandée** : 1920x1080 ou supérieure

## 📥 Installation

### Depuis les sources

1. **Clonez le dépôt** :
```bash
git clone https://github.com/votre-username/Fractals.git
cd Fractals
```

2. **Restaurez les dépendances** :
```bash
dotnet restore
```

3. **Compilez le projet** :
```bash
dotnet build
```

4. **Lancez l'application** :
```bash
dotnet run
```

### Compilation pour la distribution

**Windows (x64)** :
```bash
dotnet publish -c Release -r win-x64 --self-contained
```

**Linux (x64)** :
```bash
dotnet publish -c Release -r linux-x64 --self-contained
```

**macOS (ARM64)** :
```bash
dotnet publish -c Release -r osx-arm64 --self-contained
```

Les exécutables seront disponibles dans `bin/Release/net9.0/{runtime}/publish/`.

## 🎮 Utilisation

Au lancement, l'application affiche l'ensemble de Mandelbrot en plein écran avec un panneau d'informations sur la gauche.

### Navigation de base

1. **Déplacez votre souris** sur la fractale pour voir les coordonnées complexes
2. **Appuyez sur `+`** pour zoomer sur la position du curseur
3. **Appuyez sur `-`** pour dézoomer
4. **Appuyez sur `R`** pour réinitialiser la vue

### Export d'images

1. Naviguez vers la zone souhaitée
2. Choisissez la qualité désirée (touches `1-5`)
3. Attendez la fin de la génération
4. Appuyez sur `S` pour ouvrir le dialogue de sauvegarde
5. Choisissez l'emplacement et le nom du fichier

## ⌨️ Raccourcis clavier

| Touche | Action |
|--------|--------|
| `+` / `=` | Zoom x3 sur la position du curseur |
| `-` / `_` | Dézoom ÷3 sur la position du curseur |
| `1` | Qualité Rapide (1280x720, 150 itérations) |
| `2` | Qualité Normal (résolution écran, 300 itérations) |
| `3` | Qualité Haute (2560x1440, 500 itérations) |
| `4` | Qualité Ultra (3840x2160, 1000 itérations) |
| `5` | Qualité Extrême (7680x4320, 2000 itérations) |
| `N` | Régénérer la fractale actuelle |
| `R` | Réinitialiser complètement la vue |
| `C` | Mettre en pause / Reprendre la génération |
| `X` | Annuler la génération en cours (affiche l'image en RAM) |
| `Z` | Undo - Revenir à l'état précédent dans l'historique |
| `Y` | Redo - Avancer dans l'historique |
| `S` | Exporter l'image en PNG |
| `H` | Masquer/Afficher le panneau d'informations |
| `F11` | Basculer en mode plein écran |

## 🎯 Qualités prédéfinies

| Niveau | Nom | Résolution | Itérations max | Temps estimé* |
|--------|-----|------------|----------------|---------------|
| 1 | Rapide | 1280×720 | 150 | ~1s |
| 2 | Normal | Écran natif | 300 | ~3s |
| 3 | Haute | 2560×1440 | 500 | ~8s |
| 4 | Ultra | 3840×2160 | 1000 | ~25s |
| 5 | Extrême | 7680×4320 | 2000 | ~2min |

*_Les temps sont approximatifs et dépendent de votre processeur et du niveau de zoom._

### Choix de la qualité

- **Exploration rapide** : Utilisez les qualités 1-2 pour naviguer rapidement
- **Exports de qualité** : Utilisez les qualités 3-5 pour des images haute définition
- **Zoom extrême** : Plus vous zoomez, plus il faut d'itérations (qualités supérieures)

## 🏗️ Architecture technique

### Structure du projet

```
Fractals/
├── Program.cs              # Point d'entrée de l'application
├── App.axaml              # Configuration de l'application Avalonia
├── App.axaml.cs
├── MainWindow.axaml       # Fenêtre principale
├── MainWindow.axaml.cs
├── FractalView.axaml      # Vue de la fractale et panneau d'infos
├── FractalView.axaml.cs   # Logique de génération et d'interaction
├── Fractals.csproj        # Configuration du projet
└── app.manifest           # Manifeste Windows
```

### Technologies utilisées

- **Framework** : .NET 9.0
- **UI Framework** : Avalonia UI 11.3.7
- **Langage** : C# 12 avec code unsafe pour les performances
- **Rendu** : WriteableBitmap avec manipulation directe des pixels
- **Threading** : async/await avec Dispatcher pour l'UI

### Algorithme de génération

L'application utilise l'**algorithme d'échappement standard** pour l'ensemble de Mandelbrot :

```
Pour chaque pixel (px, py) :
    1. Convertir en coordonnées complexes (x0, y0)
    2. Itérer : z(n+1) = z(n)² + c
    3. Compter les itérations jusqu'à |z| > 2
    4. Appliquer le smooth coloring
    5. Convertir en couleur HSV → RGB
```

**Optimisations** :
- Code unsafe avec pointeurs pour accès direct aux pixels
- Génération ligne par ligne pour affichage progressif
- Calculs en double précision pour le zoom profond

### Système de coloration

L'application utilise un **algorithme de coloration lissée** (smooth coloring) pour éviter les bandes de couleur :

```csharp
smooth = iter + 1 - log(log(|z|)) / log(2)
hue = 360° × (smooth / maxIter × 3) mod 360°
```

Conversion **HSV → RGB** pour des dégradés arc-en-ciel fluides.

## 🔨 Compilation

### Configuration Release

```bash
dotnet build -c Release
```

### Optimisations activées

- `AllowUnsafeBlocks` : Autorise le code unsafe pour les performances
- `BuiltInComInteropSupport` : Support COM natif
- Compilation AOT possible pour démarrage plus rapide

### Débogage

Le mode Debug inclut Avalonia.Diagnostics pour l'inspection de l'UI en temps réel.

## 🤝 Contribution

Les contributions sont les bienvenues ! Voici comment vous pouvez aider :

1. **Fork** le projet
2. Créez une **branche** pour votre fonctionnalité (`git checkout -b feature/AmazingFeature`)
3. **Committez** vos changements (`git commit -m 'Add some AmazingFeature'`)
4. **Pushez** vers la branche (`git push origin feature/AmazingFeature`)
5. Ouvrez une **Pull Request**

### Idées d'améliorations

- [ ] Support d'autres fractales (Julia, Burning Ship, etc.)
- [ ] Palette de couleurs personnalisable
- [ ] Génération multi-threadée (parallélisation)
- [ ] Historique de navigation (undo/redo)
- [ ] Bookmarks pour sauvegarder des positions intéressantes
- [ ] Animation de zoom automatique
- [ ] Support GPU avec shaders (OpenGL/Vulkan)
- [ ] Mode vidéo pour exporter des animations

## 📄 Licence

Ce projet est distribué sous licence MIT. Voir le fichier `LICENSE` pour plus d'informations.

## 👤 Auteur

**Yoann**

## 🙏 Remerciements

- **Avalonia UI** pour le framework multi-plateforme
- **Benoît Mandelbrot** pour la découverte de cet ensemble mathématique fascinant
- La communauté open-source pour l'inspiration

---

### 📊 Statistiques du projet

- **Langage principal** : C#
- **Lignes de code** : ~600
- **Dépendances** : 4 packages NuGet
- **Plateformes supportées** : 3 (Windows, Linux, macOS)

### 🔗 Liens utiles

- [Documentation Avalonia](https://docs.avaloniaui.net/)
- [Ensemble de Mandelbrot - Wikipedia](https://fr.wikipedia.org/wiki/Ensemble_de_Mandelbrot)
- [.NET Documentation](https://docs.microsoft.com/dotnet/)

---

*Explorez l'infini mathématique, un zoom à la fois.* 🌌
