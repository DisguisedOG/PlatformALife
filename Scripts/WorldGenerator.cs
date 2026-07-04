using Godot;
using System;
using System.Collections.Generic;
using System.Text.Json;

public partial class WorldGenerator : Node
{
    [Export]
    public TileMapLayer TerrainLayer { get; set; }

    [Export]
    public PackedScene PortalScene { get; set; } = ResourceLoader.Load<PackedScene>("res://Scenes/Portal.tscn");

    [Export]
    public PackedScene ResourceNodeScene { get; set; } = ResourceLoader.Load<PackedScene>("res://Scenes/ResourceNode.tscn");

    [Export]
    public PackedScene ShopkeeperScene { get; set; } = ResourceLoader.Load<PackedScene>("res://Scenes/Shopkeeper.tscn");

    [Export]
    public PackedScene FishingSpotScene { get; set; } = ResourceLoader.Load<PackedScene>("res://Scenes/FishingSpot.tscn");

    [Export]
    public PackedScene ConstructionZoneScene { get; set; } = ResourceLoader.Load<PackedScene>("res://Scenes/ConstructionZone.tscn");

    [Export]
    public PackedScene StructureHouseScene { get; set; } = ResourceLoader.Load<PackedScene>("res://Scenes/Structure_House.tscn");

    [Export]
    public PackedScene FishermansDockScene { get; set; } = ResourceLoader.Load<PackedScene>("res://Scenes/Structure_FishermansDock.tscn");

    [Export]
    public int MapWidth { get; set; } = 200;
    
    [Export]
    public int MapHeight { get; set; } = 100;

    [Export]
    public int SurfaceLevel { get; set; } = 20;

    [Export]
    public int MapWidthInChunks { get; set; } = 15;

    [Export]
    public int ChunkWidthInTiles { get; set; } = 16;

    public enum BiomeType { Plains, Highlands, Canyons, Tropics }
    public enum ChunkType
    {
        Baseline,
        Verticality,
        Friction
    }

    public BiomeType CurrentBiome { get; private set; }

    private RandomNumberGenerator rng;
    private Random _flowRng;
    private ChunkType _previousChunkType = ChunkType.Baseline;
    private Dictionary<string, object> _worldStateObject = new Dictionary<string, object>();
    private List<string> _chunkSequenceLog = new List<string>();
    
    private Vector2I PLATFORM_ATLAS_POS = new Vector2I(1, 0); 
    private Vector2I FLOOR_ATLAS_POS = new Vector2I(1, 1); 
    private Vector2I LADDER_ATLAS_POS = new Vector2I(1, 0); 
    private Vector2I STONE_ATLAS_POS = new Vector2I(1, 1); 
    
    private int laddersSpawned = 0;

    public override void _Ready()
    {
        GenerateWorld();
        
        var bg = GetNodeOrNull<DynamicParallaxBackground>("../ParallaxBackground");
        bg?.LoadBiome((int)CurrentBiome);
        
        ScreenFader.Instance?.FadeIn();
        AudioManager.Instance?.PlayWorldBGM();
    }

    private int GetDeterministicHashCode(string str)
    {
        unchecked
        {
            int hash = (int)2166136261;
            foreach (char c in str)
            {
                hash = (hash ^ c) * 16777619;
            }
            return hash;
        }
    }

    private BiomeType GetBiomeForMap(string baseSeed, int mapIdx)
    {
        string biomeSeedString = $"{baseSeed}_biome_{mapIdx}";
        var tempRng = new RandomNumberGenerator();
        tempRng.Seed = (ulong)GetDeterministicHashCode(biomeSeedString);
        
        float val = tempRng.Randf();
        if (val < 0.33f) return BiomeType.Plains;
        if (val < 0.66f) return BiomeType.Highlands;
        return BiomeType.Canyons;
    }

