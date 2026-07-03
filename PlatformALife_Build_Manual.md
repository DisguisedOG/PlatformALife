# PlatformALife: Master Game Build & Architecture Manual

## 1. Project Overview & Architecture
* **Title:** PlatformALife
* **Genre:** 2D Pixel Platformer / RPG Life Simulator
* **Target Platforms:** PC (Windows) and Android.
* **Visual Style:** 2D Pixel Art.
* **Perspective:** 2D Side-Scroller (Overworld) mixed with Top-Down 2D (Minigames) and UI overlays.
* **Core Loop:** Gather resources -> Build town -> Fulfill jobs -> Level up stats/skills -> Adventure -> Expand.

## 2. Tech Stack & Development Environment (IDE Specs)
### Game Engine & Core Language
* **Engine:** Godot Engine 4.4+ (Provides native C# support for Android exports and excellent text-based scene architecture, making it ideal for AI-driven development).
* **Language:** C# (.NET 8+). Required for complex data structures, serialization, and managing the AI economy/job state machines.

### Networking & Multiplayer (2-Player P2P)
* **Network Architecture:** Zero-Config Peer-to-Peer (1 Host, 1 Client). No dedicated server.
* **Relay / NAT Punching:** Epic Online Services (EOS) or Steamworks SDK (PC only). Used to bypass manual port-forwarding via NAT hole punching and STUN/TURN relays.
* **API:** Godot's High-Level Multiplayer API (ENet). The Host PC holds authoritative control over world states, economy, and AI.

### Data Storage & Cloud Saves
* **Local Data Serialization:** JSON format. Save data is split into two distinct structures:
    * `WorldData.json`: Seed, town layouts, economy stats, resident NPCs.
    * `PlayerData.json`: PixelMan cosmetics, core physical stats.
* **Cloud Infrastructure:** Firebase or Microsoft PlayFab to sync player profiles seamlessly between PC and Android.

### Asset Production Integration
* **Graphics:** 2D Spritesheets packed with JSON metadata.
* **World Building:** Engine-native TileMap and TileSet nodes.
* **UI/Rendering:** Use Godot's Control nodes and procedural `_draw()` functions for minigame interfaces.

## 3. Initialization & UI Flow
### Main Menu
* **Options:** New World, Load Worlds, Options, Exit.

### World Generation
* **Input:** Seed input field or "Random" button.
* **Output:** Procedural generation of a 2D map based deterministically on the seed.

### Character Creator (PixelMan Designer)
Triggers upon creating a new world or joining a world without import permissions.
* **Gender:** Male / Female.
* **Cosmetics:** 3 Hair styles per gender, 4 Eye colors (Blue, Green, Brown, Black), 3 starter clothes items per slot per gender.
* **Starting Attributes:** Player allocates 3 initial skill points to Job Skills.

## 4. Character Systems & Stats
### Core Stats (Persistent across worlds)
* **Fitness:** Cardio, Sprint, Strength (Dictates speed, jump height, carry capacity).
* **Knowledge:** LifeLevel, ExpGain, JobExp (Dictates leveling efficiency).

### Job Skills (Locked to specific World Save)
* **Progression:** Max Level 100 per skill.
* **Active Job:** Player holds one active job at a time. Switching jobs requires a small in-game cash fee.

## 5. The Economy & Job Execution
The economy relies on a strict supply chain (e.g., Smiths buy from Miners). Jobs operate in three distinct view states:

### Overworld Gathering (2D Platformer View)
* **Miner:** Harvests ore/stone via a built "Mining Cave Entrance" (loads dedicated cave map).
* **Lumberjack:** Buys seeds from "Lumberjack Hut", plants them in the overworld, and chops mature trees.
* **Fisherman:** Interacts with "Fisherman's Dock" to enter a fishing map instance.

### Perspective Shift (Top-Down Minigames)
* **Chef:** Manages a pizzeria in a top-down instance. Gathers ingredients, preps, bakes, and serves NPC customers. Earns tips and a lump-sum payout upon "Clocking Out."

### UI Minigames (Menu-Based Overlays)
* **Blacksmith (Armorer/WeaponSmith):** Heat and timing minigames to forge weapons/armor.
* **Clothier:** Stitching/pattern minigames.
* **Doctor:** Triage minigames (CPR/Stitching) to revive downed players or NPCs.

## 6. Town Building & Real Estate
### The Deed Plan
* Player uses a "Deed Plan" item to claim land, name the town, and drop a starting structure.
* **Co-Op:** A joining friend can sign the Host's Deed or use their own Deed to build a sister town.

### Builder Profession
* Accesses a Blueprint Menu. Places holographic scaffolding in the 2D world.
* Player must deposit raw materials (wood, stone) into the scaffolding to finalize construction.

### Starting Blueprints
1. **Campfire:** Basic light/cooking.
2. **House:** Spawn point, storage.
3. **Mining Cave Entrance:** Instance generator for Miner job.
4. **Lumberjack Hut:** Vendor for tree seeds.
5. **Fisherman's Dock:** Instance generator for fishing zones.
6. **Living Complex:** Upgradable (1 to 5 dwellings). Allows NPCs to become permanent residents.

## 7. NPC AI State Machine
NPCs simulate a living multiplayer environment natively on the Host's machine.

### The Commute & Wild Maps
* **Wild Maps:** Instanced, off-screen zones where AI gatherers generate raw materials (prevents them from destroying the player's town).
* **Wandering & Trading:** NPCs commute from Wild Maps to the player's town. Players can press "Chat/Trade" to buy their gathered raw materials.
* **Day/Night Cycle:** Most NPCs leave town at night; a small random percentage stays.

### Job Monopolies & Eviction
* **Rule:** A Deed supports only one NPC per job type.
* **Player Authority:** If a player claims a job held by a resident NPC, a warning prompt appears. If accepted, the player takes the monopoly.
* **Eviction:** The fired NPC vacates their Living Complex, reverts to a roaming commuter, and re-enters the global AI job pool.

## 8. Combat & Adventuring (Action RPG)
Combat is real-time, fast-paced (MapleStory-style) featuring jump-attacks, AoE mobbing, a quick-slot hotbar, and damage numbers.

### The Hall of Champions
* **Classes:** Warrior (Melee/Tank), Archer (Ranged/Kite), Bandit (Fast/Stealth), Healer (Support).
* **Progression:** Players receive 3 starter skills. New skills are unlocked via ClassLEVEL and purchased with Cash from Class Trainers.

### AI Party System
* Players can hire AI Champions for a fee to fight alongside them in dungeons and overworld zones.

## 9. Medical & Death Mechanics
When a player or AI Champion reaches 0 HP, they enter a Downed State.

### Death Options
1. **Call for Doctor (SOS Beacon):** Player waits. An AI Doctor (or Player Doctor) navigates the 2D world to reach them. The Doctor completes a triage minigame to revive the player. The revived player keeps their inventory but pays a moderate medical fee.
2. **Force Respawn:** Player spawns at the Hospital. Incurs an "Expensive Fee" (massive Cash loss and potential ClassLEVEL EXP loss).

## 10. Co-Op Save Data Rules (Host Configurable)
When launching a multiplayer server, the Host toggles "Allow Character Import."
* **If New / No Import:** Joining friend uses PixelMan Designer to make a new character locked to that world.
* **If Import Allowed:**
    * **Transfers:** Core Stats (Fitness, Knowledge), Cosmetics.
    * **Does NOT Transfer:** Money, Job Skills, Inventory/Equipment. (Joining player always starts with $0, Level 1 jobs, and basic gear in a host's world to protect the local economy).
