# PlatformALife: Class & Job Specification Sheet

This document serves as the full specification sheet for all classes, jobs, and starter archetypes (Beneficiaries) in PlatformALife, specifically focusing on the NZ Platformer build.

## 1. NZ Platformer Starter Archetypes (Beneficiaries)
Beneficiaries are the initial archetype the player selects, determining their early available jobs, visual style, and unique mechanical quirks. 

### Māori Warrior
* **Role:** Pressure-based Melee Fighter.
* **Visuals / Art:** Features **kōwhaiwhai pattern themes** (traditional Māori motifs) on their armor.
* **Core Mechanics:**
  * **Tūwaewae Stance:** A specialized movement and combat stance imposing specific constraints.
  * **Pressure Impact Damage:** Combat relies on sustained pressure and impact feedback.
  * **Armor Penetration & Stagger:** Highly effective at breaking enemy defenses and staggering opponents.
* **Audio:** Resonant, material-based sound design tailored to stances and physical impacts.

### Cuzzi
* **Role:** Heavy-weight Buff Tank.
* **Visuals / Art:** Uses a grease-stained, heavy-weight animation set.
* **Core Mechanics:**
  * **"Crib Lunch" HP Buff Explosion:** A unique active skill where eating triggers a massive, explosive HP buff.
  * **Falcon BA Car-Cover:** A thematic defensive or utility mechanic unique to this beneficiary.
* **Audio:** Specific audio triggers tied directly to the pie-explosion buff mechanic and heavy footsteps.

### Ron
* **Role:** Unpredictable / RNG Chaos.
* **Visuals / Art:** Chaotic and randomized elements.
* **Core Mechanics:**
  * **RNG Chaos on Skill Use:** Skill outcomes are heavily randomized.
  * **Skill Failure Scatter:** Failing a skill results in radial scatter physics, potentially dropping items or causing chaos.
  * **+25% Gold Scatter Events:** Generates extra wealth but scatters it radially, requiring players to scramble to pick it up.
* **The "Ron Lockout" System:** Because Ron is highly disruptive to the game's economy, a strict **30-day real-time lockout** is enforced via persistent save checks. A player cannot create more than one Ron entity within a 30-day window.

---

## 2. Economy & Gathering Jobs
These jobs manage the town-building and resource loops. A player can hold one active job at a time and must pay an in-game cash fee to switch. Job monopolies can be claimed by evicting NPC residents.

* **Miner:** Operates in a 2D mining cave instance. Harvests ore and stone required by the Blacksmith and Builder.
* **Lumberjack:** Operates in the overworld. Purchases seeds from the Lumberjack Hut, plants them, and chops mature trees.
* **Fisherman:** Operates in dedicated fishing map instances accessible via the Fisherman's Dock.
* **Chef:** Operates in a top-down minigame perspective. Manages a pizzeria by gathering ingredients, baking, and serving NPC customers for tips.
* **Blacksmith (Armorer/WeaponSmith):** Operates via UI-based heat and timing minigames to forge weapons and armor. 
* **Clothier:** Operates via UI-based stitching and pattern minigames to craft clothing and gear.
* **Doctor:** A vital support job. Must physically navigate the 2D world to reach downed players/NPCs (SOS Beacons) and revive them using a triage minigame (CPR/Stitching).
* **Builder:** Responsible for town expansion. Accesses a Blueprint Menu to place holographic scaffolding in the 2D world, then deposits raw materials (wood/stone) to finalize construction.

---

## 3. Combat Classes (The Hall of Champions)
Action RPG classes used for adventuring, mobbing, and dungeon clearing. These are unlocked and progressed through Job Trainers. 

* **Warrior:** Melee / Tank focus. High survivability and close-range AoE mobbing.
* **Archer:** Ranged / Kite focus. High single-target and piercing damage from a distance.
* **Bandit:** Fast / Stealth focus. High mobility, quick attacks, and evasion.
* **Healer:** Support focus. Keeps the party alive during dungeons and overworld combat.

---

## 4. Progression System
All Jobs and Classes follow a strict 3-Tier advancement framework:
* **Starting Out:** Players receive 3 initial skill points.
* **Level 10:** Tier 1 Advancement unlock.
* **Level 30:** Tier 2 Advancement unlock.
* **Level 70:** Tier 3 Advancement unlock.
* **Level 100:** Maximum level per job. 

Skills are purchased with Cash from Job Trainers as the player levels up.
