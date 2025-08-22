using Godot;
using System;
using System.Threading.Tasks;

[GlobalClass]
public partial class MenuOpening : Control
{
    [Export] public AnimationPlayer AnimationPlayer { get; set; }

    public override void _Ready()
    {
        G.SS.Load();
        G.GS.Load();
        PlayOpening();
    }

    public void PlayOpening()
    {
        AnimationPlayer.Play("MenuOpening/Opening");
    }

    public async Task MainMenuLoad()
    {
        await G.GF.FadeToSceneBasic(G.GF.MenuMainScene);
    }

    public void MainMenuLoadWrapper()
    {
        _ = MainMenuLoad();
    }
}
