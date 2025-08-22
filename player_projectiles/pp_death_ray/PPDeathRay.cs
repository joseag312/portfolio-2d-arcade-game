using Godot;

[GlobalClass]
public partial class PPDeathRay : Node2D
{
    [Export] public Timer LifetimeTimer;

    public override void _Ready()
    {
        if (LifetimeTimer == null)
        {
            Log.Err("LifetimeTimer not set");
            QueueFree();
            return;
        }

        G.SFX.Play(SFX.RAY);
        LifetimeTimer.Timeout += OnLifetimeExpired;
        LifetimeTimer.Start();
    }

    private void OnLifetimeExpired()
    {
        QueueFree();
    }
}
