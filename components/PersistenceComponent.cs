using Godot;
using System.Threading.Tasks;

[GlobalClass]
public partial class PersistenceComponent : Node2D
{
	[Signal]
	public delegate void PersistenceCompleteEventHandler();

	[Export] public MoveComponent MoveComponent { get; set; }
	[Export] public int PersistenceMilliseconds { get; set; }
	[Export] public bool ShouldStay { get; set; } = false;
	[Export] public bool ShouldPersist { get; set; } = false;

	private int _margin;
	private int _leftBorder;
	private int _rightBorder;

	public override void _Ready()
	{
		_leftBorder = 0;
		_rightBorder = (int)ProjectSettings.GetSetting("display/window/size/viewport_width");
		Persist();
	}

	public async void Persist()
	{
		float speedY = MoveComponent.Velocity.Y;
		float targetStopY = 175;

		while (GlobalPosition.Y < targetStopY)
		{
			await ToSignal(GetTree(), "process_frame");
			if (!IsInstanceValid(this) || !ShouldPersist)
				return;
		}

		MoveComponent.Velocity = Vector2.Zero;

		await Task.Delay(PersistenceMilliseconds);

		if (!IsInstanceValid(this))
			return;

		EmitSignal(SignalName.PersistenceComplete);

		if (ShouldStay)
			return;

		float distanceToLeft = GlobalPosition.X - _leftBorder;
		float distanceToRight = _rightBorder - GlobalPosition.X;
		float targetSpeedX = (distanceToLeft < distanceToRight) ? -100 : 100;

		MoveComponent.Velocity = new Vector2(targetSpeedX, 0);
	}
}
