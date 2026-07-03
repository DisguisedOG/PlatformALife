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

# We want 5 columns per biome (row):
# 0: Left Edge (x=6, y=0) in 16x16 grid
# 1: Top Center (x=7, y=0)
# 2: Right Edge (x=8, y=0)
# 3: Center Dirt (x=7, y=1)
# 4: Bottom Dirt (x=7, y=2)

tile_coords = [
    (6, 0),
    (7, 0),
    (8, 0),
    (7, 1),
    (7, 2)
]

# Create new atlas: 5 tiles wide (5 * 32 = 160), 4 biomes high (4 * 32 = 128)
atlas = Image.new('RGBA', (160, 128), (0, 0, 0, 0))

for row, biome in enumerate(biomes):
    img_path = os.path.join(seasonal_dir, biome, 'Terrain (16 x 16).png')
    if os.path.exists(img_path):
        src = Image.open(img_path).convert('RGBA')
        for col, (tx, ty) in enumerate(tile_coords):
            # Crop the 16x16 tile
            box = (tx * 16, ty * 16, (tx + 1) * 16, (ty + 1) * 16)
            tile = src.crop(box)
            # Upscale to 32x32 using nearest neighbor (pixel art)
            tile_32 = tile.resize((32, 32), Image.NEAREST)
            # Paste into atlas
            atlas.paste(tile_32, (col * 32, row * 32))

out_path = os.path.join(assets_dir, 'AorakiAtlas.png')
atlas.save(out_path)
print(f"Saved AorakiAtlas.png to {out_path}")

# Now update AorakiTileSet.tres
tres_path = os.path.join(assets_dir, 'AorakiTileSet.tres')

new_source = '[sub_resource type="TileSetAtlasSource" id="TileSetAtlasSource_aoraki"]\n'
new_source += 'texture = ExtResource("1_tex")\n'
new_source += 'texture_region_size = Vector2i(32, 32)\n'

for x in range(5):
    for y in range(4):
        new_source += f'{x}:{y}/0 = 0\n'
        new_source += f'{x}:{y}/0/physics_layer_0/polygon_0/points = PackedVector2Array(-16, -16, 16, -16, 16, 16, -16, 16)\n'
        # Optional: one-way platforms? No, these are solid terrain. Wait, earlier script had one_way = true.
        # Let's keep it solid. If they were one_way, players might fall through if pressing down, but WorldGenerator might rely on it.
        # We'll just make them solid (one_way = false) unless we need them to be one_way.
        new_source += f'{x}:{y}/0/physics_layer_0/polygon_0/one_way = false\n'

new_source += '\n[resource]\n'
new_source += 'tile_size = Vector2i(32, 32)\n'
new_source += 'physics_layer_0/collision_layer = 1\n'
new_source += 'sources/0 = SubResource("TileSetAtlasSource_aoraki")\n'

full_file = '[gd_resource type="TileSet" format=3 uid="uid://c3q8f8x33abc"]\n\n'
full_file += '[ext_resource type="Texture2D" path="res://Assets/AorakiAtlas.png" id="1_tex"]\n\n'
full_file += new_source

with open(tres_path, 'w') as f:
    f.write(full_file)

print('Updated AorakiTileSet.tres successfully.')
