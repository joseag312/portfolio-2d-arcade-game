using Godot;

[GlobalClass]
public partial class StoreItemPanel : PanelContainer
{
    [Signal] public delegate void StoreItemClickedEventHandler(string weaponKey);

    [Export] public string WeaponKey;
    [Export] public TextureRect ContainerSprite;
    [Export] public TextureRect WeaponSprite;
    [Export] public bool Available;

    private Color _normalColor = new Color(150f / 255f, 150f / 255f, 150f / 255f, 255f / 255f);
    private Color _hoverColor = new Color(200f / 255f, 200f / 255f, 200f / 255f, 255f / 255f);
    private Color _clickColor = new Color(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);

    public override void _Ready()
    {
        var weaponState = G.WI.GetWeaponState(WeaponKey);

        Available = weaponState.EffectiveUnlocked;
        Modulate = _normalColor;
        this.Visible = Available;

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
        if (!Available) return;
        Modulate = _hoverColor;
    }

    private void OnMouseExited()
    {
        if (!Available) return;
        Modulate = _normalColor;
    }

    private void OnClicked()
    {
        if (!Available) return;
        EmitSignal(SignalName.StoreItemClicked, WeaponKey);
        AnimateClicked();
    }

    private async void AnimateClicked()
    {
        var colorTween = GetTree().CreateTween();
        Modulate = _clickColor;
        colorTween.TweenProperty(this, "modulate", _normalColor, 0.2f);

        var scaleTween = GetTree().CreateTween();
        Scale = Vector2.One;
        scaleTween.TweenProperty(this, "scale", new Vector2(1.05f, 1.05f), 0.1f);
        scaleTween.TweenProperty(this, "scale", Vector2.One, 0.1f);

        await ToSignal(scaleTween, "finished");
    }
}
