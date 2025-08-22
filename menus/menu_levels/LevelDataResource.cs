using Godot;

[GlobalClass]
public partial class LevelDataResource : Resource
{
    [Export] public string Key;
    [Export] public string DisplayName;
    [Export] public string ScenePath;

    [Export] public Texture2D[] EnemySprites = new Texture2D[3];
    [Export] public Texture2D[] RecommendedPowerups = new Texture2D[2];
}