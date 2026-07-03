using Godot;
using System;

public partial class MapUI : CanvasLayer
{
    private Label mapNameLabel;

    public override void _Ready()
    {
        mapNameLabel = GetNode<Label>("MapNameLabel");
    }

    public void SetMapName(string name)
    {
        if (mapNameLabel == null)
        {
            mapNameLabel = GetNodeOrNull<Label>("MapNameLabel");
        }
        
        if (mapNameLabel != null)
        {
            mapNameLabel.Text = name;
        }
    }
}
