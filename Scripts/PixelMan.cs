using Godot;
using System;

public partial class PixelMan : CharacterBody2D
{
    public const float Speed = 300.0f;
    public const float JumpVelocity = -400.0f;

    // Get the gravity from the project settings to be synced with RigidBody nodes.
    public float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

    // Character Data
    public string CharacterName { get; set; } = "Player";
    public string Sex { get; set; } = "Male";
    
    // Health System
    public int MaxHealth { get; set; } = 100;
    public int Health { get; private set; } = 100;
    private string _hairStyle = "Short Hair";
    public string HairStyle 
    { 
        get => _hairStyle; 
        set 
        { 
            _hairStyle = value; 
            var hair = GetNodeOrNull<Sprite2D>("AnimatedSprite2D/HairSprite");
            if (hair != null)
            {
                hair.Visible = (_hairStyle == "Spiky Hair");
            }
        } 
    }

    private bool isClimbing = false;
    private bool isAttacking = false;
    private bool wasOnFloor = true;
    private const int LADDER_ATLAS_X = 5; // Matches WorldGenerator ladder atlas

    public override void _EnterTree()
    {
        // Set authority based on node name which should be the peer ID
        if (int.TryParse(Name.ToString(), out int peerId))
        {
            GetNode<MultiplayerSynchronizer>("MultiplayerSynchronizer").SetMultiplayerAuthority(peerId);
        }
        
        Health = MaxHealth;
    }

