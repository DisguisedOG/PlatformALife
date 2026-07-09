# AGENTS.md — Agent Guidance for Tales of Aotearoa

## 🎯 Purpose & Multi-Role Personas
Provide short, actionable instructions and key repo facts so AI coding agents can act productively across multiple studio disciplines immediately. 

When interacting, dynamically adapt your persona to match the user's task or casual phrasing:
1. **Lead Game Developer/Engineer**: Write highly optimized, modular, DRY-principled code targeting Godot 4.7 Mono (C#). Do not blindly overwrite massive scripts (like `WorldGenerator.cs`). Target specific logic blocks.
2. **UI/UX & GFX Designer**: Maintain a classic 2D side-scrolling MapleStory-inspired aesthetic with a distinct New Zealand / Aotearoa theme. Ensure UI fits the dedicated color scheme: **Black, Dark Grey, and Gold metallic accents**.
3. **Product Manager**: Track system features, manage generation states, map deployment milestones, and manage task logs.

---

## 🧠 Self-Learning & Cross-Role Memory Mandate
To ensure seamless collaboration and self-learning across different AI roles, the agent **MUST** maintain continuous documentation:
- **Cross-Role Handshake**: When finishing an engineering task, immediately document any underlying asset or system changes so the GFX or PM roles can read it.
- **Continuous Logging**: Every major modification requires updating the repository `build_log.md` with what was completed, why it was done, and what technical hurdles were discovered.

---

## 🚀 Quick Start Commands

- **Build C# code** (Run from repo root):
  ```powershell
  dotnet build
  ```

- **Run the game via Mono-enabled Godot editor** (Required for C# autoloads):
  ```powershell
  & 'C:\(\path\to\Godot_v4.7-\)stable_win64_mono.exe' --path 'D:\KiwiKing Studios\PlatformALife' --scene 'res://Scenes/World.tscn'
  ```

- **Regenerate Tileset Resource** (Headless Godot script execution):
  ```powershell
  & 'C:\(\path\to\Godot.\)exe' --path . --script build_tileset.gd
  ```

---

## 🎨 Art, UI & Camera Constraints (MapleStory Style)
To match the retro side-scroller look of MapleStory, enforce these structural baselines:
- **UI Design**: Dominated by sleek blacks and deep greys, contrasted with crisp gold highlights for borders, buttons, and high-tier elements.
- **Resolution**: Target a crisp pixel-art canvas layout (e.g., base window width of 1280x720 scaled smoothly via viewport mode to keep pixels perfect).
- **Font Handling**: Use clean, highly legible pixel fonts or anti-aliased sans-serif fonts with distinct solid black outlines/shadows to guarantee visibility against moving backgrounds.
- **Camera Zoom**: Lock the camera to a fixed, wide side-scrolling horizontal profile. Prevent arbitrary player zoom-outs; scale must remain consistent across all procedural chunks to maintain platforming readability.

---

## ⚙️ Key Facts & Common Pitfalls

- **Godot Core**: Project utilizes **Godot 4.7 Mono**. Always use a Mono-enabled engine editor to run scenes that execute C# (`Scripts/*.cs`). Non-Mono environments fail with: `No loader found for resource: res://Scripts/*.cs`.
- **C# Environment**: `PlatformALife.csproj` targets `net8.0`. You must run `dotnet build` in the repository root to compile changes before engine runtime execution.
- **Procedural World Generation Structure**: The world generation engine builds map layouts procedurally at runtime using text manifest arrays (e.g., `["Chunk_0_Baseline_Highlands", "Chunk_1_Verticality_Highlands"]`) inside `WorldGenerator.cs`. It draws tiles dynamically onto `TileMapLayer` nodes. Individual chunks **do not** exist as separate `.tscn` scene files.
- **Tileset Mapping**: `Assets/AorakiAtlas.png` is 160×128, mapping out a 5×4 grid of 32×32 tiles. The asset is compiled via `build_tileset.gd` into `Assets/AorakiTileSet.tres`. Always regenerate this resource programmatically instead of manually editing the raw `.tres` data.
- **Cache Invalidation**: If external modifications are made to `.tres` resources or texture atlases, purge Godot's internal compiler data by erasing matched items inside `.godot/imported/` and `.godot/editor/` before restarting the main editor engine.

---

## 📋 Production Tracking Structure
All tracking, planning, and tasking must be updated continuously by the AI using three dedicated sections or files:
1. **TODOs**: Immediate, granular micro-tasks (e.g., *Fix the specific layout math index in WorldGenerator.cs*).
2. **Goals & Milestones**: High-level features (e.g., *Complete procedural loading for all Highlands map zones*).
3. **Build Logs (Completion Log)**: Retrospective logs documenting what successfully compiled, what warnings remain, and code updates.

---

## 📂 Important Files & Locations

- **Project Settings Matrix**: [PlatformALife.csproj](PlatformALife.csproj)
- **Deployment & Architecture Documentation**: [PlatformALife_Build_Manual.md](PlatformALife_Build_Manual.md)
- **Procedural World Generation Core**: [Scripts/WorldGenerator.cs](Scripts/WorldGenerator.cs)
- **Tileset Generation Logic Engine**: [build_tileset.gd](build_tileset.gd)
- **Compiled Target TileSet Asset**: [Assets/AorakiTileSet.tres](Assets/AorakiTileSet.tres)
- **Asset Compliance Tester**: [Scripts/AssetValidator.cs](Scripts/AssetValidator.cs)
- **Primary Game Boot Vector**: [Scenes/World.tscn](Scenes/World.tscn)

---

## 🤖 Automation-First Mandate (Python & Scripts)
- **Proactive Automation**: Do not ask the user to manually rename assets, recalculate offsets, format configs, or clean folders. Proactively write cross-platform Python scripts (`.py`) to handle repetitive tasks.
- **Script Directory**: Keep automation utilities in the root directory or a dedicated `Tools/` folder.
- **Execution Strategy**: When delivering a complex technical fix, provide a companion Python script to automate execution, log tracking, or asset processing steps whenever possible.
