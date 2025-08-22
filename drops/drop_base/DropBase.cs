using Godot;
using System;

[GlobalClass]
public abstract partial class DropBase : Area2D
{
    [Export] public PulseComponent PulseComponent;
    [Export] public HurtboxComponent HurtboxComponent;
    [Export] public PackedScene DropTextScene;
    [Export] public Node2D EffectContainer;

    public override void _Ready()
    {
        AddToGroup("despawnable");
        HurtboxComponent.Hurt += TriggerPickup;
    }

    public async void TriggerPickup(HitboxComponent hitboxComponent)
    {
        HandlePickup(hitboxComponent);
        PulseComponent.StopPulse();

        var tween = CreateTween();
        tween.SetParallel();

        tween.TweenProperty(this, "scale", Scale * 2f, 0.30f);
        tween.TweenProperty(this, "modulate:a", 0.0f, 0.30f);

        await ToSignal(tween, "finished");

        QueueFree();
    }

    public abstract void HandlePickup(HitboxComponent hitboxComponent);
}
