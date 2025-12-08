using System.Collections.Generic;

/// <summary>
/// Interfaz para el servicio de base de datos de usuarios
/// Principio SOLID: ISP (Interface Segregation Principle) - Interfaz específica y cohesiva
/// Principio SOLID: DIP (Dependency Inversion Principle) - Abstracción para depender
/// </summary>
public interface IUserDatabaseService
{
	/// <summary>
	/// Verifica si existe un usuario
	/// </summary>
	bool UserExists(string username);
	
	/// <summary>
	/// Obtiene un usuario por nombre
	/// </summary>
	User GetUser(string username);
	
	/// <summary>
	/// Crea/Registra un nuevo usuario
	/// </summary>
	bool AddUser(User user);
	
	/// <summary>
	/// Actualiza datos de un usuario existente
	/// </summary>
	bool UpdateUser(User user);
	
	/// <summary>
	/// Obtiene todos los usuarios
	/// </summary>
	List<User> GetAllUsers();
	
	/// <summary>
	/// Obtiene lista de nombres de usuarios
	/// </summary>
	List<string> GetAllUsernames();
	
	/// <summary>
	/// Obtiene los mejores usuarios por high score
	/// </summary>
	List<User> GetTopScores(int count = 3);
}
