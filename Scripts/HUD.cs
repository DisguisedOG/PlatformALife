using Godot;
using System;
using System.Collections.Generic;

public partial class HUD : CanvasLayer
{
    private Control panel;
    private Label goldLabel;
    private GridContainer grid;
    
    private bool isDragging = false;
    private Vector2 dragOffset;
    
    private Label healthLabel;

    public override void _Ready()
    {
        panel = GetNode<Control>("Panel");
        goldLabel = GetNode<Label>("Panel/GoldLabel");
        grid = GetNode<GridContainer>("Panel/GridContainer");
        healthLabel = GetNodeOrNull<Label>("StatusBar/HealthLabel");
        
        panel.Visible = false;
        
        panel.GuiInput += OnPanelGuiInput;
        CallDeferred(nameof(ApplySavedPosition));
    }
    
    private void ApplySavedPosition()
    {
        var data = GameManager.Instance?.LoadedSaveData;
        if (data != null && data.InventoryPosX != -1)
        {
            panel.Position = new Vector2(data.InventoryPosX, data.InventoryPosY);
        }
        else
        {
            var viewport = GetViewport().GetVisibleRect().Size;
            panel.Position = new Vector2(viewport.X - panel.Size.X - 16, 16);
        }
    }
    
    private void OnPanelGuiInput(InputEvent @event)
    {
        if (@event is InputEventMouseButton mb)
        {
            if (mb.ButtonIndex == MouseButton.Left)
            {
                if (mb.Pressed)
                {
                    isDragging = true;
                    dragOffset = mb.GlobalPosition - panel.Position;
                }
                else
                {
                    isDragging = false;
                    var data = GameManager.Instance?.LoadedSaveData;
                    if (data != null)
                    {
                        data.InventoryPosX = panel.Position.X;
                        data.InventoryPosY = panel.Position.Y;
                    }
                }
            }
        }
        else if (@event is InputEventMouseMotion mm && isDragging)
        {
            panel.Position = mm.GlobalPosition - dragOffset;
        }
    }
    
    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventKey keyEvent && keyEvent.Pressed && !keyEvent.Echo)
        {
            if (keyEvent.Keycode == Key.I)
            {
                panel.Visible = !panel.Visible;
            }
        }
    }

    public void RefreshInventory(InventoryManager inv)
    {
        if (goldLabel == null || grid == null) return;
        
        goldLabel.Text = $"Gold: {inv.Gold}";
        
        foreach (Node child in grid.GetChildren())
        {
            child.QueueFree();
        }
        
        foreach (var kvp in inv.Items)
        {
            var btn = new Button();
            btn.Text = $"{kvp.Key}\nx{kvp.Value}";
            btn.CustomMinimumSize = new Vector2(64, 64);
            grid.AddChild(btn);
        }
    }

    public void UpdateHealth(int health, int maxHealth)
    {
        if (healthLabel != null)
        {
            healthLabel.Text = $"HP: {health}/{maxHealth}";
        }
    }
}
