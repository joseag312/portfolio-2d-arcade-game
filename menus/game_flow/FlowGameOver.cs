using Godot;
using System;

[GlobalClass]
public partial class FlowGameOver : CanvasLayer
{
    [Export] public Label MessageLabel;
    [Export] public Button RetryButton;
    [Export] public Button ExitButton;
    [Export] public MenuFadeComponent MenuFadeComponent { get; set; }

    private readonly string[] _sassyMessages =
    {
        "You had one job. OIA!",
        "That was cute. Let me know when you’re ready.",
        "I’m not mad. I’m just... OI.",
        "Pilot error. Clearly not an OIA problem.",
        "Nine lives. You? Barely one.",
        "If cats had thumbs, they'd do it better.",
        "Me-ouch.",
        "Paws off the controls, rookie.",
        "Even my litter box has more structure.",
        "I’ve seen kittens chase lasers with more aim.",
        "Next time, try opening your eyes.",
        "Pro tip: Don’t explode.",
        "Don’t worry, eight respawns left."
    };

    public override void _Ready()
    {
        RetryButton.Pressed += OnRetryPressed;
        ExitButton.Pressed += OnExitPressed;

        string randomMessage = _sassyMessages[GD.Randi() % _sassyMessages.Length];
        MessageLabel.Text = randomMessage;
    }

    private async void OnRetryPressed()
    {
        await MenuFadeComponent.FadeOutAsync();
        await G.GF.FadeToSceneWithLoading(G.GF.LastPlayedScene);
    }

    private async void OnExitPressed()
    {
        await MenuFadeComponent.FadeOutAsync();
        await G.GF.FadeToSceneBasic(G.GF.MenuLevelsScene, 0.1f);
    }
}
