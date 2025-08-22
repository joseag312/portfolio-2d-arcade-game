using Godot;
using System.Collections.Generic;

public partial class AutoWeaponInventory : Node
{
    public static AutoWeaponInventory Instance { get; private set; }

    private const string SavePath = "user://savegame_weapons.dat";

    private Dictionary<string, WeaponStateComponent> _weaponStates = new();
    private Dictionary<string, string> _equippedWeapons = new();

    public override void _Ready()
    {
        if (Instance == null)
        {
            Instance = this;
            Load();
        }
        else
        {
            QueueFree();
        }
    }

    public void Save()
    {
        var weaponArray = new Godot.Collections.Array<Godot.Collections.Dictionary>();

        foreach (var pair in _weaponStates)
        {
            var s = pair.Value;

            var dict = new Godot.Collections.Dictionary
        {
            { "key", s.Key },
            { "amount", s.CurrentAmount },
            { "cooldown", s.CooldownRemaining },

            { "speed_level",        s.SpeedUpgradeLevel },
            { "damage_level",       s.DamageUpgradeLevel },
            { "damage_pct_level",   s.DamagePctUpgradeLevel },
            { "cooldown_level",     s.CooldownUpgradeLevel },
            { "storage_level",      s.StorageUpgradeLevel },

            { "override_speed",     s.OverrideSpeed },
            { "override_dmg",       s.OverrideDamage },
            { "override_dmg_pct",   s.OverrideDamagePercentage },
            { "override_cd",        s.OverrideCooldownTime },
            { "override_max",       s.OverrideMaxAmount },

            { "override_unlocked",      s.OverrideUnlocked },
            { "use_override_unlocked",  s.UseOverrideUnlocked }
        };

            weaponArray.Add(dict);
        }

        var equippedDict = new Godot.Collections.Dictionary();
        foreach (var pair in _equippedWeapons)
            equippedDict[pair.Key] = pair.Value ?? "";

        var root = new Godot.Collections.Dictionary
        {
            { "v", 2 },
            { "weapons", weaponArray },
            { "equipped", equippedDict }
        };

        using var file = FileAccess.Open(SavePath, FileAccess.ModeFlags.Write);
        file.StoreVar(root);
    }

    public void Load()
    {
        _weaponStates.Clear();
        _equippedWeapons.Clear();

        if (!FileAccess.FileExists(SavePath))
        {
            DefaultWeaponEquip();
            Save();
            return;
        }

        using var file = FileAccess.Open(SavePath, FileAccess.ModeFlags.Read);
        var root = (Godot.Collections.Dictionary)file.GetVar();

        if (root.TryGetValue("weapons", out var weaponsRaw))
        {
            foreach (Godot.Collections.Dictionary entry in (Godot.Collections.Array)weaponsRaw)
            {
                var key = (string)entry["key"];

                var baseData = G.WD.GetWeaponData(key);
                if (baseData == null)
                {
                    Log.Err($"Missing BaseData for '{key}'");
                    continue;
                }

                var s = new WeaponStateComponent(key)
                {
                    BaseData = baseData,

                    CurrentAmount = (int)entry["amount"],
                    CooldownRemaining = (float)entry["cooldown"],

                    SpeedUpgradeLevel = (int)entry["speed_level"],
                    DamageUpgradeLevel = (int)entry["damage_level"],
                    DamagePctUpgradeLevel = (int)entry["damage_pct_level"],
                    CooldownUpgradeLevel = (int)entry["cooldown_level"],
                    StorageUpgradeLevel = (int)entry["storage_level"],

                    OverrideUnlocked = (bool)entry["override_unlocked"],
                    UseOverrideUnlocked = (bool)entry["use_override_unlocked"],
                };

                s.SyncOverridesFromLevels();

                _weaponStates[key] = s;
            }
        }

        if (root.TryGetValue("equipped", out var equippedRaw))
        {
            foreach (string slot in ((Godot.Collections.Dictionary)equippedRaw).Keys)
            {
                string value = (string)((Godot.Collections.Dictionary)equippedRaw)[slot];
                _equippedWeapons[slot] = string.IsNullOrEmpty(value) ? null : value;
            }
        }
        else
        {
            DefaultWeaponEquip();
        }
    }

    public void Reset()
    {
        _weaponStates.Clear();
        _equippedWeapons.Clear();
        DefaultWeaponEquip();
        Save();
    }

    private void AddWeaponState(string key)
    {
        if (!_weaponStates.ContainsKey(key))
        {
            _weaponStates[key] = DefaultWeaponState(key);
        }
    }

    public WeaponStateComponent GetWeaponState(string key)
    {
        if (_weaponStates.TryGetValue(key, out var data))
            return data;

        var instance = DefaultWeaponState(key);
        _weaponStates[key] = instance;
        return instance;
    }

    public string GetEquippedWeaponKey(string slot)
    {
        return _equippedWeapons.TryGetValue(slot, out var key) ? key : null;
    }

    public WeaponStateComponent GetEquippedWeaponState(string slot)
    {
        string key = GetEquippedWeaponKey(slot);
        return key != null ? GetWeaponState(key) : null;
    }

    public void EquipBasicWeapon(string weaponKey)
    {
        var data = G.WD.GetWeaponData(weaponKey);
        if (data == null || data.SlotType != WeaponDataComponent.WeaponSlotType.Basic)
        {
            Log.Err($"'{weaponKey}' is not a Basic Weapon");
            return;
        }

        _equippedWeapons[WeaponSlotNames.Basic] = weaponKey;
        AddWeaponState(weaponKey);
    }

