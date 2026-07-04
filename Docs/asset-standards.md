# Asset Standards

## Tile Grid Standards
- Use either `32x32` or `64x64` pixel grids consistently across all tileset assets.
- Each sprite sheet should include metadata for tile size and atlas coordinates.
- Colliders should align to the grid to prevent half-cell collisions.
- Portal tiles should snap cleanly to adjacent ground tiles and preserve a full grid cell footprint.

## Portal Standards
- Portal collision areas must be defined using a `CollisionShape2D` that matches the visible portal width and height.
- Portal interaction regions should be an integral multiple of the chosen tile size.
- Use clear visual borders so players can immediately recognize portal entry points.
- Portals should always be placed on stable ground and not overlap other collider regions.

## Collider Metadata
- Store collider orientation and size in source data where possible.
- For Godot, ensure `TileMap` collision layers and shapes are set consistently.
- Use simple rectangles or capsules for moving interactions, and avoid overly complex polygons unless required.

## Recommended Process
1. Create asset art in the chosen grid size first.
2. Add collider and interaction metadata during import.
3. Validate the asset in Godot with a quick scene-level visual and collision test.
4. Save the asset with a naming convention that includes `32x32` or `64x64`.
