using Godot;
using System;

public partial class Pickup : Area2D
{
	[Export] public int MassIncrease = 1;
	
	public void Consume(Player player)
	{
		player.IncreaseMass(1);
	}
}
