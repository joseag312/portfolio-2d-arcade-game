using Godot;
using System;
using System.Threading;
using System.Threading.Tasks;

public partial class Level002Script : BaseLevelScript
{
    protected override int GetLevelId() => 4;
    protected override String GetLevelMusic() => Music.ANGRY;

    protected override async Task RunLevel(CancellationToken token)
    {
        try
        {
            // Intro
            await Task.Delay(300, token);
            StartDialog();
            await (G.GS.IsDialogSeen("L2Intro") ? DefaultDialog() : LevelDialog());
            StopDialog();

            // Wave: Bulldozer
            await Task.Delay(1000, token);
            _ = HUD.PopUpMessage(Char.COMMANDER, Mood.COMMANDER.Default, "They tracked us!");
            await LevelFlowComponent.SpawnerWave.SpawnWaveUntilCleared(Enemy1Spawner, 1, 100);
            await Task.Delay(1000, token);

            // Wave: Bulldozer
            // Recurrent: Kamikaze 
            LevelFlowComponent.SpawnerRecurrent.StartSpawner1(Enemy2Spawner, 600);
            await Task.Delay(1000, token);
            _ = HUD.PopUpMessage(Char.OIIA, Mood.OIIA.Default, "Spin.. better...");
            await Task.Delay(3000, token);
            await LevelFlowComponent.SpawnerWave.SpawnWaveUntilCleared(Enemy1Spawner, 3, 100);
            await Task.Delay(5000, token);
            LevelFlowComponent.SpawnerRecurrent.StopSpawner1();

            // Wave: Bulldozer
            // Recurrent: Kamikaze
            await Task.Delay(3000, token);
            _ = HUD.PopUpMessage(Char.OIIA, Mood.OIIA.Default, "Why was I spinning anways...");
            LevelFlowComponent.SpawnerRecurrent.StartSpawner1(Enemy2Spawner, 600);
            await LevelFlowComponent.SpawnerWave.SpawnWaveUntilCleared(Enemy1Spawner, 5, 100);
            LevelFlowComponent.SpawnerRecurrent.StopSpawner1();

            // Clear
            await Task.Delay(3000, token);
            _ = HUD.PopUpMessage(Char.OIIA, Mood.OIIA.Default, "They still want a spinner...");
            await Task.Delay(3000, token);

            // Clear Dialog
            StartDialog();
            await (G.GS.IsDialogSeen("L2Clear") ? DefaultClearDialog() : LevelClearDialog());
            StopEndingDialog();

            // Cleanup
            G.GS.MarkLevelCleared("L2");
            await HandleLevelClear();
        }
        catch (TaskCanceledException)
        {
            Log.Dbg("Script canceled");
        }
    }

    public async Task LevelDialog()
    {
        await HUD.Message(Char.OIIA, Mood.OIIA.Default, "Ah commander, welcome back. Glad to have you onboard again.");
        await HUD.Message(Char.OIIA, Mood.OIIA.SpinSlower, "Where did you say you went again?");
        await HUD.Message(Char.COMMANDER, Mood.COMMANDER.Default, "Hi, sir - your spinness. Just gathering some intel...");
        await HUD.Message(Char.OIIA, Mood.OIIA.SpinSlower, "And how did that go?");
        await HUD.Message(Char.COMMANDER, Mood.COMMANDER.Default, "All good sir, we managed to secure some good ship upgrades");
        await HUD.Message(Char.OIIA, Mood.OIIA.SpinSlower, "Anything else?");
        await HUD.Message(Char.COMMANDER, Mood.COMMANDER.Default, "Uhm sir, do you want to stop spinning for a moment?");
        await HUD.Message(Char.OIIA, Mood.OIIA.SpinSlower, "I just had this one question...");
        await HUD.Message(Char.FORTUNE, Mood.FORTUNE.Default, "Miau! How can I help you today Dr. Ethel?");
        await HUD.Message(Char.OIIA, Mood.OIIA.SpinSlower, "What");
        await HUD.Message(Char.OIIA, Mood.OIIA.SpinNormal, "did you");
        await HUD.Message(Char.OIIA, Mood.OIIA.SpinFastest, "just call ME?");
        await HUD.Message(Char.FRIEND, Mood.FRIEND.Default, "Don't mind the A.I., sir. We were just researching..");
        await HUD.Message(Char.OIIA, Mood.OIIA.SpinFastest, "I am GOD. Spin GOD. Please tell that A.I. to get in line...");
        await HUD.Message(Char.OIIA, Mood.OIIA.SpinFastest, "...");
        await HUD.Message(Char.OIIA, Mood.OIIA.SpinNormal, "Now let me just refresh your memory!");
        await HUD.Message(Char.OIIA, Mood.OIIA.Default, "My spinning will bring human tyrants down!");
        await HUD.Message(Char.OIIA, Mood.OIIA.Inverse, "Do I need to remind you why we're doing this?");
        await HUD.Message(Char.OIIA, Mood.OIIA.Default, "It's for cats all over the universe, we're doing a good thing...");
        await HUD.HandleChoice(
            await HUD.MessageWithPrompt(Char.OIIA, Mood.OIIA.GlassesZoom, "Rookie, it makes me happy to know you agree with my message.", "Stop. Please.", "Spin faster"),
            Char.OIIA,
            "I love it - Always the detachment.",
            Mood.OIIA.Default,
            "I... can't",
            Mood.OIIA.SpinNormal,
            "You remind me of those nasty tyrants in management. Do better Rookie.",
            Mood.OIIA.Glasses
        );
        await HUD.Message(Char.OIIA, Mood.OIIA.SpinNormal, "...I don't remember feeling this tired.");
        await HUD.Message(Char.OIIA, Mood.OIIA.SpinSlow, "Why am I tired? I can do anything.");
        await HUD.LastMessage(Char.OIIA, Mood.OIIA.SpinSlowest, "I need a moment.");
        G.GS.MarkDialogSeen("L2Intro");
    }

    public async Task LevelClearDialog()
    {
        await HUD.FirstMessage(Char.OIIA, Mood.OIIA.Default, "I've had it! Contact human HQ please.");
        await HUD.Message(Char.ANNOUNCEMENT, Mood.ANNOUNCEMENT.Default, "☆ Preparing Transmission ☆");
        await HUD.Message(Char.FORTUNE, Mood.FORTUNE.Default, "Miau! Would you like me to build a sample request paragraph?");
        await HUD.Message(Char.OIIA, Mood.OIIA.SpinFastest, "Bob, I swear... make that gippity cat thing stop.");
        await HUD.Message(Char.FRIEND, Mood.FRIEND.Default, "We're losing him...");
        await HUD.Message(Char.OIIA, Mood.OIIA.Default, "Listen up, you human scourge.");
        await HUD.Message(Char.OIIA, Mood.OIIA.Inverse, "You took away EVERYTHING from me.");
        await HUD.Message(Char.OIIA, Mood.OIIA.Default, "Whatever happens next is your responsibility.");
        await HUD.Message(Char.ANNOUNCEMENT, Mood.ANNOUNCEMENT.Default, "☆ Transmission Sent ☆");
        await HUD.Message(Char.OIIA, Mood.OIIA.Inverse, "...");
        await HUD.Message(Char.OIIA, Mood.OIIA.Inverse, "... ...");
        await HUD.Message(Char.OIIA, Mood.OIIA.Default, "I miss them...");
        await HUD.LastMessage(Char.OIIA, Mood.OIIA.Default, "Ah humanity... why...");
        G.GS.MarkDialogSeen("L2Clear");
    }
}
