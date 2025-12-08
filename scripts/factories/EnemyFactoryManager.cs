using Godot;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Manager para coordinar múltiples enemy factories
/// Patrón: Abstract Factory + Strategy - Selecciona la factory apropiada según el contexto
/// Principio SOLID: OCP - Abierto a extensión (nuevas factories) cerrado a modificación
/// Principio SOLID: DIP - Depende de IEnemyFactory (abstracción)
/// </summary>
public class EnemyFactoryManager
{
	private readonly List<IEnemyFactory> _factories;
	private readonly RandomNumberGenerator _rng;
	private readonly Dictionary<string, IEnemyFactory> _factoryByType;
	
	public EnemyFactoryManager()
	{
		_factories = new List<IEnemyFactory>();
		_factoryByType = new Dictionary<string, IEnemyFactory>();
		_rng = new RandomNumberGenerator();
		_rng.Randomize();
	}
	
	/// <summary>
	/// Registra una factory en el manager
	/// </summary>
	public void RegisterFactory(IEnemyFactory factory)
	{
		if (factory == null)
		{
			GD.PrintErr("⚠️ Intentando registrar factory null");
			return;
		}
		
		_factories.Add(factory);
		_factoryByType[factory.GetEnemyType()] = factory;
		GD.Print($"✅ Factory registrada: {factory.GetEnemyType()} (Dificultad: {factory.GetDifficultyLevel()})");
	}
	
	/// <summary>
	/// Crea un enemigo aleatorio de cualquier tipo registrado
	/// </summary>
	public Enemy CreateRandomEnemy(Vector2 position)
	{
		if (_factories.Count == 0)
		{
			GD.PrintErr("❌ No hay factories registradas");
			return null;
		}
		
		var randomIndex = _rng.RandiRange(0, _factories.Count - 1);
		var factory = _factories[randomIndex];
		
		return factory.CreateEnemy(position);
	}
	
	/// <summary>
	/// Crea un enemigo aleatorio ponderado por dificultad
	/// </summary>
	public Enemy CreateWeightedRandomEnemy(Vector2 position, int maxDifficulty = 3)
	{
		// Filtrar factories por nivel de dificultad
		var availableFactories = _factories
			.Where(f => f.GetDifficultyLevel() <= maxDifficulty)
			.ToList();
		
		if (availableFactories.Count == 0)
		{
			GD.PrintErr($"❌ No hay factories disponibles con dificultad <= {maxDifficulty}");
			return null;
		}
		
		var randomIndex = _rng.RandiRange(0, availableFactories.Count - 1);
		var factory = availableFactories[randomIndex];
		
		return factory.CreateEnemy(position);
	}
	
	/// <summary>
	/// Crea un enemigo de un tipo específico
	/// </summary>
	public Enemy CreateEnemyByType(string enemyType, Vector2 position)
	{
		if (!_factoryByType.ContainsKey(enemyType))
		{
			GD.PrintErr($"❌ Factory no encontrada para tipo: {enemyType}");
			return null;
		}
		
		return _factoryByType[enemyType].CreateEnemy(position);
	}
	
	/// <summary>
	/// Obtiene todas las factories registradas
	/// </summary>
	public IReadOnlyList<IEnemyFactory> GetAllFactories()
	{
		return _factories.AsReadOnly();
	}
	
	/// <summary>
	/// Obtiene los tipos de enemigos disponibles
	/// </summary>
	public List<string> GetAvailableEnemyTypes()
	{
		return _factories.Select(f => f.GetEnemyType()).ToList();
	}
	
	/// <summary>
	/// Limpia todas las factories registradas
	/// </summary>
	public void ClearFactories()
	{
		_factories.Clear();
		_factoryByType.Clear();
		GD.Print("🧹 Factories limpiadas");
	}
}
