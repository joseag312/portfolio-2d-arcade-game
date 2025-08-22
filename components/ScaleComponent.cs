using Godot;
using System;

[GlobalClass]
public partial class ScaleComponent : Node
{
	[Export] public Node2D Sprite { get; set; }
	[Export] public Vector2 ScaleAmount { get; set; } = new Vector2(1.5f, 1.5f);
	[Export] public float ScaleDuration { get; set; } = 0.5f;

	public void TweenScale()
	{
		Tween tween = GetTree().CreateTween();
		tween.SetTrans(Tween.TransitionType.Expo);
		tween.SetEase(Tween.EaseType.Out);
		tween.TweenProperty(Sprite, Node2D.PropertyName.Scale.ToString(), ScaleAmount, ScaleDuration * 0.1f).FromCurrent();
		tween.TweenProperty(Sprite, Node2D.PropertyName.Scale.ToString(), Vector2.One, ScaleDuration * 0.9f).From(ScaleAmount);
	}
}
