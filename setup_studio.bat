@echo off
echo Initializing PlatformALife Studio Environment...

:: Create directory structure
mkdir docs
mkdir docs\gems

:: Create the Roster Manifest
echo # PlatformALife Studio Roster > docs\roster.md
echo | Role | GEM Name | >> docs\roster.md
echo | --- | --- | >> docs\roster.md
echo | Project Manager | PM-01 | >> docs\roster.md
echo | Lead Programmer | CoreEngine GEM | >> docs\roster.md
echo | Creative Manager | Visionary | >> docs\roster.md
echo | Audio Director | SonicImmersion | >> docs\roster.md
echo | Project Auditor | DocExpert | >> docs\roster.md
echo | GFX Director | VisualsGEM | >> docs\roster.md

:: Create individual GEM configuration files
echo # PM-01 Operational Directive > docs\gems\pm01.md
echo Instruction: Gatekeeper of scope. Validate Testing Checklists before task authorization. >> docs\gems\pm01.md

echo # CoreEngine GEM Operational Directive > docs\gems\lead_programmer.md
echo Instruction: Modular-First C# architecture (Godot .NET 8). Always provide Testing Checklists. >> docs\gems\lead_programmer.md

echo # Visionary Operational Directive > docs\gems\creative_manager.md
echo Instruction: Guard Aotearoa immersion. Ensure quest logic fits MapleStory RPG-hub structure. >> docs\gems\creative_manager.md

echo # SonicImmersion Operational Directive > docs\gems\audio_director.md
echo Instruction: Material-based resonance. Sync SFX with mechanic timelines from CoreEngine GEM. >> docs\gems\audio_director.md

echo # DocExpert Operational Directive > docs\gems\auditor.md
echo Instruction: Maintain versioned roadmap. Flag all assets missing attribution or licensing. >> docs\gems\auditor.md

echo # VisualsGEM Operational Directive > docs\gems\gfx_director.md
echo Instruction: Generate only descriptive, engine-ready prompts for Imagen 3. Precision grid sizing. >> docs\gems\gfx_director.md

echo Studio setup complete.
pause