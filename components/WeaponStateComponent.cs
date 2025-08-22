using Godot;
using System;

[GlobalClass]
public partial class WeaponStateComponent : Resource
{
    private const float MinimumCooldownTime = 0.1f;
    public enum UpgradeKind { Speed, Damage, DamagePct, Cooldown, Storage }

    public WeaponDataComponent BaseData { get; set; }

    [ExportGroup("State - Basic")]
    [Export] public string Key { get; set; } = "";
    [Export] public int CurrentAmount { get; set; } = 0;
    [Export] public float CooldownRemaining { get; set; } = 0f;

    [ExportGroup("Upgrade Levels")]
    [Export] public int SpeedUpgradeLevel { get; set; } = 0;
    [Export] public int DamageUpgradeLevel { get; set; } = 0;
    [Export] public int DamagePctUpgradeLevel { get; set; } = 0;
    [Export] public int CooldownUpgradeLevel { get; set; } = 0;
    [Export] public int StorageUpgradeLevel { get; set; } = 0;

    [ExportGroup("Overrides")]
    [Export] public int OverrideSpeed { get; set; } = -1;
    [Export] public int OverrideDamage { get; set; } = -1;
    [Export] public int OverrideDamagePercentage { get; set; } = -1;
    [Export] public float OverrideCooldownTime { get; set; } = -1f;
    [Export] public int OverrideMaxAmount { get; set; } = -1;
    [Export] public bool OverrideUnlocked { get; set; } = false;
    [Export] public bool UseOverrideUnlocked { get; set; } = false;

    public bool HasOverrideSpeed => OverrideSpeed >= 0;
    public bool HasOverrideDamage => OverrideDamage >= 0;
    public bool HasOverrideDamagePercentage => OverrideDamagePercentage >= 0;
    public bool HasOverrideCooldown => OverrideCooldownTime >= 0f;
    public bool HasOverrideMaxAmount => OverrideMaxAmount >= 0;
    public bool HasOverrideUnlocked => UseOverrideUnlocked;

    public int EffectiveSpeed => HasOverrideSpeed ? OverrideSpeed : (BaseData?.Speed ?? 0);
    public int EffectiveDamage => HasOverrideDamage ? OverrideDamage : (BaseData?.Damage ?? 0);
    public int EffectiveDamagePercentage => HasOverrideDamagePercentage ? OverrideDamagePercentage : (BaseData?.DamagePercentage ?? 0);
    public float EffectiveCooldown
    {
        get
        {
            float raw = HasOverrideCooldown ? OverrideCooldownTime : (BaseData?.CooldownTime ?? 0f);
            return Mathf.Max(raw, MinimumCooldownTime);
        }
    }
    public int EffectiveMaxAmount => HasOverrideMaxAmount ? OverrideMaxAmount : (BaseData?.MaxAmount ?? 0);
    public bool EffectiveUnlocked => HasOverrideUnlocked ? OverrideUnlocked : (BaseData?.Unlocked ?? false);

    public WeaponStateComponent() { }
    public WeaponStateComponent(string key) { Key = key; }

    public bool IsUpgradeEnabled(UpgradeKind kind)
    {
        if (BaseData == null) return false;

        return kind switch
        {
            UpgradeKind.Speed => BaseData.CanUpgradeSpeed,
            UpgradeKind.Damage => BaseData.DamageModel == WeaponDataComponent.DamageMode.Flat
                                     && BaseData.CanUpgradeDamage,
            UpgradeKind.DamagePct => BaseData.DamageModel == WeaponDataComponent.DamageMode.Percentage
                                     && BaseData.CanUpgradeDamagePct,
            UpgradeKind.Cooldown => BaseData.CanUpgradeCooldown,
            UpgradeKind.Storage => BaseData.CanUpgradeStorage,
            _ => false
        };
    }

    public int GetMaxUpgrades(UpgradeKind kind)
    {
        if (BaseData == null) return 0;
        if (!IsUpgradeEnabled(kind)) return 0;

        return kind switch
        {
            UpgradeKind.Speed => BaseData.MaxSpeedUpgrades,
            UpgradeKind.Damage => BaseData.MaxDamageUpgrades,
            UpgradeKind.DamagePct => BaseData.MaxDamagePctUpgrades,
            UpgradeKind.Cooldown => BaseData.MaxCooldownUpgrades,
            UpgradeKind.Storage => BaseData.MaxStorageUpgrades,
            _ => 0
        };
    }

