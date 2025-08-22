using Godot;

public partial class AutoSettings : Node
{
    public static AutoSettings Instance { get; private set; }

    private const string SavePath = "user://settings.dat";

    public float HudOpacity { get; set; } = 1.0f;
    public float MasterVolume { get; set; } = 1.0f;
    public float SfxVolume { get; set; } = 1.0f;
    public bool IsMuted { get; set; } = false;

    public override void _Ready()
    {
        if (Instance == null)
        {
            Instance = this;
            Load();
        }
        else
        {
            QueueFree();
        }
    }

    public void Save()
    {
        var dict = new Godot.Collections.Dictionary
        {
            { "hud_opacity", HudOpacity },
            { "master_volume", MasterVolume },
            { "sfx_volume", SfxVolume },
            { "is_muted", IsMuted }
        };

        using var file = FileAccess.Open(SavePath, FileAccess.ModeFlags.Write);
        file.StoreVar(dict);
    }

    public void Load()
    {
        if (!FileAccess.FileExists(SavePath))
        {
            return;
        }

        using var file = FileAccess.Open(SavePath, FileAccess.ModeFlags.Read);
        Variant raw = file.GetVar();

        if (raw.Obj is not Godot.Collections.Dictionary dict)
        {
            return;
        }

        HudOpacity = dict.ContainsKey("hud_opacity") ? (float)dict["hud_opacity"] : 1.0f;
        MasterVolume = dict.ContainsKey("master_volume") ? (float)dict["master_volume"] : 1.0f;
        SfxVolume = dict.ContainsKey("sfx_volume") ? (float)dict["sfx_volume"] : 1.0f;
        IsMuted = dict.ContainsKey("is_muted") ? (bool)dict["is_muted"] : false;
    }

    public void Reset()
    {
        HudOpacity = 1.0f;
        MasterVolume = 1.0f;
        SfxVolume = 1.0f;
        IsMuted = false;
        Save();
    }

    public float GetHudOpacity()
    {
        return 0.2f + 0.8f * HudOpacity;
    }
}
