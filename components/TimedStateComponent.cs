using Godot;

[GlobalClass]
public partial class TimedStateComponent : StateComponent
{
    [Export] public float Duration { get; set; } = 1.0f;

    private Timer _timer = new Timer();

    public override void _Ready()
    {
        AddChild(_timer);
        _timer.Timeout += OnTimeout;
        Enabled += () => _timer.Start(Duration);
    }

    private void OnTimeout()
    {
        EmitSignal(SignalName.StateFinished);
        Disable();
    }
}
