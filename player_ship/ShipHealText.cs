using System;
using Godot;

[GlobalClass]
public partial class ShipHealText : Node2D
{
    [Export] private Label _label;
    [Export] private float _moveSpeed = 40f;
    [Export] private float _fadeDuration = 0.3f;

    private int _healValue;
    private bool _initialized = false;

    public void Initialize(int heal)
    {
        _healValue = heal;
        _initialized = true;
    }

    public override void _Ready()
    {
        if (!_initialized) return;

        if (_label == null)
        {
            Log.Err("Label is not assigned in HealText");
            return;
        }

        _label.Text = _healValue.ToString();

        Tween tween = CreateTween();

        float randomXOffset = (float)GD.RandRange(-1d, 1d) * 30f;
        float randomYOffset = (float)GD.RandRange(0.5d, 1d) * 28f;

        int direction = GD.RandRange(0, 1) == 0 ? -1 : 1;
        float magnitude = 8f * (float)Math.Log10(_healValue + 9f);
        magnitude = Mathf.Clamp(magnitude, 30f, 60f);
        float healOffset = magnitude * direction;

        Position += new Vector2(healOffset, 0);

        Vector2 targetPosition = Position + new Vector2(randomXOffset, -randomYOffset);

        float moveDuration = _fadeDuration * 1.1f;

        tween.TweenProperty(this, "position", targetPosition, moveDuration)
             .SetTrans(Tween.TransitionType.Cubic).SetEase(Tween.EaseType.Out);
        tween.Parallel().TweenProperty(this, "modulate:a", 0, _fadeDuration)
             .SetTrans(Tween.TransitionType.Linear);

        tween.TweenCallback(Callable.From(QueueFree));
    }

}
