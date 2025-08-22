using Godot;

[GlobalClass]
public partial class DropTextWhite : Node2D
{
    [Export] private Label _label;
    [Export] private float _moveSpeed = 80f;
    [Export] private float _fadeDuration = 0.5f;

    private int _dropValue;
    private string _prefix;
    private string _suffix;
    private bool _initialized = false;

    public void Initialize(int drop, string prefix = "", string suffix = "")
    {
        _dropValue = drop;
        _prefix = prefix;
        _suffix = suffix;
        _initialized = true;
    }

    public override void _Ready()
    {
        if (!_initialized) return;

        if (_label == null)
        {
            Log.Err("Label is not assigned");
            return;
        }

        _label.Text = _prefix + _dropValue.ToString() + _suffix;

        Tween tween = CreateTween();
        float randomYOffset = 70f;
        float randomXOffset = 0.0f;
        Vector2 startPosition = Position;

        startPosition.X -= _dropValue < 10 ? 0 : 0;

        Vector2 targetPosition = startPosition + new Vector2(randomXOffset, -randomYOffset);
        tween.TweenProperty(this, "position", targetPosition, _fadeDuration);
        tween.TweenProperty(this, "modulate:a", 0, _fadeDuration).SetTrans(Tween.TransitionType.Linear);
        tween.TweenCallback(Callable.From(QueueFree));
    }
}
