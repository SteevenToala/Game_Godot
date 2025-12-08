using Godot;

/// <summary>
/// Interfaz para el patrón Abstract Factory
/// Principio SOLID: DIP - Define abstracción para creación de enemigos
/// Patrón: Abstract Factory - Permite crear familias de enemigos relacionados
/// </summary>
public interface IEnemyFactory
{
	/// <summary>
	/// Crea un enemigo en la posición especificada
	/// </summary>
	Enemy CreateEnemy(Vector2 position);
	
	/// <summary>
	/// Obtiene el nombre del tipo de enemigo que crea esta factory
	/// </summary>
	string GetEnemyType();
	
	/// <summary>
	/// Obtiene la dificultad base del enemigo (para balanceo)
	/// </summary>
	int GetDifficultyLevel();
}
