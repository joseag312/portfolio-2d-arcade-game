using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

[GlobalClass]
public partial class HUDPowers : Control
{
    [ExportGroup("Big Laser")]
    [Export] public TextureRect BigLaserButton;
    [Export] public TextureRect BigLaserOverlay;

    [ExportGroup("Slot 1")]
    [Export] public TextureRect PowerUp1Container;
    [Export] public TextureRect PowerUp1Icon;
    [Export] public TextureRect PowerUp1Overlay;
    [Export] public Label PowerUp1Amount;

    [ExportGroup("Slot 2")]
    [Export] public TextureRect PowerUp2Container;
    [Export] public TextureRect PowerUp2Icon;
    [Export] public TextureRect PowerUp2Overlay;
    [Export] public Label PowerUp2Amount;

    [ExportGroup("Slot 3")]
    [Export] public TextureRect PowerUp3Container;
    [Export] public TextureRect PowerUp3Icon;
    [Export] public TextureRect PowerUp3Overlay;
    [Export] public Label PowerUp3Amount;

    [ExportGroup("Slot 4")]
    [Export] public TextureRect PowerUp4Container;
    [Export] public TextureRect PowerUp4Icon;
    [Export] public TextureRect PowerUp4Overlay;
    [Export] public Label PowerUp4Amount;

    private Color _textColor;
    private Color _baseColor;
    private Color _bigOverlayColor;
    private Color _overlayColor;
    private Color _flashColor;
    private float _flashDuration = 0.2f;

    private readonly Dictionary<TextureRect, Tween> _activeCooldownTweens = new();

    public override void _Ready()
    {
        float opacity = G.CF.GetHudOpacity();

        _textColor = new Color(255f / 255f, 255f / 255f, 255f / 255f, 0.5f + opacity * 0.5f);
        _baseColor = new Color(220f / 255f, 220f / 255f, 220f / 255f, opacity);
        _flashColor = new Color(255f / 255f, 255f / 255f, 255f / 255f, opacity);
        _overlayColor = new Color(150f / 255f, 0f / 255f, 155f / 255f, opacity);

        BigLaserButton.Modulate = _baseColor;
        BigLaserOverlay.Modulate = _overlayColor;

        PowerUp1Container.Modulate = _baseColor;
        PowerUp1Icon.Modulate = _baseColor;
        PowerUp1Overlay.Modulate = _overlayColor;
        PowerUp1Amount.Modulate = _textColor;

        PowerUp2Container.Modulate = _baseColor;
        PowerUp2Icon.Modulate = _baseColor;
        PowerUp2Overlay.Modulate = _overlayColor;
        PowerUp2Amount.Modulate = _textColor;

        PowerUp3Container.Modulate = _baseColor;
        PowerUp3Icon.Modulate = _baseColor;
        PowerUp3Overlay.Modulate = _overlayColor;
        PowerUp3Amount.Modulate = _textColor;

        PowerUp4Container.Modulate = _baseColor;
        PowerUp4Icon.Modulate = _baseColor;
        PowerUp4Overlay.Modulate = _overlayColor;
        PowerUp4Amount.Modulate = _textColor;

        BigLaserOverlay.Visible = false;
        PowerUp1Overlay.Visible = true;
        PowerUp2Overlay.Visible = true;
        PowerUp3Overlay.Visible = true;
        PowerUp4Overlay.Visible = true;

        SetWeaponsDisplay();
    }

    public override void _Process(double delta)
    {

    }

    public void SetWeaponManager(WeaponManagerComponent weaponManager)
    {
        weaponManager.Connect(WeaponManagerComponent.SignalName.WeaponFired, new Callable(this, nameof(OnWeaponFired)));
        weaponManager.Connect(WeaponManagerComponent.SignalName.PowerUsed, new Callable(this, nameof(OnPowerUsed)));
    }

