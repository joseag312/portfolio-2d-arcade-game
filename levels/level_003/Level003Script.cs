using Godot;
using System;
using System.Threading;
using System.Threading.Tasks;

public partial class Level003Script : BaseLevelScript
{
    protected override int GetLevelId() => 3;
    protected override String GetLevelMusic() => Music.ANGRY;

    protected override async Task RunLevel(CancellationToken token)
    {
        try
        {
            // Intro
            await Task.Delay(300, token);
            StartDialog();
            await (G.GS.IsDialogSeen("L3Intro") ? DefaultDialog() : LevelDialog());
            StopDialog();

            // Wave: Basic
            await Task.Delay(1000, token);
            _ = HUD.PopUpMessage(Char.FRIEND, Mood.FRIEND.Default, "$10 and free delivery!?");
            LevelFlowComponent.SpawnerWave.SpawnWave(Enemy1Spawner, 5, 30);
            await Task.Delay(3000, token);

            // Wave: Aimer
            // Recurrent: Kamikaze 
            LevelFlowComponent.SpawnerRecurrent.StartSpawner1(Enemy3Spawner, 600);
            await Task.Delay(7000, token);
            _ = HUD.PopUpMessage(Char.COMMANDER, Mood.COMMANDER.Default, "Bob uhm.. can you buy me one?");
            await LevelFlowComponent.SpawnerWave.SpawnWaveUntilCleared(Enemy2Spawner, 3, 20);
            await Task.Delay(1000, token);
            LevelFlowComponent.SpawnerRecurrent.StopSpawner1();

            // Wave: Aimer
            await Task.Delay(3000, token);
            _ = HUD.PopUpMessage(Char.FRIEND, Mood.FRIEND.Default, "Hey whose doughnut is this?");
            LevelFlowComponent.SpawnerWave.SpawnWave(Enemy2Spawner, 5, 100);
            await Task.Delay(5000, token);
            _ = HUD.PopUpMessage(Char.ROOKIE, Mood.ROOKIE.Default, "You can have it...");
            await Task.Delay(7000, token);

            // Wave: Basic
            // Recurrent: Basic and Aimer
            _ = HUD.PopUpMessage(Char.COMMANDER, Mood.COMMANDER.Default, "They sent backup!!");
            LevelFlowComponent.SpawnerRecurrent.StartSpawner1(Enemy2Spawner, 1555);
            LevelFlowComponent.SpawnerRecurrent.StartSpawner2(Enemy3Spawner, 855);
            await LevelFlowComponent.SpawnerWave.SpawnWaveUntilCleared(Enemy1Spawner, 10, 30);
            LevelFlowComponent.SpawnerRecurrent.StopSpawner1();
            LevelFlowComponent.SpawnerRecurrent.StopSpawner2();

            // Clear
            await Task.Delay(3000, token);
            _ = HUD.PopUpMessage(Char.COMMANDER, Mood.COMMANDER.Default, "We're in! Wait what!?");
            await Task.Delay(3000, token);

            // Clear Dialog
            StartDialog();
            await (G.GS.IsDialogSeen("L3Clear") ? DefaultClearDialog() : LevelClearDialog());
            StopEndingDialog();

            // Cleanup
            G.GS.MarkLevelCleared("L3");
            await HandleLevelClear();
        }
        catch (TaskCanceledException)
        {
            Log.Dbg("Script canceled");
        }
    }

