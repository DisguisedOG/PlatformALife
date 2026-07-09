extends SceneTree

func _init():
    var path = "res://Scenes/Level_Waiheke_01.tscn"
    var packed_scene = ResourceLoader.load(path)
    if not packed_scene:
        print("Failed to load scene")
        quit(1)
        return
        
    var root = packed_scene.instantiate()
    var tile_map = root.get_node("TileMapLayer")
    if not tile_map:
        print("TileMapLayer not found")
        quit(1)
        return
        
    # Paint ground (source ID 0)
    for x in range(-30, 31):
        for y in range(0, 10):
            var coord = Vector2i(1, 1) # typical dirt core
            if y == 0:
                coord = Vector2i(1, 0) # typical grass top
            
            tile_map.set_cell(Vector2i(x, y), 0, coord, 0)
    
    print("TileMapLayer populated with ", tile_map.get_used_rect())
    
    # Ensure StaticBody matches
    var col = root.get_node_or_null("StaticBody2D/CollisionPolygon2D")
    if col:
        # 30 tiles wide in each direction = 60 tiles * 32 = 1920
        var p = PackedVector2Array()
        p.push_back(Vector2(-30 * 32, 0))
        p.push_back(Vector2(31 * 32, 0))
        p.push_back(Vector2(31 * 32, 10 * 32))
        p.push_back(Vector2(-30 * 32, 10 * 32))
        col.polygon = p
    
    # Pack and save
    var new_scene = PackedScene.new()
    var result = new_scene.pack(root)
    if result == OK:
        ResourceSaver.save(new_scene, path)
        print("Successfully saved ", path)
    else:
        print("Failed to pack scene")
        
    quit(0)
