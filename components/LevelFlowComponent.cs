using Godot;
using System.Threading.Tasks;
using System.Collections.Generic;

[GlobalClass]
public partial class LevelFlowComponent : Node
{
    [Export] public Ship Ship { get; set; }
    [Export] public HUDMain HUD { get; set; }
    [Export] public ParallaxBackground ParallaxBackground { get; set; }
    [Export] public Node2D EffectContainer;
    [Export] public Node2D DropContainer;
    [Export] public Node2D ProjectileContainer;
    [Export] public Node2D EnemyContainer;
    [Export] public SpawnerRecurrentComponent SpawnerRecurrent { get; set; }
    [Export] public SpawnerWaveComponent SpawnerWave { get; set; }

    private BackgroundFadeComponent _backgroundFade;
    private double _cleanupTimer = 0.0;

    public override void _Ready()
    {
        G.BG.UnblockInput();
        G.GF.ResetTransition();
        G.GF.LastPlayedScene = GetTree().CurrentScene.SceneFilePath;
        _backgroundFade = ParallaxBackground.GetNodeOrNull<BackgroundFadeComponent>("BackgroundFadeComponent");
        ConnectShipDeath();
    }

    public override void _Process(double delta)
    {
        _cleanupTimer += delta;
        if (_cleanupTimer >= 1.0)
        {
            _cleanupTimer = 0.0;
            CleanupOffscreenEnemies();
        }
    }

    public void ConnectShipDeath()
    {
        var callable = new Callable(this, nameof(OnShipDeath));
        if (!Ship.StatsComponent.IsConnected("NoHealth", callable))
            Ship.StatsComponent.Connect("NoHealth", callable);
        if (Ship.HasNode("DestroyedComponent"))
        {
            var destroy = Ship.GetNode<DestroyedComponent>("DestroyedComponent");
            destroy.EffectTarget = EffectContainer;
        }
    }

    private async void OnShipDeath()
    {
        SpawnerRecurrent.StopAllSpawning();

        var fadeBlackTask = G.BG.FadeInBlack(0.3f);
        var cleanupTask = FadeOutAll();
        var backgroundTask = _backgroundFade.FadeOutAll(0.3f);

        await Task.WhenAll(fadeBlackTask, cleanupTask, backgroundTask);
        await G.GF.FadeToSceneWithBGFast(G.GF.GameOverScene);
    }

    public async Task FadeOutAll()
    {
        await Task.WhenAll(
            FadeOutAllDespawnables(),
            FadeOutHUD()
        );
    }

    public async Task FadeOutHUD()
    {
        if (HUD == null)
            return;

        await HUD.FadeOutHUD();
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