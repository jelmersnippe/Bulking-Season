using Godot;
using System;

public partial class Pickup : Area2D
{
	[Export] public int FuelAmount = 1;
	
	public void Consume(Player player)
	{
		player.MassComponent.AddFuel(FuelAmount);
	}
}
