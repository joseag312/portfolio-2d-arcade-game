using Godot;
using System;
using System.Threading.Tasks;

[GlobalClass]
public partial class MenuSettings : Control
{
    [Export] public MenuFadeComponent MenuFadeComponent;
    [Export] public MenuLoadComponent MenuLoadComponent;

    [ExportGroup("Preferences Section")]
    [Export] public VBoxContainer PreferencesContainerVBox;
    [Export] public HBoxContainer ReturnContainerHBox;
    [Export] public HSlider OpacitySlider;
    [Export] public Label OpacityLabel;
    [Export] public HSlider VolumeSlider;
    [Export] public Label VolumeLabel;
    [Export] public HSlider SfxSlider;
    [Export] public Label SfxLabel;
    [Export] public Button MuteToggleButton;
    [Export] public Button StartOverButton;
    [Export] public Button SaveSettingsButton;
    [Export] public Button ReturnButton;

    [ExportGroup("Reset Section")]
    [Export] public VBoxContainer StartOverContainerVBox;
    [Export] public Button ConfirmStartOverButton;
    [Export] public Button CancelStartOverButton;

    private static float LinearToDb(float linear)
    {
        if (linear <= 0.001f)
            return -80f;
        return 20f * (float)Math.Log10(linear);
    }

    public override void _Ready()
    {
        OpacitySlider.ValueChanged += OnOpacitySliderChanged;
        VolumeSlider.ValueChanged += OnVolumeSliderChanged;
        SfxSlider.ValueChanged += OnSfxSliderChanged;
        MuteToggleButton.Pressed += OnMuteToggleButtonPressed;

        SaveSettingsButton.Pressed += OnSaveSettingsPressed;
        ReturnButton.Pressed += OnReturnPressed;

        StartOverButton.Pressed += OnStartOverPressed;
        ConfirmStartOverButton.Pressed += OnConfirmStartOverPressed;
        CancelStartOverButton.Pressed += OnCancelStartOverPressed;

        OpacitySlider.Value = G.CF.HudOpacity;
        OpacityLabel.Text = $"{(int)(G.CF.HudOpacity * 100)}%";

        VolumeSlider.Value = G.CF.MasterVolume;
        VolumeLabel.Text = $"{(int)(G.CF.MasterVolume * 100)}%";

        SfxSlider.Value = G.CF.SfxVolume;
        SfxLabel.Text = $"{(int)(G.CF.SfxVolume * 100)}%";

        MuteToggleButton.ButtonPressed = G.CF.IsMuted;

        G.MS.SetVolumeDb(LinearToDb(G.CF.MasterVolume));
        G.SFX.SetVolumeDb(LinearToDb(G.CF.SfxVolume));
        AudioServer.SetBusMute(AudioServer.GetBusIndex("Music"), G.CF.IsMuted);
        AudioServer.SetBusMute(AudioServer.GetBusIndex("SFX"), G.CF.IsMuted);
    }

    private async void OnStartOverPressed()
    {
        G.SFX.Play(SFX.CLICK);
        await MenuFadeComponent.FadeOutAsync();
        PreferencesContainerVBox.Visible = false;
        ReturnContainerHBox.Visible = false;
        StartOverContainerVBox.Visible = true;
        MenuLoadComponent.FadeIn();
    }

    private async void OnCancelStartOverPressed()
    {
        G.SFX.Play(SFX.CLICK);
        await MenuFadeComponent.FadeOutAsync();
        StartOverContainerVBox.Visible = false;
        ReturnContainerHBox.Visible = true;
        PreferencesContainerVBox.Visible = true;
        MenuLoadComponent.FadeIn();
    }

    private async void OnConfirmStartOverPressed()
    {
        G.SFX.Play(SFX.MEOW);
        G.GS.Reset();
        G.SS.Reset();
        G.WI.Reset();
        G.CF.Reset();
        await MenuFadeComponent.FadeOutAsync();
        await G.GF.FadeToSceneBasic(G.GF.MenuMainScene);
    }

    private async void OnSaveSettingsPressed()
    {
        G.SFX.Play(SFX.CLICK);
        G.CF.Save();
        await MenuFadeComponent.FadeOutAsync();
        await G.GF.FadeToSceneBasic(G.GF.MenuMainScene);
    }

    private async void OnReturnPressed()
    {
        G.SFX.Play(SFX.CLICK);
        await MenuFadeComponent.FadeOutAsync();
        await G.GF.FadeToSceneBasic(G.GF.MenuMainScene);
    }

    private void OnOpacitySliderChanged(double value)
    {
        float val = (float)value;
        OpacityLabel.Text = $"{(int)(val * 100)}%";
        G.CF.HudOpacity = val;
        G.CF.Save();
    }

    private void OnVolumeSliderChanged(double value)
    {
        float val = (float)value;
        VolumeLabel.Text = $"{(int)(val * 100)}%";
        G.CF.MasterVolume = val;

        G.MS.SetVolumeDb(LinearToDb(val));
        G.CF.Save();
    }

    private void OnSfxSliderChanged(double value)
    {
        float val = (float)value;
        SfxLabel.Text = $"{(int)(val * 100)}%";
        G.CF.SfxVolume = val;

        G.SFX.SetVolumeDb(LinearToDb(val));
        G.CF.Save();
    }

    private void OnMuteToggleButtonPressed()
    {
        G.SFX.Play(SFX.CLICK);
        G.CF.IsMuted = !G.CF.IsMuted;
        AudioServer.SetBusMute(AudioServer.GetBusIndex("Music"), G.CF.IsMuted);
        AudioServer.SetBusMute(AudioServer.GetBusIndex("SFX"), G.CF.IsMuted);
        G.CF.Save();
    }
}
