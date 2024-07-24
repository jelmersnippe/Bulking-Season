using Godot;
using System;

public partial class UI : CanvasLayer
{
	[Export] public Player Player;
	
	public override void _Ready()
	{
		Player.MassChanged += (mass) => GetNode<Label>("Label").Text = "Mass: " + mass;
	}
}
