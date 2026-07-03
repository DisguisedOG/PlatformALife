using Godot;
using System;

public partial class GameManager : Node
{
	public static GameManager Instance { get; private set; }
	public SaveData LoadedSaveData { get; set; } = new SaveData();
	public int SpawnDirection { get; set; } = 0;
	
	public string WorldName { get; set; } = "Default World";
	public int GameMode { get; set; } = 0;
	public string CurrentSeed { get; set; } = "";
	public int CurrentMapIndex { get; set; } = 0;
	public int PreviousMapIndex { get; set; } = 0;

	public override void _Ready()
	{
		if (Instance == null)
		{
			Instance = this;
		}
		else
		{
			QueueFree();
			return;
		}

		GD.Print("GameManager Initialized.");
	}

	public async void LoadWorld(string seed)
	{
		GD.Print($"Loading World with seed: {seed}");
		CurrentSeed = seed;
		if (ScreenFader.Instance != null) await ScreenFader.Instance.WaitForFadeOut();
		GetTree().ChangeSceneToFile("res://Scenes/World.tscn");
	}

	public async void LoadWorld(string seed, int mapIndex, int spawnDir)
	{
		CurrentSeed = seed;
		CurrentMapIndex = mapIndex;
		SpawnDirection = spawnDir;
		GD.Print($"Loading World with seed: {seed}, map: {mapIndex}");
		if (ScreenFader.Instance != null) await ScreenFader.Instance.WaitForFadeOut();
		GetTree().ChangeSceneToFile("res://Scenes/World.tscn");
	}

	public async void LoadFishingInstance()
	{
		PreviousMapIndex = CurrentMapIndex;
		GD.Print($"Loading Fishing Instance. Saving PreviousMapIndex: {PreviousMapIndex}");
		if (ScreenFader.Instance != null) await ScreenFader.Instance.WaitForFadeOut();
		GetTree().ChangeSceneToFile("res://Scenes/FishingMapInstance.tscn");
	}

	public void ReturnToWorld()
	{
		GD.Print($"Returning to World map: {PreviousMapIndex}");
		LoadWorld(CurrentSeed, PreviousMapIndex, 0);
	}

	public void SaveCurrentGameState()
	{
		if (LoadedSaveData != null)
		{
			LoadedSaveData.WorldName = WorldName;
			LoadedSaveData.Seed = CurrentSeed;
			LoadedSaveData.GameMode = GameMode;
			LoadedSaveData.CurrentMapIndex = CurrentMapIndex;
			SaveManager.SaveGame(LoadedSaveData);
		}
	}
}
