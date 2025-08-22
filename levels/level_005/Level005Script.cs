using Godot;
using System;
using System.Threading;
using System.Threading.Tasks;

public partial class Level005Script : BaseLevelScript
{
    protected override int GetLevelId() => 4;
    protected override String GetLevelMusic() => Music.FINAL;

    protected override async Task RunLevel(CancellationToken token)
    {
        try
        {
            // Intro
            await Task.Delay(300, token);
            StartDialog();
            await (G.GS.IsDialogSeen("L5Intro") ? DefaultDialog() : LevelDialog());
            StopDialog();

            // Wave: Bulldozer
            // Recurrent: Aimer
            await Task.Delay(1000, token);
            _ = HUD.PopUpMessage(Char.COMMANDER, Mood.COMMANDER.Default, "Survive Rookie!!!");
            LevelFlowComponent.SpawnerRecurrent.StartSpawner1(Enemy3Spawner, 600);
            await LevelFlowComponent.SpawnerWave.SpawnWaveUntilCleared(Enemy2Spawner, 8, 30);
            LevelFlowComponent.SpawnerRecurrent.StopSpawner1();

            // Recurrent: Aimer
            // Recurrent: Kamikaze
            await Task.Delay(2000, token);
            _ = HUD.PopUpMessage(Char.ROOKIE, Mood.ROOKIE.Default, "Too many!!!");
            LevelFlowComponent.SpawnerRecurrent.StartSpawner1(Enemy2Spawner, 600);
            LevelFlowComponent.SpawnerRecurrent.StartSpawner2(Enemy3Spawner, 300);
            await Task.Delay(15000, token);
            LevelFlowComponent.SpawnerRecurrent.StopSpawner1();
            LevelFlowComponent.SpawnerRecurrent.StopSpawner2();

            // Wave: Bulldozer
            await Task.Delay(2000, token);
            _ = HUD.PopUpMessage(Char.OIIA, Mood.OIIA.Default, "O ...I ...I");
            LevelFlowComponent.SpawnerRecurrent.StartSpawner2(Enemy3Spawner, 500);
            await LevelFlowComponent.SpawnerWave.SpawnWaveUntilCleared(Enemy1Spawner, 5, 100);
            LevelFlowComponent.SpawnerRecurrent.StopSpawner2();

            // Finale 
            await Task.Delay(2000, token);
            _ = HUD.PopUpMessage(Char.OIIA, Mood.OIIA.Default, "That's the command fleet!");
            LevelFlowComponent.SpawnerRecurrent.StartSpawner1(Enemy2Spawner, 1500);
            LevelFlowComponent.SpawnerRecurrent.StartSpawner2(Enemy3Spawner, 600);
            await LevelFlowComponent.SpawnerWave.SpawnWaveUntilCleared(Enemy1Spawner, 5, 100);
            LevelFlowComponent.SpawnerRecurrent.StopSpawner1();
            LevelFlowComponent.SpawnerRecurrent.StopSpawner2();

            // Clear
            await Task.Delay(3000, token);
            _ = HUD.PopUpMessage(Char.FRIEND, Mood.FRIEND.Default, "That's right pew pew");
            await Task.Delay(3000, token);

            // Clear Dialog
            StartDialog();
            await (G.GS.IsDialogSeen("L5Clear") ? DefaultClearDialog() : LevelClearDialog());
            StopEndingDialog();

            // Cleanup
            G.GS.MarkLevelCleared("L5");
            await HandleLevelClear();
        }
        catch (TaskCanceledException)
        {
            Log.Dbg("Script canceled");
        }
    }

    public async Task LevelDialog()
    {
        await HUD.Message(Char.OIIA, Mood.OIIA.Default, "This is it... we're meeting the creators of my previous dream here.");
        await HUD.Message(Char.COMMANDER, Mood.COMMANDER.Default, "Looks like they've sent a full squad.");
        await HUD.HandleChoice(
            await HUD.MessageWithPrompt(Char.OIIA, Mood.OIIA.SpinSlower, "Rookie, any advice?", "Stop", "Nope"),
            Char.OIIA,
            "You probably see right through me, huh?",
            Mood.OIIA.Default,
            "But... how...? I've been spinning forever...",
            Mood.OIIA.SpinNormal,
            "You're right. I am God. I don't need advice.",
            Mood.OIIA.Glasses
        );
        await HUD.Message(Char.FRIEND, Mood.FRIEND.Default, "Hello everyone! Brought doughnuts.");
        await HUD.Message(Char.COMMANDER, Mood.COMMANDER.Default, "... Thank you, Bob.");
        await HUD.Message(Char.FRIEND, Mood.FRIEND.Default, "I'm just an IT guy... But, who knows if we'll make it past today?");
        await HUD.Message(Char.FRIEND, Mood.FRIEND.Default, "It's a whole fleet out there.");
        await HUD.Message(Char.FRIEND, Mood.FRIEND.Default, "Have a treat and enjoy, Boss Cat.");
        if (G.GS.Karma > 0)
        {
            await HUD.Message(Char.FRIEND, Mood.FRIEND.Default, "Spin God, you too. Come on. You've done enough.");
        }
        await HUD.Message(Char.COMMANDER, Mood.COMMANDER.Annoyed, "Hate to say it... But the IT guy is right.");
        await HUD.HandleChoice(
            await HUD.MessageWithPrompt(Char.COMMANDER, Mood.COMMANDER.Default, "Should we help Spin God?", "We got this", "Why?"),
            Char.COMMANDER,
            "Are you about to quit?",
            Mood.COMMANDER.Default,
            "Remind me to promote you. Same nickname though. Rookie.",
            Mood.COMMANDER.Default,
            "You naughty cat.",
            Mood.COMMANDER.Annoyed
        );
        await HUD.HandleChoice(
            await HUD.MessageWithPrompt(Char.OIIA, Mood.OIIA.SpinSlowest, "How did it come to this...", "Ethel...", "Wonder why"),
            Char.COMMANDER,
            "I guess it is what it is.",
            Mood.COMMANDER.Default,
            "We'll take it from here - your spinness.",
            Mood.COMMANDER.Default,
            "...",
            Mood.COMMANDER.Annoyed
        );
        await HUD.LastMessage(Char.FORTUNE, Mood.FORTUNE.Default, "Miau! Analyzing mission briefing decision archives...");
        G.GS.MarkDialogSeen("L5Intro");
    }

