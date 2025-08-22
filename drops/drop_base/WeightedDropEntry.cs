using Godot;
using System;

[GlobalClass]
public partial class WeightedDropEntry : Resource
{
    [Export] public PackedScene DropScene;
    [Export] public float DropChance = 0.1f;
    [Export] public int MinAmount = 1;
    [Export] public int MaxAmount = 1;
}
