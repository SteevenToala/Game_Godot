using Godot;

/// <summary>
/// Factory para crear enemigos shooters (disparan proyectiles)
/// Patrón: Concrete Factory - Implementación específica del Abstract Factory
/// </summary>
public class ShooterEnemyFactory : IEnemyFactory
{
	private readonly PackedScene _shooterScene;
	
	public ShooterEnemyFactory(PackedScene shooterScene)
	{
		_shooterScene = shooterScene;
	}
	
	public Enemy CreateEnemy(Vector2 position)
	{
		if (_shooterScene?.Instantiate() is ShooterEnemy shooter)
		{
			shooter.GlobalPosition = position;
			shooter.Initialize();
			return shooter;
		}
		
		GD.PrintErr("❌ Failed to create shooter enemy from scene");
		return null;
	}
	
	public string GetEnemyType()
	{
		return "Shooter";
	}
	
	public int GetDifficultyLevel()
	{
		return 3; // Nivel de dificultad alto
	}
}
