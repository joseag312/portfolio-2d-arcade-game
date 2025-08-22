using Godot;
using System;

[GlobalClass]
public partial class AutoSFXTriggerComponent : Node
{
    public void Play(string soundName)
    {
        G.SFX.Play(soundName);
    }

    public void OIIAFast()
    {
        G.SFX.Play(SFX.OIIA_FAST);
    }

    public void OIIASlow()
    {
        G.SFX.Play(SFX.OIIA_SLOW);
    }

    public void OIIADeath()
    {
        G.SFX.Play(SFX.OIIA_DEATH);
    }

    public void Meow()
    {
        G.SFX.Play(SFX.MEOW);
    }

    public void ForceSetVolume(float db)
    {
        G.SFX.SetVolumeDb(db);
    }
}
