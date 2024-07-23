using Godot;
using System;

public partial class HurtboxComponent : Area2D
{
	[Export] public HealthComponent HealthComponent;
	
	public override void _Ready()
	{
		AreaEntered += OnAreaEntered;
	}

	private void OnAreaEntered(Area2D area)
	{
		if (area is not HitboxComponent hitboxComponent)
		{
			return;
		}
		
		HealthComponent.TakeDamage(hitboxComponent.ContactDamage);
	}
}
