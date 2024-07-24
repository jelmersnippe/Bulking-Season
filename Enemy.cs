using Godot;
using System;

public partial class Enemy : CharacterBody2D
{
	[Export] public HealthComponent HealthComponent;
	[Export] public Knockable KnockableComponent;
	[Export] public const float Speed = 150.0f;
	public Node2D Target;

	private bool _inControl = true;

	public override void _Ready()
	{
		HealthComponent.Died += HealthComponentOnDied;
		KnockableComponent.KnockedStatusChanged += (knocked) => _inControl = !knocked;
	}

	private void HealthComponentOnDied()
	{
		QueueFree();
	}

	public override void _Process(double delta)
	{
		if (_inControl && Target != null)
		{
			Velocity = GlobalPosition.DirectionTo(Target.GlobalPosition) * Speed;
		}
		
		MoveAndSlide();
	}
}
