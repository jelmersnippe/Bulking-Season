using Godot;
using System;

public partial class MassComponent : Node
{
	[Signal] public delegate void MassChangedEventHandler(float change, float mass);
	[Signal] public delegate void FuelChangedEventHandler(float change, float fuel);
	
	[Export] public float StartingMass;
	[Export] public float StartingFuel;
	
	[Export] public float BaseBurnRate;
	[Export] public float MinBurnRate;
	[Export] public float FuelRequiredPerMass;
	[Export] public float BurnInterval = 1f;
	[Export] public float BulkRate;

	public float Mass { get; private set; }
	public float Fuel { get; private set; }
	public bool IsBulking { get; private set; }

	// Burn rate increases/decreases with mass, with a minimum to always ensure an approach to zero
	public float BurnRate => Math.Max(MinBurnRate, BaseBurnRate * Mathf.Pow(Mass / StartingMass, Mathf.Max(1, Mass - StartingMass)));

	public override void _Ready()
	{
		Mass = StartingMass;
		EmitSignal(SignalName.MassChanged, 0, Mass);
		Fuel = StartingFuel;
		EmitSignal(SignalName.FuelChanged, 0, Fuel);

		StartBurnInterval();
	}

	public void AddFuel(float amount)
	{
		Fuel += amount;
		EmitSignal(SignalName.FuelChanged, amount, Fuel);
	}

	public void ToggleBulk()
	{
		IsBulking = !IsBulking;

		if (IsBulking)
		{
			StartBulkInterval();
		}
	}

	public void ConsumeAllFuel()
	{
		var fuelChange = Fuel;
		Fuel = 0;
		EmitSignal(SignalName.FuelChanged, fuelChange, Fuel);
		
		var massChange = fuelChange / FuelRequiredPerMass;
		Mass += massChange;
		EmitSignal(SignalName.MassChanged, massChange, Mass);
	}
	
	private void ConsumeFuelForMass() {
		if (!IsBulking)
		{
			return;
		}

		if (Fuel < BulkRate)
		{
			ToggleBulk();
			return;
		}

		Fuel -= BulkRate;
		EmitSignal(SignalName.FuelChanged, -BulkRate, Fuel);
		var massChange = BulkRate / FuelRequiredPerMass;
		Mass += massChange;
		EmitSignal(SignalName.MassChanged, massChange, Mass);
	}
	
	private void StartBulkInterval()
	{
		if (!IsBulking)
		{
			return;
		}
		
		var timer = GetTree().CreateTimer(BurnInterval);
		timer.Timeout += () =>
		{
			ConsumeFuelForMass();
			StartBulkInterval();
		};
	}
	
	private void StartBurnInterval()
	{
		var timer = GetTree().CreateTimer(BurnInterval);
		timer.Timeout += () =>
		{
			ApplyBurnRate();
			StartBurnInterval();
		};
	}

	private void ApplyBurnRate()
	{
		var fuelChange = BurnRate;
		Fuel -= BurnRate;
		if (Fuel < 0)
		{
			// Negative fuel means we need to tap into mass to sustain the unit
			fuelChange -= Fuel;
			
			var massChange = Mathf.Abs(Fuel) / FuelRequiredPerMass;
			Mass = Mathf.Max(0, Mass - massChange);
			EmitSignal(SignalName.MassChanged, massChange, Mass);
				
			Fuel = 0;
		}
		EmitSignal(SignalName.FuelChanged, fuelChange, Fuel);
	}
}
