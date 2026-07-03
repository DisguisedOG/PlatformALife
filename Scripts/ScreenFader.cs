using Godot;
using System;

public partial class ScreenFader : CanvasLayer
{
    public static ScreenFader Instance { get; private set; }
    private AnimationPlayer animPlayer;

    public override void _Ready()
    {
        if (Instance == null)
        {
            Instance = this;
            animPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        }
        else
        {
            QueueFree();
        }
    }

    public void FadeOut()
    {
        animPlayer.Play("fade_out");
    }

    public void FadeIn()
    {
        animPlayer.Play("fade_in");
    }

    public SignalAwaiter WaitForFadeOut()
    {
        FadeOut();
        return ToSignal(animPlayer, "animation_finished");
    }
}
