# Space Shooter — Final Project

**Advanced Programming | Iran University of Science and Technology | Term 4042**

**Developers:** Roham Hadadi, Parsa Barooj
**Student ID:** **404521255-404521102**  
**Instructor:** Dr. Marzieh Maleki Majd

---

## Overview

`Space Shooter` is a 2D Windows Forms game written in C#. The player survives ten waves of enemies, collects coins, uses temporary power-ups, and spends persistent coins in shop.

The project is organized around object-oriented design: common game behavior lives in base classes, individual enemies own their movement and attack rules, and `GameEngine` coordinates gameplay state, collisions, rewards, and waves.

## Implemented Features

### Gameplay

- Movement through `KeyDown` / `KeyUp` with continuous held-key state (`WASD` and Arrow Keys)
- Player boundary checks: the ship cannot leave the visible game area
- Rate-limited shooting (`220 ms` base cooldown)
- Collision handling for player bullets, enemy bullets, enemy-body contact, coins, and power-ups
- `Timer`-based game loop at approximately 50 FPS
- HUD for score, coins, HP, lives, current wave, and temporary power-up timers

### Waves and Enemies

The game has **10 waves**. Each new wave increases enemy count, speed, HP, and enemy variety.

| Enemy | Behavior |
|---|---|
| Standard | Moves straight down without shooting |
| Scout | Moves downward with a zigzag pattern |
| Shooter | Fires downward at fixed intervals |
| Terrorist | Recalculates its direction every update and follows the player's current position |
| Heavy Tank | Final-wave heavy enemy with high HP and eight-direction shots |

### Coins and Power-Ups

- Silver coins are worth `1` coin; gold coins are worth `5` coins.
- Coin drop probability and gold-coin probability vary by enemy type and increase as waves progress.
- Power-ups replace a coin drop, so one defeated enemy produces one reward type.
- Heavy Tanks always drop a gold coin.

Power-ups:

- **Triple Shot** — fires three shots for 10 seconds
- **Shield** — blocks damage for 5 seconds
- **Health Pack** — restores 40 HP immediately
- **Fire Rate Boost** — reduces the cooldown to 100 ms for 10 seconds

### Shop and Persistent Progress

The shop contains four required items:

| Item | Effect |
|---|---|
| Red Eagle Skin | Changes the ship drawing to the Red Eagle design |
| Laser Bullets | Changes player bullets to a green laser style |
| Galaxy Background | Uses a galaxy-themed in-game background |
| Extra Life Pack | Consumable: grants one extra life for the next run, then is removed from inventory |

Progress is saved in `game_data.db` using **SQLite**:

- Total coins
- High score
- Owned cosmetic items
- Equipped skin, bullet style, and background theme
- Extra Life Pack inventory

### Audio and Options

- Looping embedded background music
- Separate sound effects for shots, explosions, coins, power-ups, and player damage
- Independent options for music and SFX
- Controls guide in the Options form

## Project Structure

```text
GameCore/
  AudioManager.cs      Audio playback and settings
  GameEngine.cs        Gameplay state, collisions, rewards, waves
  GameObject.cs        Abstract base for game entities
  GameSettings.cs      Shared constants and difficulty formulas
  WaveManager.cs       Wave composition and spawn scheduling

Data/
  Database.cs          SQLite persistence and player save model

Entities/
  Player.cs            Input-driven movement, shooting, power-ups, cosmetics
  Enemy.cs             Abstract enemy base
  Enemy*.cs            Concrete enemy behaviors
  Bullet.cs            Projectile behavior
  CoinDrop.cs          Coin behavior
  PowerUpDrop.cs       Power-up behavior

GameUI/
  FormMain.cs          Main menu
  FormGame.cs          Timer, input, rendering, HUD
  FormShop.cs          Shop UI and equipment/consumable purchases
  FormOptions.cs       Audio toggles and controls guide
  FormAbout.cs         Project information

Resources/
  *.wav                Embedded music and sound effects
```

## Requirements

- Windows
- .NET 8 SDK

## Running the Project
1. Download the project files or clone the repository.
2. Open `SpaceShooter.sln` in Visual Studio.
3. Let NuGet restore `Microsoft.Data.Sqlite` by building the project.
4. Run with **F5**.

## Controls

| Key | Action |
|---|---|
| Arrow Keys / `WASD` | Move ship |
| `Space` | Shoot |
| `Esc` | Pause / Resume |