    private void SetWeaponsDisplay()
    {
        var inv = G.WI;

        SetSlotDisplay(inv.GetEquippedWeaponState(WeaponSlotNames.Special1), PowerUp1Icon, PowerUp1Amount, PowerUp1Overlay);
        SetSlotDisplay(inv.GetEquippedWeaponState(WeaponSlotNames.Special2), PowerUp2Icon, PowerUp2Amount, PowerUp2Overlay);
        SetSlotDisplay(inv.GetEquippedWeaponState(WeaponSlotNames.Special3), PowerUp3Icon, PowerUp3Amount, PowerUp3Overlay);
        SetSlotDisplay(inv.GetEquippedWeaponState(WeaponSlotNames.Special4), PowerUp4Icon, PowerUp4Amount, PowerUp4Overlay);
    }

    private void SetSlotDisplay(WeaponStateComponent state, TextureRect icon, Label amountLabel, TextureRect overlay)
    {
        if (state?.BaseData == null)
        {
            icon.Texture = null;
            amountLabel.Text = "";
            StopCooldown(overlay);
            overlay.Visible = true;
            overlay.Modulate = _overlayColor;
            return;
        }

        icon.Texture = (state.BaseData.IconPath is { Length: > 0 } path)
            ? GD.Load<Texture2D>(path) : null;

        amountLabel.Text = (state.BaseData.SlotType == WeaponDataComponent.WeaponSlotType.Slot)
            ? state.CurrentAmount.ToString()
            : "";

        if (state.CurrentAmount > 0)
        {
            StopCooldown(overlay);
        }
        else
        {
            StopCooldown(overlay);
            overlay.Visible = true;
            overlay.Modulate = _overlayColor;
        }
    }

    private void OnWeaponFired(int weaponSlot, WeaponStateComponent weapon)
    {
        var slotType = weapon.BaseData.SlotType;

        switch (weaponSlot)
        {
            case 1:
                _ = FlashIcon(_flashDuration, BigLaserButton);
                break;

            case 2:
                if (weapon.CurrentAmount > 0)
                {
                    _ = FlashIcon(_flashDuration, PowerUp1Icon);
                }
                break;

            case 3:
                if (weapon.CurrentAmount > 0)
                {
                    _ = FlashIcon(_flashDuration, PowerUp2Icon);
                }
                break;

            case 4:
                if (weapon.CurrentAmount > 0)
                {
                    _ = FlashIcon(_flashDuration, PowerUp3Icon);
                }
                break;

            case 5:
                if (weapon.CurrentAmount > 0)
                {
                    _ = FlashIcon(_flashDuration, PowerUp4Icon);
                }
                break;

            default:
                Log.Err($"OnWeaponFired Unknown SlotType {slotType} for Weapon {weapon.BaseData.Key}");
                break;
        }
    }

    private void OnPowerUsed(int weaponSlot, WeaponStateComponent weapon)
    {
        var slotType = weapon.BaseData.SlotType;

        switch (weaponSlot)
        {
            case 1:
                _ = StartCooldown(weapon.EffectiveCooldown, BigLaserButton, BigLaserOverlay);
                break;

            case 2:
                if (weapon.CurrentAmount > 0)
                {
                    _ = StartCooldown(weapon.EffectiveCooldown, PowerUp1Icon, PowerUp1Overlay);
                }
                else
                {
                    _ = DisableWeapon(_flashDuration, PowerUp1Icon, PowerUp1Overlay);
                }
                UpdateAmount(weapon, PowerUp1Amount);
                break;

            case 3:
                if (weapon.CurrentAmount > 0)
                {
                    _ = StartCooldown(weapon.EffectiveCooldown, PowerUp2Icon, PowerUp2Overlay);
                }
                else
                {
                    _ = DisableWeapon(_flashDuration, PowerUp2Icon, PowerUp2Overlay);
                }
                UpdateAmount(weapon, PowerUp2Amount);
                break;

            case 4:
                if (weapon.CurrentAmount > 0)
                {
                    _ = StartCooldown(weapon.EffectiveCooldown, PowerUp3Icon, PowerUp3Overlay);
                }
                else
                {
                    _ = DisableWeapon(_flashDuration, PowerUp3Icon, PowerUp3Overlay);
                }
                UpdateAmount(weapon, PowerUp3Amount);
                break;

            case 5:
                if (weapon.CurrentAmount > 0)
                {
                    _ = StartCooldown(weapon.EffectiveCooldown, PowerUp4Icon, PowerUp4Overlay);
                }
                else
                {
                    _ = DisableWeapon(_flashDuration, PowerUp4Icon, PowerUp4Overlay);
                }
                UpdateAmount(weapon, PowerUp4Amount);
                break;

            default:
                Log.Err($"OnPowerUsed Unknown SlotType {slotType} for Weapon {weapon.BaseData.Key}");
                break;
        }
    }

