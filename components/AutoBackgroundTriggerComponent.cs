using Godot;
using System;

[GlobalClass]
public partial class AutoBackgroundTriggerComponent : Node
{
    public void BlockInput()
    {
        G.BG.BlockInput();
    }

    public void UnblockInput()
    {
        G.BG.UnblockInput();
    }

    public void ShowStars()
    {
        G.BG.ShowStars();
    }

    public void HideStars()
    {
        G.BG.HideStars();
    }

    public void FadeInStars()
    {
        _ = G.BG.FadeInStars();
    }

    public void FadeOutStars()
    {
        _ = G.BG.FadeOutStars();
    }

    public void FadeInBlack(float duration = 0.5f)
    {
        _ = G.BG.FadeInBlack(duration);
    }

    public void FadeOutBlack(float duration = 0.5f)
    {
        _ = G.BG.FadeOutBlack(duration);
    }

    public void FadeInWhite(float duration = 0.5f)
    {
        _ = G.BG.FadeInWhite(duration);
    }

    public void FadeOutWhite(float duration = 0.5f)
    {
        _ = G.BG.FadeOutWhite(duration);
    }

    public void FadeInLoading(float duration = 0.5f)
    {
        _ = G.BG.FadeInLoading(duration);
    }

    public void FadeOutLoading(float duration = 0.5f)
    {
        _ = G.BG.FadeOutLoading(duration);
    }
}
