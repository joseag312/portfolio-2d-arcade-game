using Godot;
using System;
using System.Collections.Generic;

[GlobalClass]
public partial class UpgradePowerUpContainer : Control
{
    [Signal] public delegate void UpgradeMessageEventHandler(string message);

    [Export] public Label TitleLabel { get; set; }

    [ExportGroup("Speed Row")]
    [Export] public HBoxContainer SpeedRow;
    [Export] public Label SpeedLevelLabel;
    [Export] public Button SpeedAddButton;
    [Export] public Button SpeedMinusButton;

    [ExportGroup("Damage Row")]
    [Export] public HBoxContainer DamageRow;
    [Export] public Label DamageLevelLabel;
    [Export] public Button DamageAddButton;
    [Export] public Button DamageMinusButton;

    [ExportGroup("Cooldown Row")]
    [Export] public HBoxContainer CooldownRow;
    [Export] public Label CooldownLevelLabel;
    [Export] public Button CooldownAddButton;
    [Export] public Button CooldownMinusButton;

    [ExportGroup("Storage Row")]
    [Export] public HBoxContainer StorageRow;
    [Export] public Label StorageLevelLabel;
    [Export] public Button StorageAddButton;
    [Export] public Button StorageMinusButton;

    [ExportGroup("Confirm Section")]
    [Export] public Label TotalPriceLabel;
    [Export] public Button ConfirmButton;

    private WeaponStateComponent _state;
    private WeaponDataComponent _base;
    private WeaponStateComponent.UpgradeKind _damageKind;

    private Dictionary<WeaponStateComponent.UpgradeKind, int> _pendingAmounts = new();
    private Color _normalColor = new Color(1, 1, 1);
    private Color _greenColor = new Color(0.4f, 1f, 0.4f);
    private Color _redColor = new Color(1f, 0.4f, 0.4f);

    public override void _Ready()
    {
        SpeedAddButton.Pressed += () => AddDelta(WeaponStateComponent.UpgradeKind.Speed, 1);
        SpeedMinusButton.Pressed += () => AddDelta(WeaponStateComponent.UpgradeKind.Speed, -1);

        DamageAddButton.Pressed += () => AddDelta(_damageKind, 1);
        DamageMinusButton.Pressed += () => AddDelta(_damageKind, -1);

        CooldownAddButton.Pressed += () => AddDelta(WeaponStateComponent.UpgradeKind.Cooldown, 1);
        CooldownMinusButton.Pressed += () => AddDelta(WeaponStateComponent.UpgradeKind.Cooldown, -1);

        StorageAddButton.Pressed += () => AddDelta(WeaponStateComponent.UpgradeKind.Storage, 1);
        StorageMinusButton.Pressed += () => AddDelta(WeaponStateComponent.UpgradeKind.Storage, -1);

        ConfirmButton.Pressed += OnConfirmPressed;
    }

    public void LoadContainer(string key)
    {
        _state = G.WI.GetWeaponState(key);
        if (_state == null)
        {
            Log.Err($"WeaponState missing for Key '{key}'");
            Visible = false;
            return;
        }

        _base = _state.BaseData;
        if (_base == null)
        {
            Log.Err($"BaseData missing for Key '{key}'");
            Visible = false;
            return;
        }

        TitleLabel.Text = _base.DisplayName;

        _damageKind = (_base.DamageModel == WeaponDataComponent.DamageMode.Flat)
            ? WeaponStateComponent.UpgradeKind.Damage
            : WeaponStateComponent.UpgradeKind.DamagePct;

        _pendingAmounts.Clear();
        _pendingAmounts[WeaponStateComponent.UpgradeKind.Speed] = 0;
        _pendingAmounts[_damageKind] = 0;
        _pendingAmounts[WeaponStateComponent.UpgradeKind.Cooldown] = 0;
        _pendingAmounts[WeaponStateComponent.UpgradeKind.Storage] = 0;

        InitRow(SpeedRow, SpeedLevelLabel, WeaponStateComponent.UpgradeKind.Speed);
        InitRow(DamageRow, DamageLevelLabel, _damageKind);
        InitRow(CooldownRow, CooldownLevelLabel, WeaponStateComponent.UpgradeKind.Cooldown);
        InitRow(StorageRow, StorageLevelLabel, WeaponStateComponent.UpgradeKind.Storage);

        RecalculateTotalPrice();
    }

