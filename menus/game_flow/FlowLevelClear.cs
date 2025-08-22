using Godot;
using System;
using System.Threading.Tasks;

[GlobalClass]
public partial class FlowLevelClear : CanvasLayer
{
    [Export] public Label MessageLabel;
    [Export] public MenuFadeComponent MenuFadeComponent { get; set; }

    private readonly string[] _sassyMessages =
    {
        "With my help, obviously. Sure..",
        "You survived?? Sure..",
        "I purred for luck.. wait no, food.",
        "Not bad for a non-feline..",
        "Don’t get used to it.",
        "I suppose that counts as ‘winning’.",
        "Do cats clap? No. You’re welcome.",
        "Don’t ruin it.",
        "I’ll put this in your ‘not useless’ file.",
        "Try not to shed on the controls next time.",
        "Nine lives, infinite wins. For me, not you."
    };

    public override void _Ready()
    {
        string randomMessage = _sassyMessages[GD.Randi() % _sassyMessages.Length];
        MessageLabel.Text = randomMessage;
        _ = RunFlowAsync();
    }

    private async Task RunFlowAsync()
    {
        await Task.Delay(2500);
        await MenuFadeComponent.FadeOutAsync();
        await G.GF.FadeToSceneBasic(G.GF.MenuLevelsScene, 0.1f);
    }
}