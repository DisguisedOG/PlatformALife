using Godot;
using System;

public partial class GameplayHUD : CanvasLayer
{
    private Label characterNameLabel;
    private Label hpLabel;
    private ProgressBar hpBar;
    private Label mpLabel;
    private ProgressBar mpBar;
    private ProgressBar expBar;
    private Label goldLabel;

    public override void _Ready()
    {
        characterNameLabel = GetNode<Label>("HUDControl/StatsPanel/VBox/CharacterNameLabel");
        goldLabel = GetNode<Label>("HUDControl/StatsPanel/VBox/GoldLabel");
        
        hpLabel = GetNode<Label>("HUDControl/StatsPanel/VBox/HPRow/HPLabel");
        hpBar = GetNode<ProgressBar>("HUDControl/StatsPanel/VBox/HPRow/HPBar");
        
        mpLabel = GetNode<Label>("HUDControl/StatsPanel/VBox/MPRow/MPLabel");
        mpBar = GetNode<ProgressBar>("HUDControl/StatsPanel/VBox/MPRow/MPBar");
        
        expBar = GetNode<ProgressBar>("HUDControl/ExpBarContainer/ExpBar");

        // Initialize with default values
        UpdateHP(100, 100);
        UpdateMP(50, 50);
        UpdateEXP(0, 100);
        UpdateGold(0);
        SetCharacterName("Player");
    }

    public void UpdateHP(int current, int max)
    {
        if (hpBar == null) return;
        hpBar.MaxValue = max;
        hpBar.Value = current;
        hpLabel.Text = $"HP: {current}/{max}";
    }

    public void UpdateMP(int current, int max)
    {
        if (mpBar == null) return;
        mpBar.MaxValue = max;
        mpBar.Value = current;
        mpLabel.Text = $"MP: {current}/{max}";
    }

    public void UpdateEXP(int current, int max)
    {
        if (expBar == null) return;
        expBar.MaxValue = max;
        expBar.Value = current;
        // EXP bar usually doesn't need text, just visual
    }

    public void UpdateGold(int amount)
    {
        if (goldLabel == null) return;
        goldLabel.Text = $"Gold: {amount}";
    }

    public void SetCharacterName(string name)
    {
        if (characterNameLabel == null) return;
        characterNameLabel.Text = name;
    }
}
