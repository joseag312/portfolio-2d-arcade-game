using Godot;
using System;
using System.Threading.Tasks;

[GlobalClass]
public partial class AssignContainerBox : CenterContainer
{
    [Signal] public delegate void AssignmentDoneEventHandler(string weaponKey, string slotKey);

    [Export] public string WeaponKey { get; set; }
    [Export] public AssignContainer ContainerSlot1 { get; set; }
    [Export] public AssignContainer ContainerSlot2 { get; set; }
    [Export] public AssignContainer ContainerSlot3 { get; set; }
    [Export] public AssignContainer ContainerSlot4 { get; set; }
    [Export] public float FadeDuration = 0.4f;

    private Tween _fadeTween;
    private bool _isFading;
    private AssignContainer[] _slots;

    public override void _Ready()
    {
        _slots = new[] { ContainerSlot1, ContainerSlot2, ContainerSlot3, ContainerSlot4 };

        foreach (var slot in _slots)
        {
            if (slot == null) continue;
            slot.SlotClicked += OnSlotClicked;
        }
    }

    public void OpenFor(string weaponKey)
    {
        WeaponKey = weaponKey;

        Modulate = new Color(Modulate.R, Modulate.G, Modulate.B, 0f);
        Visible = true;

        RefreshAll();
        _ = FadeIn();
    }

    public void RefreshAll()
    {
        foreach (var slot in _slots)
            slot?.Refresh();
    }

    private void OnSlotClicked(string slotKey)
    {
        if (string.IsNullOrEmpty(WeaponKey))
        {
            Log.Err("Weapon Key not set before assigning");
            return;
        }

        int targetIndex = SlotKeyToIndex(slotKey);

        int currentIndex = FindSlotIndexHoldingWeapon(WeaponKey);
        if (currentIndex != -1 && currentIndex != targetIndex)
            ClearSlot(currentIndex);

        EquipSlot(targetIndex, WeaponKey);

        RefreshAll();
        EmitSignal(SignalName.AssignmentDone, WeaponKey, slotKey);

        _ = HideAfterDelay(0.25f);
    }

    private async Task HideAfterDelay(float seconds)
    {
        await ToSignal(GetTree().CreateTimer(seconds), "timeout");
        await FadeOut();
    }

    private int FindSlotIndexHoldingWeapon(string weaponKey)
    {
        foreach (var slot in _slots)
        {
            if (slot == null) continue;

            string equipped = string.Empty;
            try { equipped = G.WI.GetEquippedWeaponKey(slot.SlotKey); }
            catch (Exception e) { Log.Err($"GetEquippedWeaponKey() threw: {e.Message}"); }

            if (!string.IsNullOrEmpty(equipped) && equipped == weaponKey)
                return SlotKeyToIndex(slot.SlotKey);
        }
        return -1;
    }

    private void EquipSlot(int slotIndex, string weaponKey)
    {
        try
        {
            G.WI.EquipSlotWeapon(slotIndex, weaponKey);
        }
        catch (Exception e)
        {
            Log.Err($"EquipSlot failed: {e.Message}");
        }
    }

    private void ClearSlot(int slotIndex)
    {
        try
        {
            G.WI.EquipSlotWeapon(slotIndex, null);
        }
        catch (Exception e)
        {
            Log.Err($"ClearSlot failed: {e.Message}");
        }
    }

    private int SlotKeyToIndex(string key) => key switch
    {
        "special_1" => 1,
        "special_2" => 2,
        "special_3" => 3,
        "special_4" => 4,
        _ => 0
    };

    private async Task FadeToAlpha(float targetAlpha, float duration)
    {
        _isFading = true;

        if (_fadeTween != null && GodotObject.IsInstanceValid(_fadeTween))
            _fadeTween.Kill();

        if (targetAlpha > 0f && !Visible)
            Visible = true;

        var prevFilter = MouseFilter;
        MouseFilter = Control.MouseFilterEnum.Ignore;

        _fadeTween = GetTree().CreateTween();
        _fadeTween.SetTrans(Tween.TransitionType.Sine).SetEase(Tween.EaseType.InOut);
        _fadeTween.TweenProperty(this, "modulate:a", targetAlpha, duration);

        await ToSignal(_fadeTween, "finished");

        if (Mathf.IsZeroApprox(targetAlpha))
            Visible = false;

        MouseFilter = prevFilter;
        _isFading = false;
    }

    private Task FadeIn() => FadeToAlpha(1f, FadeDuration);
    private Task FadeOut() => FadeToAlpha(0f, FadeDuration);
}
