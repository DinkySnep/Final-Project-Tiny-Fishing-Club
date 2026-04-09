# Fishing Game – Developer Goals & Objectives

## Core Goals

- Deliver a playable beta within 1 month
- Build a satisfying, repeatable gameplay loop
- Ensure the game supports passive / low-attention play
- Maintain a cohesive cozy, low-poly aesthetic

## Core Gameplay Objectives

- Implement casting + bite detection system
- Create rhythm-based hook minigame
- Create reeling control minigame
- Ensure smooth transition between gameplay phases

## Systems

- Fish generation system
  - Species
  - Rarity weighting
  - Weight ranges
- Inventory / collection tracking
- Save system (local)

## UI/UX

- Simple, readable interface
- Feedback for success/failure states
- Collection viewing screen

## Stretch Goals (if time allows)

- Currency system
- Shop / upgrade system
- Stat modifiers (luck, strength, etc.)

## Art Goals

- Low-poly style consistency
- Small set of reusable fish models
- Color variation system for diversity

## Technical Goals

- Stable performance
- Clean modular systems (minigames, inventory, fish data)
- Easy-to-expand architecture

Scene
├── GameManager
├── EventBus
├── Player
│ ├── Camera (First Person)
│ └── FishingRod (Model + Script)
├── Water
├── FishSystem
├── MinigameManager
├── UI
│ ├── Canvas
│ │ ├── CastButton
│ │ ├── DebugText (optional)
│ │ ├── RhythmMinigameUI (disabled)
│ │ └── ReelingMinigameUI (disabled)
