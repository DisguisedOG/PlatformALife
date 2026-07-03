using Godot;
using System;

public partial class CharacterCreationMenu : CanvasLayer
{
    private LineEdit nameInput;
    private Button createBtn;

    public override void _Ready()
    {
        nameInput = GetNode<LineEdit>("Panel/VBoxContainer/NameInput");
        createBtn = GetNode<Button>("Panel/VBoxContainer/CreateBtn");

        createBtn.Pressed += OnCreatePressed;

        // Try load existing
        if (GameManager.Instance.LoadedSaveData != null && GameManager.Instance.LoadedSaveData.HasCharacter)
        {
            SaveData data = GameManager.Instance.LoadedSaveData;
            CallDeferred("SpawnLoadedCharacter", data.CharacterName, data.Sex, data.HairStyle);
            this.Visible = false;
        }
    }

    private void SpawnLoadedCharacter(string charName, string sex, string hairStyle)
    {
        string peerId = Multiplayer.HasMultiplayerPeer() ? Multiplayer.GetUniqueId().ToString() : "1";
        var player = GetParent().GetNodeOrNull<PixelMan>("Players/" + peerId);
        
        if (player != null)
        {
            player.CharacterName = charName;
            player.Sex = sex;
            player.HairStyle = hairStyle;
            
            // Spawn at exact saved position if available, otherwise use worldgen
            // Wait, we just use worldGen.GetSpawnPosition for now unless we pass the position!
            // Let's pass the position from the data!
            var worldGen = GetNodeOrNull<WorldGenerator>("../WorldGenerator");
            if (worldGen != null)
            {
                player.GlobalPosition = worldGen.GetSpawnPosition(GameManager.Instance.SpawnDirection);
            }
        }

        var floatingCam = GetNodeOrNull<Camera2D>("../FloatingCamera");
        if (floatingCam != null)
        {
            floatingCam.QueueFree();
        }
        
        this.QueueFree();
    }

    private void OnCreatePressed()
    {
        string charName = nameInput.Text;
        if (string.IsNullOrWhiteSpace(charName))
        {
            charName = "Player";
        }

        // Hide UI
        this.Visible = false;

        // Ensure SaveData exists for this new character
        if (GameManager.Instance.LoadedSaveData == null)
        {
            GameManager.Instance.LoadedSaveData = new SaveData();
            GameManager.Instance.LoadedSaveData.WorldName = GameManager.Instance.WorldName;
            GameManager.Instance.LoadedSaveData.Seed = GameManager.Instance.CurrentSeed;
            GameManager.Instance.LoadedSaveData.GameMode = GameManager.Instance.GameMode;
        }

        GameManager.Instance.LoadedSaveData.HasCharacter = true;
        GameManager.Instance.LoadedSaveData.CharacterName = charName;
        GameManager.Instance.LoadedSaveData.Sex = GetNode<OptionButton>("Panel/VBoxContainer/SexOption").Text;
        GameManager.Instance.LoadedSaveData.HairStyle = GetNode<OptionButton>("Panel/VBoxContainer/HairOption").Text;

        // Save immediately so subsequent portal travels retain the character
        GameManager.Instance.SaveCurrentGameState();

        // Instead of instantiating, grab the multiplayer spawned character
        string peerId = Multiplayer.HasMultiplayerPeer() ? Multiplayer.GetUniqueId().ToString() : "1";
        var player = GetParent().GetNodeOrNull<PixelMan>("Players/" + peerId);
        
        if (player != null)
        {
            player.CharacterName = charName;
            player.Sex = GetNode<OptionButton>("Panel/VBoxContainer/SexOption").Text;
            player.HairStyle = GetNode<OptionButton>("Panel/VBoxContainer/HairOption").Text;
            
            // Find WorldGenerator to position player
            var worldGen = GetNodeOrNull<WorldGenerator>("../WorldGenerator");
            if (worldGen != null)
            {
                player.GlobalPosition = worldGen.GetSpawnPosition(GameManager.Instance.SpawnDirection);
            }
        }

        // Remove floating camera
        var floatingCam = GetNodeOrNull<Camera2D>("../FloatingCamera");
        if (floatingCam != null)
        {
            floatingCam.QueueFree();
        }
        
        // Free this menu completely
        this.QueueFree();
    }
}
