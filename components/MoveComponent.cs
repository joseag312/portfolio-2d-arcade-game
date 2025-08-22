using Godot;

[GlobalClass]
public partial class MoveComponent : Node
{
    [Export] public Node2D Actor { get; set; }
    [Export] public Vector2 Velocity { get; set; } = Vector2.Zero;

    public override void _Process(double delta)
    {
        Actor?.Translate(Velocity * (float)delta);
    }
}
