using Godot;

[GlobalClass]
public partial class StatsComponent : Node
{
    [Signal] public delegate void HealthChangedEventHandler();
    [Signal] public delegate void NoHealthEventHandler();

    [Export] public int MaxHealth { get; set; }
    [Export]
    public int Health
    {
        get => _health;
        set
        {
            _health = value;
            EmitSignal(SignalName.HealthChanged);
            if (_health <= 0)
                EmitSignal(SignalName.NoHealth);
        }
    }

    private int _health = 1;
    private float _multiplier;

    public override void _Ready()
    {
        if (GetParent() is not Ship)
        {
            _multiplier = Mathf.RoundToInt(G.GS.LevelMultiplier - 1);
            _multiplier *= G.GS.EnemyHealthMultiplier;
            _multiplier *= G.GS.EventEnemyHealthMultiplier;
            MaxHealth += Mathf.RoundToInt(MaxHealth * _multiplier);
        }

        _health = MaxHealth;
    }
}
