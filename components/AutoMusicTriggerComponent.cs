using Godot;
using System;

[GlobalClass]
public partial class AutoMusicTriggerComponent : Node
{
    public void PlayTrack(string name)
    {
        G.MS.PlayTrack(name);
    }

    public void StopMusic()
    {
        G.MS.Stop();
    }

    public void FadeInMusic(float duration = 1.0f)
    {
        G.MS.FadeIn(duration);
    }

    public void FadeOutMusic(float duration = 1.0f)
    {
        G.MS.FadeOut(duration);
    }

    public void ForceSetVolume(float db)
    {
        G.MS.SetVolumeDb(db);
    }
}
