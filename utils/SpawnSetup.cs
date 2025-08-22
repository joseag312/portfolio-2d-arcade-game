using Godot;

public static class SpawnSetup
{
    public static void SetupEnemy(
        Node2D enemy,
        Node enemyContainer,
        Node2D dropContainer,
        Node2D effectContainer,
        Node2D projectileContainer,
        bool shouldPersist,
        bool shouldStay,
        Node2D ship = null)
    {
        if (enemy.HasNode("DropComponent"))
        {
            var drop = enemy.GetNode<DropComponent>("DropComponent");
            drop.DropTarget = dropContainer;
            drop.EffectTarget = effectContainer;
        }

        if (enemy.HasNode("DropOnHitComponent"))
        {
            var dropper = enemy.GetNode<DropOnHitComponent>("DropOnHitComponent");
            dropper.DropTarget = dropContainer;
            dropper.EffectTarget = effectContainer;
        }

        if (enemy.HasNode("DestroyedComponent"))
        {
            var effect = enemy.GetNode<DestroyedComponent>("DestroyedComponent");
            effect.EffectTarget = effectContainer;
        }

        if (enemy.HasNode("EnemyWeaponComponent"))
        {
            var weapons = enemy.GetNode<EnemyWeaponComponent>("EnemyWeaponComponent");
            weapons.ProjectileContainer = projectileContainer;
            weapons.TargetShip = ship;
        }

        if (enemy.HasNode("PersistenceComponent"))
        {
            var persistence = enemy.GetNode<PersistenceComponent>("PersistenceComponent");
            persistence.ShouldPersist = shouldPersist;
            persistence.ShouldStay = shouldStay;
        }

        if (enemy.HasNode("DropOnHitComponent"))
        {
            var dropper = enemy.GetNode<DropOnHitComponent>("DropOnHitComponent");
            dropper.EffectTarget = effectContainer;
            dropper.DropTarget = dropContainer;
        }

        enemyContainer.AddChild(enemy);
    }
}
