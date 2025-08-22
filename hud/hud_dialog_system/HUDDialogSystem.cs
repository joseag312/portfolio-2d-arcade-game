using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

[GlobalClass]
public partial class HUDDialogSystem : CanvasLayer
{
    [Export] public PortraitLibrary PortraitLibrary { get; set; }
    [Export] public Control InputBlocker { get; set; }

    [ExportGroup("Dialog")]
    [Export] public Control DialogBox { get; set; }
    [Export] public AnimatedSprite2D DialogPortrait { get; set; }
    [Export] public Label DialogName { get; set; }
    [Export] public Label DialogText { get; set; }
    [Export] public Button NextButton { get; set; }
    [Export] public CenterContainer NextButtonContainer { get; set; }

    [ExportGroup("PopUp Dialog")]
    [Export] public Control PopUpBox { get; set; }
    [Export] public AnimatedSprite2D PopUpPortrait { get; set; }
    [Export] public Label PopUpText { get; set; }

    [ExportGroup("Button Choices")]
    [Export] public Control PromptButtonBox { get; set; }
    [Export] public Button SureButton { get; set; }
    [Export] public Button Option1Button { get; set; }
    [Export] public Button Option2Button { get; set; }

    private bool _skipTyping = false;
    private TaskCompletionSource _nextPressedTcs;
    private TaskCompletionSource<string> _promptPressedTcs = new TaskCompletionSource<string>();

    public override void _Ready()
    {
        NextButton.Pressed += OnNextPressed;
    }

    public async Task PopUpMessage(string speakerKey, string mood = Mood.DEFAULT, string message = "", float duration = 1.5f)
    {
        PopUpText.Text = message;

        if (PortraitLibrary == null)
        {
            Log.Err("PortraitLibrary is null");
            PopUpPortrait.Visible = false;
            return;
        }

        var portrait = PortraitLibrary.GetPortrait(speakerKey);

        if (portrait == null)
        {
            Log.Err($"No Portrait found for '{speakerKey}'");
            PopUpPortrait.Visible = false;
            return;
        }

        PopUpPortrait.SpriteFrames = portrait.Frames;

        var resolvedMood = string.IsNullOrWhiteSpace(mood) ? portrait.AnimationKey : mood;

        if (!portrait.Frames.HasAnimation(resolvedMood))
        {
            Log.Err($"Portrait '{speakerKey}' has no Animation named '{resolvedMood}'");
            PopUpPortrait.Visible = false;
            return;
        }

        try
        {
            PopUpPortrait.Play(resolvedMood);
            var frameTexture = portrait.Frames.GetFrameTexture(resolvedMood, 0);
            var frameSize = frameTexture.GetSize();
            PopUpPortrait.Scale = new Vector2(48f, 48f) / frameSize;

            PopUpPortrait.Visible = true;
        }
        catch (System.Exception ex)
        {
            Log.Err($"Exception during PopUpPortrait setup: {ex.Message}");
            PopUpPortrait.Visible = false;
            return;
        }

        PopUpBox.Modulate = new Color(1, 1, 1, 0);
        PopUpBox.Visible = true;

        var tween = GetTree().CreateTween();
        tween.TweenProperty(PopUpBox, "modulate:a", 1.0f, 0.5f)
             .SetTrans(Tween.TransitionType.Sine);
        await ToSignal(tween, "finished");

        await ToSignal(GetTree().CreateTimer(duration), "timeout");

        var fadeOutTween = GetTree().CreateTween();
        fadeOutTween.TweenProperty(PopUpBox, "modulate:a", 0.0f, 0.5f)
                    .SetTrans(Tween.TransitionType.Sine);
        await ToSignal(fadeOutTween, "finished");

        PopUpBox.Visible = false;
    }

    public async Task FirstMessage(string speakerKey, string mood = Mood.DEFAULT, string message = "")
    {
        InputBlocker.Visible = true;
        await ShowMessage(speakerKey, message, fadeIn: true, fadeOut: false, mood);
    }

    public async Task Message(string speakerKey, string mood = Mood.DEFAULT, string message = "")
    {
        await ShowMessage(speakerKey, message, fadeIn: false, fadeOut: false, mood);
    }

