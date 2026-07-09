using Godot;
using System;

public partial class MainMenu : Control
{
    private Button startGameBtn;
    private Button exitBtn;
    private CheckButton hostToggle;

    public override void _Ready()
    {
        startGameBtn = GetNode<Button>("MainButtons/StartGameBtn");
        exitBtn = GetNode<Button>("MainButtons/ExitBtn");
        hostToggle = GetNode<CheckButton>("MainButtons/HostToggle");
        var joinBtn = GetNode<Button>("MainButtons/JoinGameBtn");

        startGameBtn.Pressed += OnStartGamePressed;
        exitBtn.Pressed += OnExitPressed;
        joinBtn.Pressed += () => MultiplayerManager.Instance.JoinGame("127.0.0.1");
        
        ScreenFader.Instance?.FadeIn();
        AudioManager.Instance?.PlayMainMenuBGM();
    }

    private void OnStartGamePressed()
    {
        GD.Print("Successfully loading Waiheke Island!");
        
        if (GameManager.Instance == null)
        {
            GD.PrintErr("CRITICAL: GameManager.Instance is null! Autoload failed to inject.");
            return;
        }

        GameManager.Instance.WorldName = "My Static World";
        GameManager.Instance.GameMode = 0;
        GameManager.Instance.LoadedSaveData = null;

        if (hostToggle.ButtonPressed)
        {
            MultiplayerManager.Instance.HostGame();
        }

        GameManager.Instance.LoadWorld();
    }

    private void OnExitPressed()
    {
        GetTree().Quit();
    }
}
