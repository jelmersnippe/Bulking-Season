using Godot;
using System;

public partial class MassTestScene : CanvasLayer
{
	[Export] public MassComponent MassComponent;
	[Export] public Node2D MassDisplay;
	
	[Export] public Label MassLabel;
	[Export] public Label FuelLabel;
	[Export] public Label IsBulkingLabel;
	
	[Export] public Label BaseBurnRateLabel;
	[Export] public Label MinBurnRateLabel;
	[Export] public Label CurrentBurnRateLabel;
	[Export] public Label BurnIntervalLabel;
	
	[Export] public Label FuelRequiredPerMassLabel;
	[Export] public Label BulkRateLabel;
	
	public override void _Ready()
	{
		MassComponent.MassChanged += UpdateMassLabel;
		MassComponent.FuelChanged += UpdateFuelLabel;
	}
	
	public override void _Process(double delta)
	{
		IsBulkingLabel.Text = "IsBulking: " + MassComponent.IsBulking;
		
		BaseBurnRateLabel.Text = "Mass: " + MassComponent.BaseBurnRate;
		MinBurnRateLabel.Text = "MinBurnRate: " + MassComponent.MinBurnRate;
		CurrentBurnRateLabel.Text = "BurnRate: " + MassComponent.BurnRate;
		BurnIntervalLabel.Text = "BurnInterval: " + MassComponent.BurnInterval;
		
		FuelRequiredPerMassLabel.Text = "FuelRequiredPerMass: " + MassComponent.FuelRequiredPerMass;
		BulkRateLabel.Text = "BulkRate: " + MassComponent.BulkRate;
	}
	
	private void _on_add_fuel_button_pressed()
	{
		MassComponent.AddFuel(20);
	}

	private void UpdateMassLabel(float change, float mass) {
		MassLabel.Text = "Mass: " + mass;
		var scale = mass / MassComponent.StartingMass;
		MassDisplay.Scale = new Vector2(scale, scale);
	}

	private void UpdateFuelLabel(float change, float fuel) {
		FuelLabel.Text = "Fuel: " + fuel;
	}

	private void _on_bulk_toggle_pressed()
	{
		MassComponent.ToggleBulk();
	}
}
