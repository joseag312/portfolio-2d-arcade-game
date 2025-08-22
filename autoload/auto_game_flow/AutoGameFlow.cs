using Godot;
using System.Threading.Tasks;

public partial class AutoGameFlow : Node
{
    public static AutoGameFlow Instance { get; private set; }

    public string LastPlayedScene { get; set; } = "";
    public string MenuMainScene = "res://menus/menu_main/menu_main.tscn";
    public string MenuLevelsScene = "res://menus/menu_levels/menu_levels.tscn";
    public string MenuStoreScene = "res://menus/menu_store/menu_store.tscn";
    public string MenuSettingsScene = "res://menus/menu_settings/menu_settings.tscn";
    public string GameOverScene = "res://menus/game_flow/flow_game_over.tscn";
    public string LevelClearScene = "res://menus/game_flow/flow_level_clear.tscn";

    private bool _isTransitioning = false;
    private bool _isInputBlocked = false;
    public bool IsInputBlocked => _isInputBlocked;

    public override void _Ready()
    {
        if (Instance == null)
        {
            Instance = this;
            ProcessMode = ProcessModeEnum.Always;
            DisplayServer.WindowSetTitle("OIIASure");
        }
        else
        {
            QueueFree();
        }
    }

    public void BlockInput()
    {
        G.BG.BlockInput();
        _isInputBlocked = true;
    }

    public void UnblockInput()
    {
        G.BG.UnblockInput();
        _isInputBlocked = false;
    }

    public void StartDialog()
    {
        _isInputBlocked = true;
    }

    public void StopDialog()
    {
        _isInputBlocked = false;
    }

    public async Task FadeToSceneBasic(string path, float fadeDuration = 0.3f)
    {
        if (_isTransitioning) return;
        _isTransitioning = true;

        G.BG.BlockInput();
        await G.BG.FadeInBlack(fadeDuration);
        ChangeSceneSafely(path);
    }

    public async Task FadeToSceneKeepBG(string path, float fadeDuration = 0.3f)
    {
        if (_isTransitioning) return;
        _isTransitioning = true;

        G.BG.BlockInput();
        await G.BG.FadeInBlack(fadeDuration);
        ChangeSceneSafely(path);
    }

    public async Task FadeToSceneWithBG(string path, float fadeDuration = 0.3f)
    {
        if (_isTransitioning) return;
        _isTransitioning = true;

        G.BG.BlockInput();
        await G.BG.FadeInStars();
        await G.BG.FadeInBlack(fadeDuration);
        ChangeSceneSafely(path);
    }

    public async Task FadeToSceneWithBGFast(string path)
    {
        if (_isTransitioning) return;
        _isTransitioning = true;

        G.BG.BlockInput();
        await G.BG.FadeInStars(0.4f, 0.3f, 0.2f);
        await G.BG.FadeInBlack(0.2f);
        ChangeSceneSafely(path);
    }

    public async Task FadeToSceneFadeBG(string path, float fadeDuration = 0.3f)
    {
        if (_isTransitioning) return;
        _isTransitioning = true;

        G.BG.BlockInput();
        await G.BG.FadeOutStars();
        await G.BG.FadeInBlack(fadeDuration);
        ChangeSceneSafely(path);
    }

    public async Task FadeToSceneWithLoading(string path, float fadeDuration = 0.1f, float loadingFade = 0.3f, float holdDelay = 0.3f)
    {
        if (_isTransitioning) return;
        _isTransitioning = true;

        G.BG.BlockInput();
        await G.BG.FadeOutStars(0.4f, 0.3f, 0.2f);
        await G.BG.FadeInBlack(fadeDuration);
        G.SFX.Play(SFX.MEOW);
        await G.BG.FadeInLoading(loadingFade);
        await ToSignal(GetTree().CreateTimer(holdDelay), "timeout");
        await G.BG.FadeOutLoading(loadingFade);
        ChangeSceneSafely(path);
    }

    public void ResetTransition()
    {
        _isTransitioning = false;
    }

    private void ChangeSceneSafely(string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            GetTree().Quit();
        }
        else
        {
            GetTree().ChangeSceneToFile(path);
        }
    }
}
