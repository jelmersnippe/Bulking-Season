using Godot;
using System;

public partial class HitboxComponent : Area2D
{
	[Export] public int ContactDamage;
	[Export] public float KnockbackForce;
}
