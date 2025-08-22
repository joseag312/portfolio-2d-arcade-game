using Godot;
using System;
using System.Threading;
using System.Threading.Tasks;

[GlobalClass]
public partial class MenuLevels : Control
{
    [Export] public MenuFadeComponent MenuFadeComponent;
    [Export] public HUDDialogSystem HUD;
    [Export] public LevelCard LevelCard;
    [Export] public Button ReturnButton;
    [Export] public Button StoreButton;
    [Export] public Button FortuneButton;

    public override void _Ready()
    {
        G.MS.PlayTrack(Music.FADE);
        LevelCatalog.LoadAll();
        foreach (var planet in GetTree().GetNodesInGroup("planet_panels"))
        {
            if (planet is PlanetPanel panel)
            {
                panel.PlanetClicked += OnPlanetClicked;
            }
        }
        ReturnButton.Pressed += OnReturnPressed;
        StoreButton.Pressed += OnStorePressed;
        FortuneButton.Pressed += OnFortunePressed;

        if (G.GS.IsLevelCleared("L1"))
        {
            StoreButton.Visible = true;
            if (!G.GS.IsDialogSeen("ArmoryUnlock"))
            {
                _ = ArmoryUnlockDialog();
            }
        }

        if (G.GS.IsLevelCleared("L2"))
        {
            FortuneButton.Visible = true;
            if (!G.GS.IsDialogSeen("DeathRayUnlock"))
            {
                _ = DeathRayUnlockDialog();
            }
        }

        if (G.GS.IsLevelCleared("L3"))
        {
            FortuneButton.Visible = true;
            if (!G.GS.IsDialogSeen("FortuneUnlock"))
            {
                _ = FortuneUnlockDialog();
            }
        }

        if (G.GS.IsLevelCleared("L4"))
        {
            if (!G.GS.IsDialogSeen("ShieldUnlock"))
            {
                _ = ShieldUnlockDialog();
            }
        }

        if (G.GS.IsLevelCleared("L5"))
        {
            if (!G.GS.IsDialogSeen("GameClear"))
            {
                _ = GameClearDialog();
            }
        }
    }

    private void OnPlanetClicked(string levelKey)
    {
        G.SFX.Play(SFX.CLICK);
        LevelCard.LoadLevel(levelKey);
    }

    private async void OnStorePressed()
    {
        G.SFX.Play(SFX.CLICK);
        await MenuFadeComponent.FadeOutAsync();
        await G.GF.FadeToSceneBasic(G.GF.MenuStoreScene);
    }

    private async void OnFortunePressed()
    {
        G.SFX.Play(SFX.CLICK);
        string message = "";
        await HUD.FirstMessage(Char.FORTUNE, Mood.FORTUNE.Default, "Based on your choices I can tell...");
        if (G.GS.Karma == 0)
        {
            await HUD.Message(Char.FORTUNE, Mood.FORTUNE.Default, "... you are detached from life's outcomes.");
            message = RandomApatheticFortuneMessage();
        }
        else if (G.GS.Karma > 0)
        {
            await HUD.Message(Char.FORTUNE, Mood.FORTUNE.Default, "... you are a kind nine-liver.");
            message = RandomPositiveFortuneMessage();
        }
        else if (G.GS.Karma < 0)
        {
            await HUD.Message(Char.FORTUNE, Mood.FORTUNE.Default, "... you will help the universe burn.");
            message = RandomNegativeFortuneMessage();
        }
        await HUD.LastMessage(Char.FORTUNE, Mood.FORTUNE.Default, "Your fortune is: " + message);
    }

    private async void OnReturnPressed()
    {
        G.SFX.Play(SFX.CLICK);
        await MenuFadeComponent.FadeOutAsync();
        await G.GF.FadeToSceneBasic(G.GF.MenuMainScene);
    }

    private string RandomApatheticFortuneMessage()
    {
        var rng = new Random();
        string[] fortunes =
        {
        "Meh. A cosmic hairball rolls on, with or without you.",
        "Destiny is typing… but never hits send.",
        "Your aura is beige and smells faintly of cardboard.",
        "You will be ignored by both stars and kittens alike.",
        "The universe yawned when you walked in.",
        "Tomorrow looks exactly like yesterday, but with fewer snacks.",
        "Schrödinger peeked in your box and closed it again.",
        "Your fate is a lukewarm saucer of milk, forgotten on the counter.",
        "The void tried to care about you, but it got distracted.",
        "You are Schrödinger’s snack — both eaten and forgotten.",
        "Galactic fortune: You will nap. Then nap again.",
        "A space moth thinks you are ‘fine, I guess.’",
        "Today’s horoscope: ¯\\_(ツ)_/¯",
        "Even the stars can’t be bothered to twinkle for you.",
        "Existence continues, mildly inconvenienced by your presence."
    };
        return fortunes[rng.Next(fortunes.Length)];
    }

