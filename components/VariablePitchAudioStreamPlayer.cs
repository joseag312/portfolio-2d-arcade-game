using Godot;

[GlobalClass]
public partial class VariablePitchAudioStreamPlayer : AudioStreamPlayer
{
    [Export] public float PitchMin { get; set; } = 0.6f;
    [Export] public float PitchMax { get; set; } = 1.2f;
    [Export] public bool AutoPlayWithVariance { get; set; } = false;

    public override void _Ready()
    {
        if (AutoPlayWithVariance)
        {
            PlayWithVariance();
        }
    }

    public void PlayWithVariance(float fromPosition = 0.0f)
    {
        PitchScale = GD.Randf() * (PitchMax - PitchMin) + PitchMin;
        Play(fromPosition);
    }
}
