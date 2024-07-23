using Godot;

public partial class HealthComponent : Node
{
	[Signal]
	public delegate void HealthChangedEventHandler(int healthChange, int currentHealth, int maxHealth);

	[Signal]
	public delegate void DiedEventHandler();
	
	[Export] public int StartingHealth;

	public int MaxHealth { get; private set; }
	public int CurrentHealth { get; private set; }
	private bool _dead;

	public override void _Ready()
	{
		MaxHealth = StartingHealth;
		CurrentHealth = MaxHealth;
		EmitSignal(SignalName.HealthChanged, 0, CurrentHealth, MaxHealth);
	}

	public void TakeDamage(int amount)
	{
		if (_dead || amount == 0)
		{
			return;
		}
		
		var newHealth = Mathf.Clamp(CurrentHealth - amount, 0, MaxHealth);
		var healthChange = newHealth - CurrentHealth;
		CurrentHealth = newHealth;
		
		EmitSignal(SignalName.HealthChanged, healthChange, newHealth, MaxHealth);
		GD.Print(GetParent().Name + " took " + amount + " damage. " + newHealth + " health remaining");

		if (CurrentHealth == 0)
		{
			_dead = true;
			EmitSignal(SignalName.Died);
		}
	}

	public void Heal(int amount)
	{
		if (_dead)
		{
			return;
		}
		TakeDamage(-amount);
	}
}
