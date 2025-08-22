using Godot;
using System.Collections.Generic;
using System.Threading.Tasks;

[GlobalClass]
public partial class LevelCleanupComponent : Node
{
    [Export] public HUDMain HUD { get; set; }

    private double _cleanupTimer = 0.0;

    public override void _Process(double delta)
    {
        _cleanupTimer += delta;
        if (_cleanupTimer >= 1.0)
        {
            _cleanupTimer = 0.0;
            CleanupOffscreenEnemies();
        }
    }

    public async Task FadeOutAll()
    {
        await Task.WhenAll(
            FadeOutAllDespawnables(),
            FadeOutHUD()
        );
    }

    public async Task FadeOutAllDespawnables()
    {
        var awaiters = new List<SignalAwaiter>();
        var toFade = new HashSet<Node2D>();

        foreach (var node in GetTree().GetNodesInGroup("despawnable"))
            if (node is Node2D n2d) toFade.Add(n2d);

        foreach (var node in GetTree().GetNodesInGroup("spawn_wave"))
            if (node is Node2D n2d) toFade.Add(n2d);

        foreach (var n2d in toFade)
        {
            n2d.Visible = true;
            var tween = CreateTween();
            tween.TweenProperty(n2d, "modulate:a", 0.0f, 0.4f);
            tween.TweenCallback(Callable.From(() =>
            {
                if (GodotObject.IsInstanceValid(n2d))
                    n2d.QueueFree();
            }));
            awaiters.Add(ToSignal(tween, "finished"));
        }

        foreach (var awaiter in awaiters)
            await awaiter;
    }

    public async Task FadeOutHUD()
    {
        if (HUD == null)
            return;

        await HUD.FadeOutHUD();
    }

    private void CleanupOffscreenEnemies()
    {
        var viewportRect = GetViewport().GetVisibleRect();

        foreach (var node in GetTree().GetNodesInGroup("despawnable"))
        {
            if (node is Node2D n2d && !viewportRect.HasPoint(n2d.GlobalPosition))
                n2d.QueueFree();
        }
    }
}
