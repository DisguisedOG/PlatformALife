import os
import re

def update_tscn(filepath, tex_path, remove_nodes, sprite_name="Sprite2D", offset="Vector2(0, -16)"):
    with open(filepath, 'r') as f:
        content = f.read()
    
    # 1. Add texture resource if not exists
    if 'ext_resource type="Texture2D"' not in content:
        ext_res_pattern = re.compile(r'(\[ext_resource.*?\]\n)')
        matches = ext_res_pattern.findall(content)
        
        # Find next available id
        id_num = len(matches) + 2
        tex_id = f'{id_num}_tex'
        
        insert_idx = content.rfind('[ext_resource')
        if insert_idx != -1:
            end_idx = content.find('\n', insert_idx) + 1
            content = content[:end_idx] + f'[ext_resource type="Texture2D" path="{tex_path}" id="{tex_id}"]\n' + content[end_idx:]
        else:
            insert_idx = content.find('\n') + 1
            content = content[:insert_idx] + f'\n[ext_resource type="Texture2D" path="{tex_path}" id="{tex_id}"]\n' + content[insert_idx:]
    else:
        tex_id = re.search(r'\[ext_resource type="Texture2D".*?id="([^"]+)"\]', content).group(1)

    # 2. Remove specified nodes
    for node_name in remove_nodes:
        # Find node declaration
        node_decl = f'[node name="{node_name}"'
        start_idx = content.find(node_decl)
        if start_idx != -1:
            end_idx = content.find('[node', start_idx + 1)
            if end_idx == -1:
                end_idx = len(content)
            content = content[:start_idx] + content[end_idx:]

    # 3. Add Sprite2D node
    if f'[node name="{sprite_name}" type="Sprite2D" parent="."]' not in content:
        sprite_node = f'\n[node name="{sprite_name}" type="Sprite2D" parent="."]\ntexture = ExtResource("{tex_id}")\noffset = {offset}\n'
        content += sprite_node

    with open(filepath, 'w') as f:
        f.write(content)

# Define tasks
tasks = [
    (
        r'd:\KiwiKing Studios\PlatformALife\Scenes\Structure_House.tscn', 
        'res://Assets/Sprites/Structure_House.png', 
        ['ColorRect', 'Door', 'Roof'],
        'Sprite2D',
        'Vector2(0, -96)'
    ),
    (
        r'd:\KiwiKing Studios\PlatformALife\Scenes\Shopkeeper.tscn', 
        'res://Assets/Sprites/Shopkeeper.png', 
        ['ColorRect'],
        'Sprite2D',
        'Vector2(0, -48)'
    ),
    (
        r'd:\KiwiKing Studios\PlatformALife\Scenes\Structure_FishermansDock.tscn', 
        'res://Assets/Sprites/FishermansDock.png', 
        ['ColorRect'],
        'Sprite2D',
        'Vector2(0, -32)'
    ),
    (
        r'd:\KiwiKing Studios\PlatformALife\Scenes\ConstructionZone.tscn', 
        'res://Assets/Sprites/ConstructionZone.png', 
        ['ColorRect'],
        'Sprite2D',
        'Vector2(0, -24)'
    ),
    (
        r'd:\KiwiKing Studios\PlatformALife\Scenes\Portal.tscn', 
        'res://Assets/Sprites/Portal.png', 
        ['ColorRect'],
        'Sprite2D',
        'Vector2(0, -64)'
    ),
    (
        r'd:\KiwiKing Studios\PlatformALife\Scenes\ResourceNode.tscn', 
        'res://Assets/Sprites/Resource_Wood.png', 
        ['ColorRect'],
        'Sprite2D',
        'Vector2(0, -24)'
    )
]

for t in tasks:
    update_tscn(t[0], t[1], t[2], t[3], t[4])

print("Updated TSCN files successfully.")
