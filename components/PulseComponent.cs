using Godot;
using System;

[GlobalClass]
public partial class PulseComponent : Node
{
    [Export] public Node2D Target;
    [Export] public float ScaleAmplitude = 0.05f;
    [Export] public float PulseSpeed = 5.0f;
    [Export] public float FlashAmplitude = 0.3f;
    [Export] public Vector2 BaseScale = new Vector2(1, 1);
    [Export] public Color BaseColor = Colors.White;

    private float _time;

    public override void _Ready()
    {
        if (Target == null)
            Target = GetParent<Node2D>();
    }

    public override void _Process(double delta)
    {
        _time += (float)delta * PulseSpeed;

        float scaleFactor = 1.0f + Mathf.Sin(_time) * ScaleAmplitude;
        Target.Scale = BaseScale * scaleFactor;

        if (Target is CanvasItem canvasItem)
        {
            float flash = 1.0f + Mathf.Sin(_time) * FlashAmplitude;
            canvasItem.Modulate = BaseColor * flash;
        }
    }

    public void StopPulse()
    {
        SetProcess(false);
    }
}
