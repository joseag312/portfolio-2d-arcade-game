using System.Diagnostics;
using Godot;

[GlobalClass]
public partial class SpawnerComponent : Node2D
{
    [Export] private Marker2D Location { get; set; }
    [Export] public PackedScene Scene { get; set; }

    public Node Spawn(Vector2 globalSpawnPosition, Node parent = null)
    {
        if (Scene == null)
        {
            Log.Err("Scene Export is null");
        }

        Node instance = Scene.Instantiate();
        parent ??= GetTree().CurrentScene;

        parent.AddChild(instance);
        if (instance is Node2D node2D)
        {
            node2D.GlobalPosition = globalSpawnPosition;
        }

        instance.AddToGroup("despawnable");
        return instance;
    }
}