using System;
using Godot;

[GlobalClass]
public partial class WeaponSpawnComponent : Node
{
	[Export] private ScaleComponent _scaleComponent;
	[Export] private FlashComponent _flashComponent;
	[Export] private HitboxComponent _hitboxComponent;
	[Export] private Boolean _isEnemyWeapon;

	private float _multiplier;

	public override void _Ready()
	{
		_flashComponent?.Flash();
		_scaleComponent?.TweenScale();
		_hitboxComponent.HitHurtbox += OnWeaponHit;

		var parent = GetParent();
		if (_isEnemyWeapon)
		{
			_multiplier = Mathf.RoundToInt(G.GS.LevelMultiplier - 1);
			_multiplier *= G.GS.EnemyDamageMultiplier;
			_multiplier *= G.GS.EventEnemyDamageMultiplier;
			_hitboxComponent.Damage += Mathf.RoundToInt(_hitboxComponent.Damage * _multiplier); ;
		}
	}

	private void OnWeaponHit(HurtboxComponent hurtbox)
	{
		GetParent().QueueFree();
	}
}