    private string GenerateMapName(string baseSeed, int mapIdx, BiomeType biome)
    {
        string[] plainsAdjectives = { "Breezy", "Quiet", "Green", "Sunny", "Gentle" };
        string[] plainsNouns = { "Plains", "Fields", "Meadow", "Pastures", "Valley" };
        
        string[] highlandsAdjectives = { "Rocky", "Steep", "Craggy", "Windy", "High" };
        string[] highlandsNouns = { "Highlands", "Peaks", "Cliffs", "Ridge", "Mount" };
        
        string[] canyonsAdjectives = { "Deep", "Echoing", "Dusty", "Perilous", "Jagged" };
        string[] canyonsNouns = { "Canyon", "Gorge", "Ravine", "Chasm", "Abyss" };
        
        string nameSeedString = $"{baseSeed}_name_{mapIdx}";
        var tempRng = new RandomNumberGenerator();
        tempRng.Seed = (ulong)GetDeterministicHashCode(nameSeedString);
        
        string baseName = "";
        if (biome == BiomeType.Plains)
        {
            baseName = $"{plainsAdjectives[tempRng.RandiRange(0, 4)]} {plainsNouns[tempRng.RandiRange(0, 4)]}";
        }
        else if (biome == BiomeType.Highlands)
        {
            baseName = $"{highlandsAdjectives[tempRng.RandiRange(0, 4)]} {highlandsNouns[tempRng.RandiRange(0, 4)]}";
        }
        else 
        {
            baseName = $"{canyonsAdjectives[tempRng.RandiRange(0, 4)]} {canyonsNouns[tempRng.RandiRange(0, 4)]}";
        }
        
        // Scan previous maps to calculate Roman Numeral
        int count = 0;
        for (int i = -5; i <= mapIdx; i++)
        {
            BiomeType iBiome = GetBiomeForMap(baseSeed, i);
            string iNameSeedString = $"{baseSeed}_name_{i}";
            var iRng = new RandomNumberGenerator();
            iRng.Seed = (ulong)GetDeterministicHashCode(iNameSeedString);
            
            string iName = "";
            if (iBiome == BiomeType.Plains) iName = $"{plainsAdjectives[iRng.RandiRange(0, 4)]} {plainsNouns[iRng.RandiRange(0, 4)]}";
            else if (iBiome == BiomeType.Highlands) iName = $"{highlandsAdjectives[iRng.RandiRange(0, 4)]} {highlandsNouns[iRng.RandiRange(0, 4)]}";
            else iName = $"{canyonsAdjectives[iRng.RandiRange(0, 4)]} {canyonsNouns[iRng.RandiRange(0, 4)]}";
            
            if (iName == baseName)
            {
                count++;
            }
        }
        
        string[] numerals = { "", "I", "II", "III", "IV", "V", "VI", "VII", "VIII", "IX", "X", "XI" };
        if (count > 0 && count < numerals.Length)
        {
            return $"{baseName} {numerals[count]}";
        }
        return baseName;
    }

