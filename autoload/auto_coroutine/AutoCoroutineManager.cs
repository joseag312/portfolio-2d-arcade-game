using Godot;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public partial class AutoCoroutineManager : Node
{
    public static AutoCoroutineManager Instance { get; private set; }

    private Dictionary<string, CancellationTokenSource> _tasks = new();

    public override void _Ready()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            QueueFree();
        }
    }

    public void Run(string key, Func<CancellationToken, Task> taskFunc)
    {
        Stop(key);

        var cts = new CancellationTokenSource();
        _tasks[key] = cts;

        _ = taskFunc(cts.Token);
    }

    public bool IsRunning(string key) => _tasks.ContainsKey(key);

    public void Stop(string key)
    {
        if (_tasks.TryGetValue(key, out var cts))
        {
            cts.Cancel();
            cts.Dispose();
            _tasks.Remove(key);
        }
    }

    public void StopAll()
    {
        foreach (var cts in _tasks.Values)
        {
            cts.Cancel();
            cts.Dispose();
        }
        _tasks.Clear();
    }
}
