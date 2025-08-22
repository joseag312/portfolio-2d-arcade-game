using Godot;

[GlobalClass]
public partial class HurtboxComponent : Area2D
{
    [Signal] public delegate void HurtEventHandler(HitboxComponent hitbox);

    public void HandleHit(HitboxComponent hitbox)
    {
        EmitSignal(SignalName.Hurt, hitbox);
    }
}