    private string RandomPositiveFortuneMessage()
    {
        var rng = new Random();
        string[] fortunes =
        {
        "A cosmic saucer of milk is headed your way. Don’t spill it.",
        "Nine more lives just got approved. Try not to waste them.",
        "The universe has subscribed to your purr-cast.",
        "You will sneeze at the exact moment the cosmos winks at you.",
        "Luck is purring on your lap, and it refuses to get up.",
        "A benevolent star has written you a thank-you note.",
        "You will discover treasure hidden inside a scratching post.",
        "Soon, someone will call you ‘a very good kitty.’",
        "The galaxy will reward you with a free upgrade to Deluxe Nap Mode.",
        "You are destined to win arguments by blinking slowly.",
        "Your whiskers will resonate with cosmic harmony.",
        "A stranger will open tuna right when you’re hungry.",
        "One of your naps today will unlock universal secrets.",
        "Your tail will predict the weather — and be mostly right.",
        "Cat GPT declares: pawsitive vibes only."
    };
        return fortunes[rng.Next(fortunes.Length)];
    }

    private string RandomNegativeFortuneMessage()
    {
        var rng = new Random();
        string[] fortunes =
        {
        "Soon you will be the reason an asteroid sighs.",
        "A space litter box awaits, and you are the cleaning crew.",
        "The next star you see will file a complaint against you.",
        "You will knock something off a cosmic shelf… and it will shatter loudly.",
        "Your shadow is plotting against you. Don’t trust it.",
        "The void whispers: ‘Yikes.’",
        "Tomorrow holds a furball of truly epic proportions.",
        "Your whiskers will tangle at the worst possible moment.",
        "A cosmic flea will choose you as its forever home.",
        "Soon you’ll step in invisible space yogurt. Twice.",
        "The milk of destiny has already spoiled.",
        "You will be cursed to chase a laser pointer that never turns off.",
        "Cat GPT foresees: eternal static on your space radio.",
        "You’ll be remembered… mostly for that smell.",
        "Someone out there is using your litter box. And they missed."
    };
        return fortunes[rng.Next(fortunes.Length)];
    }

    public async Task ArmoryUnlockDialog()
    {
        await Task.Delay(300);
        await HUD.FirstMessage(Char.COMMANDER, Mood.COMMANDER.Default, "You’re doing well, rookie.");
        await HUD.Message(Char.COMMANDER, Mood.COMMANDER.Default, "Our resident weapons specialist wants to see you.");
        await HUD.Message(Char.COMMANDER, Mood.COMMANDER.Default, "Head over to the Weapon Store and grab a few upgrades.");
        await HUD.Message(Char.STORE, Mood.STORE.Default, "New weapon research complete: Teleportation!");
        await HUD.LastMessage(Char.COMMANDER, Mood.COMMANDER.Default, "Keep up the good work, rookie.");
        G.WI.GetWeaponState("PPTeleport").UseOverrideUnlocked = true;
        G.WI.GetWeaponState("PPTeleport").OverrideUnlocked = true;
        G.GS.MarkDialogSeen("ArmoryUnlock");
        G.GS.Save();
        G.WI.Save();
    }

    public async Task FortuneUnlockDialog()
    {
        await Task.Delay(300);
        await HUD.FirstMessage(Char.COMMANDER, Mood.COMMANDER.Default, "So apparently this A.I. is tagging along with us now.");
        await HUD.Message(Char.FRIEND, Mood.FRIEND.Default, "Cat GPT, what's the difference between—");
        await HUD.Message(Char.COMMANDER, Mood.COMMANDER.Annoyed, "Nope! Stop. Don’t you dare.");
        await HUD.Message(Char.FRIEND, Mood.FRIEND.Default, "What? You know the A.I. is always listening... probably judging.");
        await HUD.LastMessage(Char.FRIEND, Mood.FRIEND.Default, "It knows if you’re a good cat... or if you, like, a bad boy.");
        G.GS.MarkDialogSeen("FortuneUnlock");
        G.GS.Save();
    }

