using Godot;
using System;

[GlobalClass]
public partial class ScrollingComponent : Node
{
	[Export] public ParallaxLayer Background { get; set; }
	[Export] public ParallaxLayer SlowLayer { get; set; }
	[Export] public ParallaxLayer FastLayer { get; set; }

	[Export] public int SpaceSpeed { get; set; } = 1;
	[Export] public int FarStarsSpeed { get; set; } = 5;
	[Export] public int CloseStarsSpeed { get; set; } = 30;

	private Vector2 _spaceOffset;
	private Vector2 _farStarsOffset;
	private Vector2 _closeStarsOffset;

	public override void _Ready()
	{
		_spaceOffset = Background.MotionOffset;
		_farStarsOffset = SlowLayer.MotionOffset;
		_closeStarsOffset = FastLayer.MotionOffset;
	}

	public override void _Process(double delta)
	{
		_spaceOffset.Y += (float)delta * SpaceSpeed;
		_farStarsOffset.Y += (float)delta * FarStarsSpeed;
		_closeStarsOffset.Y += (float)delta * CloseStarsSpeed;

		Background.MotionOffset = _spaceOffset;
		SlowLayer.MotionOffset = _farStarsOffset;
		FastLayer.MotionOffset = _closeStarsOffset;
	}
}