    public void GenerateWorld()
    {
        if (TerrainLayer == null) return;

        string baseSeed = GameManager.Instance?.CurrentSeed ?? "DefaultSeed";
        int mapIndex = GameManager.Instance?.CurrentMapIndex ?? 0;
        int spawnDirection = GameManager.Instance?.SpawnDirection ?? 0;
        
        CurrentBiome = GetBiomeForMap(baseSeed, mapIndex);
        string mapName = GenerateMapName(baseSeed, mapIndex, CurrentBiome);
        
        var mapUI = GetNodeOrNull<MapUI>("../MapUI");
        if (mapUI != null)
        {
            mapUI.SetMapName(mapName);
        }

        // Set Dynamic Biome Tiles
        int biomeRow = 0;
        if (CurrentBiome == BiomeType.Plains) biomeRow = 0;
        else if (CurrentBiome == BiomeType.Canyons) biomeRow = 1;
        else if (CurrentBiome == BiomeType.Tropics) biomeRow = 2;
        else if (CurrentBiome == BiomeType.Highlands) biomeRow = 3;

        PLATFORM_ATLAS_POS = new Vector2I(1, biomeRow); // Default center
        FLOOR_ATLAS_POS = new Vector2I(3, biomeRow); // Dirt fill
        STONE_ATLAS_POS = new Vector2I(4, biomeRow); // Deep fill
        LADDER_ATLAS_POS = PLATFORM_ATLAS_POS; // Fallback


        string mapSeedString = $"{baseSeed}_{mapIndex}";
        int seed = GetDeterministicHashCode(mapSeedString);

        rng = new RandomNumberGenerator();
        rng.Seed = (ulong)seed;

        var spawner = GetNodeOrNull<MultiplayerSpawner>("../MultiplayerSpawner");
        if (spawner != null)
        {
            spawner.AddSpawnableScene("res://Scenes/PixelMan.tscn");
        }
        
        if (!Multiplayer.HasMultiplayerPeer() || Multiplayer.IsServer())
        {
            var playersNode = GetNodeOrNull<Node2D>("../Players");
            if (playersNode != null)
            {
                var playerScene = ResourceLoader.Load<PackedScene>("res://Scenes/PixelMan.tscn");
                SpawnInitialPlayers(playersNode, playerScene);
            }
        }

        laddersSpawned = 0;

        GD.Print($"Generating Map {mapIndex} ({mapName}) with seed: {mapSeedString}");

        TerrainLayer.Clear();

        // 1. Solid Bottom Boundary Floor
        for (int x = -MapWidth / 2; x < MapWidth / 2; x++)
        {
            TerrainLayer.SetCell(new Vector2I(x, SurfaceLevel), 0, FLOOR_ATLAS_POS);
            for (int y = SurfaceLevel + 1; y < MapHeight; y++)
            {
                if (y > SurfaceLevel + 15)
                    TerrainLayer.SetCell(new Vector2I(x, y), 0, STONE_ATLAS_POS);
                else
                    TerrainLayer.SetCell(new Vector2I(x, y), 0, FLOOR_ATLAS_POS);
            }
        }

        // 1.5 Solid Side Borders
        for (int y = -100; y < MapHeight; y++)
        {
            for (int w = 0; w < 10; w++)
            {
                // Left Border
                TerrainLayer.SetCell(new Vector2I(-MapWidth / 2 - w - 1, y), 0, FLOOR_ATLAS_POS);
                // Right Border
                TerrainLayer.SetCell(new Vector2I(MapWidth / 2 + w, y), 0, FLOOR_ATLAS_POS);
            }
        }

        // 2. Procedural flow pipeline for main path
        int currentTileX = -MapWidth / 2;
        int lastExitHeight = SurfaceLevel - 3;
        int chunkCount = Mathf.Max(1, Mathf.RoundToInt((float)MapWidth / Mathf.Max(1, ChunkWidthInTiles)) + 1);
        chunkCount = Mathf.Min(chunkCount, MapWidthInChunks > 0 ? MapWidthInChunks : chunkCount);

        _flowRng = new Random(GetDeterministicHashCode($"{baseSeed}_{mapIndex}_flow"));
        _worldStateObject.Clear();
        _chunkSequenceLog.Clear();
        _previousChunkType = ChunkType.Baseline;

        for (int i = 0; i < chunkCount; i++)
        {
            ChunkType nextChunk = ChooseFlowValidatedChunk(i);
            _chunkSequenceLog.Add($"Chunk_{i}_{nextChunk}_{CurrentBiome}");
            lastExitHeight = BuildChunkGeometry(nextChunk, CurrentBiome, currentTileX, lastExitHeight);
            currentTileX += ChunkWidthInTiles;
        }

        _worldStateObject["ChunkLayoutSequence"] = _chunkSequenceLog;
        _worldStateObject["GenerationTimestamp"] = Time.GetUnixTimeFromSystem();
        _worldStateObject["MapSeed"] = mapSeedString;
        _worldStateObject["Biome"] = CurrentBiome.ToString();
        GD.Print($"[WorldGenerator] Generation manifest complete: {JsonSerializer.Serialize(_worldStateObject)}");

        // 3. Setup Portals and Edge spawn logic
        float tilePixels = 32.0f;
        
        if (mapIndex > -5)
        {
            var leftPortal = PortalScene.Instantiate<Portal>();
            leftPortal.TargetMapIndex = mapIndex - 1;
            leftPortal.TravelDirection = 1; 
            int pY = FindFloorY(-MapWidth / 2 + 3);
            leftPortal.GlobalPosition = new Vector2((-MapWidth / 2 + 3) * tilePixels, pY * tilePixels);
            AddChild(leftPortal);
            AssetValidator.ValidatePortal(leftPortal);
        }

        if (mapIndex < 5)
        {
            var rightPortal = PortalScene.Instantiate<Portal>();
            rightPortal.TargetMapIndex = mapIndex + 1;
            rightPortal.TravelDirection = -1; 
            int pY = FindFloorY(MapWidth / 2 - 3);
            rightPortal.GlobalPosition = new Vector2((MapWidth / 2 - 3) * tilePixels, pY * tilePixels);
            AddChild(rightPortal);
            AssetValidator.ValidatePortal(rightPortal);
        }

        if (mapIndex == 0 && ShopkeeperScene != null)
        {
            var shopkeeper = ShopkeeperScene.Instantiate<Shopkeeper>();
            int pY = FindFloorY(0);
            shopkeeper.GlobalPosition = new Vector2(0, pY * tilePixels);
            AddChild(shopkeeper);

            if (FishermansDockScene != null)
            {
                var dock = FishermansDockScene.Instantiate<Node2D>();
                int dockY = FindFloorY(-5);
                dock.GlobalPosition = new Vector2(-5 * tilePixels, dockY * tilePixels);
                AddChild(dock);
            }
        }

        SpawnSavedStructures(mapIndex);
    }

