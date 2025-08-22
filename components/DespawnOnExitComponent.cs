using Godot;

[GlobalClass]
public partial class DespawnOnExitComponent : Node
{
	[Export] private VisibleOnScreenNotifier2D _visibleNotifier;

	public override void _Ready()
	{
		if (_visibleNotifier == null)
		{
			Log.Err("Notifier not assigned");
			return;
		}
		_visibleNotifier.ScreenExited += Despawn;
	}

	public override void _Process(double delta)
	{
	}

	private void Despawn()
	{
		GetParent().QueueFree();
	}
}
