using Godot;
using System;

public partial class Enemy : CharacterBody2D
{
	[Export] public HealthComponent HealthComponent;
	[Export] public Knockable KnockableComponent;
	[Export] public float Speed = 100.0f;
	[Export] public PackedScene DropOnDeath;
	public Node2D Target;

	private bool _inControl = true;

	public override void _Ready()
	{
		HealthComponent.Died += HealthComponentOnDied;
		KnockableComponent.KnockedStatusChanged += (knocked) => _inControl = !knocked;
	}

	private void HealthComponentOnDied()
	{
		if (DropOnDeath != null)
		{
			var pickup = DropOnDeath.Instantiate<Pickup>();
			pickup.GlobalPosition = GlobalPosition;
			GetTree().Root.CallDeferred("add_child", pickup);
		}
		QueueFree();
	}

	public override void _Process(double delta)
	{
		var sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		if (Target == null)
		{
			sprite.Play("Idle");
		}
		else if (!_inControl)
		{
			sprite.Play("Hurt");
		}
		
		if (_inControl && Target != null)
		{
			Velocity = GlobalPosition.DirectionTo(Target.GlobalPosition) * Speed;
			sprite.Play("Move");
		}
		
		MoveAndSlide();
	}
}
