using Godot;

[GlobalClass]
public partial class MoveStatsComponent : Resource
{
    [Export] public int Speed { get; set; } = 150;
}
