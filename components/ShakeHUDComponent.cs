using Godot;

[GlobalClass]
public partial class ShakeHUDComponent : Node
{
	[Export] public Control Target;
	[Export] public float ShakeAmount { get; set; } = 10.0f;
	[Export] public float ShakeDuration { get; set; } = 0.4f;

	public override void _Ready()
	{
		return;
	}

	public void TweenShake()
	{
		if (Target == null) return;

		Tween tween = GetTree().CreateTween();
		tween.TweenProperty(this, "Shake", 0.0f, ShakeDuration)
			 .FromCurrent()
			 .SetTrans(Tween.TransitionType.Quad)
			 .SetEase(Tween.EaseType.Out);
	}
}
