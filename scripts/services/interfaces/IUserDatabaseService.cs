using System.Collections.Generic;

/// <summary>
/// Interface para el servicio de base de datos de usuarios
/// Principio SOLID: DIP (Dependency Inversion Principle)
/// </summary>
public interface IUserDatabaseService
{
	/// <summary>
	/// Verifica si un usuario existe en la base de datos
	/// </summary>
	bool UserExists(string username);

	/// <summary>
	/// Obtiene un usuario por su nombre
	/// </summary>
	User GetUser(string username);

	/// <summary>
	/// Agrega un nuevo usuario a la base de datos
	/// </summary>
	bool AddUser(User user);

	/// <summary>
	/// Actualiza los datos de un usuario existente
	/// </summary>
	bool UpdateUser(User user);

	/// <summary>
	/// Obtiene todos los usuarios del sistema
	/// </summary>
	List<User> GetAllUsers();

	/// <summary>
	/// Obtiene todos los nombres de usuario del sistema
	/// </summary>
	List<string> GetAllUsernames();
}
