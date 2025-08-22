using Godot;
using System;

[GlobalClass]
public partial class AssignContainer : CenterContainer
{
    [Signal] public delegate void SlotClickedEventHandler(string slotKey);

    [Export] public string SlotKey;
    [Export] public TextureRect ContainerSprite;
    [Export] public TextureRect WeaponSprite;

    private Color _normalColor = new Color(150f / 255f, 150f / 255f, 150f / 255f, 1f);
    private Color _hoverColor = new Color(200f / 255f, 200f / 255f, 200f / 255f, 1f);
    private Color _clickColor = new Color(1f, 1f, 1f, 1f);
    private bool _animating;

    public override void _Ready()
    {
        Modulate = _normalColor;

        MouseEntered += OnMouseEntered;
        MouseExited += OnMouseExited;
        GuiInput += OnGuiInput;

        Refresh();
    }

    public void Refresh()
    {
        WeaponSprite.Texture = null;

        if (WeaponSprite == null)
        {
            return;
        }

        string weaponKey = null;
        try
        {
            weaponKey = G.WI.GetEquippedWeaponKey(SlotKey);
        }
        catch (Exception e)
        {
            Log.Err($"GetEquippedWeaponKey threw: {e.Message}");
        }

        if (string.IsNullOrEmpty(weaponKey))
        {
            return;
        }

        WeaponStateComponent state = null;
        try
        {
            state = G.WI.GetWeaponState(weaponKey);
        }
        catch (Exception e)
        {
            Log.Err($"GetWeaponState threw: {e.Message}");
        }

        if (state == null || state.BaseData == null || string.IsNullOrEmpty(state.BaseData.IconPath))
        {
            Log.Err($"Invalid WeaponState, BaseData or IconPath");
            return;
        }

        var tex = GD.Load<Texture2D>(state.BaseData.IconPath);
        if (tex == null)
        {
            Log.Err($"Could not load icon at {state.BaseData.IconPath}");
            return;
        }

        WeaponSprite.Texture = tex;
        WeaponSprite.Visible = true;
    }

    private void OnGuiInput(InputEvent @event)
    {
        if (@event is InputEventMouseButton mb && mb.Pressed && mb.ButtonIndex == MouseButton.Left)
            OnClicked();
    }

    private void OnMouseEntered() => Modulate = _hoverColor;
    private void OnMouseExited() => Modulate = _normalColor;

    private void OnClicked()
    {
        G.SFX.Play(SFX.CLICK);
        EmitSignal(SignalName.SlotClicked, SlotKey);
        _ = AnimateClicked();
    }

    private async System.Threading.Tasks.Task AnimateClicked()
    {
        if (_animating) return;
        _animating = true;

        var colorTween = GetTree().CreateTween();
        Modulate = _clickColor;
        colorTween.TweenProperty(this, "modulate", _normalColor, 0.2f);

        var scaleTween = GetTree().CreateTween();
        Scale = Vector2.One;
        scaleTween.TweenProperty(this, "scale", new Vector2(1.05f, 1.05f), 0.1f);
        scaleTween.TweenProperty(this, "scale", Vector2.One, 0.1f);

        await ToSignal(scaleTween, "finished");
        _animating = false;
    }
}
