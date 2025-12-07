/// <summary>
/// Interface para el servicio de gestión de puntajes
/// Principio SOLID: DIP (Dependency Inversion Principle)
/// </summary>
public interface IUserScoreService
{
	/// <summary>
	/// Actualiza el puntaje de un usuario
	/// </summary>
	/// <returns>True si es un nuevo récord</returns>
	bool UpdateScore(User user, uint newScore);

	/// <summary>
	/// Obtiene el puntaje más alto de un usuario
	/// </summary>
	uint GetHighScore(User user);
}
