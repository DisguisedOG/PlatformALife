using Godot;
using System;
using System.Text.Json;
using System.IO;
using System.Collections.Generic;

public class StructureData
{
    public string StructureType { get; set; }
    public float PositionX { get; set; }
    public float PositionY { get; set; }
}

public class SaveData
{
    public string WorldName { get; set; } = "Default World";
    public string Seed { get; set; } = "";
    public int GameMode { get; set; } = 0;

    public string CharacterName { get; set; } = "Player";
    public string Sex { get; set; } = "Male";
    public string HairStyle { get; set; } = "Short Hair";

    public int CurrentMapIndex { get; set; } = 0;
    public float PositionX { get; set; } = 0;
    public float PositionY { get; set; } = 0;
    
    // Flag to know if this is a brand new world or an existing one
    public bool HasCharacter { get; set; } = false;

    // Inventory Data
    public int Gold { get; set; } = 0;
    public Dictionary<string, int> Items { get; set; } = new Dictionary<string, int>();

    // MapIndex -> List of built structures on that map
    public Dictionary<int, List<StructureData>> BuiltStructures { get; set; } = new Dictionary<int, List<StructureData>>();

    // HUD Settings
    public float InventoryPosX { get; set; } = -1;
    public float InventoryPosY { get; set; } = -1;
}

public static class SaveManager
{
    private static readonly string SaveDirectory = ProjectSettings.GlobalizePath("user://saves/");

    public static void Initialize()
    {
        if (!Directory.Exists(SaveDirectory))
        {
            Directory.CreateDirectory(SaveDirectory);
        }
    }

    public static void SaveGame(SaveData data)
    {
        Initialize();
        
        string safeName = data.WorldName.Replace(" ", "_").Replace("/", "_").Replace("\\", "_");
        string filePath = Path.Combine(SaveDirectory, safeName + ".json");

        try
        {
            string jsonString = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filePath, jsonString);
            GD.Print($"Game saved successfully to {filePath}");
        }
        catch (Exception e)
        {
            GD.PrintErr($"Failed to save game: {e.Message}");
        }
    }

    public static SaveData LoadGame(string worldName)
    {
        string safeName = worldName.Replace(" ", "_").Replace("/", "_").Replace("\\", "_");
        string filePath = Path.Combine(SaveDirectory, safeName + ".json");

        if (File.Exists(filePath))
        {
            try
            {
                string jsonString = File.ReadAllText(filePath);
                return JsonSerializer.Deserialize<SaveData>(jsonString);
            }
            catch (Exception e)
            {
                GD.PrintErr($"Failed to load game: {e.Message}");
            }
        }
        return null;
    }

    public static List<string> GetAllSaves()
    {
        Initialize();
        List<string> saveNames = new List<string>();
        
        string[] files = Directory.GetFiles(SaveDirectory, "*.json");
        foreach (string file in files)
        {
            try
            {
                string jsonString = File.ReadAllText(file);
                SaveData data = JsonSerializer.Deserialize<SaveData>(jsonString);
                if (data != null && !string.IsNullOrEmpty(data.WorldName))
                {
                    saveNames.Add(data.WorldName);
                }
            }
            catch (Exception)
            {
                // Ignore corrupt files
            }
        }
        return saveNames;
    }
}
