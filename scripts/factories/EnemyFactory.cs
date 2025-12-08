using Godot;
using Godot.Collections;

/// <summary>
/// Factory estática para compatibilidad retroactiva
/// NOTA: Este enfoque está deprecated. Usar EnemyFactoryManager para nuevos desarrollos.
/// Patrón: Facade - Proporciona interfaz simplificada al nuevo sistema de factories
/// </summary>
public static class EnemyFactory
{
	private static EnemyFactoryManager _factoryManager;
	private static bool _isInitialized = false;
	
	/// <summary>
	/// Inicializa el sistema de factories con las escenas proporcionadas
	/// </summary>
	public static void Initialize(Array<PackedScene> enemyScenes)
	{
		if (_isInitialized)
		{
			GD.Print("⚠️ EnemyFactory ya inicializado");
			return;
		}
		
		_factoryManager = new EnemyFactoryManager();
		
		// Registrar factories basándose en las escenas
		foreach (var scene in enemyScenes)
		{
			if (scene == null) continue;
			
			// Determinar tipo de enemigo por el path de la escena
			var scenePath = scene.ResourcePath.ToLower();
			
			if (scenePath.Contains("shooter"))
			{
				_factoryManager.RegisterFactory(new ShooterEnemyFactory(scene));
			}
			else if (scenePath.Contains("meteoro"))
			{
				_factoryManager.RegisterFactory(new MeteoroEnemyFactory(scene));
			}
			else
			{
				_factoryManager.RegisterFactory(new BasicEnemyFactory(scene));
			}
		}
		
		_isInitialized = true;
		GD.Print($"✅ EnemyFactory inicializado con {enemyScenes.Count} tipos de enemigos");
	}
	
	/// <summary>
	/// Crea un enemigo desde una escena (método legacy)
	/// </summary>
	[System.Obsolete("Usar EnemyFactoryManager.CreateEnemyByType() en su lugar")]
	public static Enemy CreateEnemy(PackedScene enemyScene, Vector2 position)
	{
		if (enemyScene?.Instantiate() is Enemy enemy)
		{
			enemy.GlobalPosition = position;
			enemy.Initialize();
			return enemy;
		}
		
		GD.PrintErr("Failed to create enemy from scene");
		return null;
	}
	
	/// <summary>
	/// Crea un enemigo aleatorio (método actualizado que usa el nuevo sistema)
	/// </summary>
	public static Enemy CreateRandomEnemy(Array<PackedScene> enemyScenes, Vector2 position, RandomNumberGenerator rng)
	{
		// Si no está inicializado, usar el método legacy
		if (!_isInitialized && enemyScenes != null && enemyScenes.Count > 0)
		{
			Initialize(enemyScenes);
		}
		
		if (!_isInitialized)
		{
			GD.PrintErr("❌ EnemyFactory no inicializado");
			return null;
		}
		
		// Usar el nuevo sistema de factories
		return _factoryManager.CreateRandomEnemy(position);
	}
	
	/// <summary>
	/// Obtiene el manager de factories (para uso avanzado)
	/// </summary>
	public static EnemyFactoryManager GetFactoryManager()
	{
		if (!_isInitialized)
		{
			GD.PrintErr("❌ EnemyFactory no inicializado. Llama a Initialize() primero.");
			return null;
		}
		
		return _factoryManager;
	}
	
	/// <summary>
	/// Reinicia el factory (útil para testing)
	/// </summary>
	public static void Reset()
	{
		_factoryManager?.ClearFactories();
		_factoryManager = null;
		_isInitialized = false;
		GD.Print("🔄 EnemyFactory reiniciado");
	}
}
