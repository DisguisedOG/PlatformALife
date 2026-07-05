using Godot;
using System;
using System.Collections.Generic;

public partial class DynamicParallaxBackground : Node2D
{
    private const float SCALE_FACTOR = 4.0f;
    private const float IMAGE_WIDTH = 288f;
    
    // We store arrays of texture paths in descending order of depth (furthest to closest).
    private readonly string[] productionLayers = {
        "res://Assets/Tilesets/Production Tiles/world_Master_Biome&city_tileset.png",
        "res://Assets/Tilesets/Production Tiles/Gemini_Generated_Image_au3h1vau3h1vau3h (1).png"
    };

    public void LoadBiome(int biomeType)
    {
        // 0 = Plains, 1 = Highlands, 2 = Canyons, 3 = Tropics (Custom)
        
        // Clear existing children
        foreach (Node child in GetChildren())
        {
            child.QueueFree();
        }

        string[] layersToLoad = productionLayers;

        // Add a solid sky color layer at the very back
        var skyLayer = new Parallax2D();
        skyLayer.ScrollScale = new Vector2(0, 0);
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
            
            var layer = new Parallax2D();
            layer.ScrollScale = new Vector2(motionScale, 0.0f); // Lock Y so it never shows void
            layer.RepeatSize = new Vector2(IMAGE_WIDTH * SCALE_FACTOR, 0);
            layer.RepeatTimes = 2;
            
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
