using Godot;

[GlobalClass]
public partial class HitboxComponent : Area2D
{

    [Signal] public delegate void HitHurtboxEventHandler(HurtboxComponent hurtbox);

    [Export] public int Damage { get; set; } = 10;
    [Export] public int DamagePercentage { get; set; } = 0;

    public override void _Ready()
    {
        AreaEntered += OnHurtboxEntered;
    }

    private void OnHurtboxEntered(Area2D area)
    {
        if (area is not HurtboxComponent hurtbox) return;

        InvincibilityComponent invincibility = hurtbox.GetParent().GetNodeOrNull<InvincibilityComponent>("InvincibilityComponent");
        if (invincibility != null && invincibility.IsInvincible)
            return;

        EmitSignal(SignalName.HitHurtbox, hurtbox);
        hurtbox.EmitSignal(HurtboxComponent.SignalName.Hurt, this);
    }
}

