using Godot;
using System;

public partial class Player : CharacterBody2D
{
	[Export] public HealthComponent HealthComponent;
	[Export] public Knockable KnockableComponent;
	private bool _inControl = true;
	
	[Export] public const float AttackCooldown = 0.2f;
	private bool _canAttack = true;
	[Export]
	public const float Speed = 300.0f;

	[Export]
	public Node2D RotationPoint { get; set; }
	[Export]
	public Node2D WeaponDisplay { get; set; }
	[Export]
	public CollisionPolygon2D WeaponHitbox { get; set; }

	[Export]
	public float WeaponOffset = 70;

	private bool _weaponSide = false;
	private bool _dead = false;
	
	public override void _Ready(){ 
		WeaponDisplay.RotationDegrees = (_weaponSide ? WeaponOffset : -WeaponOffset);
		WeaponHitbox.CallDeferred("set_disabled", true);
		
		HealthComponent.Died += HealthComponentOnDied;
		
		KnockableComponent.KnockedStatusChanged += (knocked) => _inControl = !knocked;
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
		
		if (Input.IsActionJustPressed("attack") && _canAttack)
		{
			_canAttack = false;
			
			_weaponSide = !_weaponSide;
			WeaponDisplay.RotationDegrees = (_weaponSide ? WeaponOffset : -WeaponOffset);
			WeaponHitbox.CallDeferred("set_disabled", false);
			
			var hitboxTimer = GetTree().CreateTimer(0.1);
			hitboxTimer.Timeout += () => WeaponHitbox.Disabled = true;
			
			var attackCooldownTimer = GetTree().CreateTimer(AttackCooldown);
			attackCooldownTimer.Timeout += () => _canAttack = true;
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
		_dead = false;
	}
}
