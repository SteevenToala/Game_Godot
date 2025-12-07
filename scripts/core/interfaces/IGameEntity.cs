using Godot;

/// <summary>
/// Interfaz base para todas las entidades del juego (Player, Enemy, etc.)
/// Establece un contrato común para cumplir con el Principio de Sustitución de Liskov (LSP)
/// </summary>
public interface IGameEntity : IDamageable, IInitializable
{
	/// <summary>
	/// Componente de salud de la entidad
	/// </summary>
	Health HealthComponent { get; }
	
	/// <summary>
	/// Componente de movimiento de la entidad
	/// </summary>
	Movement MovementComponent { get; }
	
	/// <summary>
	/// Indica si la entidad está activa en el juego
	/// </summary>
	bool IsActive { get; }
	
	/// <summary>
	/// Destruye la entidad del juego
	/// </summary>
	void Destroy();
	
	/// <summary>
	/// Maneja la muerte de la entidad
	/// </summary>
	void OnDied();
}
