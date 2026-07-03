import os

filepath = r'd:\KiwiKing Studios\SimPixelz\Scripts\WorldGenerator.cs'

with open(filepath, 'r') as f:
    content = f.read()

old_foliage_logic = """                        else if (!hasConstructionZone && !forceFlat && rng.Randf() < 0.35f)
                        {
                            int objIdx = rng.RandiRange(1, 21);
                            var tex = ResourceLoader.Load<Texture2D>($"res://Assets/Tilesets/TryTileRealistic/PNG/Object_{objIdx}.png");"""

new_foliage_logic = """                        else if (!hasConstructionZone && !forceFlat && rng.Randf() < 0.35f)
                        {
                            string[] foliageNames = { "Foliage_Fern", "Foliage_Kauri", "Foliage_CabbageTree", "Foliage_Flax" };
                            string randomFoliage = foliageNames[rng.RandiRange(0, foliageNames.Length - 1)];
                            var tex = ResourceLoader.Load<Texture2D>($"res://Assets/Sprites/Foliage/{randomFoliage}.png");"""

if old_foliage_logic in content:
    content = content.replace(old_foliage_logic, new_foliage_logic)
else:
    print("Warning: Could not find old foliage logic.")

with open(filepath, 'w') as f:
    f.write(content)

print("Updated WorldGenerator.cs for Item #10 (Foliage).")