    public void EquipBigWeapon(string weaponKey)
    {
        var data = G.WD.GetWeaponData(weaponKey);
        if (data == null || data.SlotType != WeaponDataComponent.WeaponSlotType.Big)
        {
            Log.Err($"'{weaponKey}' is not a Big Weapon");
            return;
        }

        _equippedWeapons[WeaponSlotNames.Big] = weaponKey;
        AddWeaponState(weaponKey);
    }

    public void EquipSlotWeapon(int index, string weaponKey)
    {
        if (index < 1 || index > 4)
        {
            Log.Err("Slot index must be 1–4");
            return;
        }

        string targetSlot = WeaponSlotNames.GetSpecialSlot(index);

        if (string.IsNullOrWhiteSpace(weaponKey))
        {
            _equippedWeapons[targetSlot] = null;
            Save();
            return;
        }

        var data = G.WD.GetWeaponData(weaponKey);
        if (data == null || data.SlotType != WeaponDataComponent.WeaponSlotType.Slot)
        {
            Log.Err($"'{weaponKey}' is not a Slot Weapon");
            return;
        }

        foreach (var kvp in _equippedWeapons)
        {
            if (kvp.Key != targetSlot && kvp.Value == weaponKey && kvp.Key.StartsWith("special_"))
            {
                _equippedWeapons[kvp.Key] = null;
            }
        }

        _equippedWeapons[targetSlot] = weaponKey;
        AddWeaponState(weaponKey);
        Save();
    }

    public void UnequipSlotWeapon(int index)
    {
        EquipSlotWeapon(index, null);
    }


    private void DefaultWeaponEquip()
    {
        EquipBasicWeapon("PPBasicBlue");
        EquipBigWeapon("PPBigBlue");
        EquipSlotWeapon(1, "PPMissileMach");
        EquipSlotWeapon(2, "");
        EquipSlotWeapon(3, "");
        EquipSlotWeapon(4, "");
    }

    private WeaponStateComponent DefaultWeaponState(string key)
    {
        var baseData = G.WD.GetWeaponData(key);
        if (baseData == null)
        {
            Log.Err($"No BaseData found for Weapon Key '{key}'");
            return new WeaponStateComponent(key);
        }

        bool isSlotWeapon = baseData.SlotType == WeaponDataComponent.WeaponSlotType.Slot;

        var state = new WeaponStateComponent(key)
        {
            BaseData = baseData,
            CurrentAmount = isSlotWeapon ? 5 : 0,
            CooldownRemaining = 0f,
            OverrideSpeed = -1,
            OverrideDamage = -1,
            OverrideDamagePercentage = -1,
            OverrideCooldownTime = -1f,
            OverrideMaxAmount = -1,
            OverrideUnlocked = false,
            UseOverrideUnlocked = false
        };

        return state;
    }

    public void DebugPrintSaveContents()
    {
        if (!FileAccess.FileExists(SavePath))
        {
            return;
        }

        using var file = FileAccess.Open(SavePath, FileAccess.ModeFlags.Read);
        var root = (Godot.Collections.Dictionary)file.GetVar();

        Log.Dbg("Save File Contents:");
        Log.Dbg($"  Schema Version: {root.GetValueOrDefault("v", 2)}");

        if (root.TryGetValue("weapons", out var weaponsRaw))
        {
            Log.Dbg("  Weapons:");
            foreach (Godot.Collections.Dictionary w in (Godot.Collections.Array)weaponsRaw)
            {
                Log.Dbg($"    Key: {w.GetValueOrDefault("key", "UNKNOWN")}");
                Log.Dbg($"      Amount: {w.GetValueOrDefault("amount", -1)}");
                Log.Dbg($"      Cooldown: {w.GetValueOrDefault("cooldown", -1f)}");
                Log.Dbg($"      Levels -> speed:{w.GetValueOrDefault("speed_level", 0)}, dmg:{w.GetValueOrDefault("damage_level", 0)}, dmg%:{w.GetValueOrDefault("damage_pct_level", 0)}, cd:{w.GetValueOrDefault("cooldown_level", 0)}, storage:{w.GetValueOrDefault("storage_level", 0)}");
                Log.Dbg($"      Overrides -> speed:{w.GetValueOrDefault("override_speed", -1)}, dmg:{w.GetValueOrDefault("override_dmg", -1)}, dmg%:{w.GetValueOrDefault("override_dmg_pct", -1)}, cd:{w.GetValueOrDefault("override_cd", -1f)}, max:{w.GetValueOrDefault("override_max", -1)}");
                Log.Dbg($"      Unlock -> override:{w.GetValueOrDefault("override_unlocked", false)}, use_override:{w.GetValueOrDefault("use_override_unlocked", false)}");
            }
        }
        else
        {
            Log.Dbg("  Weapons: NONE");
        }

        if (root.TryGetValue("equipped", out var equippedRaw))
        {
            Log.Dbg("  Equipped:");
            foreach (string slot in ((Godot.Collections.Dictionary)equippedRaw).Keys)
            {
                string value = (string)((Godot.Collections.Dictionary)equippedRaw)[slot];
                Log.Dbg($"    {slot}: {(string.IsNullOrEmpty(value) ? "EMPTY" : value)}");
            }
        }
        else
        {
            Log.Dbg("  Equipped: NONE");
        }
    }
}