    private void InitRow(HBoxContainer row, Label levelLabel, WeaponStateComponent.UpgradeKind kind)
    {
        if (!_state.IsUpgradeEnabled(kind))
        {
            row.Visible = false;
            return;
        }

        row.Visible = true;
        levelLabel.Text = _state.GetLevel(kind).ToString();
        levelLabel.Modulate = _normalColor;
    }

    private void AddDelta(WeaponStateComponent.UpgradeKind kind, int delta)
    {
        G.SFX.Play(SFX.CLICK);
        int currentLevel = _state.GetLevel(kind);
        int maxLevel = _state.GetMaxUpgrades(kind);

        if (!_pendingAmounts.ContainsKey(kind))
            _pendingAmounts[kind] = 0;

        _pendingAmounts[kind] = Mathf.Clamp(_pendingAmounts[kind] + delta, 0, maxLevel - currentLevel);

        UpdateRowLevel(kind);

        RecalculateTotalPrice();
    }

    private void UpdateRowLevel(WeaponStateComponent.UpgradeKind kind)
    {
        int pending = _pendingAmounts[kind];
        int currentLevel = _state.GetLevel(kind);

        Label targetLabel = kind switch
        {
            WeaponStateComponent.UpgradeKind.Speed => SpeedLevelLabel,
            WeaponStateComponent.UpgradeKind.Damage => DamageLevelLabel,
            WeaponStateComponent.UpgradeKind.DamagePct => DamageLevelLabel,
            WeaponStateComponent.UpgradeKind.Cooldown => CooldownLevelLabel,
            WeaponStateComponent.UpgradeKind.Storage => StorageLevelLabel,
            _ => null
        };

        if (targetLabel == null) return;

        if (pending > 0)
        {
            targetLabel.Text = (currentLevel + pending).ToString();
            targetLabel.Modulate = _greenColor;
        }
        else
        {
            targetLabel.Text = currentLevel.ToString();
            targetLabel.Modulate = _normalColor;
        }
    }

    private void RecalculateTotalPrice()
    {
        int total = 0;

        foreach (var kvp in _pendingAmounts)
        {
            var kind = kvp.Key;
            int pending = kvp.Value;
            int currentLevel = _state.GetLevel(kind);

            for (int i = 0; i < pending; i++)
            {
                total += _state.GetPriceAtLevel(kind, currentLevel + i);
            }
        }

        TotalPriceLabel.Text = total.ToString();

        if (total > G.GS.Pawllars)
        {
            TotalPriceLabel.Modulate = _redColor;
            ConfirmButton.Disabled = true;
        }
        else
        {
            TotalPriceLabel.Modulate = _normalColor;
            ConfirmButton.Disabled = total == 0;
        }
    }

    private void OnConfirmPressed()
    {
        int totalCost = int.Parse(TotalPriceLabel.Text);
        if (totalCost > G.GS.Pawllars || totalCost <= 0)
        {
            EmitSignal(SignalName.UpgradeMessage, "Aren't you missing something?");
            return;
        }

        G.SFX.Play(SFX.COIN);
        G.GS.Pawllars -= totalCost;

        foreach (var kvp in _pendingAmounts)
        {
            var kind = kvp.Key;
            int pending = kvp.Value;
            for (int i = 0; i < pending; i++)
            {
                _state.TryApplyUpgrade(kind);
            }
        }

        EmitSignal(SignalName.UpgradeMessage, "Upgrades applied!");
        G.WI.Save();
        LoadContainer(_state.Key);
    }
}
