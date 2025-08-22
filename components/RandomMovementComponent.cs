using Godot;
using System;
using System.Threading.Tasks;

[GlobalClass]
public partial class RandomMovementComponent : Node
{
    [Export] public Node2D OwnerNode { get; set; }
    [Export] public PersistenceComponent PersistenceComponent { get; set; }
    [Export] public FlashComponent FlashComponent { get; set; }
    [Export] public Vector2 TopLeft { get; set; } = new Vector2(280, 200);
    [Export] public Vector2 BottomRight { get; set; } = new Vector2(1000, 350);
    [Export] public float MinDistance { get; set; } = 100f;
    [Export] public float MinSpeed { get; set; } = 100f;
    [Export] public float MaxSpeed { get; set; } = 400f;
    [Export] public float PauseBetweenMoves { get; set; } = 1.0f;

    private Vector2 _targetPosition;
    private Vector2 _currentVelocity;
    private bool _isMoving = false;
    private bool _isActive = false;

    public override void _Ready()
    {
        if (PersistenceComponent != null)
        {
            PersistenceComponent.PersistenceComplete += OnPersistenceComplete;
        }
        else
        {
            Log.Err("PersistenceComponent not assigned");
        }
    }

    private void OnPersistenceComplete()
    {
        Activate();
    }

    public void Activate()
    {
        if (_isActive)
            return;

        if (OwnerNode == null)
        {
            Log.Err("OwnerNode not assigned");
            return;
        }

        _isActive = true;
        StartNextMove();
    }

    public override void _Process(double delta)
    {
        if (!_isActive || !_isMoving)
            return;

        Vector2 toTarget = _targetPosition - OwnerNode.GlobalPosition;
        float distanceToMove = (float)(delta * _currentVelocity.Length());

        if (toTarget.Length() <= distanceToMove)
        {
            OwnerNode.GlobalPosition = _targetPosition;
            _isMoving = false;
            _ = PauseAndMoveAgain();
        }
        else
        {
            Vector2 moveStep = _currentVelocity * (float)delta;
            OwnerNode.GlobalPosition += moveStep;
        }
    }

    private async Task PauseAndMoveAgain()
    {
        await Task.Delay((int)(PauseBetweenMoves * 1000));
        if (!IsInstanceValid(this) || !_isActive)
            return;

        StartNextMove();
    }

    private void StartNextMove()
    {
        const int maxAttempts = 10;
        Vector2 newTarget = OwnerNode.GlobalPosition;

        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            newTarget = new Vector2(
                (float)GD.RandRange(TopLeft.X, BottomRight.X),
                (float)GD.RandRange(TopLeft.Y, BottomRight.Y)
            );

            if ((newTarget - OwnerNode.GlobalPosition).Length() >= MinDistance)
                break;
        }

        FlashComponent?.Flash();
        _targetPosition = newTarget;

        float speed = (float)GD.RandRange(MinSpeed, MaxSpeed);
        Vector2 direction = (_targetPosition - OwnerNode.GlobalPosition).Normalized();
        _currentVelocity = direction * speed;

        _isMoving = true;
    }
}
