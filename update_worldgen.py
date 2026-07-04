import os
import re

filepath = r'd:\KiwiKing Studios\PlatformALife\Scripts\WorldGenerator.cs'

with open(filepath, 'r') as f:
    content = f.read()

# 1. Add BiomeType.Tropics
content = content.replace(
    'public enum BiomeType { Plains, Highlands, Canyons }',
    'public enum BiomeType { Plains, Highlands, Canyons, Tropics }'
)

# 2. Update biome assignment logic (lines 163-186)
old_biome_logic = """        if (CurrentBiome == BiomeType.Plains)
        {
            PLATFORM_ATLAS_POS = new Vector2I(1, 0);
            FLOOR_ATLAS_POS = new Vector2I(1, 1);
            STONE_ATLAS_POS = new Vector2I(1, 2);
        }
        else if (CurrentBiome == BiomeType.Highlands)
        {
            PLATFORM_ATLAS_POS = new Vector2I(1, 3);
            FLOOR_ATLAS_POS = new Vector2I(1, 4);
            STONE_ATLAS_POS = new Vector2I(1, 5);
        }
        else if (CurrentBiome == BiomeType.Canyons)
        {
            PLATFORM_ATLAS_POS = new Vector2I(1, 6);
            FLOOR_ATLAS_POS = new Vector2I(1, 7);
            STONE_ATLAS_POS = new Vector2I(1, 8);
        }
        else // Tropics
        {
            PLATFORM_ATLAS_POS = new Vector2I(1, 9);
            FLOOR_ATLAS_POS = new Vector2I(1, 10);
            STONE_ATLAS_POS = new Vector2I(1, 11);
        }"""

new_biome_logic = """        int biomeRow = 0;
        if (CurrentBiome == BiomeType.Plains) biomeRow = 0;
        else if (CurrentBiome == BiomeType.Canyons) biomeRow = 1;
        else if (CurrentBiome == BiomeType.Tropics) biomeRow = 2;
        else if (CurrentBiome == BiomeType.Highlands) biomeRow = 3;

        PLATFORM_ATLAS_POS = new Vector2I(1, biomeRow); // Default center
        FLOOR_ATLAS_POS = new Vector2I(3, biomeRow); // Dirt fill
        STONE_ATLAS_POS = new Vector2I(4, biomeRow); // Deep fill"""

if old_biome_logic in content:
    content = content.replace(old_biome_logic, new_biome_logic)
else:
    print("Warning: Could not find old biome logic block.")

# 3. Update the Flat path loop to use left/right edges
old_flat_loop = """                for (int i = 0; i < segmentLength; i++)
                {
                    TerrainLayer.SetCell(new Vector2I(currentX + i, currentY), 0, PLATFORM_ATLAS_POS);
                    
                    for (int y = currentY + 1; y <= currentY + 4; y++)
                    {
                        TerrainLayer.SetCell(new Vector2I(currentX + i, y), 0, FLOOR_ATLAS_POS);
                    }"""

new_flat_loop = """                int biomeRow = (int)CurrentBiome; // Simplified since enum values 0-3 match our rows! Wait, let's use PLATFORM_ATLAS_POS.Y
                int bRow = PLATFORM_ATLAS_POS.Y;
                for (int i = 0; i < segmentLength; i++)
                {
                    Vector2I platformTile = new Vector2I(1, bRow);
                    if (i == 0) platformTile = new Vector2I(0, bRow);
                    else if (i == segmentLength - 1) platformTile = new Vector2I(2, bRow);
                    
                    TerrainLayer.SetCell(new Vector2I(currentX + i, currentY), 0, platformTile);
                    
                    for (int y = currentY + 1; y <= currentY + 4; y++)
                    {
                        TerrainLayer.SetCell(new Vector2I(currentX + i, y), 0, FLOOR_ATLAS_POS);
                    }"""

if old_flat_loop in content:
    content = content.replace(old_flat_loop, new_flat_loop)
else:
    print("Warning: Could not find old flat loop block.")

with open(filepath, 'w') as f:
    f.write(content)

print("Updated WorldGenerator.cs for Item #1 and #2.")
