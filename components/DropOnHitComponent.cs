using Godot;
using System;
using System.Collections.Generic;

[GlobalClass]
public partial class DropOnHitComponent : Node
{
    [Export] public HurtboxComponent HurtboxComponent;
    [Export] public WeightedDropTable DropTable;
    [Export] public DropSourceType Type = DropSourceType.Common;
    [Export] public Node2D DropTarget;
    [Export] public Node2D EffectTarget;

    public override void _Ready()
    {
        if (HurtboxComponent != null)
        {
            HurtboxComponent.Hurt += OnHit;
        }
        else
        {
            Log.Err("HurtboxComponent not assigned");
        }
    }

    private void OnHit(HitboxComponent hitbox)
    {
        Vector2 position = hitbox.Position;

        if (DropTable == null)
            return;

        float totalChance = 0f;
        var validEntries = new List<WeightedDropEntry>();

        foreach (var entryObj in DropTable.Entries)
        {
            if (entryObj is WeightedDropEntry entry && entry.DropScene != null)
            {
                totalChance += entry.DropChance;
                validEntries.Add(entry);
            }
        }

        if (validEntries.Count == 0)
            return;

        float roll = GD.Randf();
        float cumulative = 0f;

        foreach (var entry in validEntries)
        {
            cumulative += entry.DropChance;

            if (roll <= cumulative)
            {
                var drop = entry.DropScene.Instantiate() as Node2D;
                if (drop == null)
                    break;

                if (drop is DropBase dropBase)
                {
                    dropBase.EffectContainer = EffectTarget;
                }

                drop.GlobalPosition = GetParent<Node2D>()?.GlobalPosition ?? Vector2.Zero;

                if (drop is IDropAmount dropAmount)
                {
                    int amount = (int)(GD.Randi() % (entry.MaxAmount - entry.MinAmount + 1) + entry.MinAmount);

                    var context = new DropContext
                    {
                        SourceType = Type
                    };

                    dropAmount.SetAmount(amount, context);
                }

                AddDropDeferred(drop);
                return;
            }
        }
    }

    private void AddDropDeferred(Node drop)
    {
        if (DropTarget == null || !IsInstanceValid(drop))
            return;

        DropTarget.CallDeferred("add_child", drop);
    }
}
