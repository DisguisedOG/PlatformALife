# Sprint 01: NZ Platformer Alpha Build

## Sprint Goal
Deliver the first integrated build slice for PlatformALife by establishing core systems, team roles, and the foundational content required for the NZ platformer directive.

## Sprint Duration
2 weeks

## Sprint Objectives
1. Build an initial job and progression framework.
2. Stabilize core asset standards for tilesets, portals, and character visuals.
3. Wire local Ollama host support and project agent coordination.
4. Create foundational documentation and team directives.

## Tasks

### A. Core Systems (Lead Programmer)
- [x] Implement the 3-tier Job Advancement system (Lv 10, 30, 70) in the Player/Job state machine.
- [x] Add a persistent Ron creation lockout check and save-state enforcement.
- [ ] Build the base structure for Māori Warrior stance and pressure-impact damage.
- [ ] Ensure tile/portal code supports 32x32 and 64x64 grid standards.
- [ ] Add a simple testing checklist to each completed implementation.

### B. Art / Assets (Creative Director + GFX Director)
- [ ] Standardize tileset and portal assets on 32x32 or 64x64 grids.
- [ ] Define collider metadata requirements for portal and tileset assets.
- [ ] Draft detailed engine-ready prompt specs for:
  - Māori Warrior kōwhaiwhai-style armor and stance
  - Cuzzi grease-stained heavy-weight animation set
  - Portal sprites and tile borders matching the NZ theme
- [ ] Create or validate at least one prototype tileset asset with the standard grid.

### C. Audio (Audio Director)
- [ ] Define mechanic-linked SFX for:
  - Cuzzi "Crib Lunch" buff explosion
  - Ron RNG chaos/gold scatter events
  - Māori Warrior stance and impact feedback
- [ ] Provide a timing map that ties each SFX to game events in the mechanic timeline.

### D. Documentation & Coordination (Project Manager + Auditor)
- [x] Finalize and publish the GEM directive files under `docs/gems/`.
- [x] Create the Sprint 01 plan document in `docs/sprint-01.md`.
- [ ] Review the build manual and identify any missing project flow documentation.
- [ ] Record unresolved blockers or missing dependencies in a sprint issue list.

### E. Tooling / Integration
- [x] Confirm local Ollama host integration in `godot_ai.py` and save the host setting.
- [ ] Validate the patcher integration path and confirm the manifest workflow.
- [x] Create a simple `docs/agent-setup.md` or `.agent.md` note describing how to use the custom agent.

## Blockers
- Missing approved asset grid size for any new tileset or portal art.
- Unclear physics definitions for Māori Warrior stance or Cuzzi movement.
- Ollama host not reachable or not pulled models locally.

## Acceptance Criteria
- Code for the 3-tier job progression exists and is testable.
- Ron lockout is enforced with a saved state check.
- Asset standards are documented and one prototype tileset is defined.
- Local Ollama host persistence works and `godot_ai.py` can launch with the saved host.
- Team directives live in `docs/gems/` and `docs/sprint-01.md`.

## Notes
- Keep all work aligned with the NZ PLATFORMER directive.
- Use the PM to raise any dependency or input requests as a Blocker Alert.
- Deliver in small, reviewable increments rather than a single large implementation.
