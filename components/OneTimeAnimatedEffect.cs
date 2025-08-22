using Godot;

[GlobalClass]
public partial class OneTimeAnimatedEffect : AnimatedSprite2D
{
    public override void _Ready()
    {
        AnimationFinished += QueueFree;
        AnimationLooped += QueueFree;
    }
}
