using Godot;
using System;

public partial class FishingSpot : Area2D
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
            var inv = playerRef.GetNodeOrNull<InventoryManager>("InventoryManager");
            if (inv != null)
            {
                if (inv.HasItem("Fishing Rod"))
                {
                    var fishingUI = GetTree().Root.GetNodeOrNull<FishingUI>("World/FishingUI") ?? GetTree().Root.GetNodeOrNull<FishingUI>("FishingMapInstance/World/FishingUI");
                    if (fishingUI != null)
                    {
                        fishingUI.StartMinigame(playerRef, this);
                        GetTree().Paused = true;
                        GetViewport().SetInputAsHandled();
                    }
                }
                else
                {
                    GD.Print("You need a Fishing Rod to fish here!");
                }
            }
        }
    }
}
