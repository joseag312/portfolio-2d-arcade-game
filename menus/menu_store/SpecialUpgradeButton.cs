using Godot;
using System;

[GlobalClass]
public partial class SpecialUpgradeButton : Button
{
    [Signal] public delegate void SpecialItemClickedEventHandler(string specialKey);

    [Export] public string SpecialKey;
    [Export] public TextureRect IconSprite;

    private Color _normalColor = new Color(150f / 255f, 150f / 255f, 150f / 255f, 255f / 255f);
    private Color _hoverColor = new Color(200f / 255f, 200f / 255f, 200f / 255f, 255f / 255f);
    private Color _clickColor = new Color(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
    private Vector2 _iconBaseScale = Vector2.One;

    public override void _Ready()
    {
        Modulate = _normalColor;

        if (IconSprite != null)
        {
            IconSprite.Modulate = _normalColor;
            _iconBaseScale = IconSprite.Scale;
        }

        MouseEntered += OnMouseEntered;
        MouseExited += OnMouseExited;
        GuiInput += OnGuiInput;
    }

    private void OnGuiInput(InputEvent @event)
    {
        if (@event is InputEventMouseButton mouseEvent &&
            mouseEvent.Pressed &&
            mouseEvent.ButtonIndex == MouseButton.Left)
        {
            OnClicked();
        }
    }

    private void OnMouseEntered()
    {
        if (IconSprite == null) return;
        IconSprite.Modulate = _hoverColor;
    }

    private void OnMouseExited()
    {
        if (IconSprite == null) return;
        IconSprite.Modulate = _normalColor;
        IconSprite.Scale = _iconBaseScale;
    }

    private void OnClicked()
    {
        EmitSignal(SignalName.SpecialItemClicked, SpecialKey);
        AnimateIconClicked();
    }

    private async void AnimateIconClicked()
    {
        if (IconSprite == null) return;

        var colorTween = GetTree().CreateTween();
        IconSprite.Modulate = _clickColor;
        colorTween.TweenProperty(IconSprite, "modulate", _hoverColor, 0.2f);

        var scaleTween = GetTree().CreateTween();
        IconSprite.Scale = _iconBaseScale;
        scaleTween.TweenProperty(IconSprite, "scale", _iconBaseScale * 1.05f, 0.1f);
        scaleTween.TweenProperty(IconSprite, "scale", _iconBaseScale, 0.1f);

        await ToSignal(scaleTween, "finished");
    }
}
