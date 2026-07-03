using Godot;
using System;

public partial class FishermansDock : Area2D
{
    private bool isPlayerInside = false;

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
            }
        }
    }

    public override void _Process(double delta)
    {
        if (isPlayerInside && Input.IsActionJustPressed("move_up"))
        {
            GD.Print("Entering Fisherman's Dock!");
            GameManager.Instance.SaveCurrentGameState();
            GameManager.Instance.LoadFishingInstance();
        }
    }
}