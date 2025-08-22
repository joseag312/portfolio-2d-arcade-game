using System;
using System.Collections.Generic;
using Godot;

public partial class AutoWeaponDatabase : Node
{
    public static AutoWeaponDatabase Instance { get; private set; }

    private Dictionary<string, WeaponDataComponent> _weaponMapping;

    public override void _Ready()
    {
        if (Instance == null)
        {
            Instance = this;
            ProcessMode = ProcessModeEnum.Always;
        }
        else
        {
            QueueFree();
            return;
        }

        _weaponMapping = new Dictionary<string, WeaponDataComponent>();
        LoadWeapons();
    }

    private void LoadWeapons()
    {
        LoadWeaponResourcesFromFolder("res://resources/weapons/");
    }

    private void LoadWeaponResourcesFromFolder(string folderPath)
    {
        var dir = DirAccess.Open(folderPath);
        if (dir == null)
        {
            Log.Err($"WeaponCatalog - Could not open {folderPath}");
            return;
        }

        if (!folderPath.EndsWith("/"))
            folderPath += "/";

        foreach (string file in dir.GetFiles())
        {
            if (string.IsNullOrEmpty(file) || file.StartsWith("."))
                continue;

            string candidate = file.EndsWith(".remap", StringComparison.OrdinalIgnoreCase)
                ? file.Substring(0, file.Length - ".remap".Length)
                : file;

            bool isTresOrRes =
                candidate.EndsWith(".tres", StringComparison.OrdinalIgnoreCase) ||
                candidate.EndsWith(".res", StringComparison.OrdinalIgnoreCase);

            if (!isTresOrRes)
                continue;

            string fullPath = folderPath + candidate;

            var weapon = ResourceLoader.Load<WeaponDataComponent>(fullPath);
            if (weapon == null)
            {
                Log.Err($"Failed to load Weapon Resource '{fullPath}'");
                continue;
            }

            if (string.IsNullOrEmpty(weapon.Key))
            {
                Log.Err($"Weapon Resource missing Key: '{fullPath}'");
                continue;
            }

            if (_weaponMapping.ContainsKey(weapon.Key))
            {
                Log.Err($"Duplicate Weapon Key: {weapon.Key}");
                continue;
            }

            _weaponMapping[weapon.Key] = weapon;
        }
    }

    public WeaponDataComponent GetWeaponData(string key)
    {
        if (!_weaponMapping.TryGetValue(key, out var data))
        {
            Log.Err($"Weapon ID '{key}' not found");
            return null;
        }

        return data;
    }

    public List<string> GetAllWeaponKeys()
    {
        return new List<string>(_weaponMapping.Keys);
    }

    public IEnumerable<WeaponDataComponent> GetAllWeapons()
    {
        return _weaponMapping.Values;
    }

    public IEnumerable<WeaponDataComponent> GetWeaponsBySlotType(WeaponDataComponent.WeaponSlotType slotType)
    {
        foreach (var weapon in _weaponMapping.Values)
        {
            if (weapon.SlotType == slotType)
                yield return weapon;
        }
    }
}
