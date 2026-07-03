using Godot;
using System;

public partial class FishingMapInstance : Node2D
{
    private readonly Vector2I PLATFORM_ATLAS_POS = new Vector2I(1, 0); // Grass Top Center
    private readonly Vector2I FLOOR_ATLAS_POS = new Vector2I(4, 1); // Dirt Center

    public override void _Ready()
    {
        var spawner = GetNodeOrNull<MultiplayerSpawner>("MultiplayerSpawner");
        if (spawner != null)
        {
            spawner.AddSpawnableScene("res://Scenes/PixelMan.tscn");
        }
        
        var bg = GetNodeOrNull<DynamicParallaxBackground>("ParallaxBackground");
        bg?.LoadBiome(3);

        var tileMap = GetNodeOrNull<TileMapLayer>("TileMapLayer");
        if (tileMap != null)
        {
            for (int i = -15; i < 20; i++)
            {
                tileMap.SetCell(new Vector2I(i, 0), 0, PLATFORM_ATLAS_POS); 
                for (int y = 1; y < 15; y++)
                {
                    tileMap.SetCell(new Vector2I(i, y), 0, FLOOR_ATLAS_POS); 
                }
            }
        }

        if (!Multiplayer.HasMultiplayerPeer() || Multiplayer.IsServer())
        {
            var playersNode = GetNodeOrNull<Node2D>("Players");
            if (playersNode != null)
            {
                var playerScene = ResourceLoader.Load<PackedScene>("res://Scenes/PixelMan.tscn");
                SpawnInitialPlayers(playersNode, playerScene);
            }
        }
    }

    private void SpawnInitialPlayers(Node2D playersNode, PackedScene playerScene)
    {
        var mpm = GetNodeOrNull<MultiplayerManager>("/root/MultiplayerManager");
        
        if (mpm != null && !string.IsNullOrEmpty(mpm.PendingJoinIp)) return;

        if (mpm != null && mpm.ConnectedPlayers.Count > 0)
        {
            foreach (long id in mpm.ConnectedPlayers)
            {
                var player = playerScene.Instantiate<PixelMan>();
                player.Name = id.ToString();
                player.GlobalPosition = new Vector2(0, -64);
                playersNode.AddChild(player, true);
            }
        }
        else
        {
            var player = playerScene.Instantiate<PixelMan>();
            player.Name = "1";
            player.GlobalPosition = new Vector2(0, -64);
            playersNode.AddChild(player, true);
        }
    }
}