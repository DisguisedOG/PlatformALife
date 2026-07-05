using Godot;
using System;

public partial class ResourceNode : Area2D
{
    public enum ResourceType
    {
        Wood,
        Stone
    }

    [Export]
    public ResourceType NodeType { get; set; } = ResourceType.Wood;

    [Export]
    public string ResourceName { get; set; } = "Wood";
    
    [Export]
    public int ResourceAmount { get; set; } = 1;
    
    [Export]
    public int GoldValue { get; set; } = 5;

    private bool isPlayerInside = false;
    private PixelMan playerRef = null;
    private Sprite2D _sprite;

    public override void _Ready()
    {
        _sprite = GetNodeOrNull<Sprite2D>("Sprite2D");
        ZIndex = -5;
        UpdateSprite();
        BodyEntered += OnBodyEntered;
        BodyExited += OnBodyExited;
    }

    private void UpdateSprite()
    {
        if (_sprite == null)
            return;

        switch (NodeType)
        {
            case ResourceType.Stone:
                _sprite.Texture = ResourceLoader.Load<Texture2D>("res://Assets/Sprites/Resource_Stone.png");
                break;
            case ResourceType.Wood:
            default:
                _sprite.Texture = ResourceLoader.Load<Texture2D>("res://Assets/Sprites/Resource_Wood.png");
                break;
        }
        _sprite.ZIndex = -5;
    }

    public void SetResourceType(ResourceType type)
    {
        NodeType = type;
        ResourceName = type == ResourceType.Stone ? "Stone" : "Wood";
        ResourceAmount = type == ResourceType.Stone ? 2 : 3;
        GoldValue = type == ResourceType.Stone ? 10 : 5;
        UpdateSprite();
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
