# 🗡️ Project Awakening

> **3D Offline Third-Person Open-World Action RPG**  
> Built with Unity 6 & C#

---

## 🎯 Project Overview & Philosophy

Project Awakening is built modularly following a **Data-Driven Architecture** and strict **Vibe Coding** principles:
- One feature at a time.
- Decoupled logic from data using ScriptableObjects.
- Reusable component architectures over monolithic managers.
- Target first milestone: **A complete MVP Vertical Slice** from Character Creation & Awakening to Boss defeat and Save/Load.

---

## 🏗️ 28-Phase Implementation Roadmap

### Stage A — Project Foundation
- **Phase 0**: Project Setup (Unity 6, Git, Folder Structure, Docs)
- **Phase 1**: Input System (New Input System Actions)
- **Phase 2**: Player Movement (CharacterController, Physics, Locomotion)
- **Phase 3**: Third-Person Camera (Orbit, Zoom, Collision)
- **Phase 4**: Character & Animation (Animator States, Blend Trees)

### Stage B — RPG Core
- **Phase 5**: Core Game State & Flow
- **Phase 6**: Player Stats System
- **Phase 7**: Health, Damage & Death System
- **Phase 8**: XP & Level Progression (Level 1–10 for MVP)

### Stage C — Profession & Awakening
- **Phase 9**: Profession Data Architecture (ScriptableObjects)
- **Phase 10**: Profession Randomizer (C: 60%, B: 30%, A: 10%)
- **Phase 11**: Awakening Sequence & Visuals

### Stage D — Combat & Monsters
- **Phase 12**: Player Combat (Light/Heavy Attack, Dodge, Skills)
- **Phase 13**: Monster Base Architecture (Slime, Wolf, Goblin, etc.)
- **Phase 14**: Monster AI (State Machine: Idle, Chase, Attack, Hurt, Death)

### Stage E — Items & Progression
- **Phase 15**: Loot Table & Drop System
- **Phase 16**: Inventory System (Add, Remove, Stack, Sort, Use)
- **Phase 17**: Equipment & Affinity System

### Stage F — World & Interaction
- **Phase 18**: Generic Interaction System (`IInteractable`)
- **Phase 19**: NPC System (Dialogue, Quests)
- **Phase 20**: Quest System (Objectives, Tracking, Rewards)
- **Phase 21**: Village Prototype (NPCs, Spawn, Environment)
- **Phase 22**: Forest Zone (Terrain, Monster Spawners, Nest)

### Stage G — Endgame MVP Content
- **Phase 23**: Monster Nest (Waves, Goblins, Shaman)
- **Phase 24**: Boss Fight (Goblin Chief with Phase 1 & Enrage Phase 2)

### Stage H — Systems & Polish
- **Phase 25**: Save / Load System (JSON serialization)
- **Phase 26**: UI / HUD (HUD, Menus, Inventory UI)
- **Phase 27**: Audio, VFX & Camera Polish
- **Phase 28**: MVP QA & Playtest Loop Verification

---

## 🛠️ Tech Stack
- **Engine**: Unity 6
- **Language**: C#
- **Render Pipeline**: Universal Render Pipeline (URP)
- **IDE**: VS Code (C# Dev Kit + Unity Extension)
- **VCS**: Git + GitHub
