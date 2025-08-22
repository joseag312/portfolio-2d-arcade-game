using System;
using System.Collections.Generic;
using Godot;

[GlobalClass]
public partial class WeaponManagerComponent : Node
{
	[Signal]
	public delegate void WeaponFiredEventHandler(int slot, WeaponStateComponent weapon);

	[Signal]
	public delegate void PowerUsedEventHandler(int slot, WeaponStateComponent weapon);

	[Export] private NodePath _leftMuzzlePath;
	[Export] private NodePath _rightMuzzlePath;
	[Export] private NodePath _centerCannonPath;
	[Export] private NodePath _centerPath;

	private Marker2D _center;
	private Marker2D _leftMuzzle;
	private Marker2D _rightMuzzle;
	private Marker2D _centerCannon;

	private WeaponStateComponent _basicWeapon;
	private WeaponStateComponent _bigWeapon;
	private WeaponStateComponent[] _specialWeapons = new WeaponStateComponent[4];

	private Dictionary<string, float> _weaponCooldowns = new();
	private Dictionary<string, float> _currentCooldowns = new();

	public override void _Ready()
	{
		ProcessMode = ProcessModeEnum.Always;
		_center = GetNode<Marker2D>(_centerPath);
		_leftMuzzle = GetNode<Marker2D>(_leftMuzzlePath);
		_rightMuzzle = GetNode<Marker2D>(_rightMuzzlePath);
		_centerCannon = GetNode<Marker2D>(_centerCannonPath);
		AssignWeapons();
	}

	public override void _Process(double delta)
	{
		if (_currentCooldowns.Count == 0)
			return;

		foreach (var key in _currentCooldowns.Keys)
		{
			if (_currentCooldowns[key] > 0f)
			{
				_currentCooldowns[key] -= (float)delta;
				if (_currentCooldowns[key] < 0f)
					_currentCooldowns[key] = 0f;
			}
		}
	}

	public void AssignWeapons()
	{
		var inventory = G.WI;

		string basicKey = inventory.GetEquippedWeaponKey(WeaponSlotNames.Basic);
		string bigKey = inventory.GetEquippedWeaponKey(WeaponSlotNames.Big);
		string[] slotKeys =
		{
			inventory.GetEquippedWeaponKey(WeaponSlotNames.Special1),
			inventory.GetEquippedWeaponKey(WeaponSlotNames.Special2),
			inventory.GetEquippedWeaponKey(WeaponSlotNames.Special3),
			inventory.GetEquippedWeaponKey(WeaponSlotNames.Special4)
		};

		_basicWeapon = null;
		_bigWeapon = null;
		for (int i = 0; i < 4; i++)
			_specialWeapons[i] = null;

		if (!string.IsNullOrEmpty(basicKey))
			_basicWeapon = inventory.GetWeaponState(basicKey);

		if (!string.IsNullOrEmpty(bigKey))
			_bigWeapon = inventory.GetWeaponState(bigKey);

		for (int i = 0; i < 4; i++)
		{
			if (!string.IsNullOrEmpty(slotKeys[i]))
				_specialWeapons[i] = inventory.GetWeaponState(slotKeys[i]);
		}

		_weaponCooldowns.Clear();
		_currentCooldowns.Clear();

		if (_basicWeapon?.BaseData != null)
			_weaponCooldowns[basicKey] = _basicWeapon.EffectiveCooldown;

		if (_bigWeapon?.BaseData != null)
			_weaponCooldowns[bigKey] = _bigWeapon.EffectiveCooldown;

		for (int i = 0; i < 4; i++)
		{
			var state = _specialWeapons[i];
			if (state?.BaseData != null)
				_weaponCooldowns[slotKeys[i]] = state.EffectiveCooldown;
		}

		foreach (var key in _weaponCooldowns.Keys)
			_currentCooldowns[key] = 0f;
	}

