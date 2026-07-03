using Godot;
using System;

public partial class Shopkeeper : Area2D
{
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
            var shopUI = GetTree().Root.GetNodeOrNull<ShopUI>("World/ShopUI");
            if (shopUI != null)
            {
                // Open shop UI
                shopUI.OpenShop(playerRef);
                // Pause the game tree so you don't keep moving while shopping
                GetTree().Paused = true;
                
                // Consume the event so it doesn't trigger jumping
                GetViewport().SetInputAsHandled();
            }
        }
    }
}