    private ChunkType ChooseFlowValidatedChunk(int chunkIndex)
    {
        if (chunkIndex == 0)
        {
            _previousChunkType = ChunkType.Baseline;
            return ChunkType.Baseline;
        }

        if (_previousChunkType == ChunkType.Friction && _flowRng.Next(0, 100) < 60)
        {
            _previousChunkType = ChunkType.Baseline;
            return ChunkType.Baseline;
        }

        int roll = _flowRng.Next(0, 100);
        ChunkType chosen;
        if (roll < 50)
        {
            chosen = ChunkType.Baseline;
        }
        else if (roll < 85)
        {
            chosen = ChunkType.Verticality;
        }
        else
        {
            chosen = ChunkType.Friction;
        }

        _previousChunkType = chosen;
        return chosen;
    }

    private int BuildChunkGeometry(ChunkType type, BiomeType biome, int startX, int startY)
    {
        int localHeight = startY;
        int atlasRow = Mathf.Max(0, PLATFORM_ATLAS_POS.Y);

        for (int x = 0; x < ChunkWidthInTiles; x++)
        {
            int globalX = startX + x;
            Vector2I terrainTile = new Vector2I(1, atlasRow);
            Vector2I fillTile = new Vector2I(3, atlasRow);
            Vector2I deepTile = new Vector2I(4, atlasRow);

            switch (type)
            {
                case ChunkType.Baseline:
                    if (x % 4 == 0)
                    {
                        localHeight += _flowRng.Next(-1, 2);
                    }
                    localHeight = Mathf.Clamp(localHeight, SurfaceLevel - 6, SurfaceLevel - 1);
                    terrainTile = new Vector2I(1, atlasRow);
                    break;

                case ChunkType.Verticality:
                    if (x % 3 == 0)
                    {
                        localHeight -= 2;
                    }
                    localHeight = Mathf.Clamp(localHeight, SurfaceLevel - 8, SurfaceLevel - 1);
                    terrainTile = new Vector2I(1, atlasRow);
                    break;

                case ChunkType.Friction:
                    localHeight = Mathf.Clamp(localHeight, SurfaceLevel - 5, SurfaceLevel - 1);
                    terrainTile = new Vector2I(4, atlasRow);
                    TerrainLayer.SetCell(new Vector2I(globalX, localHeight - 3), 0, new Vector2I(1, atlasRow));
                    break;
            }

            TerrainLayer.SetCell(new Vector2I(globalX, localHeight), 0, terrainTile);
            for (int fillY = localHeight + 1; fillY <= SurfaceLevel; fillY++)
            {
                if (fillY <= SurfaceLevel - 2)
                {
                    TerrainLayer.SetCell(new Vector2I(globalX, fillY), 0, fillTile);
                }
                else
                {
                    TerrainLayer.SetCell(new Vector2I(globalX, fillY), 0, deepTile);
                }
            }
        }

        return localHeight;
    }

