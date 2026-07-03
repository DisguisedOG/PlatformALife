using Godot;
using System;

public partial class Portal : Area2D
{
    [Export]
    public int TargetMapIndex { get; set; } = 0;
    
    // 1 if going left, -1 if going right
    [Export]
    public int TravelDirection { get; set; } = 1; 

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
            GD.Print($"Entering Portal to Map {TargetMapIndex}");
            GameManager.Instance.SaveCurrentGameState();
            GameManager.Instance.LoadWorld(GameManager.Instance.CurrentSeed, TargetMapIndex, TravelDirection);
        }
    }
}
