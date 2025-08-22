using Godot;

[GlobalClass]
public partial class PositionClampComponent : Node2D
{
    [Export] public Node2D Actor { get; set; }
    [Export] public float Margin { get; set; } = 8f;
    [Export] public bool Enabled { get; set; } = false;

    private readonly float _leftBorder = 0;
    private readonly float _rightBorder = (float)ProjectSettings.GetSetting("display/window/size/viewport_width");
    private readonly float _topBorder = 0;
    private readonly float _bottomBorder = (float)ProjectSettings.GetSetting("display/window/size/viewport_height");

    public override void _Process(double delta)
    {
        if (!Enabled || Actor == null)
            return;

        Actor.GlobalPosition = new Vector2(
            Mathf.Clamp(Actor.GlobalPosition.X, _leftBorder + Margin, _rightBorder - Margin),
            Mathf.Clamp(Actor.GlobalPosition.Y, _topBorder + Margin, _bottomBorder - Margin)
        );
    }
}