    private void UpdateAmount(WeaponStateComponent state, Label amountLabel)
    {
        if (state != null && state.BaseData.SlotType == WeaponDataComponent.WeaponSlotType.Slot)
            amountLabel.Text = state.CurrentAmount.ToString();
        else
            amountLabel.Text = "";
    }

    private void UpdateAllAmounts()
    {
        var inv = G.WI;

        UpdateAmount(inv.GetEquippedWeaponState(WeaponSlotNames.Special1), PowerUp1Amount);
        UpdateAmount(inv.GetEquippedWeaponState(WeaponSlotNames.Special2), PowerUp2Amount);
        UpdateAmount(inv.GetEquippedWeaponState(WeaponSlotNames.Special3), PowerUp3Amount);
        UpdateAmount(inv.GetEquippedWeaponState(WeaponSlotNames.Special4), PowerUp4Amount);
    }

    private async Task FlashIcon(float duration, TextureRect icon)
    {
        icon.Modulate = _baseColor;

        Tween tween = GetTree().CreateTween();
        tween.SetTrans(Tween.TransitionType.Sine).SetEase(Tween.EaseType.InOut);
        tween.Chain().TweenProperty(icon, "modulate", _flashColor, duration / 2);
        tween.Chain().TweenProperty(icon, "modulate", _baseColor, duration / 2);
        await Task.Delay(50);
    }

    private async Task StartCooldown(float duration, TextureRect icon, TextureRect overlay)
    {
        StopCooldown(overlay);

        await Task.Delay((int)(_flashDuration * 1000f));

        overlay.Visible = true;
        overlay.Modulate = _overlayColor;

        var tween = GetTree().CreateTween()
            .SetTrans(Tween.TransitionType.Sine)
            .SetEase(Tween.EaseType.InOut);

        _activeCooldownTweens[overlay] = tween;

        tween.TweenProperty(overlay, "modulate:a", 0.35f, duration);
        tween.TweenProperty(overlay, "modulate:a", 0.0f, 0.1f);

        await ToSignal(tween, "finished");

        if (_activeCooldownTweens.TryGetValue(overlay, out var current) && current == tween)
        {
            _activeCooldownTweens.Remove(overlay);
            overlay.Visible = false;
            overlay.Modulate = _baseColor;
        }
    }

    private void StopCooldown(TextureRect overlay)
    {
        if (_activeCooldownTweens.TryGetValue(overlay, out var t) && IsInstanceValid(t))
            t.Kill();

        _activeCooldownTweens.Remove(overlay);
        overlay.Visible = false;
        overlay.Modulate = _baseColor;
    }

    private async Task DisableWeapon(float duration, TextureRect icon, TextureRect overlay)
    {
        StopCooldown(overlay);

        icon.Modulate = _baseColor;

        var tween1 = GetTree().CreateTween()
            .SetTrans(Tween.TransitionType.Sine)
            .SetEase(Tween.EaseType.InOut);

        tween1.Chain().TweenProperty(icon, "modulate", _flashColor, duration / 2);
        tween1.Chain().TweenProperty(icon, "modulate", _baseColor, duration / 2);

        await Task.Delay(200);

        overlay.Visible = true;
        overlay.Modulate = _overlayColor;
    }
}
