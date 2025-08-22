using Godot;
using System;
using System.Threading.Tasks;

[GlobalClass]
public partial class MenuFadeComponent : Node
{
    [Export] public CenterContainer CanvasContainer;
    [Export] public float FadeDuration = 0.5f;

    public override void _Ready() { }

    public async Task FadeOutAsync()
    {
        if (CanvasContainer == null)
        {
            Log.Err("CanvasContainer is not set");
            return;
        }

        CanvasContainer.Visible = true;
        CanvasContainer.Modulate = new Color(1, 1, 1, 1);

        var tween = GetTree().CreateTween();
        tween.TweenProperty(CanvasContainer, "modulate:a", 0f, FadeDuration);

        await ToSignal(tween, "finished");

        G.BG.UnblockInput();
        G.GF.ResetTransition();
    }
}