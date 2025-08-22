using Godot;

[GlobalClass]
public partial class BorderBounceComponent : Node
{
    [Export] public int Margin { get; set; } = 8;
    [Export] public Node2D Actor { get; set; }
    [Export] public MoveComponent MoveComponent { get; set; }

    private int _leftBorder = 0;
    private int _rightBorder = (int)ProjectSettings.GetSetting("display/window/size/viewport_width");

    public override void _Process(double delta)
    {
        if (Actor.GlobalPosition.X < _leftBorder + Margin)
        {
            Actor.GlobalPosition = new Vector2(_leftBorder + Margin, Actor.GlobalPosition.Y);
            MoveComponent.Velocity = MoveComponent.Velocity.Bounce(Vector2.Right);
        }
        else if (Actor.GlobalPosition.X > _rightBorder - Margin)
        {
            Actor.GlobalPosition = new Vector2(_rightBorder - Margin, Actor.GlobalPosition.Y);
            MoveComponent.Velocity = MoveComponent.Velocity.Bounce(Vector2.Left);
        }
    }
}