    private void SpawnInitialPlayers(Node2D playersNode, PackedScene playerScene)
    {
        var mpm = GetNodeOrNull<MultiplayerManager>("/root/MultiplayerManager");
        
        // If we are currently joining a server, do NOT spawn a local dummy player.
        // Wait for the server to spawn our replicated player instead!
        if (mpm != null && !string.IsNullOrEmpty(mpm.PendingJoinIp))
        {
            return;
        }

        if (mpm != null && mpm.ConnectedPlayers.Count > 0)
        {
            foreach (long id in mpm.ConnectedPlayers)
            {
                var player = playerScene.Instantiate<PixelMan>();
                player.Name = id.ToString();
                player.GlobalPosition = GetSpawnPosition(0);
                playersNode.AddChild(player, true);
            }
        }
        else
        {
            // Singleplayer fallback
            var player = playerScene.Instantiate<PixelMan>();
            player.Name = "1";
            player.GlobalPosition = GetSpawnPosition(0);
            playersNode.AddChild(player, true);
        }
    }

    public void SpawnStructure(string type, Vector2 pos)
    {
        if (type == "House" && StructureHouseScene != null)
        {
            var house = StructureHouseScene.Instantiate<Node2D>();
            house.GlobalPosition = pos;
            AddChild(house);
        }
        else if (type == "FishermansDock" && FishermansDockScene != null)
        {
            var dock = FishermansDockScene.Instantiate<Node2D>();
            dock.GlobalPosition = pos;
            AddChild(dock);
        }
    }

    private void SpawnSavedStructures(int mapIndex)
    {
        var gm = GetTree().Root.GetNodeOrNull<GameManager>("GameManager");
        if (gm != null && gm.LoadedSaveData != null)
        {
            if (gm.LoadedSaveData.BuiltStructures.ContainsKey(mapIndex))
            {
                foreach (var sData in gm.LoadedSaveData.BuiltStructures[mapIndex])
                {
                    SpawnStructure(sData.StructureType, new Vector2(sData.PositionX, sData.PositionY));
                }
            }
        }
    }

    public void ClearMap()
    {
        TerrainLayer.Clear();
        
        foreach (Node child in GetChildren())
        {
            if (child is Portal || child is ResourceNode || child is Shopkeeper || child is FishingSpot || child is ConstructionZone || child.Name.ToString().Contains("Structure"))
            {
                child.QueueFree();
            }
        }
    }

    public Vector2 GetSpawnPosition(int spawnDirection)
    {
        float tilePixels = 32.0f;
        float spawnX = 0; 
        
        if (spawnDirection == 1) spawnX = (MapWidth / 2 - 5) * tilePixels;
        else if (spawnDirection == -1) spawnX = (-MapWidth / 2 + 5) * tilePixels;
        
        int pX = Mathf.RoundToInt(spawnX / tilePixels);
        int startY = FindFloorY(pX);
        
        return new Vector2(spawnX, (startY * tilePixels) - 64);
    }

    private void GenerateUpperDeck(int startX, int pathY, int length, int maxLadders)
    {
        int deckY = pathY - rng.RandiRange(5, 7); // Ensure minimum 5 tiles of vertical clearance
        
        int bRow = PLATFORM_ATLAS_POS.Y;
        for (int i = 0; i < length; i++)
        {
            Vector2I platformTile = new Vector2I(1, bRow);
            if (i == 0) platformTile = new Vector2I(0, bRow);
            else if (i == length - 1) platformTile = new Vector2I(2, bRow);
            
            // Only generate the 1-way platform so players can jump through it and walk under it!
            TerrainLayer.SetCell(new Vector2I(startX + i, deckY), 0, platformTile);
        }
    }

    private int FindFloorY(int xCoord)
    {
        for (int y = SurfaceLevel - 25; y <= SurfaceLevel; y++)
        {
            if (TerrainLayer.GetCellSourceId(new Vector2I(xCoord, y)) != -1)
            {
                return y;
            }
        }
        return SurfaceLevel;
    }
}
