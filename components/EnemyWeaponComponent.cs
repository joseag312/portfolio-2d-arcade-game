using Godot;
using System;

[GlobalClass]
public partial class EnemyWeaponComponent : Node
{
    [ExportGroup("Weapon Slot 1")]
    [Export] public Marker2D Muzzle1 { get; set; }
    [Export] public PackedScene ProjectileScene1 { get; set; }
    [Export] public bool AimAtPlayer1 { get; set; } = true;
    [Export] public float Velocity1 { get; set; } = 200f;
    [Export] public Timer Timer1 { get; set; }
    [Export] public float Cooldown1Min { get; set; } = 2f;
    [Export] public float Cooldown1Max { get; set; } = 2f;

    [ExportGroup("Weapon Slot 2")]
    [Export] public Marker2D Muzzle2 { get; set; }
    [Export] public PackedScene ProjectileScene2 { get; set; }
    [Export] public bool AimAtPlayer2 { get; set; } = true;
    [Export] public float Velocity2 { get; set; } = 200f;
    [Export] public Timer Timer2 { get; set; }
    [Export] public float Cooldown2Min { get; set; } = 2f;
    [Export] public float Cooldown2Max { get; set; } = 2f;

    [ExportGroup("Weapon Slot 3")]
    [Export] public Marker2D Muzzle3 { get; set; }
    [Export] public PackedScene ProjectileScene3 { get; set; }
    [Export] public bool AimAtPlayer3 { get; set; } = true;
    [Export] public float Velocity3 { get; set; } = 200f;
    [Export] public Timer Timer3 { get; set; }
    [Export] public float Cooldown3Min { get; set; } = 2f;
    [Export] public float Cooldown3Max { get; set; } = 2f;

    [ExportGroup("Global")]
    [Export] public Node2D ProjectileContainer { get; set; }
    [Export] public Node2D TargetShip { get; set; }

    public override void _Ready()
    {
        if (Timer1 != null)
            Timer1.Timeout += Fire1;

        if (Timer2 != null)
            Timer2.Timeout += Fire2;

        if (Timer3 != null)
            Timer3.Timeout += Fire3;

        Timer1?.Start(GD.Randf() * (Cooldown1Max - Cooldown1Min) + Cooldown1Min);
        Timer2?.Start(GD.Randf() * (Cooldown2Max - Cooldown2Min) + Cooldown2Min);
        Timer3?.Start(GD.Randf() * (Cooldown3Max - Cooldown3Min) + Cooldown3Min);
    }


    private void Fire1()
    {
        FireProjectile(Muzzle1, ProjectileScene1, Velocity1, AimAtPlayer1);

        if (Timer1 != null)
        {
            Timer1.Stop();
            Timer1.Start(GD.Randf() * (Cooldown1Max - Cooldown1Min) + Cooldown1Min);
        }
    }

    private void Fire2()
    {
        FireProjectile(Muzzle2, ProjectileScene2, Velocity2, AimAtPlayer2);

        if (Timer2 != null)
        {
            Timer2.Stop();
            Timer2.Start(GD.Randf() * (Cooldown2Max - Cooldown2Min) + Cooldown2Min);
        }
    }

    private void Fire3()
    {
        FireProjectile(Muzzle3, ProjectileScene3, Velocity3, AimAtPlayer3);

        if (Timer3 != null)
        {
            Timer3.Stop();
            Timer3.Start(GD.Randf() * (Cooldown3Max - Cooldown3Min) + Cooldown3Min);
        }
    }

    private void FireProjectile(Marker2D muzzle, PackedScene scene, float velocity, bool aimAtPlayer)
    {
        if (muzzle == null)
        {
            Log.Err("Muzzle is null, cannot fire projectile");
            return;
        }

        if (scene == null)
        {
            Log.Err("Projectile scene is null");
            return;
        }

        Node2D projectile = scene.Instantiate<Node2D>();
        projectile.GlobalPosition = muzzle.GlobalPosition;

        Vector2 direction = Vector2.Down;
        if (aimAtPlayer && IsInstanceValid(TargetShip))
        {
            direction = (TargetShip.GlobalPosition - muzzle.GlobalPosition).Normalized();
        }

        MoveComponent move = projectile.GetNodeOrNull<MoveComponent>("MoveComponent");
        if (move != null)
        {
            move.Velocity = direction * velocity;
            move.Actor = projectile;
        }
        else
        {
            Log.Err("MoveComponent not found on projectile");
        }

        Node parent = ProjectileContainer ?? GetTree().CurrentScene;
        parent.AddChild(projectile);
        projectile.AddToGroup("despawnable");
    }
}
