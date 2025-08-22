using Godot;

[GlobalClass]
public partial class DestroyedComponent : Node
{
    [Signal]
    public delegate void DestroyedEventHandler(Vector2 globalPosition);

    [Export] public Node2D Actor { get; set; }
    [Export] public StatsComponent StatsComponent { get; set; }
    [Export] public SpawnerComponent DestroyEffectSpawnerComponent { get; set; }
    [Export] public Node2D EffectTarget { get; set; }

    public override void _Ready()
    {
        StatsComponent.NoHealth += Destroy;
    }

    private void Destroy()
    {
        EmitSignal(SignalName.Destroyed, Actor.GlobalPosition);
        DestroyEffectSpawnerComponent.Spawn(Actor.GlobalPosition, EffectTarget);
        Actor.QueueFree();
    }
}
