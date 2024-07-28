using Godot;
using System;

public partial class Knockable : Area2D
{
	[Signal] public delegate void KnockedStatusChangedEventHandler(bool knocked);
	[Export] public CharacterBody2D Body;
	[Export] public float KnockbackRecoverySpeed = 5f;
	private bool _knocked = false;

	public override void _Ready()
	{
		AreaEntered += OnAreaEntered;
	}

	private void OnAreaEntered(Area2D area)
	{
		GD.Print(area.GetParent().Name);
		if (area is not HitboxComponent hitboxComponent)
		{
			return;
		}
		
		Knockback(hitboxComponent.GlobalPosition.DirectionTo(GlobalPosition), hitboxComponent.KnockbackForce);
	}

	public override void _Process(double delta)
	{
		if (!_knocked)
		{
			return;
		}
		
		Body.Velocity -= Body.Velocity.Normalized() * KnockbackRecoverySpeed;

		if (Body.Velocity.Length() < 5)
		{
			Body.Velocity = Vector2.Zero;
			_knocked = false;
			EmitSignal(SignalName.KnockedStatusChanged, _knocked);
		}
	}

	private void Knockback(Vector2 direction, float force)
	{
		GD.Print(GetParent().Name + " knocked with force " + force);
		Body.Velocity = direction.Normalized() * force;
		_knocked = true;
		EmitSignal(SignalName.KnockedStatusChanged, _knocked);
	}
}
