import os

tres_path = r'd:\KiwiKing Studios\SimPixelz\Assets\AorakiTileSet.tres'

new_source = '[sub_resource type="TileSetAtlasSource" id="TileSetAtlasSource_aoraki"]\n'
new_source += 'texture = ExtResource("1_tex")\n'
new_source += 'texture_region_size = Vector2i(32, 32)\n'

for x in range(3):
    for y in range(12):
        new_source += f'{x}:{y}/0 = 0\n'
        new_source += f'{x}:{y}/0/physics_layer_0/polygon_0/points = PackedVector2Array(-16, -16, 16, -16, 16, 16, -16, 16)\n'
        new_source += f'{x}:{y}/0/physics_layer_0/polygon_0/one_way = true\n'
        new_source += f'{x}:{y}/0/physics_layer_0/polygon_0/one_way_margin = 1.0\n'

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
