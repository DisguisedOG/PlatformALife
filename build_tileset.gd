extends SceneTree

func _init():
    var tileset = TileSet.new()
    tileset.tile_size = Vector2i(32, 32)
    
    # Create collision layer
    tileset.add_physics_layer()
    tileset.set_physics_layer_collision_layer(0, 1)
    tileset.set_physics_layer_collision_mask(0, 1)
    
    var atlas_texture = load("res://Assets/AorakiAtlas.png") as Texture2D
    var columns = int(atlas_texture.get_width() / 32)
    var rows = int(atlas_texture.get_height() / 32)

    var source = TileSetAtlasSource.new()
    source.texture = atlas_texture
    source.texture_region_size = Vector2i(32, 32)
    
    # Grass Platform (3x3)
    for x in range(3):
        for y in range(3):
            _create_tile_if_valid(source, Vector2i(x, y), columns, rows)
            if y == 0 and _has_tile(source, Vector2i(x, y)):
                var poly = PackedVector2Array([Vector2(-16, -16), Vector2(16, -16), Vector2(16, 16), Vector2(-16, 16)])
                source.set_tile_data(Vector2i(x, y), 0)
                var data = source.get_tile_data(Vector2i(x, y), 0)
                data.add_collision_polygon(0)
                data.set_collision_polygon_points(0, 0, poly)

    # Dirt Platform (3x3) starts at x=3
    for x in range(3, 5):
        for y in range(3):
            _create_tile_if_valid(source, Vector2i(x, y), columns, rows)
            if _has_tile(source, Vector2i(x, y)):
                var poly = PackedVector2Array([Vector2(-16, -16), Vector2(16, -16), Vector2(16, 16), Vector2(-16, 16)])
                var data = source.get_tile_data(Vector2i(x, y), 0)
                data.add_collision_polygon(0)
                data.set_collision_polygon_points(0, 0, poly)
            
    tileset.add_source(source, 0)
    
    ResourceSaver.save(tileset, "res://Assets/AorakiTileSet.tres")
    print("TileSet saved to res://Assets/AorakiTileSet.tres")
    
    quit()

func _create_tile_if_valid(source: TileSetAtlasSource, position: Vector2i, columns: int, rows: int) -> bool:
    if position.x < 0 or position.x >= columns or position.y < 0 or position.y >= rows:
        printerr("Skipping invalid atlas tile coordinate: %s" % position)
        return false
    source.create_tile(position)
    return true

func _has_tile(source: TileSetAtlasSource, position: Vector2i) -> bool:
    return source.has_tile(position)
