# Build Log & Production Tracking

## 🎯 Goals & Milestones
- [x] Unblock World Generation overlap bug (Dynamic widths implemented)
- [x] Automate AI Art extraction to Godot TileSets (`process_ai_atlas.py`)
- [x] Complete transition to static, hand-crafted level architecture (LevelManager)
- [x] Define Regional World Map Architecture (NZ Regions mapped to MapleStory hubs)
- [x] Establish "Waiheke Island" as the official game start (Tutorial Hub)
- [ ] Construct complete hand-crafted layout for `Level_Waiheke_01.tscn` (Maple Island)
- [ ] Construct complete hand-crafted layout for `Level_Whangarei_Heads_01.tscn` (Perion/Highlands)
- [ ] Construct hand-crafted layout for Auckland (Lith Harbor)
- [ ] Construct hand-crafted layout for Hamilton (Henesys)
- [ ] Construct hand-crafted layout for Wellington (Orbis)

## 📋 Immediate TODOs
- [ ] Paint the new `Level_Highlands_01.tscn` using `AorakiTileSet.tres`.
- [ ] Verify if the AI-generated player/shopkeeper sprites require a similar Python auto-packing script.

---

## 🛠️ Completion Log

### Date: July 9, 2026 (Phase 3)
**Role:** UI/UX & GFX Designer
**What was completed:**
1. **Resolution Upgrade:** Updated `project.godot` to force a 1920x1080 canvas resolution with `stretch/mode = "viewport"` and `stretch/aspect = "keep"` to guarantee pixel art scales perfectly on full HD screens.
2. **GameplayHUD Reconstruction:** Restructured `Scenes/GameplayHUD.tscn`. The main `HUDControl` was shifted away from full-screen stretch to a dedicated 72px-high bottom dock (`offset_top = -72.0`, `anchor_top = 1.0`, `anchor_bottom = 1.0`).
3. **Region Rect Alignment:** Locked the HUD `MainPanel`'s `region_rect` back to cleanly match the exact coordinates of our UI slice (`Rect2(0, 55, 1024, 145)`).

**Why it was done:**
User requested a clean 1080p canvas and fixing the HUD layout which was stretching across the whole screen.

**Technical Hurdles Discovered:**
- Migrating the HUD required adjusting anchors and minimum height carefully so Godot's canvas layer handles bottom docking properly without distorting nine-patch margins.

### Date: July 9, 2026 (Phase 2)
**Role:** Lead Game Developer & PM
**What was completed:**
1. **Architectural Pivot:** Deprecated and deleted the procedural `WorldGenerator.cs` script entirely in favor of a new `LevelManager.cs`. The game now statically loads pre-made `.tscn` level files.
2. **Scene Surgery:** Stripped the `TileMapLayer` and `WorldGenerator` from `Scenes/World.tscn` and installed the `LevelManager` node with a dedicated `LevelContainer`.
3. **Placeholder Generation:** Created `res://Scenes/Level_Highlands_01.tscn` with an assigned `TileMapLayer` (`AorakiTileSet.tres`) and a basic StaticBody2D physics boundary to act as the starting canvas.
4. **Automated Cleanup:** Wrote and executed `Tools/cleanup_procedural_files.py` to recursively destroy deprecated generator logic, `ChunkInstantiator.cs`, and orphaned python scripts to keep the repository clean.

**Why it was done:**
User mandate to move from procedural generation to static, hand-crafted maps to allow for more precise control and beginner-friendly level design workflows.

**Technical Hurdles Discovered:**
- Swapping the root geometry logic required carefully extracting the MultiplayerSpawner logic that was historically coupled to the procedural generator and re-housing it inside `LevelManager.cs`.

### Date: July 9, 2026 (Phase 1)
**Role:** Lead Game Developer & PM
**What was completed:**
1. **WorldGenerator.cs Refactor:** Fixed the looping/overlap bug in the procedural chunk generation. `BuildChunkGeometry` now calculates and returns dynamic chunk widths via a C# Tuple `(int ExitHeight, int ChunkWidth)`, allowing `GenerateWorld` to accurately offset the next chunk's start position (`currentTileX`).
2. **Invisible Floor Fix (Highlands):** Addressed the invisible floor bug in the Highlands biome by updating `build_tileset.gd` to iterate through all 4 rows (`y=3`), ensuring Godot actually allocates memory for the deeper biome tiles.
3. **Asset Pipeline Automation:** Wrote `process_ai_atlas.py` to automatically ingest raw, massive AI concept art (`AorakiAtlas.jpeg`), perfectly extract the relevant grass/dirt tiles, scrub out magenta backgrounds, and stitch them into a rigid 160x128 grid for Godot to consume.

**Why it was done:**
Chunks were failing to offset correctly because `ChunkWidthInTiles` was hardcoded globally instead of dynamically scaling per chunk type (e.g. Friction vs Verticality). The tileset pipeline was breaking because AI output is too chaotic for direct engine ingestion without a programmatic packing script.

**Technical Hurdles Discovered:**
- Godot 4.7 Mono C# integration: Overwriting the massive 500+ line `WorldGenerator.cs` with `.gd` files was a hazard. C# logic had to be surgically patched in-place.
- The previous tileset builder stopped at `y=2`, meaning any biome assigned to `y=3` (like Highlands) silently failed to render.