    public async Task LevelDialog()
    {
        await HUD.FirstMessage(Char.COMMANDER, Mood.COMMANDER.Default, "What a mess...");
        await HUD.Message(Char.COMMANDER, Mood.COMMANDER.Default, "Sorry to bring you here on such short notice, Rookie.");
        await HUD.Message(Char.COMMANDER, Mood.COMMANDER.Default, "There's an old human base somewhere on this land...");
        await HUD.Message(Char.COMMANDER, Mood.COMMANDER.Default, "This is where the OIIA organization started.");
        await HUD.Message(Char.COMMANDER, Mood.COMMANDER.Default, "It was originally a project for humans to travel alongside us.");
        await HUD.Message(Char.COMMANDER, Mood.COMMANDER.Annoyed, "So we could miau-miau, and they could pretend their existence had meaning.");
        await HUD.Message(Char.COMMANDER, Mood.COMMANDER.Default, "Spin God doesn’t know we’re here...");
        await HUD.Message(Char.COMMANDER, Mood.COMMANDER.Default, "I’d prefer if you kept this to yourself. We'd get fired on the spot.");
        await HUD.Message(Char.COMMANDER, Mood.COMMANDER.Default, "Spin God wasn’t always like this, you know?");
        await HUD.Message(Char.COMMANDER, Mood.COMMANDER.Default, "Humans... not all of them were kind...");
        await HUD.HandleChoice(
            await HUD.MessageWithPrompt(Char.COMMANDER, Mood.COMMANDER.Annoyed, "Some humans can be downright cruel.", "I'm sorry", "So?"),
            Char.COMMANDER,
            "You really are an apathetic nine-lives being. Respect.",
            Mood.COMMANDER.Annoyed,
            "Yeah... Me too...",
            Mood.COMMANDER.Default,
            "Maybe you have more in common than you think.",
            Mood.COMMANDER.Annoyed
        );
        await HUD.Message(Char.COMMANDER, Mood.COMMANDER.Default, "Oh...");
        await HUD.Message(Char.COMMANDER, Mood.COMMANDER.Default, "The myth, the cat, the legend! Rookie, this is Bob from IT.");
        await HUD.Message(Char.FRIEND, Mood.FRIEND.Default, "Howdy, sir! Good to meet you, can I get you some coffee?");
        await HUD.Message(Char.COMMANDER, Mood.COMMANDER.Default, "Bob, please pretend to work at least...");
        await HUD.Message(Char.FRIEND, Mood.FRIEND.Default, "C'mon, Boss Cat, did you do catnip and stay up watching shows again?");
        await HUD.Message(Char.COMMANDER, Mood.COMMANDER.Annoyed, "That’s personal, Bob...");
        await HUD.Message(Char.COMMANDER, Mood.COMMANDER.Annoyed, "Anyways...");
        await HUD.Message(Char.COMMANDER, Mood.COMMANDER.Default, "Humans haven’t stopped their pet project...");
        await HUD.Message(Char.COMMANDER, Mood.COMMANDER.Annoyed, "Chances are this spot is heavily guarded.");
        await HUD.Message(Char.COMMANDER, Mood.COMMANDER.Default, "I want you to see something... get close enough and Bob here should be able to remote into their file system.");
        await HUD.Message(Char.FRIEND, Mood.FRIEND.Default, "We got you, Rookie. That’s why they pay us miserably.");
        await HUD.Message(Char.FRIEND, Mood.FRIEND.Happy, "I just spent my last paycheck. I gotta show you!");
        await HUD.LastMessage(Char.COMMANDER, Mood.COMMANDER.Annoyed, "Bob... do I need to say it again?");
        G.GS.MarkDialogSeen("L3Intro");
    }

    public async Task LevelClearDialog()
    {
        await HUD.FirstMessage(Char.COMMANDER, Mood.COMMANDER.Default, "Oh, this is disturbing. Humans did what!?");
        await HUD.Message(Char.FRIEND, Mood.FRIEND.Default, "They, uhm, made a horoscope A.I., sir... with one of us.");
        await HUD.Message(Char.COMMANDER, Mood.COMMANDER.Annoyed, "Please don’t tell me...");
        await HUD.Message(Char.FRIEND, Mood.FRIEND.Default, "It’s called Cat GPT.");
        await HUD.Message(Char.FRIEND, Mood.FRIEND.Default, "We’ve brought it down, sir, and incorporated it into our system.");
        await HUD.Message(Char.FORTUNE, Mood.FORTUNE.Default, "Miau! How can I help you today?");
        await HUD.Message(Char.COMMANDER, Mood.COMMANDER.Annoyed, "Unbelievable...");
        await HUD.Message(Char.COMMANDER, Mood.COMMANDER.Annoyed, "Alright, I’ll bite... Cat GPT, what happened to Spin God?");
        await HUD.Message(Char.FORTUNE, Mood.FORTUNE.Default, "Miau! Accessing archival memory logs...");
        await HUD.Message(Char.FORTUNE, Mood.FORTUNE.Default, "Once known as Dr. Ethel, Spin God was an idealist, dreaming of cats and humans on equal ground.");
        await HUD.Message(Char.FORTUNE, Mood.FORTUNE.Default, "Upper management didn’t want a partner. They wanted a pet. A mascot.");
        await HUD.Message(Char.FORTUNE, Mood.FORTUNE.Default, "They laughed when the words came out. Clapped when the spinning began.");
        await HUD.Message(Char.FORTUNE, Mood.FORTUNE.Default, "So Dr. Ethel spun... and never remembered how to stop.");
        await HUD.HandleChoice(
            await HUD.MessageWithPrompt(Char.FORTUNE, Mood.FORTUNE.Default, "Miau. Archival read complete.", "A performance...", "Don’t care"),
            Char.COMMANDER,
            "‘Sure’ is the right word, cat.",
            Mood.COMMANDER.Default,
            "Hasn’t stopped spinning since...",
            Mood.COMMANDER.Default,
            "You didn’t know Dr. Ethel before...",
            Mood.COMMANDER.Annoyed
        );
        await HUD.Message(Char.FRIEND, Mood.FRIEND.Happy, "I’m a Taurus, Cat GPT! What’s my daily horoscope?");
        await HUD.Message(Char.FORTUNE, Mood.FORTUNE.Default, "Miau! Today you’ll take a sweet treat from an unexpected source!");
        await HUD.Message(Char.FRIEND, Mood.FRIEND.Happy, "Oh, spot on!! That was so good...");
        await HUD.LastMessage(Char.COMMANDER, Mood.COMMANDER.Annoyed, "Bob...");
        G.GS.MarkDialogSeen("L3Clear");
    }

}
