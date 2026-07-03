using Godot;
using System;
public partial class test_coords : SceneTree {
    public override void _Initialize() {
        var tm = new TileMapLayer();
        var ts = new TileSet();
        ts.TileSize = new Vector2I(32, 32);
        tm.TileSet = ts;
        GD.Print("Cell (1, 1) MapToLocal: ", tm.MapToLocal(new Vector2I(1, 1)));
        
        var spr = new Sprite2D();
        var tex = new PlaceholderTexture2D();
        tex.Size = new Vector2(32, 32);
        spr.Texture = tex;
        spr.Position = new Vector2(100, 100);
        GD.Print("Sprite2D at 100,100 center is: ", spr.Position);
        
        Quit();
    }
}