    public async Task LastMessage(string speakerKey, string mood = Mood.DEFAULT, string message = "")
    {
        InputBlocker.Visible = false;
        await ShowMessage(speakerKey, message, fadeIn: false, fadeOut: true, mood);
    }

    private async Task ShowMessage(string speakerKey, string message, bool fadeIn, bool fadeOut, string mood = Mood.DEFAULT)
    {
        DialogText.Text = "";

        var portrait = PortraitLibrary?.GetPortrait(speakerKey);

        if (portrait == null)
        {
            Log.Err($"No Portrait found for '{speakerKey}'");
            DialogPortrait.Visible = false;
            DialogName.Text = speakerKey;
        }
        else
        {
            DialogName.Text = portrait.DisplayName;
            DialogPortrait.SpriteFrames = portrait.Frames;

            var resolvedMood = string.IsNullOrWhiteSpace(mood) ? portrait.AnimationKey : mood;

            if (!portrait.Frames.HasAnimation(resolvedMood))
            {
                Log.Err($"Portrait '{speakerKey}' has no Animation named '{resolvedMood}'");
                DialogPortrait.Visible = false;
            }
            else
            {
                try
                {
                    DialogPortrait.Play(resolvedMood);
                    var frameTexture = portrait.Frames.GetFrameTexture(resolvedMood, 0);
                    var frameSize = frameTexture.GetSize();
                    DialogPortrait.Scale = new Vector2(96f, 96f) / frameSize;
                    DialogPortrait.Visible = true;
                }
                catch (System.Exception ex)
                {
                    Log.Err($"Exception during DialogPortrait setup: {ex.Message}");
                    DialogPortrait.Visible = false;
                }
            }
        }

        NextButtonContainer.Visible = true;

        if (fadeIn)
        {
            DialogBox.Modulate = new Color(1, 1, 1, 0);
            DialogBox.Visible = true;

            var tween = GetTree().CreateTween();
            tween.TweenProperty(DialogBox, "modulate:a", 1.0f, 0.5f)
                 .SetTrans(Tween.TransitionType.Sine);
            await ToSignal(tween, "finished");
        }
        else
        {
            DialogBox.Visible = true;
            DialogBox.Modulate = new Color(1, 1, 1, 1);
        }

        await TypeText(message);

        _nextPressedTcs = new TaskCompletionSource();
        await _nextPressedTcs.Task;

        if (fadeOut)
        {
            var fadeOutTween = GetTree().CreateTween();
            fadeOutTween.TweenProperty(DialogBox, "modulate:a", 0.0f, 0.5f)
                        .SetTrans(Tween.TransitionType.Sine);
            await ToSignal(fadeOutTween, "finished");

            DialogBox.Visible = false;
        }
    }

    private void OnNextPressed()
    {
        _nextPressedTcs?.TrySetResult();
    }

    public async Task<string> MessageWithPrompt(string speakerKey, string mood = Mood.DEFAULT, string message = "", string optimistic = "", string pessimistic = "")
    {
        var portrait = PortraitLibrary?.GetPortrait(speakerKey);

        if (portrait == null)
        {
            Log.Err($"No Portrait found for '{speakerKey}'");
            DialogPortrait.Visible = false;
            DialogName.Text = speakerKey;
        }
        else
        {
            DialogName.Text = portrait.DisplayName;
            DialogPortrait.SpriteFrames = portrait.Frames;

            var resolvedMood = string.IsNullOrWhiteSpace(mood) ? portrait.AnimationKey : mood;

            if (!portrait.Frames.HasAnimation(resolvedMood))
            {
                Log.Err($"Portrait '{speakerKey}' has no Animation named '{resolvedMood}'");
                DialogPortrait.Visible = false;
            }
            else
            {
                try
                {
                    DialogPortrait.Play(resolvedMood);
                    var frameTexture = portrait.Frames.GetFrameTexture(resolvedMood, 0);
                    var frameSize = frameTexture.GetSize();
                    DialogPortrait.Scale = new Vector2(96f, 96f) / frameSize;

                    DialogPortrait.Visible = true;
                }
                catch (System.Exception ex)
                {
                    Log.Err($"Exception during DialogPortrait setup: {ex.Message}");
                    DialogPortrait.Visible = false;
                }
            }
        }

        NextButtonContainer.Visible = false;
        PromptButtonBox.Visible = true;

        SureButton.Text = "Sure";
        Option1Button.Text = optimistic;
        Option2Button.Text = pessimistic;

        Option1Button.Visible = !string.IsNullOrWhiteSpace(optimistic);
        Option2Button.Visible = !string.IsNullOrWhiteSpace(pessimistic);

        SureButton.AddThemeColorOverride("font_color", new Color(0f / 255f, 200 / 255f, 200 / 255f));

        DialogBox.Visible = true;

        void OnSkipPressed() => _skipTyping = true;
        SureButton.Pressed += OnSkipPressed;
        Option1Button.Pressed += OnSkipPressed;
        Option2Button.Pressed += OnSkipPressed;

        await TypeText(message);

        SureButton.Pressed -= OnSkipPressed;
        Option1Button.Pressed -= OnSkipPressed;
        Option2Button.Pressed -= OnSkipPressed;

        _promptPressedTcs = new TaskCompletionSource<string>();

        SureButton.Pressed += OnPrompt0Pressed;
        Option1Button.Pressed += OnPrompt1Pressed;
        Option2Button.Pressed += OnPrompt2Pressed;

        var result = await _promptPressedTcs.Task;

        SureButton.Pressed -= OnPrompt0Pressed;
        Option1Button.Pressed -= OnPrompt1Pressed;
        Option2Button.Pressed -= OnPrompt2Pressed;

        PromptButtonBox.Visible = false;
        DialogBox.Visible = false;

        return result;
    }

