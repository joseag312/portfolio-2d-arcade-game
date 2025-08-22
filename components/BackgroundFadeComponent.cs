using Godot;
using System.Collections.Generic;
using System.Threading.Tasks;

[GlobalClass]
public partial class BackgroundFadeComponent : Node
{
    [Export] public CanvasItem BackgroundLayer { get; set; }
    [Export] public CanvasItem SlowLayer { get; set; }
    [Export] public CanvasItem FastLayer { get; set; }

    private readonly List<Tween> _activeTweens = new();

    public async Task FadeOutAll(float duration = 0.5f)
    {
        ClearTweens();
        var awaiters = new List<SignalAwaiter>
        {
            TweenAlpha(BackgroundLayer, 0.0f, duration),
            TweenAlpha(SlowLayer, 0.0f, duration),
            TweenAlpha(FastLayer, 0.0f, duration)
        };

        foreach (var awaiter in awaiters)
        {
            if (awaiter != null)
                await awaiter;
        }
    }

    public async Task FadeInAll(float duration = 0.5f)
    {
        ClearTweens();
        var awaiters = new List<SignalAwaiter>
        {
            TweenAlpha(BackgroundLayer, 1.0f, duration),
            TweenAlpha(SlowLayer, 1.0f, duration),
            TweenAlpha(FastLayer, 1.0f, duration)
        };

        foreach (var awaiter in awaiters)
        {
            if (awaiter != null)
                await awaiter;
        }
    }

    public async Task FadeOutSlow(float duration = 0.5f)
    {
        ClearTweens();
        var awaiter = TweenAlpha(SlowLayer, 0.0f, duration);
        if (awaiter != null)
            await awaiter;
    }

    public async Task FadeInSlow(float duration = 0.5f)
    {
        ClearTweens();
        var awaiter = TweenAlpha(SlowLayer, 1.0f, duration);
        if (awaiter != null)
            await awaiter;
    }

    public async Task FadeOutFast(float duration = 0.5f)
    {
        ClearTweens();
        var awaiter = TweenAlpha(FastLayer, 0.0f, duration);
        if (awaiter != null)
            await awaiter;
    }

    public async Task FadeInFast(float duration = 0.5f)
    {
        ClearTweens();
        var awaiter = TweenAlpha(FastLayer, 1.0f, duration);
        if (awaiter != null)
            await awaiter;
    }

    public void SetAllAlpha(float alpha)
    {
        SetAlpha(BackgroundLayer, alpha);
        SetAlpha(SlowLayer, alpha);
        SetAlpha(FastLayer, alpha);
    }

    private SignalAwaiter TweenAlpha(CanvasItem layer, float alpha, float duration)
    {
        if (layer == null) return null;

        layer.Visible = true;

        var color = layer.Modulate;
        var target = new Color(color.R, color.G, color.B, alpha);

        var tween = GetTree().CreateTween();
        tween.TweenProperty(layer, "modulate", target, duration);
        _activeTweens.Add(tween);

        return ToSignal(tween, "finished");
    }

    private void SetAlpha(CanvasItem layer, float alpha)
    {
        if (layer == null) return;

        var color = layer.Modulate;
        layer.Modulate = new Color(color.R, color.G, color.B, alpha);
    }

    private void ClearTweens()
    {
        foreach (var tween in _activeTweens)
            if (IsInstanceValid(tween))
                tween.Kill();

        _activeTweens.Clear();
    }
}
