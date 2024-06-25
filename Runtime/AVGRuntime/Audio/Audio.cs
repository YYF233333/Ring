using System.Diagnostics;
using Godot;

public partial class Audio : AudioStreamPlayer
{
    public void Play(string audioFilePath)
    {
        Trace.Assert(audioFilePath.StartsWith("res://"));
        Stream = GD.Load<AudioStream>(audioFilePath);
        Play();
    }

    public void Play(AudioStream audio)
    {
        Stream = audio;
        Play();
    }
}
