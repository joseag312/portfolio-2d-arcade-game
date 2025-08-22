using Godot;
using System;
using System.Threading.Tasks;

[GlobalClass]
public partial class MenuMain : Control
{
    [Export] public Button StartButton { get; set; }
    [Export] public Button SettingsButton { get; set; }
    [Export] public Button QuitButton { get; set; }
    [Export] public MenuFadeComponent MenuFadeComponent { get; set; }

    private string _nextScenePath;

    public override void _Ready()
    {
        StartButton.Pressed += OnStartPressed;
        SettingsButton.Pressed += OnSettingsPressed;
        QuitButton.Pressed += OnQuitPressed;
        G.MS.PlayTrack(Music.MAIN);
    }

    private async void OnStartPressed()
    {
        G.MS.Stop();
        G.SFX.Play(SFX.OIIA_SLOW);
        await MenuFadeComponent.FadeOutAsync();
        await ToSignal(GetTree().CreateTimer(1f), "timeout");
        await G.GF.FadeToSceneBasic(G.GF.MenuLevelsScene);
    }

    private async void OnSettingsPressed()
    {
        G.SFX.Play(SFX.CLICK);
        await MenuFadeComponent.FadeOutAsync();
        await G.GF.FadeToSceneBasic(G.GF.MenuSettingsScene);
    }

    private async void OnQuitPressed()
    {
        G.SFX.Play(SFX.CLICK);
        await MenuFadeComponent.FadeOutAsync();
        await G.GF.FadeToSceneFadeBG("");
    }
}
