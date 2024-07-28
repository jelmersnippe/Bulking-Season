using Godot;
using System;

public partial class UI : CanvasLayer
{
	[Export] public Player Player;
	[Export] public Label MassLabel;
	[Export] public Label FuelLabel;
	
	public override void _Ready()
	{
		MassLabel.Text = "Mass: " + Player.MassComponent.Mass;
		FuelLabel.Text = "Fuel: " + Player.MassComponent.Fuel;
		
		Player.MassComponent.MassChanged += (change, mass) => MassLabel.Text = "Mass: " + mass;
		Player.MassComponent.FuelChanged += (change, fuel) => FuelLabel.Text = "Fuel: " + fuel;
	}
}
