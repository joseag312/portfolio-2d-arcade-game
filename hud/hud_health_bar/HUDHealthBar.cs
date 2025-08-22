using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;

[GlobalClass]
public partial class HUDHealthBar : Control
{
	[Export] public TextureProgressBar HealthBar { get; set; }
	[Export] public TextureProgressBar BackgroundBar { get; set; }
	[Export] public Label HealthValue { get; set; }
	[Export] private float _lostHealthDuration = 0.5f;

	private StatsComponent _statsComponent;
	private Tween _tween;

	public override void _Ready()
	{
		Node2D ship = GetTree().CurrentScene.GetNodeOrNull<Node2D>("ShipContainer/Ship");
		if (ship != null)
		{
			ship.Connect("ShipReady", new Callable(this, nameof(InitFromShip)));
		}
	}

	private void InitFromShip()
	{
		Node2D ship = GetTree().CurrentScene.GetNodeOrNull<Node2D>("ShipContainer/Ship");
		if (ship == null) return;

		_statsComponent = ship.GetNodeOrNull<StatsComponent>("StatsComponent");
		ship.Connect(nameof(Ship.HealthChanged), new Callable(this, nameof(OnHealthChanged)));

		if (_statsComponent != null)
		{
			HealthBar.MaxValue = _statsComponent.MaxHealth;
			HealthBar.Value = _statsComponent.Health;
			HealthValue.Text = $"{_statsComponent.Health}/{_statsComponent.MaxHealth}";
		}

		HealthBar.Size = new Vector2(200, 200);
		Position = new Vector2(20, 40);
	}

	private void OnHealthChanged(int oldHealth, int newHealth)
	{
		int delta = newHealth - oldHealth;
		if (delta == 0)
		{
			return;
		}

		if (newHealth < 0)
		{
			newHealth = 0;
		}
		HealthBar.Value = newHealth;
		HealthValue.Text = $"{newHealth}/{_statsComponent.MaxHealth}";
		Color originalColor = new Color(97f / 255f, 1f, 1f, 1f);
		Color flashColor = delta < 0
			? new Color(1f, 0.6f, 0.6f, 1f)
			: new Color(0.8f, 0.90f, 0.4f, 1f);

		var tween = GetTree().CreateTween();

		tween.TweenProperty(HealthBar, "modulate", flashColor, 0.1f)
			 .SetTrans(Tween.TransitionType.Linear);

		tween.TweenInterval(0.1f);

		tween.TweenProperty(HealthBar, "modulate", originalColor, 0.2f)
			 .SetTrans(Tween.TransitionType.Linear);
	}
}