    public int GetLevel(UpgradeKind kind) => kind switch
    {
        UpgradeKind.Speed => SpeedUpgradeLevel,
        UpgradeKind.Damage => DamageUpgradeLevel,
        UpgradeKind.DamagePct => DamagePctUpgradeLevel,
        UpgradeKind.Cooldown => CooldownUpgradeLevel,
        UpgradeKind.Storage => StorageUpgradeLevel,
        _ => 0
    };

    public int GetPriceAtLevel(UpgradeKind kind, int level)
    {
        if (BaseData == null || !IsUpgradeEnabled(kind))
            return int.MaxValue;

        int basePrice = kind switch
        {
            UpgradeKind.Speed => BaseData.BaseSpeedPrice,
            UpgradeKind.Damage => BaseData.BaseDamagePrice,
            UpgradeKind.DamagePct => BaseData.BaseDamagePctPrice,
            UpgradeKind.Cooldown => BaseData.BaseCooldownPrice,
            UpgradeKind.Storage => BaseData.BaseStoragePrice,
            _ => 0
        };

        float growth = kind switch
        {
            UpgradeKind.Speed => (BaseData.SpeedPriceGrowth > 0 ? BaseData.SpeedPriceGrowth : BaseData.GlobalPriceGrowth),
            UpgradeKind.Damage => (BaseData.DamagePriceGrowth > 0 ? BaseData.DamagePriceGrowth : BaseData.GlobalPriceGrowth),
            UpgradeKind.DamagePct => (BaseData.DamagePctPriceGrowth > 0 ? BaseData.DamagePctPriceGrowth : BaseData.GlobalPriceGrowth),
            UpgradeKind.Cooldown => (BaseData.CooldownPriceGrowth > 0 ? BaseData.CooldownPriceGrowth : BaseData.GlobalPriceGrowth),
            UpgradeKind.Storage => (BaseData.StoragePriceGrowth > 0 ? BaseData.StoragePriceGrowth : BaseData.GlobalPriceGrowth),
            _ => BaseData.GlobalPriceGrowth
        };

        double price = basePrice * Math.Pow(growth, level);
        return Mathf.CeilToInt((float)price);
    }

    public int GetNextPrice(UpgradeKind kind)
    {
        return GetPriceAtLevel(kind, GetLevel(kind));
    }

    public float PeekNextValue(UpgradeKind kind)
    {
        if (BaseData == null) return 0f;

        int nextLevel = GetLevel(kind) + 1;

        return kind switch
        {
            UpgradeKind.Speed => BaseData.Speed + nextLevel * BaseData.SpeedUpgradeStep,
            UpgradeKind.Damage => BaseData.Damage + nextLevel * BaseData.DamageUpgradeStep,
            UpgradeKind.DamagePct => BaseData.DamagePercentage + nextLevel * BaseData.DamagePctUpgradeStep,
            UpgradeKind.Cooldown => Math.Max(MinimumCooldownTime, BaseData.CooldownTime - nextLevel * BaseData.CooldownUpgradeStep),
            UpgradeKind.Storage => BaseData.MaxAmount + nextLevel * BaseData.StorageUpgradeStep,
            _ => 0f
        };
    }

    public bool TryApplyUpgrade(UpgradeKind kind)
    {
        if (BaseData == null || !IsUpgradeEnabled(kind)) return false;

        int level = GetLevel(kind);
        int max = GetMaxUpgrades(kind);
        if (level >= max) return false;

        switch (kind)
        {
            case UpgradeKind.Speed:
                SpeedUpgradeLevel++;
                OverrideSpeed = BaseData.Speed + SpeedUpgradeLevel * BaseData.SpeedUpgradeStep;
                break;

            case UpgradeKind.Damage:
                if (BaseData.DamageModel != WeaponDataComponent.DamageMode.Flat) return false;
                DamageUpgradeLevel++;
                OverrideDamage = BaseData.Damage + DamageUpgradeLevel * BaseData.DamageUpgradeStep;
                break;

            case UpgradeKind.DamagePct:
                if (BaseData.DamageModel != WeaponDataComponent.DamageMode.Percentage) return false;
                DamagePctUpgradeLevel++;
                OverrideDamagePercentage = BaseData.DamagePercentage + DamagePctUpgradeLevel * BaseData.DamagePctUpgradeStep;
                break;

            case UpgradeKind.Cooldown:
                CooldownUpgradeLevel++;
                OverrideCooldownTime = Math.Max(
                    MinimumCooldownTime,
                    BaseData.CooldownTime - (CooldownUpgradeLevel * BaseData.CooldownUpgradeStep)
                );
                break;

            case UpgradeKind.Storage:
                StorageUpgradeLevel++;
                OverrideMaxAmount = BaseData.MaxAmount + StorageUpgradeLevel * BaseData.StorageUpgradeStep;
                break;
        }

        return true;
    }

