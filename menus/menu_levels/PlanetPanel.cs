using Godot;

[GlobalClass]
public partial class PlanetPanel : PanelContainer
{
    [Signal]
    public delegate void PlanetClickedEventHandler(string levelKey);

    [Export] public string LevelKey;
    [Export] public AnimatedSprite2D PlanetSprite;
    [Export] public Sprite2D StatusSprite;
    [Export] public bool Available;
    [Export] public bool Cleared;

    private Color _disabledColor = new Color(20f / 255f, 20f / 255f, 20f / 255f, 255f / 255f);
    private Color _normalColor = new Color(200f / 255f, 200f / 255f, 200f / 255f, 255f / 255f);
    private Color _hoverColor = new Color(230f / 255f, 230f / 255f, 230f / 255f, 255f / 255f);
    private Color _clickColor = new Color(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);

    public override void _Ready()
    {
        Cleared = G.GS.IsLevelCleared(LevelKey);
        Available = G.GS.IsLevelAvailable(LevelKey);

        Modulate = Available ? _normalColor : _disabledColor;
        StatusSprite.Visible = Available && !Cleared;
        StatusSprite.Offset = new Vector2(95, 95);
        PlanetSprite.Offset = new Vector2(10, 10);

        MouseEntered += OnMouseEntered;
        MouseExited += OnMouseExited;
        GuiInput += OnGuiInput;
    }

    private void OnGuiInput(InputEvent @event)
    {
        if (@event is InputEventMouseButton mouseEvent && mouseEvent.Pressed && mouseEvent.ButtonIndex == MouseButton.Left)
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
        EmitSignal(SignalName.PlanetClicked, LevelKey);
        AnimateClicked();
    }

    private async void AnimateClicked()
    {
        var colorTween = GetTree().CreateTween();
        Modulate = new Color(1f, 1f, 1f, 1f);
        colorTween.TweenProperty(this, "modulate", _normalColor, 0.2f);

        var scaleTween = GetTree().CreateTween();
        Scale = Vector2.One;
        scaleTween.TweenProperty(this, "scale", new Vector2(1.05f, 1.05f), 0.1f);
        scaleTween.TweenProperty(this, "scale", Vector2.One, 0.1f);

        await ToSignal(scaleTween, "finished");
    }
}
