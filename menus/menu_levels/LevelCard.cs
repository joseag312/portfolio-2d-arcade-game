using Godot;
using System;

[GlobalClass]
public partial class LevelCard : VBoxContainer
{

    [Export] public MenuFadeComponent MenuFadeComponent;
    [Export] public Label DisplayName;
    [Export] public Sprite2D enemy1;
    [Export] public Sprite2D enemy2;
    [Export] public Sprite2D enemy3;
    [Export] public Sprite2D powerUp1;
    [Export] public Sprite2D powerUp2;
    [Export] public Label EnemyLabel;
    [Export] public Label PowerUpLabel;
    [Export] public HBoxContainer EnemyContainer;
    [Export] public HBoxContainer PowerUpContainer;
    [Export] public HBoxContainer PlayContainer;
    [Export] public Button PlayButton;
    [Export] public string LevelPath { get; private set; }

    public override void _Ready()
    {
        PlayButton.Pressed += OnPlayButtonPressed;
    }

    public void LoadLevel(string levelId)
    {
        var data = LevelCatalog.Get(levelId);
        if (data == null)
        {
            Log.Err($"No data found for LevelId: {levelId}");
            return;
        }

        DisplayName.Text = data.DisplayName;
        LevelPath = data.ScenePath;

        enemy1.Texture = GetOrNull(data.EnemySprites, 0);
        enemy2.Texture = GetOrNull(data.EnemySprites, 1);
        enemy3.Texture = GetOrNull(data.EnemySprites, 2);

        powerUp1.Texture = GetOrNull(data.RecommendedPowerups, 0);
        powerUp2.Texture = GetOrNull(data.RecommendedPowerups, 1);

        DisplayName.Visible = true;
        EnemyLabel.Visible = true;
        PowerUpLabel.Visible = true;
        EnemyContainer.Visible = true;
        PowerUpContainer.Visible = true;
        PlayContainer.Visible = true;
    }

    private Texture2D GetOrNull(Texture2D[] array, int index)
    {
        return array != null && index < array.Length ? array[index] : null;
    }

    public async void OnPlayButtonPressed()
    {
        if (!string.IsNullOrEmpty(LevelPath))
        {
            G.SFX.Play(SFX.CLICK);
            await MenuFadeComponent.FadeOutAsync();
            G.MS.Stop();
            await G.GF.FadeToSceneWithLoading(LevelPath);
        }
        else
        {
            Log.Err("LevelPath is null or empty, can't load scene");
        }
    }
}
