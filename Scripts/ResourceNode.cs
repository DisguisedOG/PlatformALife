using Godot;
using System;

public partial class ResourceNode : Area2D
{
    [Export]
    public string ResourceName { get; set; } = "Wood";
    
    [Export]
    public int ResourceAmount { get; set; } = 1;
    
    [Export]
    public int GoldValue { get; set; } = 5;

    private bool isPlayerInside = false;
    private PixelMan playerRef = null;

    public override void _Ready()
    {
        BodyEntered += OnBodyEntered;
        BodyExited += OnBodyExited;
    }

    private void OnBodyEntered(Node2D body)
    {
        if (body is PixelMan p)
        {
            var sync = p.GetNodeOrNull<MultiplayerSynchronizer>("MultiplayerSynchronizer");
            if (!Multiplayer.HasMultiplayerPeer() || (sync != null && sync.GetMultiplayerAuthority() == Multiplayer.GetUniqueId()))
            {
                isPlayerInside = true;
                playerRef = p;
            }
        }
    }

    private void OnBodyExited(Node2D body)
    {
        if (body is PixelMan p)
        {
            var sync = p.GetNodeOrNull<MultiplayerSynchronizer>("MultiplayerSynchronizer");
            if (!Multiplayer.HasMultiplayerPeer() || (sync != null && sync.GetMultiplayerAuthority() == Multiplayer.GetUniqueId()))
            {
                isPlayerInside = false;
                playerRef = null;
            }
        }
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (isPlayerInside && @event.IsActionPressed("ui_accept") && playerRef != null)
        {
            var inv = playerRef.GetNodeOrNull<InventoryManager>("InventoryManager");
            if (inv != null)
            {
                inv.AddItem(ResourceName, ResourceAmount);
                inv.AddGold(GoldValue);
                GD.Print($"Harvested {ResourceAmount} {ResourceName} and {GoldValue} Gold!");
                
                // Remove the node after harvesting
                QueueFree();
            }
        }
    }
}
