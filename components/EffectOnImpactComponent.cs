using Godot;

[GlobalClass]
public partial class EffectOnImpactComponent : Node
{
    [Export] public string EffectScene;
    [Export] private HitboxComponent _hitboxComponent;
    [Export] private Marker2D _position;

    public override void _Ready()
    {
        _hitboxComponent.HitHurtbox += SpawnEffect;
        _hitboxComponent.Damage = 0;
        _hitboxComponent.DamagePercentage = 0;
    }

    private void SpawnEffect(HurtboxComponent hurtbox)
    {
        Node targetParent = GetParent()?.GetParent();
        if (targetParent == null)
        {
            Log.Err("Could not find target Node");
            return;
        }

        if (string.IsNullOrEmpty(EffectScene))
        {
            Log.Err("EffectScene path is not set");
            return;
        }

        var packedScene = GD.Load<PackedScene>(EffectScene);
        if (packedScene == null)
        {
            Log.Err($"Could not load scene at path {EffectScene}");
            return;
        }

        var instance = packedScene.Instantiate<Node2D>();
        if (instance == null)
        {
            Log.Err("Instantiated Effect is not a Node2D");
            return;
        }

        if (_position != null)
            instance.GlobalPosition = _position.GlobalPosition;
        else
            instance.GlobalPosition = GetParent<Node2D>().GlobalPosition;

        targetParent.CallDeferred("add_child", instance);
    }
}
