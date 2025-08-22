using Godot;
using System;
using System.Threading;
using System.Threading.Tasks;

public partial class Level001Script : BaseLevelScript
{
	protected override int GetLevelId() => 1;
	protected override String GetLevelMusic() => Music.AMAZING;

	protected override async Task RunLevel(CancellationToken token)
	{
		try
		{
			// Intro
			await Task.Delay(300, token);
			StartDialog();
			await (G.GS.IsDialogSeen("L1Intro") ? DefaultDialog() : LevelDialog());
			StopDialog();

			// Intro PopUp
			await Task.Delay(1000, token);
			_ = HUD.PopUpMessage(Char.ROOKIE, Mood.ROOKIE.Default, "Survive!!!");

			// Wave: Basic
			LevelFlowComponent.SpawnerWave.SpawnWave(Enemy2Spawner, 10, 15);
			LevelFlowComponent.SpawnerRecurrent.StartSpawner1(Enemy1Spawner, 300);
			await Task.Delay(20000, token);
			LevelFlowComponent.SpawnerRecurrent.StopSpawner1();

			// Wave: Basic
			await Task.Delay(3000, token);
			LevelFlowComponent.SpawnerWave.SpawnWave(Enemy2Spawner, 10, 30);
			_ = HUD.PopUpMessage(Char.COMMANDER, Mood.COMMANDER.Default, "Humans...");
			await Task.Delay(3000, token);

			// Recurrent: Kamikaze
			LevelFlowComponent.SpawnerRecurrent.StartSpawner1(Enemy1Spawner, 300);
			_ = HUD.PopUpMessage(Char.COMMANDER, Mood.COMMANDER.Default, "Return at once, cat!");
			await Task.Delay(20000, token);
			LevelFlowComponent.SpawnerRecurrent.StopSpawner1();
			await Task.Delay(500, token);

			// Wave: Basic
			await Task.Delay(1500, token);
			_ = HUD.PopUpMessage(Char.ROOKIE, Mood.ROOKIE.Default, "They're blocking our return!");
			await LevelFlowComponent.SpawnerWave.SpawnWaveUntilCleared(Enemy2Spawner, 10, 30);

			// Clear
			await Task.Delay(3000, token);
			G.GS.MarkLevelCleared("L1");
			await HandleLevelClear();
		}
		catch (TaskCanceledException)
		{
			Log.Dbg("Script canceled");
		}
	}

	public async Task LevelDialog()
	{
		await HUD.FirstMessage(Char.OIIA, Mood.OIIA.Default, "Ah, there he is! The cat I’ve been hearing so much about!");
		await HUD.Message(Char.OIIA, Mood.OIIA.Inverse, "Now... what was that word...");
		await HUD.MessageWithPrompt(Char.OIIA, Mood.OIIA.Default, "Your favorite word in the whole galaxy?");
		await HUD.Message(Char.OIIA, Mood.OIIA.Default, "I knew it. Glad to have you on board, Rookie! Now listen up.");
		await HUD.Message(Char.OIIA, Mood.OIIA.Inverse, "This galaxy isn’t like the others... our main goal is survival.");
		await HUD.Message(Char.OIIA, Mood.OIIA.Glasses, "Your ship isn’t almighty. Only I am.");
		await HUD.Message(Char.OIIA, Mood.OIIA.GlassesZoom, "Now check this out!");
		G.SFX.Play("oiia_fast");
		await HUD.MessageWithPrompt(Char.OIIA, Mood.OIIA.SpinNormal, "See how beautiful this spin is?");
		await HUD.Message(Char.OIIA, Mood.OIIA.Inverse, "I knew you were a cat with taste! Now... Commander, show this new furball around!");
		await HUD.Message(Char.COMMANDER, Mood.COMMANDER.Default, "Yes sir, your Spinningness!");
		string instructions = "";
		do
		{
			await HUD.Message(Char.COMMANDER, Mood.COMMANDER.Default, "Listen up, Rookie! Very simple:");
			await HUD.Message(Char.COMMANDER, Mood.COMMANDER.Default, "Move with: ↑ ↓ ← →");
			await HUD.Message(Char.COMMANDER, Mood.COMMANDER.Default, "Fire cannon with: Space");
			await HUD.Message(Char.COMMANDER, Mood.COMMANDER.Default, "Use powers with: ① ② ③ ④");
			instructions = await HUD.MessageWithPrompt(Char.OIIA, Mood.OIIA.Default, "You got all that?", "Yes sir!", "Huh?");
			if (instructions.Equals("Pessimist"))
			{
				await HUD.Message(Char.OIIA, Mood.OIIA.Inverse, "You were too busy admiring my spinning greatness, weren’t you!");
			}
		}
		while (!(instructions.Equals("Sure") || instructions.Equals("Optimist")));
		await HUD.Message(Char.COMMANDER, Mood.COMMANDER.Default, "Smart cat!");
		if (instructions.Equals("Sure"))
		{
			await HUD.Message(Char.OIIA, Mood.OIIA.Default, "S u r ely, ha!");
			await HUD.Message(Char.COMMANDER, Mood.COMMANDER.Annoyed, "Funny as always, sir...");
		}
		await HUD.Message(Char.COMMANDER, Mood.COMMANDER.Default, "We’re closing in, Rookie — heads up!");
		await HUD.Message(Char.COMMANDER, Mood.COMMANDER.Default, "Today we’re scouting Earth’s defenses.");
		await HUD.Message(Char.OIIA, Mood.OIIA.Inverse, "These humans think they’re the absolute best... only I am.");
		await HUD.Message(Char.COMMANDER, Mood.COMMANDER.Default, "Gather intel on their defenses, then head back immediately.");
		await HUD.LastMessage(Char.ROOKIE, Mood.ROOKIE.Default, "Good luck out there!");
		G.GS.MarkDialogSeen("L1Intro");
	}
}