using Godot;

/// <summary>
/// Factory para crear enemigos meteoros
/// Patrón: Concrete Factory - Implementación específica del Abstract Factory
/// </summary>
public class MeteoroEnemyFactory : IEnemyFactory
{
	private readonly PackedScene _meteoroScene;
	
	public MeteoroEnemyFactory(PackedScene meteoroScene)
	{
		_meteoroScene = meteoroScene;
	}
	
	public Enemy CreateEnemy(Vector2 position)
	{
		if (_meteoroScene?.Instantiate() is MeteoroEnemy meteoro)
		{
			meteoro.GlobalPosition = position;
			meteoro.Initialize();
			return meteoro;
		}
		
		GD.PrintErr("❌ Failed to create meteoro enemy from scene");
		return null;
	}
	
	public string GetEnemyType()
	{
		return "Meteoro";
	}
	
	public int GetDifficultyLevel()
	{
		return 2; // Nivel de dificultad medio
	}
}
