using Godot;
using System;

[GlobalClass]
public partial class MenuLoadComponent : Node
{
    [Export] public CenterContainer CanvasContainer;
    [Export] public float FadeDuration = 0.5f;

    public override void _Ready()
    {
        if (CanvasContainer == null)
        {
            Log.Err("CanvasRoot is not set");
            return;
        }

        FadeIn();
    }

    public void FadeIn()
    {
        CanvasContainer.Visible = true;
        CanvasContainer.Modulate = new Color(1, 1, 1, 0);

        var tween = GetTree().CreateTween();
        tween.TweenProperty(CanvasContainer, "modulate:a", 1f, FadeDuration);

        tween.Finished += () =>
        {
            G.BG.UnblockInput();
            G.GF.ResetTransition();
        };
    }
}
