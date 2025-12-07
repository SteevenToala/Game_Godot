using System.Collections.Generic;

/// <summary>
/// Interface para el servicio de gestión de usuarios
/// Principio SOLID: DIP (Dependency Inversion Principle)
/// </summary>
public interface IUserService
{
	/// <summary>
	/// Obtiene un usuario por su nombre
	/// </summary>
	User GetUser(string username);

	/// <summary>
	/// Verifica si un usuario existe
	/// </summary>
	bool UserExists(string username);

	/// <summary>
	/// Crea un nuevo usuario
	/// </summary>
	User CreateUser(string username, string passwordHash);

	/// <summary>
	/// Actualiza los datos de un usuario
	/// </summary>
	void UpdateUser(User user);

	/// <summary>
	/// Obtiene todos los usuarios del sistema
	/// </summary>
	List<User> GetAllUsers();

	/// <summary>
	/// Obtiene todos los nombres de usuario
	/// </summary>
	List<string> GetAllUsernames();

	/// <summary>
	/// Desbloquea un usuario
	/// </summary>
	void UnlockUser(string username);

	/// <summary>
	/// Obtiene la lista de usuarios bloqueados
	/// </summary>
	List<string> GetLockedUsers();

	/// <summary>
	/// Obtiene el número total de usuarios
	/// </summary>
	int GetUserCount();
}
