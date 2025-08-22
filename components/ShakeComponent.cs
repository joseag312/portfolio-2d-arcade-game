using Godot;

[GlobalClass]
public partial class ShakeComponent : Node
{
    [Export] public Node2D TargetNode { get; set; }
    [Export] public float ShakeAmount { get; set; } = 2.0f;
    [Export] public float ShakeDuration { get; set; } = 0.4f;

    private float _shake = 0.0f;

    public void TweenShake()
    {
        _shake = ShakeAmount;

        Tween tween = GetTree().CreateTween();
        tween.TweenProperty(this, "Shake", 0.0f, ShakeDuration).FromCurrent();
    }

    public override void _PhysicsProcess(double delta)
    {
        TargetNode.Position = new Vector2(
            GD.Randf() * (_shake * 2) - _shake,
            GD.Randf() * (_shake * 2) - _shake
        );
    }

    private float Shake
    {
        get => _shake;
        set => _shake = value;
    }
}
