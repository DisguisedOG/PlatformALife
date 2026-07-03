using Godot;
using System;
using System.Collections.Generic;

public partial class DynamicParallaxBackground : ParallaxBackground
{
    private const float SCALE_FACTOR = 4.0f;
    private const float IMAGE_WIDTH = 288f;
    
    // We store arrays of texture paths in descending order of depth (furthest to closest).
    private readonly string[] plainsLayers = {
        "res://Assets/Tilesets/Seasonal Tilesets/1 - Grassland/Background parts/5 - Sky_color.png",
        "res://Assets/Tilesets/Seasonal Tilesets/1 - Grassland/Background parts/4 - Cloud_cover_2.png",
        "res://Assets/Tilesets/Seasonal Tilesets/1 - Grassland/Background parts/3 - Cloud_cover_1.png",
        "res://Assets/Tilesets/Seasonal Tilesets/1 - Grassland/Background parts/2 - Hills.png",
        "res://Assets/Tilesets/Seasonal Tilesets/1 - Grassland/Background parts/1 - Foreground_scenery.png"
    };

    private readonly string[] autumnLayers = {
        "res://Assets/Tilesets/Seasonal Tilesets/2 - Autumn Forest/Background parts/6 - Distant_trees.png",
        "res://Assets/Tilesets/Seasonal Tilesets/2 - Autumn Forest/Background parts/5 - Tree_row_BG_2.png",
        "res://Assets/Tilesets/Seasonal Tilesets/2 - Autumn Forest/Background parts/4 - Tree_row_BG_1.png",
        "res://Assets/Tilesets/Seasonal Tilesets/2 - Autumn Forest/Background parts/3 - Bottom_leaf_piles.png",
        "res://Assets/Tilesets/Seasonal Tilesets/2 - Autumn Forest/Background parts/2 - Trees.png",
        "res://Assets/Tilesets/Seasonal Tilesets/2 - Autumn Forest/Background parts/1 - Leaf_top.png"
    };

    private readonly string[] winterLayers = {
        "res://Assets/Tilesets/Seasonal Tilesets/4 - Winter World/Background parts/3 - Big_mountain_BG.png",
        "res://Assets/Tilesets/Seasonal Tilesets/4 - Winter World/Background parts/2 - Smaller_mountains.png",
        "res://Assets/Tilesets/Seasonal Tilesets/4 - Winter World/Background parts/1 - Snowy_foreground_area.png"
    };

    private readonly string[] tropicsLayers = {
        "res://Assets/Tilesets/Seasonal Tilesets/3 - Tropics/Background parts/5 - Sky_color.png",
        "res://Assets/Tilesets/Seasonal Tilesets/3 - Tropics/Background parts/4 - Background_clouds.png",
        "res://Assets/Tilesets/Seasonal Tilesets/3 - Tropics/Background parts/3 - Floating_clouds.png",
        "res://Assets/Tilesets/Seasonal Tilesets/3 - Tropics/Background parts/2 - Waters_version_2.png",
        "res://Assets/Tilesets/Seasonal Tilesets/3 - Tropics/Background parts/1 - Waters_version_1.png"
    };

    public void LoadBiome(int biomeType)
    {
        // 0 = Plains, 1 = Highlands, 2 = Canyons, 3 = Tropics (Custom)
        
        // Clear existing children
        foreach (Node child in GetChildren())
        {
            child.QueueFree();
        }

        string[] layersToLoad = plainsLayers;
        if (biomeType == 1) layersToLoad = winterLayers; // Highlands
        else if (biomeType == 2) layersToLoad = autumnLayers; // Canyons
        else if (biomeType == 3) layersToLoad = tropicsLayers; // Tropics (Fishing)

        // Add a solid sky color layer at the very back
        var skyLayer = new ParallaxLayer();
        skyLayer.MotionScale = new Vector2(0, 0);
        var skyRect = new ColorRect();
        skyRect.Size = new Vector2(4000, 4000);
        skyRect.Position = new Vector2(-2000, -2000);
        
        if (biomeType == 0) skyRect.Color = new Color("#87CEEB"); // Plains
        else if (biomeType == 1) skyRect.Color = new Color("#B0E0E6"); // Winter
        else if (biomeType == 2) skyRect.Color = new Color("#FFDAB9"); // Autumn
        else if (biomeType == 3) skyRect.Color = new Color("#4682B4"); // Tropics
        
        skyLayer.AddChild(skyRect);
        AddChild(skyLayer);

        float layerStep = 0.8f / (layersToLoad.Length > 1 ? (layersToLoad.Length - 1) : 1);

        for (int i = 0; i < layersToLoad.Length; i++)
        {
            float motionScale = (i == 0) ? 0.0f : (i * layerStep);
            
            var layer = new ParallaxLayer();
            layer.MotionScale = new Vector2(motionScale, 0.0f); // Lock Y so it never shows void
            layer.MotionMirroring = new Vector2(IMAGE_WIDTH * SCALE_FACTOR, 0);
            
            var sprite = new Sprite2D();
            sprite.Texture = GD.Load<Texture2D>(layersToLoad[i]);
            sprite.Centered = false;
            sprite.Scale = new Vector2(SCALE_FACTOR, SCALE_FACTOR);
            // Center vertically to cover the whole viewport
            sprite.Position = new Vector2(0, -150); 

            layer.AddChild(sprite);
            AddChild(layer);
        }
    }
}