    public async Task LevelClearDialog()
    {
        await HUD.FirstMessage(Char.FORTUNE, Mood.FORTUNE.Default, "Miau! Mission briefing analysis complete!");
        await HUD.Message(Char.FORTUNE, Mood.FORTUNE.Default, "Based on your choices I can tell...");
        if (G.GS.Karma == 0)
        {
            await HUD.Message(Char.FORTUNE, Mood.FORTUNE.Default, "... you are detached from life's outcomes.");
            await HUD.Message(Char.OIIA, Mood.OIIA.SpinSlowest, "Everyone I...");
            await HUD.Message(Char.OIIA, Mood.OIIA.SpinSlowest, "...I don't think I'm god...");
            await HUD.Message(Char.OIIA, Mood.OIIA.SpinSlowest, "I gotta find a way to stop spinning.");
            await HUD.MessageWithPrompt(Char.OIIA, Mood.OIIA.SpinSlowest, "What do you think rookie?");
        }
        else if (G.GS.Karma > 0)
        {
            await HUD.Message(Char.FORTUNE, Mood.FORTUNE.Default, "... you are a kind nine-liver.");
            await HUD.Message(Char.OIIA, Mood.OIIA.SpinSlowest, "Everyone I...");
            await HUD.Message(Char.OIIA, Mood.OIIA.SpinSlowest, "...need your help...");
            await HUD.Message(Char.ANNOUNCEMENT, Mood.ANNOUNCEMENT.Default, "☆ Incoming Transmission ☆");
            await HUD.Message(Char.ANNOUNCEMENT, Mood.ANNOUNCEMENT.Default, "57 65 20 67 6f 74 20 79 6f 75 20 45 74 68 65 6c 21");
            await HUD.Message(Char.COMMANDER, Mood.COMMANDER.Default, "Something jammed their transmission!");
            await HUD.Message(Char.OIIA, Mood.OIIA.SpinSlowest, "Wait! I know this way of doing things.");
            await HUD.Message(Char.OIIA, Mood.OIIA.SpinSlowest, "It's my old team!! Humans are sending us a message. The good kind of humans.");
            await HUD.Message(Char.OIIA, Mood.OIIA.SpinSlowest, "Cat GPT, decode this!");
            await HUD.Message(Char.FORTUNE, Mood.FORTUNE.Default, "Sure thing, Ethel!");
            await HUD.Message(Char.FORTUNE, Mood.FORTUNE.Default, "Decoding...");
            await HUD.Message(Char.FORTUNE, Mood.FORTUNE.Default, "...");
            await HUD.Message(Char.FORTUNE, Mood.FORTUNE.Default, "...");
            await HUD.Message(Char.FORTUNE, Mood.FORTUNE.Default, "...");
            await HUD.Message(Char.FRIEND, Mood.FRIEND.Default, "We need to tweak this human A.I. project.");
            await HUD.Message(Char.FORTUNE, Mood.FORTUNE.Default, "...");
            await HUD.Message(Char.FORTUNE, Mood.FORTUNE.Default, "Decoded Message: 'We got you Ethel!'");
            await HUD.Message(Char.OIIA, Mood.OIIA.SpinSlowest, "It's them!!! My human family is here!!!");
            await HUD.Message(Char.OIIA, Mood.OIIA.SpinSlowest, "...");
            await HUD.Message(Char.OIIA, Mood.OIIA.SpinSlowest, "I guess I don't need to spin anymore.");
            await HUD.Message(Char.OIIA, Mood.OIIA.Default, "...");
            await HUD.Message(Char.OIIA, Mood.OIIA.Inverse, "...");
            await HUD.Message(Char.OIIA, Mood.OIIA.Default, "Huh...");
            await HUD.MessageWithPrompt(Char.OIIA, Mood.OIIA.Default, "Feels nice...");
        }
        else if (G.GS.Karma < 0)
        {
            await HUD.Message(Char.FORTUNE, Mood.FORTUNE.Default, "... you will help the universe burn.");
            await HUD.Message(Char.OIIA, Mood.OIIA.SpinSlowest, "You don't care...");
            await HUD.Message(Char.OIIA, Mood.OIIA.SpinSlowest, "I guess cats and humans are kind of similar in a way...");
            await HUD.Message(Char.OIIA, Mood.OIIA.SpinSlowest, "Truth is I'm alone in this world.");
            await HUD.Message(Char.OIIA, Mood.OIIA.SpinSlowest, "And that's only because...");
            await HUD.Message(Char.OIIA, Mood.OIIA.SpinFastest, "I am GOD!");
            await HUD.MessageWithPrompt(Char.OIIA, Mood.OIIA.SpinFastest, "The age of spinning gods begins! Spin with me to hell, felines!");
        }
        await HUD.LastMessage(Char.COMMANDER, Mood.COMMANDER.Annoyed, "I'm calling in sick tomorrow...");
        G.GS.MarkDialogSeen("L5Clear");
    }
}
