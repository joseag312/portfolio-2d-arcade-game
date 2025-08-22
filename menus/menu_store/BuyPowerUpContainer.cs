using Godot;
using System;

[GlobalClass]
public partial class BuyPowerUpContainer : Control
{
    [Signal] public delegate void PurchaseMessageEventHandler(string message);

    [Export] public Label TitleLabel { get; set; }
    [Export] public Button Add1Button;
    [Export] public Button Minus1Button;
    [Export] public Button Add10Button;
    [Export] public Button Minus10Button;
    [Export] public Label CurrentLabel;
    [Export] public Label AmountLabel;
    [Export] public Label PriceLabel;
    [Export] public Button ConfirmButton;

    private WeaponStateComponent _state;
    private WeaponDataComponent _base;
    private int _purchaseAmount;
    private int _purchasePrice;
    private Color _normalColor = new Color(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
    private Color _disabledColor = new Color(100f / 255f, 100f / 255f, 100f / 255f, 255f / 255f);
    private Color _blockColor = new Color(255f / 255f, 100f / 255f, 100f / 255f, 255f / 255f);

    public override void _Ready()
    {
        Add1Button.Pressed += OnAdd1Pressed;
        Minus1Button.Pressed += OnMinus1Pressed;
        Add10Button.Pressed += OnAdd10Pressed;
        Minus10Button.Pressed += OnMinus10Pressed;
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

        _purchaseAmount = 0;
        _purchasePrice = 0;

        TitleLabel.Text = _state.BaseData.DisplayName;
        CurrentLabel.Text = _state.CurrentAmount.ToString() + "/" + _state.EffectiveMaxAmount.ToString();
        AmountLabel.Text = "";
        PriceLabel.Text = "0";
        UpdateButtonsAvailability();
    }

    public void OnConfirmPressed()
    {
        if (_purchaseAmount == 0)
        {
            EmitSignal(SignalName.PurchaseMessage, "How many?");
            return;
        }

        CalculatePrice();
        if (_purchasePrice > G.GS.Mewnits)
        {
            EmitSignal(SignalName.PurchaseMessage, "Aren't you missing something?");
            return;
        }

        G.SFX.Play(SFX.COIN);
        G.GS.Mewnits -= _purchasePrice;
        _state.CurrentAmount += _purchaseAmount;
        G.WI.Save();
        LoadContainer(_state.Key);

        EmitSignal(SignalName.PurchaseMessage, "Thank you for your business!");
    }

    public void OnAdd1Pressed() => AddDelta(1);
    public void OnMinus1Pressed() => AddDelta(-1);
    public void OnAdd10Pressed() => AddDelta(10);
    public void OnMinus10Pressed() => AddDelta(-10);

    private void AddDelta(int delta)
    {
        if (_state == null) return;
        G.SFX.Play(SFX.CLICK);

        int max = _state.EffectiveMaxAmount;
        int owned = _state.CurrentAmount;
        int remaining = Math.Max(0, max - owned);

        _purchaseAmount = Mathf.Clamp(_purchaseAmount + delta, 0, remaining);

        AmountLabel.Text = _purchaseAmount > 0 ? "+" + _purchaseAmount : "";
        CalculatePrice();
        UpdateButtonsAvailability();
    }

    private void UpdateButtonsAvailability()
    {
        if (_state == null) return;

        int remaining = Math.Max(0, _state.EffectiveMaxAmount - _state.CurrentAmount);

        bool canAdd = remaining > 0 && _purchaseAmount < remaining;
        bool canRemove = _purchaseAmount > 0;

        Add1Button.Disabled = !canAdd;
        Add10Button.Disabled = !canAdd;
        Add1Button.Modulate = canAdd ? _normalColor : _disabledColor;
        Add10Button.Modulate = canAdd ? _normalColor : _disabledColor;

        Minus1Button.Disabled = !canRemove;
        Minus10Button.Disabled = !canRemove;
        Minus1Button.Modulate = canRemove ? _normalColor : _disabledColor;
        Minus10Button.Modulate = canRemove ? _normalColor : _disabledColor;

        bool canConfirm = (_purchaseAmount > 0 &&
                           remaining > 0 &&
                           _purchasePrice <= G.GS.Mewnits);
        ConfirmButton.Disabled = !canConfirm;
        ConfirmButton.Modulate = canConfirm ? _normalColor : _disabledColor;
    }

    public void CalculatePrice()
    {
        _purchasePrice = _base.UnitPrice * _purchaseAmount;
        PriceLabel.Text = _purchasePrice.ToString();
        if (_purchasePrice > G.GS.Mewnits)
        {
            PriceLabel.Modulate = _blockColor;
        }
        else
        {
            PriceLabel.Modulate = _normalColor;
        }
    }
}
