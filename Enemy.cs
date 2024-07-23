using Godot;
using System;

public partial class Enemy : CharacterBody2D
{
	[Export] public HealthComponent HealthComponent;
	public const float Speed = 50.0f;

	public override void _Ready()
	{
		HealthComponent.Died += HealthComponentOnDied;
	}

	private void HealthComponentOnDied()
	{
		QueueFree();
	}

	public override void _Process(double delta)
	{
		MoveAndSlide();
	}
}