    private void OnPrompt0Pressed() => _promptPressedTcs?.TrySetResult("Sure");
    private void OnPrompt1Pressed() => _promptPressedTcs?.TrySetResult("Optimist");
    private void OnPrompt2Pressed() => _promptPressedTcs?.TrySetResult("Pessimist");

    public async Task HandleChoice(string choice, string reactionSpeakerKey, string sureLine, string sureReaction, string optimistLine, string optimistReaction, string pessimistLine, string pessimistReaction)
    {
        switch (choice)
        {
            case "Sure":
                await Message(reactionSpeakerKey, sureReaction, sureLine);
                break;
            case "Optimist":
                G.GS.Karma++;
                await Message(reactionSpeakerKey, optimistReaction, optimistLine);
                break;
            case "Pessimist":
                G.GS.Karma--;
                await Message(reactionSpeakerKey, pessimistReaction, pessimistLine);
                break;
            default:
                Log.Err($"Unexpected choice '{choice}' in HandleChoice");
                break;
        }
    }

    private async Task TypeText(string fullText, float delay = 0.02f)
    {
        DialogText.Text = "";
        _skipTyping = false;

        void OnSkipPressed() => _skipTyping = true;
        NextButton.Pressed += OnSkipPressed;
        string wrappedText = WrapTextToWidth(fullText);

        foreach (char c in wrappedText)
        {
            if (_skipTyping)
            {
                DialogText.Text = wrappedText;
                break;
            }

            DialogText.Text += c;
            await ToSignal(GetTree().CreateTimer(delay), "timeout");
        }

        NextButton.Pressed -= OnSkipPressed;
    }

    private string WrapTextToWidth(string text, float maxWidth = 500f, int fontSize = 24)
    {
        var font = DialogText.GetThemeDefaultFont();
        if (font == null)
        {
            Log.Err("No font found for DialogText");
            return text;
        }

        var words = text.Split(' ');
        List<string> lines = new();
        string currentLine = "";
        string testLine = "";
        int lastPreferredBreak = -1;

        for (int i = 0; i < words.Length; i++)
        {
            string word = words[i];
            testLine = currentLine.Length == 0 ? word : $"{currentLine} {word}";
            float width = font.GetStringSize(testLine, HorizontalAlignment.Left, -1, fontSize).X;

            if (width <= maxWidth)
            {
                currentLine = testLine;

                if (word.EndsWith(".") || word.EndsWith("!") || word.EndsWith("?") || word.EndsWith(","))
                    lastPreferredBreak = i;
            }
            else
            {
                if (currentLine.Length == 0)
                {
                    lines.Add(word);
                    continue;
                }

                lines.Add(currentLine);
                currentLine = word;
            }
        }

        if (!string.IsNullOrEmpty(currentLine))
            lines.Add(currentLine);

        return string.Join("\n", lines);
    }
}
