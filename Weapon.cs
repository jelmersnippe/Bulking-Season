using Godot;
using System;

public partial class Weapon : Node2D
{
	[Export] public PackedScene ProjectileScene;
	[Export]
	public float WeaponOffset = 70;
	
	[Export]
	public Node2D WeaponDisplay { get; set; }
	
	[Export] public const float AttackCooldown = 0.2f;
	private bool _canAttack = true;

	private bool _weaponSide = false;
	
	public override void _Ready(){ 
		WeaponDisplay.RotationDegrees = (_weaponSide ? WeaponOffset : -WeaponOffset);
	}
	
	public void Attack(float strength)
	{
		if (!_canAttack)
		{
			return;
		}
		
		_canAttack = false;
		
		_weaponSide = !_weaponSide;
		WeaponDisplay.RotationDegrees = (_weaponSide ? WeaponOffset : -WeaponOffset);

		var projectile = ProjectileScene.Instantiate<Projectile>();
		projectile.GlobalPosition = GlobalPosition;
		projectile.Rotation = GlobalRotation;
		projectile.SetMultiplier(strength);
		GetTree().Root.CallDeferred("add_child", projectile);
		
		var projectileLifeTimer = GetTree().CreateTimer(0.1);
		projectileLifeTimer.Timeout += () => projectile.QueueFree();
		
		var attackCooldownTimer = GetTree().CreateTimer(AttackCooldown);
		attackCooldownTimer.Timeout += () => _canAttack = true;
	}
}
