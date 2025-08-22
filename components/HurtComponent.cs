using System;
using Godot;

[GlobalClass]
public partial class HurtComponent : Node
{
    [Export] public StatsComponent StatsComponent { get; set; }
    [Export] public HurtboxComponent HurtboxComponent { get; set; }
    [Export] public InvincibilityComponent InvincibilityComponent { get; set; }
    [Export] public String DamageTextPath { get; set; }

    public override void _Ready()
    {
        if (HurtboxComponent != null)
            HurtboxComponent.Hurt += OnHurt;
    }

    private void OnHurt(HitboxComponent hitboxComponent)
    {
        int oldHealth = StatsComponent.Health;

        int baseDamage = hitboxComponent.Damage;
        int percentDamage = 0;

        if (hitboxComponent.DamagePercentage > 0)
        {
            percentDamage = (int)((hitboxComponent.DamagePercentage / 100.0f) * StatsComponent.MaxHealth);
            percentDamage = Math.Max(percentDamage, 1);
        }

        int damageAmount = Math.Max(baseDamage, percentDamage);

        if (damageAmount == 0)
        {
            return;
        }

        SpawnDamageText(damageAmount);
        StatsComponent.Health -= damageAmount;
        InvincibilityComponent?.StartInvincibility();

        if (Owner is Ship ship)
        {
            G.SFX.Play(SFX.THUD);
            ship.EmitSignal("HealthChanged", oldHealth, StatsComponent.Health);
        }
    }

    private void SpawnDamageText(int damage)
    {
        PackedScene damageTextScene = ResourceLoader.Load<PackedScene>(DamageTextPath);
        if (damageTextScene == null)
        {
            Log.Err("DamageText scene could not be loaded");
            return;
        }

        Node2D parent = (Node2D)GetParent();
        if (parent is Ship)
        {
            ShipDamageText damageText = damageTextScene.Instantiate<ShipDamageText>();
            damageText.Initialize(damage);
            parent.AddChild(damageText);
            damageText.GlobalPosition = parent.GlobalPosition + new Vector2(0, -10);
        }
        else
        {
            EnemyDamageText damageText = damageTextScene.Instantiate<EnemyDamageText>();
            damageText.Initialize(damage);
            parent.AddChild(damageText);
            damageText.GlobalPosition = parent.GlobalPosition + new Vector2(0, -10);
        }
    }
}
