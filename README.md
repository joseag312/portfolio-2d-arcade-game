# 🚀 [2D Space Shooter](https://github.com/joseag312/portfolio-2d-arcade-game)

👾 **2D space shooter game built with Godot Engine**

A surreal and absurd 2D shoot-em-up built with Godot 4 (C#).
Arcade-style space shooter where players battle waves of enemies, and collect power-ups to survive.
Designed as a portfolio piece to showcase game architecture, modular systems, and polish.

---

## ✨ Portfolio Focus

This project was designed not only as a game, but as a **demonstration of software engineering practices** applied to game development.  
Key highlights include:

### 🎯 Architecture & Design Patterns

- **Component-Based Design**: Ships, enemies, drops, and HUD features are built from modular components — easy to extend or reuse.
- **Separation of Concerns**: Level orchestration vs. system logic (keeps classes lean and focused).
- **Autoload Singletons**: Central managers handle global state and services, applying the **Service Locator** and **Singleton** patterns with a focus on the Single-Responsibility Principle.
- **Signal/Event-Driven Flow**: Loose coupling between gameplay elements through Godot signals and async flows, following the **Observer pattern**.
- **Catalogs & Data Resources**: Weapons and levels defined externally as resources, applying **Repository** and **Data-Driven Design** patterns.
- **Weapon State System**: Runtime weapon state wraps base data using the **Decorator pattern**.
- **Spawner System**: Enemy spawners function as a **Factory pattern**, instantiating enemies and bosses at runtime.
- **Input & AI Movement**: Encapsulated strategies for player vs. enemy movement, aligning with the **Strategy pattern**.
- **Level Flow Management**: Levels shift cleanly between intro, play, and completion states, inspired by the **State pattern**.

### 💾 Data Persistence

- **Save/Load Systems**: Weapon inventory, progression, and settings are serialized to user storage.
- **Schema Versioning**: Save data includes schema version checks to ensure compatibility with future changes.
- **Catalog-Driven Data**: Weapons and levels are defined as resources and loaded through catalogs for **data-driven design**.

### 🧰 Systems Engineering

- **Weapon System**: Unified runtime model manages cooldowns, upgrades, and overrides, all driven by base data.
- **Economy & Store**: Balanced currency system with upgrade purchasing, integrated into persistence.
- **Drop System**: Loot drops (currency, health) handled via reusable prefabs with collision-based pickup and despawn logic.
- **Dialog System**: Modular HUD dialog box supporting messages, fading popups, and choice prompts.

### 🎨 UX & Polish

- **HUD Systems**: Power cooldown overlays, animated floating damage/currency text, responsive settings menu.
- **Transitions**: Fades and load components for smooth scene swaps and polished presentation.
- **Mobile-Friendly Input**: Architecture supports joystick/buttons for mobile export.

### 🔧 Refactoring & Maintainability

- **Consistent Naming Conventions**: C# PascalCase properties, private `_camelCase` fields, constants, and error/debug logging standards.
- **Reusable Components**: Enemy behavior, spawners, and level flow can be swapped or extended.
- **Portfolio-Oriented Refactor**: Code was cleaned and reorganized to emphasize clarity, modularity, and readability for interviews.

---

## 🎮 **Features**

- **5 Playable Levels**  
  Progress through handcrafted stages featuring unique enemy waves and bosses.

- **Weapon System**  
  Modular weapons with upgrades, cooldown management, and visual HUD integration.

- **Economy & Store**  
  Earn in-game currency, spend in the store, and upgrade loadouts.

- **HUD & UI Systems**

  - Dialog system (choices, fading messages, prompts)
  - Power HUD with cooldown animations
  - Currency and life tracking
  - Game flow screens (game over, level complete, menu, settings)

- **Polished UX**

  - Fade-in menus
  - Settings with volume/mute, HUD opacity
  - Mobile-friendly input (touch/joystick ready for adaptation)

- **Extensible Architecture**  
  Every feature built with modular components:
  - Movement, hitboxes/hurtboxes, enemy drops, spawners
  - LevelFlowComponent system for reusability
  - Centralized autoloads (settings, music, SFX, game stats)

---

## 🛠️ Tech Stack

- **Engine**: Godot 4 (C#)
- **Language**: C# with Godot API
- **Tools**: Git, React (portfolio site)

---

## 📸 Screenshots

### 🎮 Gameplay

![Gameplay](screenshots/gameplay.png)

### 🛒 Store

![Store](screenshots/store.png)

### 🪐 Levels

![Levels](screenshots/levels.png)

### ⚙️ Settings

![Settings](screenshots/settings.png)

### 💬 Dialog

![Dialog](screenshots/dialog.png)

---