    public void SyncOverridesFromLevels()
    {
        if (BaseData == null) return;

        OverrideSpeed = SpeedUpgradeLevel > 0
            ? BaseData.Speed + SpeedUpgradeLevel * BaseData.SpeedUpgradeStep
            : -1;

        if (BaseData.DamageModel == WeaponDataComponent.DamageMode.Flat)
        {
            OverrideDamage = DamageUpgradeLevel > 0
                ? BaseData.Damage + DamageUpgradeLevel * BaseData.DamageUpgradeStep
                : -1;
            OverrideDamagePercentage = -1;
        }
        else
        {
            OverrideDamagePercentage = DamagePctUpgradeLevel > 0
                ? BaseData.DamagePercentage + DamagePctUpgradeLevel * BaseData.DamagePctUpgradeStep
                : -1;
            OverrideDamage = -1;
        }

        OverrideCooldownTime = CooldownUpgradeLevel > 0
            ? Math.Max(MinimumCooldownTime, BaseData.CooldownTime - CooldownUpgradeLevel * BaseData.CooldownUpgradeStep)
            : -1f;

        OverrideMaxAmount = StorageUpgradeLevel > 0
            ? BaseData.MaxAmount + StorageUpgradeLevel * BaseData.StorageUpgradeStep
            : -1;
    }

    public override string ToString()
    {
        return
        $@"
        WeaponStateComponent {{
        Key: {Key}
        BaseData: {(BaseData != null ? BaseData.Key : "null")}
        SlotType: {BaseData?.SlotType}
        WeaponType: {BaseData?.Type}
        DamageModel: {BaseData?.DamageModel}

        --- Effective ---
        Speed: {EffectiveSpeed} {(HasOverrideSpeed ? "(Override)" : "(Base)")}
        Damage: {EffectiveDamage} {(HasOverrideDamage ? "(Override)" : "(Base)")}
        Damage%: {EffectiveDamagePercentage} {(HasOverrideDamagePercentage ? "(Override)" : "(Base)")}
        Cooldown: {EffectiveCooldown:0.00}s {(HasOverrideCooldown ? "(Override)" : "(Base)")}
        MaxAmount: {EffectiveMaxAmount} {(HasOverrideMaxAmount ? "(Override)" : "(Base)")}
        Unlocked: {EffectiveUnlocked} {(HasOverrideUnlocked ? "(Override)" : "(Base)")}

        --- Current ---
        CurrentAmount: {CurrentAmount}
        CooldownRemaining: {CooldownRemaining:0.00}s

        --- Upgrade Levels ---
        Speed Lvl: {SpeedUpgradeLevel} / {GetMaxUpgrades(UpgradeKind.Speed)}
        Damage Lvl: {DamageUpgradeLevel} / {GetMaxUpgrades(UpgradeKind.Damage)}
        Damage% Lvl: {DamagePctUpgradeLevel} / {GetMaxUpgrades(UpgradeKind.DamagePct)}
        Cooldown Lvl: {CooldownUpgradeLevel} / {GetMaxUpgrades(UpgradeKind.Cooldown)}
        Storage Lvl: {StorageUpgradeLevel} / {GetMaxUpgrades(UpgradeKind.Storage)}

        --- Overrides Raw ---
        OverrideSpeed: {OverrideSpeed}
        OverrideDamage: {OverrideDamage}
        OverrideDamage%: {OverrideDamagePercentage}
        OverrideCooldown: {OverrideCooldownTime}
        OverrideMaxAmount: {OverrideMaxAmount}
        OverrideUnlocked: {OverrideUnlocked}
        UseOverrideUnlocked: {UseOverrideUnlocked}
        }}";
    }
}
