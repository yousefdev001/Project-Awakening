# 🏛️ Technical Architecture & Coding Standards

## 1. Core Principles
1. **Modular & Component-Based**: Each component must do one thing well. Avoid God-Objects (e.g., monolithic `GameManager`).
2. **Data-Driven Design**: Separate data definition from execution logic using `ScriptableObject`s (`ProfessionData`, `MonsterData`, `ItemData`, `SkillData`, `QuestData`).
3. **Event-Driven Decoupling**: Use C# events/actions to communicate between systems without tight coupling.
4. **Clean Code**: Meaningful names, XML comments for public APIs, explicit namespaces (`Awakening.Core`, `Awakening.Player`, `Awakening.Combat`, etc.).

---

## 2. Directory Conventions (`Assets/`)
```text
Assets/
├── Art/
├── Audio/
├── Materials/
├── Models/
├── Animations/
├── Prefabs/
├── Scenes/
├── ScriptableObjects/
│   ├── Professions/
│   ├── Monsters/
│   ├── Items/
│   └── Quests/
└── Scripts/
    ├── Core/
    ├── Input/
    ├── Player/
    ├── Combat/
    ├── Monsters/
    ├── Professions/
    ├── Inventory/
    ├── Equipment/
    ├── Items/
    ├── Quests/
    ├── NPC/
    ├── World/
    ├── Interaction/
    ├── UI/
    ├── Save/
    ├── Debug/
    └── Managers/
```

---

## 3. ScriptableObject Guidelines
- Data containers must NOT hold runtime state or mutable operational logic.
- Runtime state lives on MonoBehaviour components (e.g., `PlayerStats`, `MonsterHealth`).
- Data flows: `ScriptableObject Data` ➔ `Runtime Component` ➔ `Logic / State Machine`.

---

## 4. Definition of Done (DoD)
Before marking any Phase / Feature as complete:
- [ ] Code compiles without errors or warnings.
- [ ] No relevant console errors in Play Mode.
- [ ] Expected behavior functions accurately.
- [ ] Edge cases tested (null references, boundary values, zero health, etc.).
- [ ] Integrates cleanly with existing systems.
- [ ] Git commit created with clear conventional commit message.
- [ ] Walkthrough / Developer docs updated.
