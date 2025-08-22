using Godot;
using System;
using System.Threading;
using System.Threading.Tasks;

[GlobalClass]
public partial class SpawnerRecurrentComponent : Node
{
    [Export] public Node2D EnemyContainer;
    [Export] public Node2D DropContainer;
    [Export] public Node2D EffectContainer;
    [Export] public Node2D ProjectileContainer;
    [Export] public Node2D Ship;
    [Export] public int Margin = 8;

    private int _leftBorder;
    private int _rightBorder;

    private bool _shouldSpawn1;
    private bool _shouldSpawn2;
    private bool _shouldSpawn3;

    public override void _Ready()
    {
        _leftBorder = Margin;
        _rightBorder = (int)ProjectSettings.GetSetting("display/window/size/viewport_width") - Margin;
    }

    public void StartSpawner1(SpawnerComponent spawner, int delay)
    {
        _shouldSpawn1 = true;
        G.CR.Run($"{GetInstanceId()}_Spawner1", token => RecurrentSpawn1(spawner, delay, token));
    }

    public void StartSpawner2(SpawnerComponent spawner, int delay)
    {
        _shouldSpawn2 = true;
        G.CR.Run($"{GetInstanceId()}_Spawner2", token => RecurrentSpawn2(spawner, delay, token));
    }

    public void StartSpawner3(SpawnerComponent spawner, int delay)
    {
        _shouldSpawn3 = true;
        G.CR.Run($"{GetInstanceId()}_Spawner3", token => RecurrentSpawn3(spawner, delay, token));
    }

    public void StopSpawner1()
    {
        _shouldSpawn1 = false;
        G.CR.Stop($"{GetInstanceId()}_Spawner1");
    }

    public void StopSpawner2()
    {
        _shouldSpawn2 = false;
        G.CR.Stop($"{GetInstanceId()}_Spawner2");
    }

    public void StopSpawner3()
    {
        _shouldSpawn3 = false;
        G.CR.Stop($"{GetInstanceId()}_Spawner3");
    }

    public void StopAllSpawning()
    {
        StopSpawner1();
        StopSpawner2();
        StopSpawner3();
    }

    private async Task RecurrentSpawn1(SpawnerComponent spawner, int time, CancellationToken token)
    {
        if (spawner.Scene == null) return;

        int spawnCount = 0;
        while (_shouldSpawn1 && !token.IsCancellationRequested && IsInsideTree())
        {
            try { await Task.Delay(time, token); }
            catch (TaskCanceledException) { break; }

            int x = (int)GD.RandRange(_leftBorder + Margin, _rightBorder - Margin);
            Vector2 position = new Vector2(x, -120);

            Node2D enemy = spawner.Scene.Instantiate<Node2D>();
            enemy.GlobalPosition = position;
            enemy.AddToGroup("despawnable");
            enemy.Name = $"Enemy_{spawnCount++}";

            SpawnSetup.SetupEnemy(enemy, EnemyContainer, DropContainer, EffectContainer, ProjectileContainer, false, false, Ship);
        }
    }

    private async Task RecurrentSpawn2(SpawnerComponent spawner, int time, CancellationToken token)
    {
        if (spawner.Scene == null) return;

        int spawnCount = 0;
        while (_shouldSpawn2 && !token.IsCancellationRequested && IsInsideTree())
        {
            try { await Task.Delay(time, token); }
            catch (TaskCanceledException) { break; }

            int x = (int)GD.RandRange(_leftBorder + Margin, _rightBorder - Margin);
            Vector2 position = new Vector2(x, -120);

            Node2D enemy = spawner.Scene.Instantiate<Node2D>();
            enemy.GlobalPosition = position;
            enemy.AddToGroup("despawnable");
            enemy.Name = $"Enemy_{spawnCount++}";

            SpawnSetup.SetupEnemy(enemy, EnemyContainer, DropContainer, EffectContainer, ProjectileContainer, false, false, Ship);
        }
    }

    private async Task RecurrentSpawn3(SpawnerComponent spawner, int time, CancellationToken token)
    {
        if (spawner.Scene == null) return;

        int spawnCount = 0;
        while (_shouldSpawn3 && !token.IsCancellationRequested && IsInsideTree())
        {
            try { await Task.Delay(time, token); }
            catch (TaskCanceledException) { break; }

            int x = (int)GD.RandRange(_leftBorder + Margin, _rightBorder - Margin);
            Vector2 position = new Vector2(x, -120);

            Node2D enemy = spawner.Scene.Instantiate<Node2D>();
            enemy.GlobalPosition = position;
            enemy.AddToGroup("despawnable");
            enemy.Name = $"Enemy_{spawnCount++}";

            SpawnSetup.SetupEnemy(enemy, EnemyContainer, DropContainer, EffectContainer, ProjectileContainer, false, false, Ship);
        }
    }
}
