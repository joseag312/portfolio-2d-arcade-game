using Godot;
using System;

[GlobalClass]
public partial class SFXOnSpawnComponent : Node
{
    [Export] public string SoundKey = "";

    public override void _Ready()
    {
        if (!string.IsNullOrEmpty(SoundKey))
        {
            G.SFX.Play(SoundKey);
        }
    }
}