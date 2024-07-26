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

	private float _mass;
	private float _fuel;
	public bool IsBulking { get; private set; }

	// Burn rate increases/decreases with mass, with a minimum to always ensure an approach to zero
	public float BurnRate => Math.Max(MinBurnRate, BaseBurnRate * Mathf.Pow(_mass / StartingMass, Mathf.Max(1, _mass - StartingMass)));

	public override void _Ready()
	{
		_mass = StartingMass;
		EmitSignal(SignalName.MassChanged, 0, _mass);
		_fuel = StartingFuel;
		EmitSignal(SignalName.FuelChanged, 0, _fuel);

		StartBurnInterval();
	}

	public void AddFuel(float amount)
	{
		_fuel += amount;
		EmitSignal(SignalName.FuelChanged, amount, _fuel);
	}

	public void ToggleBulk()
	{
		IsBulking = !IsBulking;

		if (IsBulking)
		{
			StartBulkInterval();
		}
	}
	
	private void ConsumeFuelForMass() {
		if (!IsBulking)
		{
			return;
		}

		if (_fuel < BulkRate)
		{
			ToggleBulk();
			return;
		}

		_fuel -= BulkRate;
		EmitSignal(SignalName.FuelChanged, -BulkRate, _fuel);
		var massChange = BulkRate / FuelRequiredPerMass;
		_mass += massChange;
		EmitSignal(SignalName.MassChanged, massChange, _mass);
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
		_fuel -= BurnRate;
		if (_fuel < 0)
		{
			// Negative fuel means we need to tap into mass to sustain the unit
			fuelChange -= _fuel;
			
			var massChange = Mathf.Abs(_fuel) / FuelRequiredPerMass;
			_mass = Mathf.Max(0, _mass - massChange);
			EmitSignal(SignalName.MassChanged, massChange, _mass);
				
			_fuel = 0;
		}
		EmitSignal(SignalName.FuelChanged, fuelChange, _fuel);
	}
}
