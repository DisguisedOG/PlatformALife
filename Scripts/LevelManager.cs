using Godot;
using System;

public partial class LevelManager : Node
{
    [Export]
    public string InitialLevelPath { get; set; } = "res://Scenes/Level_Waiheke_01.tscn";

    public static LevelManager Instance { get; private set; }

    // Regional Map Dictionary (New Zealand to MapleStory mapping)
    public readonly System.Collections.Generic.Dictionary<string, string> RegionalZones = new()
    {
        { "Waiheke Island", "Maple Island / Tutorial" },
        { "Auckland", "Lith Harbor / Port" },
        { "Hamilton", "Henesys / Grasslands" },
        { "Whangarei Heads", "Perion / Highlands" },
        { "Wellington", "Orbis / Sky Hub" }
    };

    private Node2D _currentLevel;
    private Node2D _levelContainer;
    private MultiplayerSpawner _spawner;
    private Node2D _playersNode;
    
    public GameplayHUD HUD { get; private set; }

    public override void _Ready()
    {
        if (Instance == null) Instance = this;

        _levelContainer = GetNodeOrNull<Node2D>("../LevelContainer");
        if (_levelContainer == null)
        {
            GD.PushWarning("LevelManager could not find LevelContainer. Creating one dynamically.");
            _levelContainer = new Node2D();
            _levelContainer.Name = "LevelContainer";
            GetParent().CallDeferred("add_child", _levelContainer);
        }

        _spawner = GetNodeOrNull<MultiplayerSpawner>("../MultiplayerSpawner");
        if (_spawner != null)
        {
            _spawner.AddSpawnableScene("res://Scenes/PixelMan.tscn");
        }

        _playersNode = GetNodeOrNull<Node2D>("../Players");

        // Auto-instantiate the Gameplay HUD
        var hudScene = ResourceLoader.Load<PackedScene>("res://Scenes/GameplayHUD.tscn");
        if (hudScene != null)
        {
            HUD = hudScene.Instantiate<GameplayHUD>();
            GetParent().CallDeferred("add_child", HUD);
        }

        if (!string.IsNullOrEmpty(InitialLevelPath))
        {
            CallDeferred(nameof(LoadLevel), InitialLevelPath);
        }
        
        ScreenFader.Instance?.FadeIn();
    }

    public void LoadLevel(string scenePath)
    {
        if (_currentLevel != null)
        {
            _currentLevel.QueueFree();
            _currentLevel = null;
        }

        if (!ResourceLoader.Exists(scenePath))
        {
            GD.PushError($"Level not found at {scenePath}");
            return;
        }

        PackedScene levelScene = GD.Load<PackedScene>(scenePath);
        _currentLevel = levelScene.Instantiate<Node2D>();
        
        if (_levelContainer != null)
        {
            _levelContainer.AddChild(_currentLevel);
        }
        else
        {
            GetParent().AddChild(_currentLevel);
        }

        SpawnInitialPlayers();
    }

    private void SpawnInitialPlayers()
    {
        if (_playersNode == null) return;
        
        if (!Multiplayer.HasMultiplayerPeer() || Multiplayer.IsServer())
        {
            var playerScene = ResourceLoader.Load<PackedScene>("res://Scenes/PixelMan.tscn");
            
            var mpm = GetNodeOrNull<Node>("/root/MultiplayerManager");
            if (mpm != null)
            {
                var pendingJoinIp = (string)mpm.Get("PendingJoinIp");
                if (!string.IsNullOrEmpty(pendingJoinIp)) return;
                
                var connectedPlayers = (Godot.Collections.Array)mpm.Get("ConnectedPlayers");
                if (connectedPlayers != null && connectedPlayers.Count > 0)
                {
                    foreach (long id in connectedPlayers)
                    {
                        var player = playerScene.Instantiate<Node2D>();
                        player.Name = id.ToString();
                        player.GlobalPosition = new Vector2(0, -64);
                        _playersNode.AddChild(player, true);
                    }
                    return;
                }
            }
            
            var singlePlayer = playerScene.Instantiate<Node2D>();
            singlePlayer.Name = "1";
            singlePlayer.GlobalPosition = new Vector2(0, -64);
            _playersNode.AddChild(singlePlayer, true);
        }
    }

    // --- Legacy API support for other scripts ---
    public int MapWidth = 200;
    public int SurfaceLevel = 20;

    public Vector2 GetSpawnPosition(int spawnDirection)
    {
        // Simple default spawn position for static levels
        return new Vector2(0, -64);
    }

    public void SpawnStructure(string type, Vector2 pos)
    {
        // Handle structure spawning here later if needed
    }

    public void ClearMap()
    {
        if (_currentLevel != null)
        {
            _currentLevel.QueueFree();
            _currentLevel = null;
        }
    }
}
