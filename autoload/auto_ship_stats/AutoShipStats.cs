using Godot;

public partial class AutoShipStats : Node
{
	public static AutoShipStats Instance { get; private set; }

	public enum UpgradeKind { Health, Speed }

	[Export] private int _baseHealth = 25;
	[Export] private int _baseSpeed = 250;

	[Export] private int _healthPerLevel = 5;
	[Export] private int _speedPerLevel = 10;

	[Export] private int _healthLevel = 0;
	[Export] private int _speedLevel = 0;

	[Export] private int _healthMaxLevel = 100;
	[Export] private int _speedMaxLevel = 20;

	[Export] private int _healthBasePrice = 30;
	[Export] private float _healthPriceRatio = 1.09f;

	[Export] private int _speedBasePrice = 50;
	[Export] private float _speedPriceRatio = 1.5f;

	private const string SavePath = "user://savegame_ship.dat";

	public int Health => _baseHealth + _healthLevel * _healthPerLevel;
	public int Speed => _baseSpeed + _speedLevel * _speedPerLevel;

	public override void _Ready()
	{
		if (Instance == null)
		{
			Instance = this;
			ProcessMode = ProcessModeEnum.Always;
			Load();
		}
		else
		{
			QueueFree();
		}
	}

	public int GetLevel(UpgradeKind kind) => kind switch
	{
		UpgradeKind.Health => _healthLevel,
		UpgradeKind.Speed => _speedLevel,
		_ => 0
	};

	public int GetMaxLevel(UpgradeKind kind) => kind switch
	{
		UpgradeKind.Health => _healthMaxLevel,
		UpgradeKind.Speed => _speedMaxLevel,
		_ => 0
	};

	public int GetPriceAtLevel(UpgradeKind kind, int level)
	{
		int basePrice;
		float ratio;

		if (kind == UpgradeKind.Health)
		{
			basePrice = _healthBasePrice;
			ratio = _healthPriceRatio;
			level = Mathf.Clamp(level, 0, _healthMaxLevel);
		}
		else
		{
			basePrice = _speedBasePrice;
			ratio = _speedPriceRatio;
			level = Mathf.Clamp(level, 0, _speedMaxLevel);
		}

		return Mathf.RoundToInt(basePrice * Mathf.Pow(ratio, level - 1));
	}

	public bool CanUpgrade(UpgradeKind kind) => GetLevel(kind) < GetMaxLevel(kind);

	public bool TryApplyUpgrade(UpgradeKind kind)
	{
		switch (kind)
		{
			case UpgradeKind.Health:
				if (_healthLevel >= _healthMaxLevel) return false;
				_healthLevel++;
				Save();
				return true;

			case UpgradeKind.Speed:
				if (_speedLevel >= _speedMaxLevel) return false;
				_speedLevel++;
				Save();
				return true;
		}
		return false;
	}

	public void Save()
	{
		var data = new Godot.Collections.Dictionary<string, Variant>
		{
			{ "health_level", _healthLevel },
			{ "speed_level",  _speedLevel  }
		};

		using var file = FileAccess.Open(SavePath, FileAccess.ModeFlags.Write);
		file.StoreVar(data);
	}

	public void Load()
	{
		if (!FileAccess.FileExists(SavePath))
		{
			Save();
			return;
		}

		using var file = FileAccess.Open(SavePath, FileAccess.ModeFlags.Read);
		var data = (Godot.Collections.Dictionary)file.GetVar();

		_healthLevel = data.ContainsKey("health_level") ? (int)data["health_level"] : 1;
		_speedLevel = data.ContainsKey("speed_level") ? (int)data["speed_level"] : 1;

	}

	public void Reset()
	{
		_healthLevel = 0;
		_speedLevel = 0;
		Save();
	}
}