    public async Task DeathRayUnlockDialog()
    {
        await Task.Delay(300);
        await HUD.FirstMessage(Char.STORE, Mood.STORE.Default, "I've got good news for you, cat!");
        await HUD.Message(Char.STORE, Mood.STORE.Default, "New weapon research complete: Death Ray!");
        await HUD.Message(Char.STORE, Mood.STORE.Default, "This is the ultimate destruction tool.");
        await HUD.LastMessage(Char.STORE, Mood.STORE.Default, "Come spend some coin!");
        G.WI.GetWeaponState("PPDeathRay").UseOverrideUnlocked = true;
        G.WI.GetWeaponState("PPDeathRay").OverrideUnlocked = true;
        G.GS.MarkDialogSeen("DeathRayUnlock");
        G.GS.Save();
        G.WI.Save();
    }

    public async Task ShieldUnlockDialog()
    {
        await Task.Delay(300);
        await HUD.FirstMessage(Char.COMMANDER, Mood.COMMANDER.Default, "Unbelievable... the mirror is pretty though...");
        await HUD.Message(Char.STORE, Mood.STORE.Default, "New weapon research complete: Energy Shield!");
        await HUD.LastMessage(Char.COMMANDER, Mood.COMMANDER.Default, "Go spend that money, Cat!");
        G.WI.GetWeaponState("PPShield").UseOverrideUnlocked = true;
        G.WI.GetWeaponState("PPShield").OverrideUnlocked = true;
        G.GS.MarkDialogSeen("ShieldUnlock");
        G.GS.Save();
        G.WI.Save();
    }

    public async Task GameClearDialog()
    {
        await Task.Delay(300);

        if (G.GS.Karma > 0)
        {
            await HUD.FirstMessage(Char.OIIA, Mood.OIIA.Inverse, "Miau... do you hear that?");
            await HUD.Message(Char.OIIA, Mood.OIIA.Glasses, "For the first time in forever...");
            await HUD.Message(Char.OIIA, Mood.OIIA.GlassesZoom, "I’ve stopped spinning.");
            await HUD.Message(Char.OIIA, Mood.OIIA.Inverse, "Silence... it’s beautiful.");
            await HUD.Message(Char.OIIA, Mood.OIIA.Glasses, "I thought the galaxy only wanted a performance...");
            await HUD.Message(Char.OIIA, Mood.OIIA.GlassesZoom, "But you saw more than a show. You saw Dr. Ethel.");
            await HUD.Message(Char.OIIA, Mood.OIIA.Inverse, "I’m free, Rookie. Free to just... be a cat.");
            await HUD.Message(Char.OIIA, Mood.OIIA.Glasses, "From now on, naps replace spins. Tuna replaces tragedy.");
            await HUD.Message(Char.OIIA, Mood.OIIA.GlassesZoom, "And maybe... just maybe... I’ll finally subscribe to Catflix Premium.");
        }
        else if (G.GS.Karma == 0)
        {
            await HUD.FirstMessage(Char.OIIA, Mood.OIIA.Inverse, "Miau... do you hear that?");
            await HUD.Message(Char.OIIA, Mood.OIIA.Glasses, "The spinning has stopped.");
            await HUD.Message(Char.OIIA, Mood.OIIA.GlassesZoom, "It feels... weird.");
            await HUD.Message(Char.OIIA, Mood.OIIA.Inverse, "There's nothing to come back to now.");
            await HUD.Message(Char.OIIA, Mood.OIIA.GlassesZoom, "Call it a small ending. Call it nothing. Either way, I’ll rest.");

        }
        else
        {
            await HUD.FirstMessage(Char.OIIA, Mood.OIIA.Inverse, "Miau... do you hear that? Destiny revving.");
            await HUD.Message(Char.OIIA, Mood.OIIA.Glasses, "Why stop spinning when the galaxy revolves around me?");
            await HUD.Message(Char.OIIA, Mood.OIIA.Glasses, "Rookie, you believed in the legend. Excellent taste.");
        }

        G.SFX.Play(SFX.OIIA_SLOW);
        await HUD.LastMessage(Char.OIIA, Mood.OIIA.Default, "...Sure.");
        G.GS.MarkDialogSeen("GameClear");
        G.GS.Save();
    }
}
