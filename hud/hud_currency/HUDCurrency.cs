using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

[GlobalClass]
public partial class HUDCurrency : Control
{
    [Export] public Label MewnitsLabel { get; set; }
    [Export] public Label PawllarsLabel { get; set; }

    private const int ScrambleSteps = 6;
    private const float StepDuration = 0.06f;

    private bool _isConnected;
    private int _lastMewnits;
    private int _lastPawllars;
    private int _displayMewnits, _displayPawllars;
    private int _targetMewnits, _targetPawllars;
    private bool _animatingMewnits, _animatingPawllars;

    private readonly Color _gainColor = new Color(0.55f, 1f, 0.55f);
    private readonly Color _spendColor = new Color(1f, 0.55f, 0.55f);
    private readonly Color _normalColor = new Color(1f, 1f, 1f);

    public override void _Ready()
    {
        _displayMewnits = _targetMewnits = _lastMewnits = G.GS.Mewnits;
        _displayPawllars = _targetPawllars = _lastPawllars = G.GS.Pawllars;
        SetLabel(MewnitsLabel, _displayMewnits);
        SetLabel(PawllarsLabel, _displayPawllars);

        if (!_isConnected)
        {
            G.GS.CurrencyChanged += OnCurrencyChanged;
            _isConnected = true;
        }
    }

    public override void _ExitTree()
    {
        if (_isConnected)
        {
            G.GS.CurrencyChanged -= OnCurrencyChanged;
            _isConnected = false;
        }
    }

    private void OnCurrencyChanged()
    {
        if (_targetMewnits != G.GS.Mewnits)
        {
            _targetMewnits = G.GS.Mewnits;
            _lastMewnits = _targetMewnits;
            if (!_animatingMewnits) _ = AnimateUntilStable_Mewnits();
        }

        if (_targetPawllars != G.GS.Pawllars)
        {
            _targetPawllars = G.GS.Pawllars;
            _lastPawllars = _targetPawllars;
            if (!_animatingPawllars) _ = AnimateUntilStable_Pawllars();
        }
    }


    private void SetLabel(Label label, int value)
    {
        label.Text = $"{value}";
        label.Modulate = _normalColor;
    }

    private async Task AnimateUntilStable_Mewnits()
    {
        _animatingMewnits = true;
        try
        {
            while (_displayMewnits != _targetMewnits)
            {
                int from = _displayMewnits;
                int to = _targetMewnits;
                await AnimateNumberChange(MewnitsLabel, from, to);
                _displayMewnits = to;
                SetLabel(MewnitsLabel, _displayMewnits);
            }
        }
        finally { _animatingMewnits = false; }
    }

    private async Task AnimateUntilStable_Pawllars()
    {
        _animatingPawllars = true;
        try
        {
            while (_displayPawllars != _targetPawllars)
            {
                int from = _displayPawllars;
                int to = _targetPawllars;
                await AnimateNumberChange(PawllarsLabel, from, to);
                _displayPawllars = to;
                SetLabel(PawllarsLabel, _displayPawllars);
            }
        }
        finally { _animatingPawllars = false; }
    }

    private async Task AnimateNumberChange(Label label, int oldVal, int newVal)
    {
        bool gained = newVal > oldVal;
        var targetColor = gained ? _gainColor : _spendColor;
        Punch(label, targetColor);

        int digits = Math.Max(1, Math.Max(oldVal, newVal).ToString().Length);
        int min = digits == 1 ? 0 : (int)Mathf.Pow(10, digits - 1);
        int max = (int)Mathf.Pow(10, digits) - 1;

        var rng = new Random();

        for (int i = 0; i < ScrambleSteps - 1; i++)
        {
            int fake = rng.Next(min, max + 1);
            label.Text = fake.ToString().PadLeft(digits, '0');
            await ToSignal(GetTree().CreateTimer(StepDuration), "timeout");
        }

        label.Text = $"{newVal}";
        await ToSignal(GetTree().CreateTimer(StepDuration), "timeout");

        var tween = GetTree().CreateTween();
        tween.TweenProperty(label, "modulate", _normalColor, 0.15f);
        await ToSignal(tween, "finished");
    }

    private void Punch(Label label, Color flashColor)
    {
        var tween = GetTree().CreateTween();
        tween.TweenProperty(label, "modulate", flashColor, 0.05f);

        var baseScale = label.Scale;
        tween.Parallel().TweenProperty(label, "scale", baseScale * 1.05f, 0.08f)
            .SetTrans(Tween.TransitionType.Cubic).SetEase(Tween.EaseType.Out);
        tween.TweenProperty(label, "scale", baseScale, 0.12f)
            .SetTrans(Tween.TransitionType.Cubic).SetEase(Tween.EaseType.In);
    }
}
