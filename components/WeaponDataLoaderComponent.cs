using Godot;
using System;

[GlobalClass]
public partial class WeaponDataLoaderComponent : Node
{
	[Export] public string WeaponKey = "basic_blaster";
	[Export] public HitboxComponent HitboxComponent;
	[Export] public MoveComponent MoveComponent;

	public override void _Ready()
	{
		var state = G.WI.GetWeaponState(WeaponKey);
		if (state == null)
		{
			Log.Err($"No WeaponState found for '{WeaponKey}'");
			return;
		}

		if (state.BaseData == null)
		{
			state.BaseData = G.WD.GetWeaponData(WeaponKey);
			if (state.BaseData == null)
			{
				Log.Err($"No BaseData found for '{WeaponKey}'");
				return;
			}
		}

		if (MoveComponent != null)
		{
			MoveComponent.Velocity = new Vector2(0, -state.EffectiveSpeed);
		}
		if (HitboxComponent != null)
		{
			HitboxComponent.Damage = state.EffectiveDamage;
			HitboxComponent.DamagePercentage = state.EffectiveDamagePercentage;
		}
	}
}
