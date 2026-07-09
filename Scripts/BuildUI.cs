using Godot;
using System;

public partial class BuildUI : CanvasLayer
{
    private PixelMan currentPlayer;
    private ConstructionZone currentZone;

    public override void _Ready()
    {
        GetNode<Button>("Panel/VBoxContainer/BuildHouseBtn").Pressed += () => TryBuildHouse();
        GetNode<Button>("Panel/VBoxContainer/CloseBtn").Pressed += CloseMenu;
        this.Visible = false;
    }

    public void OpenBuildMenu(PixelMan player, ConstructionZone zone)
    {
        currentPlayer = player;
        currentZone = zone;
        this.Visible = true;
    }

    public void CloseMenu()
    {
        this.Visible = false;
        GetTree().Paused = false;
    }

    private void TryBuildHouse()
    {
        if (currentPlayer == null || currentZone == null) return;
        
        var inv = currentPlayer.GetNodeOrNull<InventoryManager>("InventoryManager");
        if (inv != null && inv.HasItem("Wood", 10) && inv.HasItem("Stone", 5))
        {
            // Consume Deed and Resources
            inv.RemoveItem("Land Deed", 1);
            inv.RemoveItem("Wood", 10);
            inv.RemoveItem("Stone", 5);
            
            // Rebuild logic handled by GameManager / WorldGenerator
            var gm = GetTree().Root.GetNodeOrNull<GameManager>("GameManager");
            if (gm != null)
            {
                int mapIndex = gm.LoadedSaveData.CurrentMapIndex;
                if (!gm.LoadedSaveData.BuiltStructures.ContainsKey(mapIndex))
                {
                    gm.LoadedSaveData.BuiltStructures[mapIndex] = new System.Collections.Generic.List<StructureData>();
                }
                
                gm.LoadedSaveData.BuiltStructures[mapIndex].Add(new StructureData {
                    StructureType = "House",
                    PositionX = currentZone.GlobalPosition.X,
                    PositionY = currentZone.GlobalPosition.Y
                });
                
                // Tell WorldGenerator to spawn it now
                var wg = GetTree().Root.GetNodeOrNull<LevelManager>("World/LevelManager");
                if (wg != null)
                {
                    wg.SpawnStructure("House", currentZone.GlobalPosition);
                }
                
                currentZone.DestroyZone();
                GD.Print("House Built!");
                CloseMenu();
            }
        }
        else
        {
            GD.Print("Not enough resources to build House!");
        }
    }
}
