using Godot;
using System;

public partial class HealthBar : ProgressBar
{
	[Export] public HealthComponent HealthComponent;
	
	public override void _Ready()
	{
		HealthComponentOnHealthChanged(0, HealthComponent.CurrentHealth, HealthComponent.MaxHealth);
		HealthComponent.HealthChanged += HealthComponentOnHealthChanged;
	}

	private void HealthComponentOnHealthChanged(int healthChange, int currentHealth, int maxHealth)
	{
		MaxValue = maxHealth;
		Value = currentHealth;
	}
}
