using Godot;
using System;

public partial class Player : CharacterBody2D
{
	[Export] public MassComponent MassComponent;
	[Export] public HealthComponent HealthComponent;
	[Export] public Knockable KnockableComponent;
	[Export] public Area2D PickupRange;
	private bool _inControl = true;
	
	[Export]
	public const float Speed = 300.0f;

	[Export]
	public Node2D RotationPoint { get; set; }

	[Export] public PackedScene WeaponScene;
	private Weapon _activeWeapon;

	private bool _dead = false;

	public override void _Ready(){ 
		HealthComponent.Died += HealthComponentOnDied;
		
		KnockableComponent.KnockedStatusChanged += (knocked) => _inControl = !knocked;
		
		PickupRange.AreaEntered += PickupRangeOnAreaEntered;

		MassComponent.MassChanged += (change, mass) =>
			Scale = new Vector2(mass / MassComponent.StartingMass, mass / MassComponent.StartingMass);

		if (WeaponScene != null)
		{
			var weapon = WeaponScene.Instantiate<Weapon>();
			RotationPoint.CallDeferred("add_child", weapon);
			_activeWeapon = weapon;
		}
	}

	private void PickupRangeOnAreaEntered(Area2D area)
	{
		if (area is not Pickup pickup)
		{
			return;
		}
		
		pickup.Consume(this);
		pickup.QueueFree();
	}

	public override void _Process(double delta)
	{
		if (_dead || !_inControl)
		{
			MoveAndSlide();
			return;
		}
		
		var direction = Input.GetVector("move_left", "move_right", "move_up", "move_down").Normalized();
		Velocity = direction * Speed;
		MoveAndSlide();
		
		if (Input.IsActionJustPressed("attack") && _activeWeapon != null)
		{
			_activeWeapon.Attack(MassComponent.Mass / MassComponent.StartingMass);
		}

		if (direction != Vector2.Zero)
		{
			RotationPoint.RotationDegrees = GetRotationFromDirection(direction);
		}
	}
	
	public float GetRotationFromDirection(Vector2 direction)
	{
		// Ensure the direction is normalized
		direction = direction.Normalized();
		
		// Get the angle in radians using atan2
		float angleRadians = Mathf.Atan2(direction.Y, direction.X);
		
		// Convert the angle to degrees
		float angleDegrees = Mathf.RadToDeg(angleRadians);
		
		// Normalize the angle to be between 0 and 360 degrees
		angleDegrees = (angleDegrees + 360) % 360;
		
		// Define the 8 possible directions in degrees
		float[] directionsDegrees = { 0, 45, 90, 135, 180, 225, 270, 315 };
		
		// Find the closest direction
		float closestDirection = directionsDegrees[0];
		float minDiff = Mathf.Abs(angleDegrees - directionsDegrees[0]);
		
		foreach (float directionDegree in directionsDegrees)
		{
			float diff = Mathf.Abs(angleDegrees - directionDegree);
			if (diff < minDiff)
			{
				closestDirection = directionDegree;
				minDiff = diff;
			}
		}
		
		return closestDirection;
	}

	private void HealthComponentOnDied()
	{
		_dead = true;
	}

	public override void _Input(InputEvent @event)
	{
		if (@event.IsAction("consume"))
		{
			MassComponent.ConsumeAllFuel();
		}
	}
}
