using Godot;
using System;

public partial class GameManager : Node
{
	public static GameManager Instance { get; private set; }
	public SaveData LoadedSaveData { get; set; } = new SaveData();
	public int SpawnDirection { get; set; } = 0;
	
	public string WorldName { get; set; } = "Default World";
	public int GameMode { get; set; } = 0;
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
		JobSystem.InitializeJobData(LoadedSaveData);
	}

	public async void LoadWorld()
	{
		GD.Print($"Loading World");
		if (ScreenFader.Instance != null) await ScreenFader.Instance.WaitForFadeOut();
		GetTree().ChangeSceneToFile("res://Scenes/World.tscn");
	}

	public async void LoadWorld(int mapIndex, int spawnDir)
	{
		CurrentMapIndex = mapIndex;
		SpawnDirection = spawnDir;
		GD.Print($"Loading World with map index: {mapIndex}");
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
		LoadWorld(PreviousMapIndex, 0);
	}

	public void SaveCurrentGameState()
	{
		if (LoadedSaveData != null)
		{
			LoadedSaveData.WorldName = WorldName;
			LoadedSaveData.GameMode = GameMode;
			LoadedSaveData.CurrentMapIndex = CurrentMapIndex;
			JobSystem.InitializeJobData(LoadedSaveData);
			SaveManager.SaveGame(LoadedSaveData);
		}
	}

	public int GainJobXP(string job, int xp)
	{
		if (LoadedSaveData == null)
		{
			return 0;
		}

		JobSystem.InitializeJobData(LoadedSaveData);
		int newLevel = JobSystem.GainJobXP(LoadedSaveData, job, xp);
		SaveCurrentGameState();
		return newLevel;
	}

	public bool ActivateJob(string job)
	{
		if (LoadedSaveData == null)
		{
			return false;
		}

		LoadedSaveData.ActiveJob = job;
		SaveCurrentGameState();
		return true;
	}

	public bool CanCreateRon()
	{
		if (LoadedSaveData == null)
		{
			return true;
		}

		return JobSystem.CanCreateRon(LoadedSaveData);
	}

	public void RecordRonCreation()
	{
		if (LoadedSaveData == null)
		{
			return;
		}

		JobSystem.RecordRonCreation(LoadedSaveData);
		SaveCurrentGameState();
	}
}
