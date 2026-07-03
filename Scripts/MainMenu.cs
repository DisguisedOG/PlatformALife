using Godot;
using System;

public partial class MainMenu : Control
{
    private Button newWorldBtn;
    private Button exitBtn;
    
    private VBoxContainer mainButtons;
    private Panel worldCreationPanel;
    
    private LineEdit worldNameInput;
    private LineEdit seedInput;
    private OptionButton gameModeOption;
    private Button createBtn;
    private Button cancelBtn;
    private CheckButton hostToggle;

    private Button loadWorldBtn;
    private Panel loadWorldPanel;
    private ItemList saveList;
    private Button loadBtn;
    private Button cancelLoadBtn;

    public override void _Ready()
    {
        mainButtons = GetNode<VBoxContainer>("MainButtons");
        worldCreationPanel = GetNode<Panel>("WorldCreationPanel");
        
        newWorldBtn = GetNode<Button>("MainButtons/NewWorldBtn");
        exitBtn = GetNode<Button>("MainButtons/ExitBtn");
        
        worldNameInput = GetNode<LineEdit>("WorldCreationPanel/VBoxContainer/WorldNameInput");
        seedInput = GetNode<LineEdit>("WorldCreationPanel/VBoxContainer/SeedInput");
        gameModeOption = GetNode<OptionButton>("WorldCreationPanel/VBoxContainer/GameModeOption");
        createBtn = GetNode<Button>("WorldCreationPanel/VBoxContainer/HBoxContainer/CreateBtn");
        cancelBtn = GetNode<Button>("WorldCreationPanel/VBoxContainer/HBoxContainer/CancelBtn");

        loadWorldBtn = GetNode<Button>("MainButtons/LoadWorldBtn");
        loadWorldPanel = GetNode<Panel>("LoadWorldPanel");
        saveList = GetNode<ItemList>("LoadWorldPanel/VBoxContainer/SaveList");
        loadBtn = GetNode<Button>("LoadWorldPanel/VBoxContainer/HBoxContainer/LoadBtn");
        cancelLoadBtn = GetNode<Button>("LoadWorldPanel/VBoxContainer/HBoxContainer/CancelLoadBtn");

        newWorldBtn.Pressed += OnNewWorldPressed;
        loadWorldBtn.Pressed += OnLoadWorldBtnPressed;
        exitBtn.Pressed += OnExitPressed;
        
        var joinBtn = GetNode<Button>("MainButtons/JoinGameBtn");
        hostToggle = GetNode<CheckButton>("MainButtons/HostToggle");
        
        joinBtn.Pressed += () => MultiplayerManager.Instance.JoinGame("127.0.0.1");
        
        createBtn.Pressed += OnCreatePressed;
        cancelBtn.Pressed += OnCancelPressed;
        
        loadBtn.Pressed += OnLoadPressed;
        cancelLoadBtn.Pressed += OnCancelLoadPressed;
        
        worldCreationPanel.Visible = false;
        loadWorldPanel.Visible = false;
        mainButtons.Visible = true;
        
        ScreenFader.Instance?.FadeIn();
        AudioManager.Instance?.PlayMainMenuBGM();
    }

    private void OnNewWorldPressed()
    {
        mainButtons.Visible = false;
        worldCreationPanel.Visible = true;
        loadWorldPanel.Visible = false;
    }
    
    private void OnCancelPressed()
    {
        worldCreationPanel.Visible = false;
        mainButtons.Visible = true;
    }

    private void OnLoadWorldBtnPressed()
    {
        mainButtons.Visible = false;
        worldCreationPanel.Visible = false;
        loadWorldPanel.Visible = true;

        saveList.Clear();
        var saves = SaveManager.GetAllSaves();
        foreach (var save in saves)
        {
            saveList.AddItem(save);
        }
    }

    private void OnCancelLoadPressed()
    {
        loadWorldPanel.Visible = false;
        mainButtons.Visible = true;
    }

    private void OnLoadPressed()
    {
        var selected = saveList.GetSelectedItems();
        if (selected.Length > 0)
        {
            string worldName = saveList.GetItemText(selected[0]);
            SaveData data = SaveManager.LoadGame(worldName);
            if (data != null)
            {
                GameManager.Instance.WorldName = data.WorldName;
                GameManager.Instance.GameMode = data.GameMode;
                GameManager.Instance.LoadedSaveData = data;
                
                if (hostToggle.ButtonPressed)
                {
                    MultiplayerManager.Instance.HostGame();
                }
                
                GameManager.Instance.LoadWorld(data.Seed, data.CurrentMapIndex, 0);
            }
        }
    }

    private void OnCreatePressed()
    {
        string seed = seedInput.Text;
        if (string.IsNullOrWhiteSpace(seed))
        {
            seed = GD.Randi().ToString();
        }
        
        string worldName = worldNameInput.Text;
        if (string.IsNullOrWhiteSpace(worldName))
        {
            worldName = "My New World";
        }
        
        int gameMode = gameModeOption.Selected; // 0 = Normal, 1 = Sandbox

        // Store these in GameManager
        GameManager.Instance.WorldName = worldName;
        GameManager.Instance.GameMode = gameMode;
        GameManager.Instance.LoadedSaveData = null; // Clear any old save data

        if (hostToggle.ButtonPressed)
        {
            MultiplayerManager.Instance.HostGame();
        }

        // Load the world
        GameManager.Instance.LoadWorld(seed);
    }

    private void OnExitPressed()
    {
        GetTree().Quit();
    }
}
