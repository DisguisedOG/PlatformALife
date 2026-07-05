using Godot;
using System;

public partial class Portal : Area2D
{
    [Export]
    public int TargetMapIndex { get; set; } = 0;
    
    // 1 if going left, -1 if going right
    [Export]
    public int TravelDirection { get; set; } = 1;

    private Sprite2D _sprite;
    private float _pulseTime = 0.0f;
    private Vector2 _baseScale = new Vector2(1, 1);
    private bool isPlayerInside = false;

    public override void _Ready()
    {
        ZIndex = -10;
        _sprite = GetNodeOrNull<Sprite2D>("Sprite2D");
        if (_sprite != null)
        {
            _sprite.ZIndex = -10;
            _baseScale = _sprite.Scale;
        }

        BodyEntered += OnBodyEntered;
        BodyExited += OnBodyExited;
        AssetValidator.ValidatePortal(this);
    }

    public override void _Process(double delta)
    {
        _pulseTime += (float)delta;
        if (_sprite != null)
        {
            float pulse = 1.0f + 0.08f * Mathf.Sin(_pulseTime * 4.0f);
            _sprite.Scale = _baseScale * pulse;
        }

        if (isPlayerInside && Input.IsActionJustPressed("move_up"))
        {
            GD.Print($"Entering Portal to Map {TargetMapIndex}");
            GameManager.Instance.SaveCurrentGameState();
            GameManager.Instance.LoadWorld(GameManager.Instance.CurrentSeed, TargetMapIndex, TravelDirection);
        }
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
}

