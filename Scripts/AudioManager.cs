using Godot;

public partial class AudioManager : Node
{
    public static AudioManager Instance { get; private set; }

    private AudioStreamPlayer currentPlayer;
    private AudioStreamPlayer fadePlayer;
    private string currentTrackPath = "";

    private const string MAIN_MENU_BGM = "res://Assets/Audio/BGM/MainMenu/Kiwi Lofi Platformer Menu Music.mp3";
    private const string WORLD_BGM = "res://Assets/Audio/BGM/World/World_Music_Kiwiana.mp3";
    private const float FADE_DURATION = 1.0f;
    private const float BGM_VOLUME_DB = -10.0f;

    private Tween activeTween;

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

        currentPlayer = new AudioStreamPlayer();
        currentPlayer.Bus = "Master";
        currentPlayer.VolumeDb = BGM_VOLUME_DB;
        AddChild(currentPlayer);

        fadePlayer = new AudioStreamPlayer();
        fadePlayer.Bus = "Master";
        fadePlayer.VolumeDb = -80.0f;
        AddChild(fadePlayer);
    }

    public void PlayMainMenuBGM()
    {
        PlayTrack(MAIN_MENU_BGM);
    }

    public void PlayWorldBGM()
    {
        PlayTrack(WORLD_BGM);
    }

    private void PlayTrack(string trackPath)
    {
        // Don't restart if same track is already playing
        if (currentTrackPath == trackPath && currentPlayer.Playing)
            return;

        var stream = GD.Load<AudioStream>(trackPath);
        if (stream == null)
        {
            GD.PrintErr($"AudioManager: Failed to load track: {trackPath}");
            return;
        }

        // Enable looping
        if (stream is AudioStreamMP3 mp3)
        {
            mp3.Loop = true;
        }

        // Kill any existing tween
        activeTween?.Kill();

        if (currentPlayer.Playing)
        {
            // Crossfade: move current to fadePlayer, start new on currentPlayer
            fadePlayer.Stop();
            fadePlayer.Stream = currentPlayer.Stream;
            fadePlayer.VolumeDb = currentPlayer.VolumeDb;
            fadePlayer.Play(currentPlayer.GetPlaybackPosition());

            currentPlayer.Stream = stream;
            currentPlayer.VolumeDb = -80.0f;
            currentPlayer.Play();

            // Tween crossfade
            activeTween = CreateTween();
            activeTween.SetParallel(true);
            activeTween.TweenProperty(currentPlayer, "volume_db", BGM_VOLUME_DB, FADE_DURATION);
            activeTween.TweenProperty(fadePlayer, "volume_db", -80.0f, FADE_DURATION);
            activeTween.SetParallel(false);
            activeTween.TweenCallback(Callable.From(() => fadePlayer.Stop()));
        }
        else
        {
            // No music playing, just start fresh with fade in
            currentPlayer.Stream = stream;
            currentPlayer.VolumeDb = -80.0f;
            currentPlayer.Play();

            activeTween = CreateTween();
            activeTween.TweenProperty(currentPlayer, "volume_db", BGM_VOLUME_DB, FADE_DURATION);
        }

        currentTrackPath = trackPath;
    }

    public void StopBGM()
    {
        activeTween?.Kill();

        if (currentPlayer.Playing)
        {
            activeTween = CreateTween();
            activeTween.TweenProperty(currentPlayer, "volume_db", -80.0f, FADE_DURATION);
            activeTween.TweenCallback(Callable.From(() =>
            {
                currentPlayer.Stop();
                currentTrackPath = "";
            }));
        }

        fadePlayer.Stop();
    }
}
