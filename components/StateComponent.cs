using Godot;

[GlobalClass]
public partial class StateComponent : Node
{
    [Signal] public delegate void EnabledEventHandler();
    [Signal] public delegate void DisabledEventHandler();
    [Signal] public delegate void StateFinishedEventHandler();

    public void Disable()
    {
        EmitSignal(SignalName.Disabled);
        ProcessMode = ProcessModeEnum.Disabled;
    }

    public void Enable()
    {
        EmitSignal(SignalName.Enabled);
        ProcessMode = ProcessModeEnum.Inherit;
    }
}
