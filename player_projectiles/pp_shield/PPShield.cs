using Godot;
using System;

[GlobalClass]
public partial class PPShield : Node2D
{
    [Export] public HurtboxComponent Hurtbox;
    [Export] public Timer Timer;
    [Export] public int MaxHits = 10;

    private InvincibilityComponent _invincibilityComponent;
    private int _hitCount = 0;

    public override void _Ready()
    {
        if (Hurtbox == null)
        {
            Log.Err("HurtboxComponent not set");
            QueueFree();
            return;
        }

        Hurtbox.Connect(HurtboxComponent.SignalName.Hurt, new Callable(this, nameof(OnHitReceived)));

        _invincibilityComponent = GetParent().GetNodeOrNull<InvincibilityComponent>("InvincibilityComponent");
        if (_invincibilityComponent == null)
        {
            Log.Err("InvincibilityComponent not found on parent");
            QueueFree();
            return;
        }

        Timer.Timeout += Despawn;
        Timer.Start();

        _invincibilityComponent.ForceStartInvincibility();
    }


    private void OnHitReceived(HitboxComponent hitbox)
    {
        _hitCount++;

        if (_hitCount >= MaxHits)
            Despawn();
    }

    private void Despawn()
    {
        if (_invincibilityComponent != null)
            _invincibilityComponent.ForceEndInvincibility();

        QueueFree();
    }
}