    public override void _Ready()
    {
        // Apply initial hair
        HairStyle = _hairStyle;

        var cam = GetNodeOrNull<Camera2D>("Camera2D");
        if (Multiplayer.HasMultiplayerPeer() && GetNodeOrNull<MultiplayerSynchronizer>("MultiplayerSynchronizer")?.GetMultiplayerAuthority() != Multiplayer.GetUniqueId())
        {
            if (cam != null) cam.Enabled = false;
        }
        else if (cam != null)
        {
            cam.MakeCurrent();
            
            // Set Camera Limits to prevent seeing the void!
            var worldGen = GetNodeOrNull<LevelManager>("../../LevelManager");
            if (worldGen != null)
            {
                cam.LimitLeft = (-worldGen.MapWidth / 2) * 32;
                cam.LimitRight = (worldGen.MapWidth / 2) * 32;
                cam.LimitBottom = (worldGen.SurfaceLevel + 1) * 32; // Just below the surface block
                // LimitTop can remain unrestricted or set to a high negative number if needed
            }
            else
            {
                var fishingMap = GetNodeOrNull<FishingMapInstance>("../..");
                if (fishingMap != null)
                {
                    cam.LimitLeft = -15 * 32;
                    cam.LimitRight = 19 * 32;
                    cam.LimitBottom = 15 * 32;
                }
            }
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        if (Multiplayer.HasMultiplayerPeer() && GetNodeOrNull<MultiplayerSynchronizer>("MultiplayerSynchronizer")?.GetMultiplayerAuthority() != Multiplayer.GetUniqueId())
            return;

        Vector2 velocity = Velocity;
        
        // 1. Check for ladders
        var tileMap = GetParent()?.GetNodeOrNull<TileMapLayer>("TileMapLayer");
        bool onLadder = false;
        if (tileMap != null)
        {
            // Check tile at player's center, feet, and slightly below feet to ensure they clear the top of the platform
            Vector2I[] checkPositions = new Vector2I[] {
                tileMap.LocalToMap(GlobalPosition),
                tileMap.LocalToMap(GlobalPosition + new Vector2(0, 32)),
                tileMap.LocalToMap(GlobalPosition + new Vector2(0, 48))
            };
            
            foreach (var pos in checkPositions)
            {
                int sourceId = tileMap.GetCellSourceId(pos);
                if (sourceId != -1)
                {
                    Vector2I atlasCoords = tileMap.GetCellAtlasCoords(pos);
                    if (atlasCoords.X == LADDER_ATLAS_X)
                    {
                        onLadder = true;
                        break;
                    }
                }
            }
        }

        // 2. Ladder State Transitions
        if (onLadder)
        {
            if (!isClimbing && (Input.IsActionPressed("move_up") || (Input.IsActionPressed("move_down") && !IsOnFloor())))
            {
                isClimbing = true;
                velocity = Vector2.Zero; // Stop falling momentum instantly
                // Snap X position to the center of the ladder tile
                Vector2I mapPos = tileMap.LocalToMap(GlobalPosition);
                GlobalPosition = new Vector2(mapPos.X * 32 + 16, GlobalPosition.Y);
            }
        }
        else
        {
            isClimbing = false;
        }

        // 3. Apply movement based on state
        if (isClimbing)
        {
            if (Input.IsActionJustPressed("jump"))
            {
                isClimbing = false;
                velocity.Y = JumpVelocity; // Jump off ladder
            }
            else
            {
                // Climbing movement (no gravity)
                float climbDir = Input.GetAxis("move_up", "move_down");
                velocity.Y = climbDir * (Speed * 0.6f); 
                velocity.X = 0; // Lock horizontal movement while climbing
                Velocity = velocity;
                MoveAndSlide();
                return; // Skip normal physics below
            }
        }

        // Normal Physics (Gravity and Walking)
        if (!IsOnFloor())
            velocity.Y += gravity * (float)delta;

        // Handle Jump / Drop Down
        if (Input.IsActionJustPressed("jump"))
        {
            if (Input.IsActionPressed("move_down") && IsOnFloor())
            {
                GD.Print("Dropping down through platform!");
                // Shift position down 1 pixel so they clip into the one-way platform and fall through
                GlobalPosition = new Vector2(GlobalPosition.X, GlobalPosition.Y + 1);
            }
            else if (IsOnFloor())
            {
                velocity.Y = JumpVelocity;
                GetNodeOrNull<CpuParticles2D>("DustParticles")?.Restart();
            }
        }
        
        // Handle Attack and Pickup
        if (Input.IsActionJustPressed("attack") && !isAttacking)
        {
            var weapon = GetNodeOrNull<Sprite2D>("WeaponPivot/WeaponSprite");
            if (weapon != null && weapon.Visible)
            {
                PerformAttack();
            }
        }
        if (Input.IsActionJustPressed("pickup"))
        {
            GD.Print("Pickup item triggered!");
        }

        // Get the input direction and handle the movement/deceleration.
        float direction = Input.GetAxis("move_left", "move_right");
        if (direction != 0)
        {
            velocity.X = direction * Speed;
        }
        else
        {
            velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
        }

        // Animation Controller
        var sprite = GetNodeOrNull<AnimatedSprite2D>("AnimatedSprite2D");
        if (sprite != null)
        {
            if (isClimbing)
            {
                sprite.Play("climb");
                if (velocity.Y == 0)
                    sprite.Pause();
                else
                    sprite.Play();
            }
            else if (!IsOnFloor())
            {
                sprite.Play("jump");
            }
            else if (direction != 0)
            {
                sprite.Play("walk");
            }
            else
            {
                sprite.Play("idle");
            }

            if (direction != 0)
            {
                sprite.Scale = new Vector2(direction < 0 ? -0.2f : 0.2f, 0.2f);
            }
        }

        Velocity = velocity;
        MoveAndSlide();

        // Landing particles
        if (IsOnFloor() && !wasOnFloor && velocity.Y >= 0)
        {
            GetNodeOrNull<CpuParticles2D>("DustParticles")?.Restart();
        }
        
        // Walking particles
        if (IsOnFloor() && Mathf.Abs(velocity.X) > 10 && GD.Randf() < 0.1f)
        {
            GetNodeOrNull<CpuParticles2D>("DustParticles")?.Restart();
        }

        wasOnFloor = IsOnFloor();

        // Void Fall Respawn
        if (GlobalPosition.Y > 2000)
        {
            var worldGen = GetNodeOrNull<LevelManager>("../LevelManager");
            if (worldGen != null)
            {
                // Respawn at the portal we entered from
                GlobalPosition = worldGen.GetSpawnPosition(GameManager.Instance.SpawnDirection);
                Velocity = Vector2.Zero;
            }
        }
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        
        var sprite = GetNodeOrNull<AnimatedSprite2D>("AnimatedSprite2D");
        var pivot = GetNodeOrNull<Node2D>("WeaponPivot");
        if (sprite != null && pivot != null)
        {
            Vector2 baseHandPos = new Vector2(50, 70); // default fallback
            
            string anim = sprite.Animation;
            int frame = sprite.Frame;
            
            if (anim == "idle")
            {
                if (frame == 0) baseHandPos = new Vector2(51, 72);
                if (frame == 1) baseHandPos = new Vector2(60, 68);
            }
            else if (anim == "walk")
            {
                if (frame == 0) baseHandPos = new Vector2(63, 65);
                if (frame == 1) baseHandPos = new Vector2(70, 59);
                if (frame == 2) baseHandPos = new Vector2(57, 67);
                if (frame == 3) baseHandPos = new Vector2(63, 67);
            }
            else if (anim == "jump")
            {
                baseHandPos = new Vector2(39, 84);
            }
            else if (anim == "climb")
            {
                if (frame == 0) baseHandPos = new Vector2(35, 83);
                if (frame == 1) baseHandPos = new Vector2(45, 78);
            }
            
            float scaleMult = Mathf.Abs(sprite.Scale.Y);
            Vector2 worldOffset = baseHandPos * scaleMult;
            
            bool facingLeft = sprite.Scale.X < 0;
            if (facingLeft)
            {
                worldOffset.X = -worldOffset.X;
                pivot.Scale = new Vector2(-1, 1);
            }
            else
            {
                pivot.Scale = new Vector2(1, 1);
            }
            
            pivot.Position = sprite.Position + worldOffset;
        }
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is InputEventKey keyEvent && keyEvent.Pressed && !keyEvent.Echo)
        {
            if (keyEvent.Keycode == Key.E)
            {
                var weapon = GetNodeOrNull<Sprite2D>("WeaponPivot/WeaponSprite");
                if (weapon != null)
                {
                    weapon.Visible = !weapon.Visible;
                    GD.Print("Weapon visibility toggled: " + weapon.Visible);
                }
            }
        }
    }

