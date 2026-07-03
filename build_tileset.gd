extends SceneTree

func _init():
    var tileset = TileSet.new()
    tileset.tile_size = Vector2i(32, 32)
    
    # Create collision layer
    tileset.add_physics_layer()
    tileset.set_physics_layer_collision_layer(0, 1)
    tileset.set_physics_layer_collision_mask(0, 1)
    
    var source = TileSetAtlasSource.new()
    source.texture = load("res://Assets/AorakiAtlas.png")
    source.texture_region_size = Vector2i(32, 32)
    
    # Grass Platform (3x3)
    for x in range(3):
        for y in range(3):
            source.create_tile(Vector2i(x, y))
            # Top row has collision
            if y == 0:
                var poly = PackedVector2Array([Vector2(-16, -16), Vector2(16, -16), Vector2(16, 16), Vector2(-16, 16)])
                source.set_tile_data(Vector2i(x, y), 0)
                var data = source.get_tile_data(Vector2i(x, y), 0)
                data.add_collision_polygon(0)
                data.set_collision_polygon_points(0, 0, poly)
            # Center and Bottom rows are solid (if needed for walls, but old generator just needed top row for platforms, wait no, dirt uses solid)
    
    # Dirt Platform (3x3) starts at x=3
    for x in range(3, 6):
        for y in range(3):
            source.create_tile(Vector2i(x, y))
            # Solid block
            var poly = PackedVector2Array([Vector2(-16, -16), Vector2(16, -16), Vector2(16, 16), Vector2(-16, 16)])
            var data = source.get_tile_data(Vector2i(x, y), 0)
            data.add_collision_polygon(0)
            data.set_collision_polygon_points(0, 0, poly)
            
    tileset.add_source(source, 0)
    
    ResourceSaver.save(tileset, "res://Assets/AorakiTileSet.tres")
    print("TileSet saved to res://Assets/AorakiTileSet.tres")
    
    quit()
