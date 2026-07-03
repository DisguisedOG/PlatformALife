import os

filepath = r'd:\KiwiKing Studios\SimPixelz\Scripts\WorldGenerator.cs'

with open(filepath, 'r') as f:
    content = f.read()

# 1. Ramp (Staircase) loop
old_ramp_loop = """                for (int i = 0; i < segmentLength; i++)
                {
                    TerrainLayer.SetCell(new Vector2I(currentX + i, currentY), 0, PLATFORM_ATLAS_POS);
                    for (int fy = currentY + 1; fy <= SurfaceLevel; fy++)
                    {
                        TerrainLayer.SetCell(new Vector2I(currentX + i, fy), 0, FLOOR_ATLAS_POS);
                    }
                    if (i % 2 == 0) currentY += yDir;
                    currentY = Mathf.Clamp(currentY, SurfaceLevel - 6, SurfaceLevel - 2);
                }"""

new_ramp_loop = """                for (int i = 0; i < segmentLength; i++)
                {
                    int bRow = PLATFORM_ATLAS_POS.Y;
                    Vector2I platformTile = new Vector2I(1, bRow);
                    if (i == 0) platformTile = new Vector2I(0, bRow);
                    else if (i == segmentLength - 1) platformTile = new Vector2I(2, bRow);

                    TerrainLayer.SetCell(new Vector2I(currentX + i, currentY), 0, platformTile);
                    for (int fy = currentY + 1; fy <= SurfaceLevel; fy++)
                    {
                        TerrainLayer.SetCell(new Vector2I(currentX + i, fy), 0, FLOOR_ATLAS_POS);
                    }
                    if (i % 2 == 0) currentY += yDir;
                    currentY = Mathf.Clamp(currentY, SurfaceLevel - 6, SurfaceLevel - 2);
                }"""

if old_ramp_loop in content:
    content = content.replace(old_ramp_loop, new_ramp_loop)
else:
    print("Warning: Could not find old ramp loop block.")

# 2. GenerateUpperDeck loop
old_deck_loop = """        for (int i = 0; i < length; i++)
        {
            // Only generate the 1-way platform so players can jump through it and walk under it!
            TerrainLayer.SetCell(new Vector2I(startX + i, deckY), 0, PLATFORM_ATLAS_POS);
        }"""

new_deck_loop = """        int bRow = PLATFORM_ATLAS_POS.Y;
        for (int i = 0; i < length; i++)
        {
            Vector2I platformTile = new Vector2I(1, bRow);
            if (i == 0) platformTile = new Vector2I(0, bRow);
            else if (i == length - 1) platformTile = new Vector2I(2, bRow);
            
            // Only generate the 1-way platform so players can jump through it and walk under it!
            TerrainLayer.SetCell(new Vector2I(startX + i, deckY), 0, platformTile);
        }"""

if old_deck_loop in content:
    content = content.replace(old_deck_loop, new_deck_loop)
else:
    print("Warning: Could not find old deck loop block.")

with open(filepath, 'w') as f:
    f.write(content)

print("Updated WorldGenerator.cs Ramp and UpperDeck for Item #2.")
