using Godot;
using System.Threading.Tasks;

public partial class AutoBackground : ParallaxBackground
{
    public static AutoBackground Instance { get; private set; }

    [Export] private ParallaxLayer _background;
    [Export] private ParallaxLayer _slowLayer;
    [Export] private ParallaxLayer _fastLayer;

    [Export] private ColorRect _fadeBlack;
    [Export] private ColorRect _fadeWhite;
    [Export] private AnimatedSprite2D _loading;

    [Export] private Control _inputBlocker;

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
        }
        _inputBlocker.GuiInput += OnInputBlockerGuiInput;
    }

    public void ShowStars()
    {
        _background.Visible = true;
        _slowLayer.Visible = true;
        _fastLayer.Visible = true;

        _background.Modulate = new Color(1, 1, 1, 1);
        _slowLayer.Modulate = new Color(1, 1, 1, 1);
        _fastLayer.Modulate = new Color(1, 1, 1, 1);
    }

    public void HideStars()
    {
        _background.Visible = false;
        _slowLayer.Visible = false;
        _fastLayer.Visible = false;

        _background.Modulate = new Color(1, 1, 1, 0);
        _slowLayer.Modulate = new Color(1, 1, 1, 0);
        _fastLayer.Modulate = new Color(1, 1, 1, 0);
    }

    public async Task FadeInStars(float backgroundDuration = 1.2f, float slowDuration = 0.8f, float fastDuration = 0.5f)
    {
        _background.Visible = true;
        _slowLayer.Visible = true;
        _fastLayer.Visible = true;

        var tween = GetTree().CreateTween();
        tween.TweenProperty(_background, "modulate:a", 1f, backgroundDuration);
        tween.TweenProperty(_slowLayer, "modulate:a", 1f, slowDuration);
        tween.TweenProperty(_fastLayer, "modulate:a", 1f, fastDuration);
        await ToSignal(tween, "finished");
    }

    public async Task FadeOutStars(float backgroundDuration = 1.2f, float slowDuration = 0.8f, float fastDuration = 0.5f)
    {
        var tween = GetTree().CreateTween();
        tween.TweenProperty(_fastLayer, "modulate:a", 0f, fastDuration);
        tween.TweenProperty(_slowLayer, "modulate:a", 0f, slowDuration);
        tween.TweenProperty(_background, "modulate:a", 0f, backgroundDuration);
        await ToSignal(tween, "finished");

        _background.Visible = false;
        _slowLayer.Visible = false;
        _fastLayer.Visible = false;
    }

    public async Task FadeInBlack(float duration = 0.5f)
    {
        _fadeBlack.Visible = true;

        var tween = GetTree().CreateTween();
        tween.TweenProperty(_fadeBlack, "modulate:a", 1f, duration);
        await ToSignal(tween, "finished");
    }

    public async Task FadeOutBlack(float duration = 0.5f)
    {
        var tween = GetTree().CreateTween();
        tween.TweenProperty(_fadeBlack, "modulate:a", 0f, duration);
        await ToSignal(tween, "finished");

        _fadeBlack.Visible = false;
    }

    public async Task FadeInWhite(float duration = 0.5f)
    {
        _fadeWhite.Visible = true;

        var tween = GetTree().CreateTween();
        tween.TweenProperty(_fadeWhite, "modulate:a", 1f, duration);
        await ToSignal(tween, "finished");
    }

    public async Task FadeOutWhite(float duration = 0.5f)
    {
        var tween = GetTree().CreateTween();
        tween.TweenProperty(_fadeWhite, "modulate:a", 0f, duration);
        await ToSignal(tween, "finished");

        _fadeWhite.Visible = false;
    }

    public async Task FadeInLoading(float duration = 0.5f)
    {
        _loading.Visible = true;

        var tween = GetTree().CreateTween();
        tween.TweenProperty(_loading, "modulate:a", 1f, duration);
        await ToSignal(tween, "finished");
    }

    public async Task FadeOutLoading(float duration = 0.5f)
    {
        var tween = GetTree().CreateTween();
        tween.TweenProperty(_loading, "modulate:a", 0f, duration);
        await ToSignal(tween, "finished");

        _loading.Visible = false;
    }

    public void BlockInput()
    {
        _inputBlocker.Visible = true;
    }

    public void UnblockInput()
    {
        _inputBlocker.Visible = false;
    }

    private void OnInputBlockerGuiInput(InputEvent @event)
    {
        if (@event is InputEventMouseButton mouseEvent && mouseEvent.Pressed)
        {
            //Log.Dbg("Input blocker clicked");
        }
    }
}
