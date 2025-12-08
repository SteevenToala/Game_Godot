using Godot;

/// <summary>
/// Factory para crear enemigos básicos (Enemy estándar)
/// Patrón: Concrete Factory - Implementación específica del Abstract Factory
/// </summary>
public class BasicEnemyFactory : IEnemyFactory
{
	private readonly PackedScene _enemyScene;
	
	public BasicEnemyFactory(PackedScene enemyScene)
	{
		_enemyScene = enemyScene;
	}
	
	public Enemy CreateEnemy(Vector2 position)
	{
		if (_enemyScene?.Instantiate() is Enemy enemy)
		{
			enemy.GlobalPosition = position;
			enemy.Initialize();
			return enemy;
		}
		
		GD.PrintErr("❌ Failed to create basic enemy from scene");
		return null;
	}
	
	public string GetEnemyType()
	{
		return "Basic";
	}
	
	public int GetDifficultyLevel()
	{
		return 1; // Nivel de dificultad bajo
	}
}
