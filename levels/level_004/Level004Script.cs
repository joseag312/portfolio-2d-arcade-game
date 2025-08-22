using Godot;
using System;
using System.Threading;
using System.Threading.Tasks;

public partial class Level004Script : BaseLevelScript
{
	protected override int GetLevelId() => 4;
	protected override String GetLevelMusic() => Music.SNEAKY;

	protected override async Task RunLevel(CancellationToken token)
	{
		try
		{
			// Intro
			await Task.Delay(300, token);
			StartDialog();
			await (G.GS.IsDialogSeen("L4Intro") ? DefaultDialog() : LevelDialog());
			StopDialog();

			// Wave: Aimer
			await Task.Delay(1000, token);
			_ = HUD.PopUpMessage(Char.ROOKIE, Mood.ROOKIE.Default, "They've opened fire!");
			LevelFlowComponent.SpawnerWave.SpawnWave(Enemy2Spawner, 3, 100);
			await Task.Delay(15000, token);

			// Recurrent: Aimer 
			LevelFlowComponent.SpawnerRecurrent.StartSpawner1(Enemy2Spawner, 600);
			await Task.Delay(1000, token);
			_ = HUD.PopUpMessage(Char.COMMANDER, Mood.COMMANDER.Default, "Quick, dodge them!");
			await Task.Delay(7000, token);
			LevelFlowComponent.SpawnerRecurrent.StopSpawner1();

			// Wave: Aimer
			await Task.Delay(3000, token);
			_ = HUD.PopUpMessage(Char.OIIA, Mood.OIIA.Default, "I gotta spin faster...");
			LevelFlowComponent.SpawnerWave.SpawnWave(Enemy2Spawner, 3, 100);
			await Task.Delay(5000, token);
			_ = HUD.PopUpMessage(Char.OIIA, Mood.OIIA.Default, "Faster!!");
			await Task.Delay(7000, token);

			// Recurrent: Aimer and Kamikaze
			_ = HUD.PopUpMessage(Char.COMMANDER, Mood.COMMANDER.Default, "Survive Rookie!");
			LevelFlowComponent.SpawnerRecurrent.StartSpawner1(Enemy1Spawner, 355);
			LevelFlowComponent.SpawnerRecurrent.StartSpawner2(Enemy2Spawner, 555);
			await Task.Delay(8000, token);
			LevelFlowComponent.SpawnerRecurrent.StopSpawner1();
			LevelFlowComponent.SpawnerRecurrent.StopSpawner2();

			// Wave: Aimer
			await LevelFlowComponent.SpawnerWave.SpawnWaveUntilCleared(Enemy2Spawner, 5, 100);
			await Task.Delay(2000, token);
			_ = HUD.PopUpMessage(Char.COMMANDER, Mood.COMMANDER.Default, "Hey Earl, check this out.");
			await Task.Delay(5000, token);
			_ = HUD.PopUpMessage(Char.COMMANDER, Mood.COMMANDER.Default, "It's $20 at Ykea...");
			await Task.Delay(8000, token);
			_ = HUD.PopUpMessage(Char.COMMANDER, Mood.COMMANDER.Default, "Is this thing on?");
			await Task.Delay(5000, token);
			_ = HUD.PopUpMessage(Char.COMMANDER, Mood.COMMANDER.Default, "Go to the side!");
			await Task.Delay(3000, token);

			// Wave: Kamikaze
			await LevelFlowComponent.SpawnerWave.SpawnWaveUntilCleared(Enemy1Spawner, 9, 30);
			await Task.Delay(3000, token);
			await LevelFlowComponent.SpawnerWave.SpawnWaveUntilCleared(Enemy1Spawner, 9, 30);
			await Task.Delay(3000, token);

			// Clear
			_ = HUD.PopUpMessage(Char.COMMANDER, Mood.COMMANDER.Default, "I can see the coast!");
			await Task.Delay(3000, token);
			G.GS.MarkLevelCleared("L4");
			await HandleLevelClear();
		}
		catch (TaskCanceledException)
		{
			Log.Dbg("Script canceled");
		}
	}

	public async Task LevelDialog()
	{
		await HUD.FirstMessage(Char.COMMANDER, Mood.COMMANDER.Default, "We're back on this wasteland humans call home.");
		await HUD.Message(Char.COMMANDER, Mood.COMMANDER.Default, "There is something else we need to do here.");
		await HUD.Message(Char.COMMANDER, Mood.COMMANDER.Default, "We need to get...");
		await HUD.Message(Char.COMMANDER, Mood.COMMANDER.Default, "How do I tell him...");
		await HUD.HandleChoice(
			await HUD.MessageWithPrompt(Char.OIIA, Mood.OIIA.GlassesZoom, "A mirror! Humans make the best ones, I need to perfect my spinning.", "Let's go!", "Really?"),
			Char.OIIA,
			"There he goes saying sure!! Aww man you're great. Miau miau to you.",
			Mood.OIIA.Default,
			"I know. I'm the best spinning cat, a true Spin God.",
			Mood.OIIA.Default,
			"Sorry you feel that way. You just don't get it... don't be selfish.",
			Mood.OIIA.Glasses
		);
		await HUD.Message(Char.ANNOUNCEMENT, Mood.ANNOUNCEMENT.Default, "☆ Incoming Transmission ☆");
		await HUD.Message(Char.ANNOUNCEMENT, Mood.ANNOUNCEMENT.Default, "OIIA. This is a direct violation of the truce.");
		await HUD.Message(Char.ANNOUNCEMENT, Mood.ANNOUNCEMENT.Default, "Just buy the mirror at Ykea like everyone else...");
		await HUD.Message(Char.OIIA, Mood.OIIA.Default, "No!");
		await HUD.Message(Char.OIIA, Mood.OIIA.Default, "I am the absolute Spin God in this world, you should be giving me the mirror for exisiting.");
		await HUD.LastMessage(Char.COMMANDER, Mood.COMMANDER.Annoyed, "I hate this job...");
		G.GS.MarkDialogSeen("L4Intro");
	}
}
