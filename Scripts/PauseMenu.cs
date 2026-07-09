using Godot;
using System;

public partial class PauseMenu : CanvasLayer
{
    private Button resumeBtn;
    private Button saveBtn;
    private Button optionsBtn;
    private Button menuBtn;
    private Button desktopBtn;
    private Button unstickBtn;

    public override void _Ready()
    {
        resumeBtn = GetNode<Button>("Panel/VBoxContainer/ResumeBtn");
        saveBtn = GetNode<Button>("Panel/VBoxContainer/SaveBtn");
        optionsBtn = GetNode<Button>("Panel/VBoxContainer/OptionsBtn");
        menuBtn = GetNode<Button>("Panel/VBoxContainer/MenuBtn");
        desktopBtn = GetNode<Button>("Panel/VBoxContainer/DesktopBtn");
        unstickBtn = GetNode<Button>("Panel/UnstickBtn");

        resumeBtn.Pressed += OnResumePressed;
        saveBtn.Pressed += OnSavePressed;
        optionsBtn.Pressed += OnOptionsPressed;
        menuBtn.Pressed += OnMenuPressed;
        desktopBtn.Pressed += OnDesktopPressed;
        unstickBtn.Pressed += OnUnstickPressed;

        this.Visible = false;
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event.IsActionPressed("ui_cancel"))
        {
            TogglePause();
        }
    }

    private void TogglePause()
    {
        this.Visible = !this.Visible;
        GetTree().Paused = this.Visible;
    }

    private void OnResumePressed()
    {
        TogglePause();
    }

    private void OnUnstickPressed()
    {
        string peerId = Multiplayer.HasMultiplayerPeer() ? Multiplayer.GetUniqueId().ToString() : "1";
        var player = GetTree().Root.GetNodeOrNull<PixelMan>("World/Players/" + peerId);
        var worldGen = GetTree().Root.GetNodeOrNull<LevelManager>("World/LevelManager");

        if (player != null && worldGen != null && GameManager.Instance != null)
        {
            player.GlobalPosition = worldGen.GetSpawnPosition(GameManager.Instance.SpawnDirection);
            
            // Re-enable physics simulation by closing the menu
            TogglePause();
            GD.Print("Player Unstuck!");
        }
    }

    private void OnSavePressed()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SaveCurrentGameState();
            GD.Print("Game Saved via Pause Menu.");
        }
    }

    private void OnOptionsPressed()
    {
        GD.Print("Options menu coming soon!");
    }

    private void OnMenuPressed()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SaveCurrentGameState();
        }
        
        GetTree().Paused = false;
        GetTree().ChangeSceneToFile("res://Scenes/MainMenu.tscn");
    }

    private void OnDesktopPressed()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SaveCurrentGameState();
        }
        GetTree().Quit();
    }
}
