using Godot;
using System;

[GlobalClass]
public partial class WeightedDropTable : Resource
{
    [Export] public Godot.Collections.Array<WeightedDropEntry> Entries = new();
}
