using Godot;
using System;

public partial class FishingUI : CanvasLayer
{
    private ColorRect cursor;
    private ColorRect catchZone;
    private Label instructionLabel;
    
    private float cursorSpeed = 300.0f;
    private float barWidth = 300.0f;
    
    private float currentX = 0f;
    private int direction = 1;
    private bool isFishing = false;

    private PixelMan currentPlayer;
    private FishingSpot currentSpot;

    public override void _Ready()
    {
        cursor = GetNode<ColorRect>("Panel/Cursor");
        catchZone = GetNode<ColorRect>("Panel/CatchZone");
        instructionLabel = GetNode<Label>("Panel/InstructionLabel");
        
        this.Visible = false;
    }

    public void StartMinigame(PixelMan player, FishingSpot spot)
    {
        currentPlayer = player;
        currentSpot = spot;
        isFishing = true;
        
        var rng = new RandomNumberGenerator();
        rng.Randomize();
        float zoneX = rng.RandfRange(0, barWidth - catchZone.Size.X);
        catchZone.Position = new Vector2(zoneX, catchZone.Position.Y);
        
        currentX = 0;
        direction = 1;
        instructionLabel.Text = "Press Spacebar to catch!";
        this.Visible = true;
    }

    public override void _Process(double delta)
    {
        if (!isFishing) return;

        currentX += direction * cursorSpeed * (float)delta;
        
        if (currentX >= barWidth - cursor.Size.X)
        {
            currentX = barWidth - cursor.Size.X;
            direction = -1;
        }
        else if (currentX <= 0)
        {
            currentX = 0;
            direction = 1;
        }

        cursor.Position = new Vector2(currentX, cursor.Position.Y);
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (isFishing && @event.IsActionPressed("ui_accept"))
        {
            isFishing = false;
            
            // Allow a small leniency on the cursor bounds
            if (currentX + (cursor.Size.X/2) >= catchZone.Position.X && currentX + (cursor.Size.X/2) <= catchZone.Position.X + catchZone.Size.X)
            {
                instructionLabel.Text = "You caught a Fish!";
                if (currentPlayer != null)
                {
                    var inv = currentPlayer.GetNodeOrNull<InventoryManager>("InventoryManager");
                    if (inv != null) inv.AddItem("Fish", 1);
                }
            }
            else
            {
                instructionLabel.Text = "The fish got away...";
            }
            
            var timer = GetTree().CreateTimer(1.0f);
            timer.Timeout += CloseMinigame;
            
            GetViewport().SetInputAsHandled();
        }
    }

    private void CloseMinigame()
    {
        this.Visible = false;
        GetTree().Paused = false;
        if (currentSpot != null)
        {
            currentSpot.QueueFree();
        }
    }
}
