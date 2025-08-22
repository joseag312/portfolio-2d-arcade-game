using Godot;

[GlobalClass]
public partial class WeaponDataComponent : Resource
{
    public const int LeftMuzzle = 0;
    public const int RightMuzzle = 1;
    public const int BothMuzzles = 2;
    public const int CenterCannon = 3;
    public const int Center = 4;

    public enum WeaponSlotType { Basic, Big, Slot }
    public enum WeaponType { Machine, Plasma, Ethereal, Chaos }
    public enum DamageMode { Flat, Percentage }

    [ExportGroup("Weapon - Basic")]
    [Export] public string Key { get; set; } = "";
    [Export] public string DisplayName { get; set; } = "";
    [Export] public string IconPath { get; set; } = "";
    [Export] public string ProjectilePath { get; set; }
    [Export(PropertyHint.Enum, "Left Muzzle,Right Muzzle,Both Muzzles,Center Cannon,Center")]
    public int SpawnLocation { get; set; } = LeftMuzzle;

    [ExportGroup("Weapon - Type")]
    [Export(PropertyHint.Enum, "Basic,Big,Slot")]
    public WeaponSlotType SlotType { get; set; } = WeaponSlotType.Slot;
    [Export(PropertyHint.Enum, "Machine,Plasma,Ethereal,Chaos")]
    public WeaponType Type { get; set; } = WeaponType.Machine;

    [ExportGroup("Weapon - Behavior")]
    [Export(PropertyHint.Enum, "Flat,Percentage")]
    public DamageMode DamageModel { get; set; } = DamageMode.Flat;
    [Export] public bool ConditionalSuccess { get; set; } = false;
    [Export] public string EmitSuccessSignalName { get; set; } = "";

    [ExportGroup("Store - Consumables")]
    [Export] public int UnitPrice { get; set; } = 5;

    [ExportGroup("Upgrades - Properties")]
    [Export] public int Speed { get; set; }
    [Export] public int Damage { get; set; }
    [Export] public int DamagePercentage { get; set; }
    [Export] public float CooldownTime { get; set; } = 0.4f;
    [Export] public int MaxAmount { get; set; } = 10;
    [Export] public bool Unlocked { get; set; } = false;

    [ExportGroup("Upgrades - Toggles")]
    [Export] public bool CanUpgradeSpeed { get; set; } = true;
    [Export] public bool CanUpgradeDamage { get; set; } = true;
    [Export] public bool CanUpgradeDamagePct { get; set; } = false;
    [Export] public bool CanUpgradeCooldown { get; set; } = true;
    [Export] public bool CanUpgradeStorage { get; set; } = true;

    [ExportGroup("Upgrades - Max Levels")]
    [Export] public int MaxSpeedUpgrades { get; set; } = 25;
    [Export] public int MaxDamageUpgrades { get; set; } = 25;
    [Export] public int MaxDamagePctUpgrades { get; set; } = 25;
    [Export] public int MaxCooldownUpgrades { get; set; } = 25;
    [Export] public int MaxStorageUpgrades { get; set; } = 48;

    [ExportGroup("Upgrades - Steps per Level")]
    [Export] public int SpeedUpgradeStep { get; set; } = 5;
    [Export] public int DamageUpgradeStep { get; set; } = 1;
    [Export] public int DamagePctUpgradeStep { get; set; } = 1;
    [Export] public float CooldownUpgradeStep { get; set; } = 0.05f;
    [Export] public int StorageUpgradeStep { get; set; } = 5;

    [ExportGroup("Upgrades - Base Prices")]
    [Export] public int BaseSpeedPrice { get; set; } = 1;
    [Export] public int BaseDamagePrice { get; set; } = 1;
    [Export] public int BaseDamagePctPrice { get; set; } = 1;
    [Export] public int BaseCooldownPrice { get; set; } = 1;
    [Export] public int BaseStoragePrice { get; set; } = 1;

    [ExportGroup("Upgrades - Price Growth")]
    [Export] public float GlobalPriceGrowth { get; set; } = 1.5f;
    [Export] public float SpeedPriceGrowth { get; set; } = -1f;
    [Export] public float DamagePriceGrowth { get; set; } = -1f;
    [Export] public float DamagePctPriceGrowth { get; set; } = -1f;
    [Export] public float CooldownPriceGrowth { get; set; } = -1f;
    [Export] public float StoragePriceGrowth { get; set; } = 1.05f;
}
