using Godot;
using System;
using System.Threading;
using System.Threading.Tasks;

[GlobalClass]
public abstract partial class BaseLevelScript : Node
{
	[Export] public Ship Ship { get; set; }
	[Export] public LevelFlowComponent LevelFlowComponent { get; set; }
	[Export] public SpawnerComponent Enemy1Spawner { get; set; }
	[Export] public SpawnerComponent Enemy2Spawner { get; set; }
	[Export] public SpawnerComponent Enemy3Spawner { get; set; }
	[Export] public HUDMain HUD { get; set; }
	[Export] public int LevelMultiplier = 1;
	[Export] public float CurrencyMultiplier = 1f;
	[Export] public float EnemyHealthMultiplier = 3f;
	[Export] public float EnemyDamageMultiplier = 2f;

	public override void _Ready()
	{
		G.MS.PlayTrack(GetLevelMusic());

		Ship.StatsComponent.Connect("NoHealth", new Callable(this, nameof(OnShipDeath)));
		HUD.Powers.SetWeaponManager(Ship.WeaponManager);

		G.GS.LevelMultiplier = LevelMultiplier;
		G.GS.CurrencyMultiplier = CurrencyMultiplier;
		G.GS.EnemyHealthMultiplier = EnemyHealthMultiplier;
		G.GS.EnemyDamageMultiplier = EnemyDamageMultiplier;

		G.CR.Run("LevelScript", RunLevelScript);
	}

	private async Task RunLevelScript(CancellationToken token)
	{
		try
		{
			await RunLevel(token);
		}
		catch (TaskCanceledException)
		{
			Log.Dbg("Script canceled");
		}
	}

	protected abstract String GetLevelMusic();
	protected abstract int GetLevelId();
	protected abstract Task RunLevel(CancellationToken token);

	protected void StartDialog()
	{
		Ship.MoveComponent.Velocity = Vector2.Zero;
		G.GF.StartDialog();
		Ship.StopFiring();
	}

	protected void StopDialog()
	{
		G.GF.StopDialog();
		Ship.StartFiring();
	}

	protected void StopEndingDialog()
	{
		G.GF.StopDialog();
	}

	protected async Task HandleLevelClear()
	{
		G.GS.Save();
		G.WI.Save();
		G.SS.Save();
		G.BG.BlockInput();
		G.GF.BlockInput();
		await LevelFlowComponent.FadeOutAll();

		Ship.StopFiring();
		Ship.PositionClampComponent.Enabled = false;
		Ship.MoveComponent.Velocity = new Vector2(0, -250);

		await Task.Delay(3000);
		G.MS.FadeOut();

		await G.BG.FadeInBlack(0.3f);
		G.GF.UnblockInput();

		await G.GF.FadeToSceneWithBG(G.GF.LevelClearScene);
	}

	private void OnShipDeath()
	{
		G.GS.Load();
		G.WI.Load();
		G.SS.Load();
		G.MS.FadeOut();
		G.SFX.Play(SFX.OIIA_DEATH);
		G.CR.Stop("LevelScript");
	}

	protected async Task DefaultDialog()
	{
		await HUD.FirstMessage(Char.COMMANDER, Mood.COMMANDER.Default, "Allright rookie, we've done this before.");
		await HUD.LastMessage(Char.ROOKIE, Mood.ROOKIE.Default, "You got this!");
	}

	protected async Task DefaultClearDialog()
	{
		await HUD.FirstMessage(Char.COMMANDER, Mood.COMMANDER.Default, "Good job rookie!");
		await HUD.LastMessage(Char.COMMANDER, Mood.COMMANDER.Default, "See you on the next run.");
	}
}
