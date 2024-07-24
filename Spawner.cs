using Godot;
using System;

public partial class Spawner : Node
{
	[Export] public Node2D Player;
	[Export] public PackedScene EnemyScene;
	[Export] public float SpawnInterval = 2f;
	[Export] public int MaxEnemies = 10;

	private Vector2 _spawnOffset = Vector2.Zero;
	private int _activeEnemies = 0;

	public override void _Ready()
	{
		_spawnOffset = GetViewport().GetVisibleRect().Size / 2f;

		var timer = GetTree().CreateTimer(SpawnInterval);
		timer.Timeout += SpawnOnInterval;
	}

	private void SpawnOnInterval()
	{
		SpawnEnemy();
		
		var timer = GetTree().CreateTimer(SpawnInterval);
		timer.Timeout += SpawnOnInterval;
	}

	private void SpawnEnemy()
	{
		if (_activeEnemies >= MaxEnemies)
		{
			return;
		}
		var enemy = EnemyScene.Instantiate<Enemy>();

		var rng = new RandomNumberGenerator();
		var spawnPosition = Player.GlobalPosition + new Vector2(rng.RandfRange(-_spawnOffset.X, _spawnOffset.X),
			rng.RandfRange(-_spawnOffset.Y, _spawnOffset.Y));
		enemy.GlobalPosition = spawnPosition;
		
		enemy.HealthComponent.Died += () => _activeEnemies--;
		enemy.HealthComponent.Died += SpawnEnemy;
		enemy.Target = Player;

		CallDeferred("add_child", enemy);
		_activeEnemies++;
	}
}