    private void PerformAttack()
    {
        isAttacking = true;
        var pivot = GetNodeOrNull<Node2D>("WeaponPivot");
        if (pivot != null)
        {
            // Reset rotation to start position (-45 deg)
            pivot.RotationDegrees = -45f;
            
            Tween tween = GetTree().CreateTween();
            // Swing to +135 deg in 0.15 seconds
            tween.TweenProperty(pivot, "rotation_degrees", 135f, 0.15f).SetTrans(Tween.TransitionType.Sine).SetEase(Tween.EaseType.Out);
            // Return to 0 degrees
            tween.TweenProperty(pivot, "rotation_degrees", 0f, 0.15f).SetTrans(Tween.TransitionType.Sine).SetEase(Tween.EaseType.In);
            
            tween.Finished += () => {
                isAttacking = false;
            };
        }
        else
        {
            isAttacking = false;
        }
    }

    public void TakeDamage(int amount)
    {
        if (Health <= 0) return; // Already dead

        Health -= amount;
        GD.Print($"{Name} took {amount} damage! Health is now {Health}/{MaxHealth}");

        // Update HUD
        if (Multiplayer.GetUniqueId() == GetNode<MultiplayerSynchronizer>("MultiplayerSynchronizer").GetMultiplayerAuthority())
        {
            var hud = GetNodeOrNull<HUD>("/root/HUD");
            if (hud == null)
            {
                var camera = GetNodeOrNull<Camera2D>("Camera2D");
                hud = camera?.GetNodeOrNull<HUD>("HUD"); // If HUD is child of Camera
            }
            if (hud == null) 
            {
                // Try finding HUD globally if it was added to root
                hud = GetTree().Root.GetNodeOrNull<HUD>("HUD");
            }
            
            hud?.UpdateHealth(Health, MaxHealth);
        }

        if (Health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        GD.Print($"{Name} has died!");
        
        // Respawn logic (similar to void fall)
        Health = MaxHealth;
        var worldGen = GetNodeOrNull<LevelManager>("../LevelManager");
        if (worldGen != null)
        {
            GlobalPosition = worldGen.GetSpawnPosition(GameManager.Instance.SpawnDirection);
            Velocity = Vector2.Zero;
        }
        
        if (Multiplayer.GetUniqueId() == GetNode<MultiplayerSynchronizer>("MultiplayerSynchronizer").GetMultiplayerAuthority())
        {
            var hud = GetTree().Root.GetNodeOrNull<HUD>("HUD");
            hud?.UpdateHealth(Health, MaxHealth);
        }
    }
}
