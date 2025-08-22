using Godot;

[GlobalClass]
public partial class HealthBarComponent : Node
{
	[Export] public StatsComponent StatsComponent { get; set; }
	[Export] public ScaleComponent ScaleComponent { get; set; }
	[Export] public FlashComponent FlashComponent { get; set; }
	[Export] public HurtboxComponent HurtboxComponent { get; set; }
	[Export] public TextureProgressBar HealthBar { get; set; }
	[Export] public AnimatedSprite2D AnimatedSprite { get; set; }

	private const float HealthBarHeightOffset = 10f;

	public override void _Ready()
	{
		HealthBar.MaxValue = StatsComponent.Health;
		HealthBar.Value = StatsComponent.Health;
		HurtboxComponent.Hurt += OnHurt;

		AdjustHealthBar();
	}

	private void AdjustHealthBar()
	{
		if (AnimatedSprite == null || HealthBar == null)
			return;

		SpriteFrames frames = AnimatedSprite.SpriteFrames;
		if (!frames.HasAnimation("move"))
			return;

		Texture2D frameTexture = frames.GetFrameTexture("move", 0);
		if (frameTexture == null)
			return;

		Vector2 spriteSize = frameTexture.GetSize() * AnimatedSprite.Scale;

		float minWidth = 50f;
		int newHealthBarWidth = (int)Mathf.Max(spriteSize.X, minWidth);

		Gradient gradientUnder = new Gradient();
		gradientUnder.SetColor(0, new Color(1, 0, 0, 1));
		gradientUnder.SetColor(1, new Color(1, 0, 0, 1));
		GradientTexture2D gradientTextureUnder = new GradientTexture2D
		{
			Gradient = gradientUnder,
			Width = newHealthBarWidth,
			Height = 5
		};

		Gradient gradientProgress = new Gradient();
		gradientProgress.SetColor(0, new Color(0, 1, 0, 1));
		gradientProgress.SetColor(1, new Color(0, 1, 0, 1));
		GradientTexture2D gradientTextureProgress = new GradientTexture2D
		{
			Gradient = gradientProgress,
			Width = newHealthBarWidth,
			Height = 5
		};

		HealthBar.TextureUnder = gradientTextureUnder;
		HealthBar.TextureProgress = gradientTextureProgress;
		HealthBar.Position = new Vector2(-gradientTextureUnder.GetSize().X / 2, -spriteSize.Y / 2 - HealthBarHeightOffset);
		HealthBar.Visible = false;
	}

	private void OnHurt(HitboxComponent hitboxComponent)
	{
		HealthBar.Visible = true;
		ScaleComponent.TweenScale();
		FlashComponent.Flash();
		UpdateHealthBar();
	}

	private void UpdateHealthBar()
	{
		HealthBar.Value = StatsComponent.Health;
	}

	public override void _ExitTree()
	{
		HurtboxComponent.Hurt -= OnHurt;
	}
}
