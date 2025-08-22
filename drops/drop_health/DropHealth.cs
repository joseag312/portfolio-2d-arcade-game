#nullable enable
using Godot;
using System;

[GlobalClass]
public partial class DropHealth : DropBase, IDropAmount
{
    [Export] public int HealPercent = 10;

    public void SetAmount(int amount, DropContext? context = null)
    {
        HealPercent = amount;
    }

    public override void HandlePickup(HitboxComponent hitboxComponent)
    {
        if (hitboxComponent.Owner is Ship ship)
        {
            G.SFX.Play(SFX.HEAL);
            ship.Heal(HealPercent);
        }
    }
}