	public void SpawnWeapon(int weaponSlot, Node2D targetContainer)
	{
		WeaponStateComponent weapon = weaponSlot == 0 ? _basicWeapon :
									  weaponSlot == 1 ? _bigWeapon :
									  _specialWeapons[weaponSlot - 2];

		if (weapon == null || weapon.BaseData == null)
			return;

		if (_currentCooldowns.TryGetValue(weapon.Key, out float value) && value > 0)
			return;

		if (weapon.BaseData.SlotType == WeaponDataComponent.WeaponSlotType.Slot && weapon.CurrentAmount == 0)
			return;

		var scenePath = weapon.BaseData.ProjectilePath;
		PackedScene weaponScene = ResourceLoader.Load<PackedScene>(scenePath);
		if (weaponScene == null)
			return;

		bool isConditional = weapon.BaseData.ConditionalSuccess;

		switch (weapon.BaseData.SpawnLocation)
		{
			case WeaponDataComponent.LeftMuzzle:
				SpawnAtPosition(weaponScene, _leftMuzzle.GlobalPosition, WeaponDataComponent.LeftMuzzle, targetContainer, isConditional, weaponSlot, weapon);
				break;

			case WeaponDataComponent.RightMuzzle:
				SpawnAtPosition(weaponScene, _rightMuzzle.GlobalPosition, WeaponDataComponent.RightMuzzle, targetContainer, isConditional, weaponSlot, weapon);
				break;

			case WeaponDataComponent.BothMuzzles:
				SpawnAtPosition(weaponScene, _leftMuzzle.GlobalPosition, WeaponDataComponent.LeftMuzzle, targetContainer, isConditional, weaponSlot, weapon);
				SpawnAtPosition(weaponScene, _rightMuzzle.GlobalPosition, WeaponDataComponent.RightMuzzle, targetContainer, isConditional, weaponSlot, weapon);
				break;

			case WeaponDataComponent.CenterCannon:
				SpawnAtPosition(weaponScene, _centerCannon.GlobalPosition, WeaponDataComponent.CenterCannon, targetContainer, isConditional, weaponSlot, weapon);
				break;

			case WeaponDataComponent.Center:
				SpawnAtPosition(weaponScene, _center.GlobalPosition, WeaponDataComponent.Center, targetContainer, isConditional, weaponSlot, weapon);
				break;

			default:
				return;
		}

		if (weaponSlot > 0)
		{
			EmitSignal(SignalName.WeaponFired, weaponSlot, weapon);
		}
	}

	private void SpawnAtPosition(PackedScene weaponScene, Vector2 position, int locationType, Node2D targetContainer,
							 bool isConditional, int weaponSlot, WeaponStateComponent weapon)
	{
		Node2D weaponInstance = (Node2D)weaponScene.Instantiate();
		weaponInstance.AddToGroup("despawnable");

		if (locationType == WeaponDataComponent.Center)
		{
			Node2D ship = GetParent<Node2D>();
			ship.AddChild(weaponInstance);
			weaponInstance.Position = ship.ToLocal(position);

			if (weaponInstance is IEffectSpawningWeapon effectSpawner)
			{
				effectSpawner.EffectContainer = targetContainer;
			}
		}
		else
		{
			if (targetContainer == null)
			{
				Log.Err("Target Container is null. Cannot add Projectile");
				return;
			}

			weaponInstance.GlobalPosition = position;
			targetContainer.AddChild(weaponInstance);
		}

		string successSignal = weapon.BaseData.EmitSuccessSignalName;
		if (isConditional && !string.IsNullOrEmpty(successSignal) && weaponInstance.HasSignal(successSignal))
		{
			try
			{
				weaponInstance.Connect(successSignal, Callable.From(() => OnPowerSucceeded(weaponSlot, weapon)));
			}
			catch (Exception e)
			{
				Log.Err($"Failed to connect Signal '{successSignal}': {e.Message}");
			}
		}
		else if (!isConditional)
		{
			OnPowerSucceeded(weaponSlot, weapon);
		}
	}

	private void OnPowerSucceeded(int weaponSlot, WeaponStateComponent weapon)
	{
		_currentCooldowns[weapon.Key] = _weaponCooldowns[weapon.Key];

		if (weapon.BaseData.SlotType == WeaponDataComponent.WeaponSlotType.Slot)
		{
			if (weapon.CurrentAmount <= 0)
			{
				Log.Err($"Tried to use Weapon '{weapon.Key}' but amount is already 0");
			}
			else
			{
				weapon.CurrentAmount -= 1;
			}
		}

		if (weaponSlot > 0)
			EmitSignal(SignalName.PowerUsed, weaponSlot, weapon);
	}
}