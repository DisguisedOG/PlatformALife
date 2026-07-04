using Godot;
using System;
using System.Collections.Generic;

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

    public enum BiomeType { Plains, Highlands, Canyons, Tropics }
    public BiomeType CurrentBiome { get; private set; }

    private RandomNumberGenerator rng;
    
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

        // 2. Walker Algorithm for Main Path
        int currentX = -MapWidth / 2;
        int endX = MapWidth / 2;
        int currentY = SurfaceLevel - 3;
        int lastSegmentType = 0; // 0 = Flat, 1 = Ramp, 2 = Gap
        
        while (currentX < endX)
        {
            float action = rng.Randf();
            int segmentLength;
            
            // Default probabilities (Plains)
            float flatChance = 0.8f;
            float rampChance = 0.1f;
            float upperDeckChance = 0.2f;
            int maxLadders = 2;

            if (CurrentBiome == BiomeType.Highlands)
            {
                flatChance = 0.3f;
                rampChance = 0.6f;
                upperDeckChance = 0.6f;
                maxLadders = 5;
            }
            else if (CurrentBiome == BiomeType.Canyons)
            {
                flatChance = 0.4f;
                rampChance = 0.2f;
                upperDeckChance = 0.3f;
                maxLadders = 3;
            }

            bool forceFlat = (currentX < -MapWidth/2 + 10) || (currentX > endX - 10) || (currentX > -5 && currentX < 5) || lastSegmentType != 0;

            if (forceFlat || action < flatChance) // Flat Path
            {
                segmentLength = rng.RandiRange(6, 15);
                if (currentX + segmentLength > endX) segmentLength = endX - currentX;

                bool hasConstructionZone = !forceFlat && (rng.Randf() < 0.05f) && ConstructionZoneScene != null;
                
                int specialX = currentX + rng.RandiRange(2, segmentLength - 3);

                for (int i = 0; i < segmentLength; i++)
                {
                    int bRow = PLATFORM_ATLAS_POS.Y;
                    Vector2I platformTile = new Vector2I(1, bRow);
                    if (i == 0) platformTile = new Vector2I(0, bRow);
                    else if (i == segmentLength - 1) platformTile = new Vector2I(2, bRow);

                    TerrainLayer.SetCell(new Vector2I(currentX + i, currentY), 0, platformTile);
                    for (int fy = currentY + 1; fy <= SurfaceLevel; fy++)
                    {
                        TerrainLayer.SetCell(new Vector2I(currentX + i, fy), 0, FLOOR_ATLAS_POS);
                    }
                    
                    if (hasConstructionZone && currentX + i == specialX)
                    {
                        var cz = ConstructionZoneScene.Instantiate<ConstructionZone>();
                        cz.GlobalPosition = new Vector2((currentX + i) * 32.0f + 16.0f, currentY * 32.0f);
                        AddChild(cz);
                    }
                        else if (!hasConstructionZone && !forceFlat && rng.Randf() < 0.2f && ResourceNodeScene != null)
                        {
                            var resNode = ResourceNodeScene.Instantiate<ResourceNode>();
                            // Give it random resources based on biome
                            if (CurrentBiome == BiomeType.Highlands) { 
                                resNode.ResourceName = "Stone"; 
                                resNode.GoldValue = 10; 
                                resNode.GetNode<Sprite2D>("Sprite2D").Texture = ResourceLoader.Load<Texture2D>("res://Assets/Sprites/Resource_Stone.png"); 
                            }
                            else if (CurrentBiome == BiomeType.Plains) { 
                                resNode.ResourceName = "Wood"; 
                                resNode.GoldValue = 5; 
                                resNode.GetNode<Sprite2D>("Sprite2D").Texture = ResourceLoader.Load<Texture2D>("res://Assets/Sprites/Resource_Wood.png"); 
                            }
                            else { 
                                resNode.ResourceName = "Clay"; 
                                resNode.GoldValue = 8; 
                                resNode.GetNode<Sprite2D>("Sprite2D").Texture = ResourceLoader.Load<Texture2D>("res://Assets/Sprites/Resource_Clay.png"); 
                            }
                            
                            resNode.GlobalPosition = new Vector2((currentX + i) * 32.0f + 16.0f, currentY * 32.0f + 16.0f);
                            AddChild(resNode);
                        }
                        else if (!hasConstructionZone && !forceFlat && rng.Randf() < 0.35f)
                        {
                            string[] foliageNames = { "Foliage_Fern", "Foliage_Kauri", "Foliage_CabbageTree", "Foliage_Flax" };
                            string randomFoliage = foliageNames[rng.RandiRange(0, foliageNames.Length - 1)];
                            var tex = ResourceLoader.Load<Texture2D>($"res://Assets/Sprites/Foliage/{randomFoliage}.png");
                            if (tex != null)
                            {
                                var sprite = new Sprite2D();
                                sprite.Texture = tex;
                                float grassTop = currentY * 32.0f + 16.0f;
                                sprite.GlobalPosition = new Vector2((currentX + i) * 32.0f + 16.0f, grassTop - tex.GetHeight() / 2.0f);
                                // Ensure it renders behind characters
                                sprite.ZIndex = -1;
                                AddChild(sprite);
                            }
                        }
                }

                if (rng.Randf() < upperDeckChance && segmentLength >= 6 && currentY > SurfaceLevel - 6 && !forceFlat)
                {
                    GenerateUpperDeck(currentX, currentY, segmentLength, maxLadders);
                }

                currentX += segmentLength;
                lastSegmentType = 0;
            }
            else if (action < flatChance + rampChance) // Ramp (Staircase)
            {
                segmentLength = rng.RandiRange(3, 7);
                if (currentX + segmentLength > endX) segmentLength = endX - currentX;
                
                int yDir = 0;
                if (currentY <= SurfaceLevel - 6) yDir = 1; 
                else if (currentY >= SurfaceLevel - 2) yDir = -1; 
                else yDir = rng.Randf() < 0.5f ? -1 : 1; 

                for (int i = 0; i < segmentLength; i++)
                {
                    int bRow = PLATFORM_ATLAS_POS.Y;
                    Vector2I platformTile = new Vector2I(1, bRow);
                    if (i == 0) platformTile = new Vector2I(0, bRow);
                    else if (i == segmentLength - 1) platformTile = new Vector2I(2, bRow);

                    TerrainLayer.SetCell(new Vector2I(currentX + i, currentY), 0, platformTile);
                    for (int fy = currentY + 1; fy <= SurfaceLevel; fy++)
                    {
                        TerrainLayer.SetCell(new Vector2I(currentX + i, fy), 0, FLOOR_ATLAS_POS);
                    }
                    if (i % 2 == 0) currentY += yDir;
                    currentY = Mathf.Clamp(currentY, SurfaceLevel - 6, SurfaceLevel - 2);
                }
                currentX += segmentLength;
                lastSegmentType = 1;
            }
            else // Jump Gap
            {
                segmentLength = rng.RandiRange(1, 2); 
                if (currentX + segmentLength > endX) segmentLength = endX - currentX;
                
                for (int i = 0; i < segmentLength; i++)
                {
                    // Draw a 2-tile deep pit, then fill the rest with solid floor to the boundary
                    for (int fy = currentY + 3; fy <= SurfaceLevel; fy++)
                    {
                        TerrainLayer.SetCell(new Vector2I(currentX + i, fy), 0, FLOOR_ATLAS_POS);
                    }
                }
                
                currentX += segmentLength;
                lastSegmentType = 2;
            }
        }
        
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
