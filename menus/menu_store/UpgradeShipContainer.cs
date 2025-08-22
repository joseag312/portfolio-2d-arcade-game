using Godot;
using System.Collections.Generic;

[GlobalClass]
public partial class UpgradeShipContainer : Control
{
    [Signal] public delegate void UpgradeMessageEventHandler(string message);

    [Export] public Label TitleLabel { get; set; }

    [ExportGroup("Health Row")]
    [Export] public HBoxContainer HealthRow { get; set; }
    [Export] public Label HealthValueLabel { get; set; }
    [Export] public Button HealthAddButton { get; set; }
    [Export] public Button HealthMinusButton { get; set; }

    [ExportGroup("Speed Row")]
    [Export] public HBoxContainer SpeedRow { get; set; }
    [Export] public Label SpeedValueLabel { get; set; }
    [Export] public Button SpeedAddButton { get; set; }
    [Export] public Button SpeedMinusButton { get; set; }

    [ExportGroup("Confirm Section")]
    [Export] public Label TotalPriceLabel { get; set; }
    [Export] public Button ConfirmButton { get; set; }

    private readonly Dictionary<AutoShipStats.UpgradeKind, int> _pending = new();
    private readonly Color _normal = new Color(1, 1, 1);
    private readonly Color _green = new Color(0.4f, 1f, 0.4f);
    private readonly Color _red = new Color(1f, 0.4f, 0.4f);

    public override void _Ready()
    {
        TitleLabel.Text = "Ship";

        _pending[AutoShipStats.UpgradeKind.Health] = 0;
        _pending[AutoShipStats.UpgradeKind.Speed] = 0;

        HealthAddButton.Pressed += () => AddDelta(AutoShipStats.UpgradeKind.Health, +1);
        HealthMinusButton.Pressed += () => AddDelta(AutoShipStats.UpgradeKind.Health, -1);

        SpeedAddButton.Pressed += () => AddDelta(AutoShipStats.UpgradeKind.Speed, +1);
        SpeedMinusButton.Pressed += () => AddDelta(AutoShipStats.UpgradeKind.Speed, -1);

        ConfirmButton.Pressed += OnConfirmPressed;

        RefreshAll();
    }

    private void RefreshAll()
    {
        if (G.SS == null)
        {
            Log.Err("AutoShipStats missing");
            Visible = false;
            return;
        }

        HealthRow.Visible = true;
        SpeedRow.Visible = true;

        UpdateRowValue(AutoShipStats.UpgradeKind.Health);
        UpdateRowValue(AutoShipStats.UpgradeKind.Speed);

        RecalculateTotalPrice();
        UpdateButtonsEnabled();
    }

    private void AddDelta(AutoShipStats.UpgradeKind kind, int delta)
    {
        G.SFX.Play(SFX.CLICK);
        int currentLevel = G.SS.GetLevel(kind);
        int maxLevel = G.SS.GetMaxLevel(kind);

        int pending = _pending[kind];
        pending = Mathf.Clamp(pending + delta, 0, maxLevel - currentLevel);
        _pending[kind] = pending;

        UpdateRowValue(kind);
        RecalculateTotalPrice();
        UpdateButtonsEnabled();
    }

    private void UpdateRowValue(AutoShipStats.UpgradeKind kind)
    {
        int baseLevel = G.SS.GetLevel(kind);
        int pending = _pending[kind];
        int previewLvl = baseLevel + pending;

        if (kind == AutoShipStats.UpgradeKind.Health)
        {
            HealthValueLabel.Text = previewLvl.ToString();
            HealthValueLabel.Modulate = (pending > 0) ? _green : _normal;
        }
        else
        {
            SpeedValueLabel.Text = previewLvl.ToString();
            SpeedValueLabel.Modulate = (pending > 0) ? _green : _normal;
        }
    }

    private void RecalculateTotalPrice()
    {
        int total = 0;

        foreach (var kvp in _pending)
        {
            var kind = kvp.Key;
            int pending = kvp.Value;
            int currentLevel = G.SS.GetLevel(kind);

            for (int i = 0; i < pending; i++)
                total += G.SS.GetPriceAtLevel(kind, currentLevel + i);
        }

        TotalPriceLabel.Text = total.ToString();

        if (total > G.GS.Pawllars)
        {
            TotalPriceLabel.Modulate = _red;
            ConfirmButton.Disabled = true;
        }
        else
        {
            TotalPriceLabel.Modulate = _normal;
            ConfirmButton.Disabled = (total == 0);
        }
    }

    private void UpdateButtonsEnabled()
    {
        HealthAddButton.Disabled =
            G.SS.GetLevel(AutoShipStats.UpgradeKind.Health) + _pending[AutoShipStats.UpgradeKind.Health] >=
            G.SS.GetMaxLevel(AutoShipStats.UpgradeKind.Health);
        HealthMinusButton.Disabled = _pending[AutoShipStats.UpgradeKind.Health] == 0;

        SpeedAddButton.Disabled =
            G.SS.GetLevel(AutoShipStats.UpgradeKind.Speed) + _pending[AutoShipStats.UpgradeKind.Speed] >=
            G.SS.GetMaxLevel(AutoShipStats.UpgradeKind.Speed);
        SpeedMinusButton.Disabled = _pending[AutoShipStats.UpgradeKind.Speed] == 0;
    }

    private void OnConfirmPressed()
    {
        int totalCost = int.Parse(TotalPriceLabel.Text);
        if (totalCost <= 0 || totalCost > G.GS.Pawllars)
        {
            EmitSignal(SignalName.UpgradeMessage, "Aren't you missing something?");
            return;
        }

        G.SFX.Play(SFX.COIN);
        G.GS.Pawllars -= totalCost;

        foreach (var kvp in _pending)
        {
            var kind = kvp.Key;
            int pending = kvp.Value;
            for (int i = 0; i < pending; i++)
                G.SS.TryApplyUpgrade(kind);
        }

        _pending[AutoShipStats.UpgradeKind.Health] = 0;
        _pending[AutoShipStats.UpgradeKind.Speed] = 0;

        EmitSignal(SignalName.UpgradeMessage, "Upgrades applied!");
        G.GS.Save();
        RefreshAll();
    }
}
