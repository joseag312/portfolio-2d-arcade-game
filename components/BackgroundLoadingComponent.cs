using Godot;
using System.Collections.Generic;

[GlobalClass]
public partial class BackgroundLoadingComponent : Node
{
    [Export] public CanvasItem BackgroundLayer { get; set; }
    [Export] public CanvasItem SlowLayer { get; set; }
    [Export] public CanvasItem FastLayer { get; set; }
    [Export] public AutoBackgroundTriggerComponent autoBackgroundTriggerComponent;

    private readonly List<Tween> _activeTweens = new();

    public override void _Ready()
    {
        ResetLayer(BackgroundLayer);
        ResetLayer(SlowLayer);
        ResetLayer(FastLayer);
        FadeInAll(new Color(1, 1, 1, 1), 1.0f);
        autoBackgroundTriggerComponent.FadeOutBlack();
        autoBackgroundTriggerComponent.FadeOutWhite();
    }

    private void ResetLayer(CanvasItem layer)
    {
        if (layer == null) return;
        layer.Visible = false;
        layer.Modulate = new Color(1, 1, 1, 0);
    }

    private void TweenLayer(CanvasItem layer, Color targetColor, float duration)
    {
        if (layer == null) return;

        layer.Visible = true;

        var tween = GetTree().CreateTween();
        tween.TweenProperty(layer, "modulate", targetColor, duration);
        _activeTweens.Add(tween);
    }

    public void FadeInAll(Color color, float duration = 1.0f)
    {
        ClearTweens();
        TweenLayer(BackgroundLayer, color, duration);
        TweenLayer(SlowLayer, color, duration);
        TweenLayer(FastLayer, color, duration);
    }

    public void FadeInSlow(Color color, float duration = 1.0f)
    {
        ClearTweens();
        TweenLayer(SlowLayer, color, duration);
    }

    public void FadeInFast(Color color, float duration = 1.0f)
    {
        ClearTweens();
        TweenLayer(FastLayer, color, duration);
    }

    public void SetAllModulate(Color color)
    {
        if (BackgroundLayer != null) BackgroundLayer.Modulate = color;
        if (SlowLayer != null) SlowLayer.Modulate = color;
        if (FastLayer != null) FastLayer.Modulate = color;
    }

    private void ClearTweens()
    {
        foreach (var tween in _activeTweens)
            if (IsInstanceValid(tween))
                tween.Kill();

        _activeTweens.Clear();
    }
}
