using Godot;
using System;
using System.Collections.Generic;

[GlobalClass]
public partial class DropComponent : Node
{
    [Export] public DestroyedComponent DestroyedComponent;
    [Export] public WeightedDropTable DropTable;
    [Export] public DropSourceType Type = DropSourceType.Common;
    [Export] public Node2D DropTarget;
    [Export] public Node2D EffectTarget;

    public override void _Ready()
    {
        DestroyedComponent.Destroyed += TrySpawnDrop;
    }

    public void TrySpawnDrop(Vector2 position)
    {
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

        float roll = GD.Randf();

        float cumulative = 0f;

        foreach (var entry in validEntries)
        {
            cumulative += entry.DropChance;

            if (roll <= cumulative)
            {
                var drop = entry.DropScene.Instantiate() as Node2D;
                if (drop == null)
                {
                    Log.Err($"Failed to instantiate Drop from {entry.DropScene.ResourcePath}");
                    break;
                }

                if (drop is DropBase dropBase)
                {
                    dropBase.EffectContainer = EffectTarget;
                }

                drop.GlobalPosition = position;

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
        if (DropTarget == null)
        {
            Log.Err("DropTarget is null");
            return;
        }

        if (!IsInstanceValid(drop))
        {
            Log.Err("Drop instance is invalid or already freed");
            return;
        }

        DropTarget.CallDeferred("add_child", drop);
    }
}
