#nullable enable
using Godot;
using System;

[GlobalClass]
public partial class DropPawllars : DropBase, IDropAmount
{
    [Export] public int Amount = 10;

    public void SetAmount(int amount, DropContext? context = null)
    {
        float multiplier = 1f;

        if (context != null)
        {
            multiplier *= G.GS.EventCurrencyMultiplier;
            multiplier *= G.GS.CurrencyMultiplier;
            multiplier *= 1f + Mathf.Clamp(G.GS.Karma * 0.01f, -0.5f, 0.5f);

            switch (context.SourceType)
            {
                case DropSourceType.Rare:
                    multiplier *= 1f;
                    break;
                case DropSourceType.Epic:
                    multiplier *= 1f;
                    break;
                case DropSourceType.Legend:
                    multiplier *= 1.5f;
                    break;
                case DropSourceType.Boss:
                    multiplier *= 2.5f;
                    break;
                case DropSourceType.Common:
                    multiplier *= 1f;
                    break;
                default:
                    break;
            }
        }
        Amount = Mathf.RoundToInt(amount * multiplier);
        if (Amount < 1) Amount = 1;
    }

    public override void HandlePickup(HitboxComponent hitboxComponent)
    {
        G.SFX.Play(SFX.COIN);
        G.GS.Pawllars += Amount;

        if (DropTextScene != null)
        {
            var dropTextInstance = DropTextScene.Instantiate<DropTextWhite>();
            dropTextInstance.Initialize(Amount);
            dropTextInstance.GlobalPosition = GlobalPosition;

            if (EffectContainer != null)
            {
                EffectContainer.AddChild(dropTextInstance);
            }
            else
            {
                Log.Err("EffectContainer not assigned, using current Scene");
                GetTree().CurrentScene.AddChild(dropTextInstance);
            }
        }
        else
        {
            Log.Err("DropTextScene not assigned");
        }
    }
}
