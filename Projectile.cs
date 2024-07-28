using Godot;
using System;

public partial class Projectile : Node2D
{
	[Export] public HitboxComponent HitboxComponent;

	public void SetMultiplier(float multiplier)
	{
		HitboxComponent.ContactDamage = Mathf.RoundToInt(HitboxComponent.ContactDamage * multiplier);
		HitboxComponent.KnockbackForce = HitboxComponent.KnockbackForce * multiplier;
	}
}
