using Godot;

[GlobalClass]
public partial class InvincibilityComponent : Node
{
	[Export] public float InvincibilityDuration { get; set; } = 1.0f;
	[Export] public Node2D OwnerNode { get; set; }

	private bool _isInvincible = false;
	private Timer _invincibilityTimer;
	private Tween _flashTween;

	public bool IsInvincible
	{
		get => _isInvincible;
		private set
		{
			_isInvincible = value;

			if (OwnerNode != null)
			{
				FlashEffect(_isInvincible);
			}
		}
	}

	public override void _Ready()
	{
		if (OwnerNode == null)
			OwnerNode = GetParent<Node2D>();

		_invincibilityTimer = new Timer
		{
			WaitTime = InvincibilityDuration,
			OneShot = true
		};

		AddChild(_invincibilityTimer);
		_invincibilityTimer.Timeout += OnInvincibilityEnd;
	}

	public void StartInvincibility()
	{
		if (!_isInvincible)
		{
			IsInvincible = true;
			_invincibilityTimer.Start();
		}
	}

	private void OnInvincibilityEnd()
	{
		IsInvincible = false;
	}

	private void FlashEffect(bool enable)
	{
		if (OwnerNode == null) return;

		if (_flashTween != null && _flashTween.IsRunning())
			_flashTween.Kill();

		if (enable)
		{
			_flashTween = OwnerNode.GetTree().CreateTween();
			_flashTween.SetLoops((int)(InvincibilityDuration / 0.2f));
			_flashTween.TweenProperty(OwnerNode, "modulate", new Color(1.5f, 1.5f, 1.5f, 0.5f), 0.1f);
			_flashTween.TweenProperty(OwnerNode, "modulate", new Color(1.0f, 1.0f, 1.0f, 1.0f), 0.1f);
		}
		else
		{
			OwnerNode.Modulate = new Color(1, 1, 1, 1);
		}
	}

	public void ForceStartInvincibility()
	{
		if (!_isInvincible)
		{
			IsInvincible = true;
		}

		if (_invincibilityTimer != null && _invincibilityTimer.TimeLeft > 0)
			_invincibilityTimer.Stop();
	}

	public void ForceEndInvincibility()
	{
		IsInvincible = false;

		if (_invincibilityTimer != null && _invincibilityTimer.TimeLeft > 0)
			_invincibilityTimer.Stop();
	}

}
