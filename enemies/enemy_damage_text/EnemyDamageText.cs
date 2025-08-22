using Godot;

[GlobalClass]
public partial class EnemyDamageText : Node2D
{
	[Export] private Label _label;
	[Export] private float _moveSpeed = 80f;
	[Export] private float _fadeDuration = 0.5f;

	private int _damageValue;
	private bool _initialized = false;

	public void Initialize(int damage)
	{
		_damageValue = damage;
		_initialized = true;
	}

	public override void _Ready()
	{
		if (!_initialized) return;

		if (_label == null)
		{
			Log.Err("Label is not assigned in DamageText");
			return;
		}

		_label.Text = _damageValue.ToString();

		Tween tween = CreateTween();
		float randomXOffset = (float)GD.RandRange(-1d, 1d) * 40f;
		float randomYOffset = (float)GD.RandRange(0.5d, 1d) * 80f;
		Vector2 startPosition = Position;

		startPosition.X -= _damageValue < 10 ? 12 : 24;

		Vector2 targetPosition = startPosition + new Vector2(randomXOffset, -randomYOffset);
		tween.TweenProperty(this, "position", targetPosition, _fadeDuration);
		tween.TweenProperty(this, "modulate:a", 0, _fadeDuration).SetTrans(Tween.TransitionType.Linear);
		tween.TweenCallback(Callable.From(QueueFree));
	}
}
