import os
from PIL import Image

assets_dir = r'd:\KiwiKing Studios\SimPixelz\Assets'
seasonal_dir = os.path.join(assets_dir, 'Tilesets', 'Seasonal Tilesets')

biomes = [
    '1 - Grassland',
    '2 - Autumn Forest',
    '3 - Tropics',
    '4 - Winter World'
]

# Correct coordinates from Pixel Adventure Terrain (16x16).png
# 0: Left Edge (x=3, y=1)
# 1: Top Center (x=4, y=1)
# 2: Right Edge (x=5, y=1)
# 3: Center Dirt (x=4, y=2)
# 4: Bottom Dirt (x=4, y=3)

tile_coords = [
    (3, 1),
    (4, 1),
    (5, 1),
    (4, 2),
    (4, 3)
]

atlas = Image.new('RGBA', (160, 128), (0, 0, 0, 0))

for row, biome in enumerate(biomes):
    img_path = os.path.join(seasonal_dir, biome, 'Terrain (16 x 16).png')
    if os.path.exists(img_path):
        src = Image.open(img_path).convert('RGBA')
        for col, (tx, ty) in enumerate(tile_coords):
            box = (tx * 16, ty * 16, (tx + 1) * 16, (ty + 1) * 16)
            tile = src.crop(box)
            tile_32 = tile.resize((32, 32), Image.NEAREST)
            atlas.paste(tile_32, (col * 32, row * 32))

out_path = os.path.join(assets_dir, 'AorakiAtlas.png')
atlas.save(out_path)
print(f"Saved AorakiAtlas.png to {out_path} with proper tiles!")
