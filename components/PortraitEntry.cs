using Godot;

[GlobalClass]
public partial class PortraitEntry : Resource
{
    [Export] public string Key { get; set; }
    [Export] public string DisplayName { get; set; }
    [Export] public string AnimationKey { get; set; }
    [Export] public SpriteFrames Frames { get; set; }
}