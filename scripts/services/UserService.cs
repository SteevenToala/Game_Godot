using Godot;
using System.Collections.Generic;

/// <summary>
/// Servicio para operaciones CRUD de usuarios
/// Principio SOLID: SRP (Single Responsibility - solo gestiona datos de usuarios)
/// Principio SOLID: DIP (Dependency Inversion - depende de interfaces)
/// </summary>
public class UserService : IUserService
{
	private readonly IUserDatabaseService _userDatabase;
	private readonly IUserLockService _lockService;

	public UserService(IUserDatabaseService userDatabase, IUserLockService lockService)
	{
		_userDatabase = userDatabase;
		_lockService = lockService;
	}

	/// <summary>
	/// Carga los datos de un usuario específico
	/// </summary>
	public User GetUser(string username)
	{
		return _userDatabase.GetUser(username);
	}

	/// <summary>
	/// Verifica si un usuario existe en la base de datos
	/// </summary>
	public bool UserExists(string username)
	{
		return _userDatabase.UserExists(username);
	}

	/// <summary>
	/// Crea un nuevo usuario en la base de datos
	/// </summary>
	public User CreateUser(string username, string passwordHash)
	{
		var newUser = new User(username, passwordHash);
		_userDatabase.AddUser(newUser);
		GD.Print($"🆕 Usuario creado en base de datos: {username}");
		return newUser;
	}

	/// <summary>
	/// Actualiza los datos de un usuario en la base de datos
	/// </summary>
	public void UpdateUser(User user)
	{
		_userDatabase.UpdateUser(user);
	}

	/// <summary>
	/// Obtiene todos los usuarios del sistema
	/// </summary>
	public List<User> GetAllUsers()
	{
		return _userDatabase.GetAllUsers();
	}

	/// <summary>
	/// Obtiene todos los nombres de usuario del sistema
	/// </summary>
	public List<string> GetAllUsernames()
	{
		return _userDatabase.GetAllUsernames();
	}

	/// <summary>
	/// Desbloquea un usuario bloqueado
	/// </summary>
	public void UnlockUser(string username)
	{
		_lockService.UnlockUser(username);
		GD.Print($"🔓 Usuario desbloqueado: {username}");
	}

	/// <summary>
	/// Obtiene la lista de usuarios bloqueados
	/// </summary>
	public List<string> GetLockedUsers()
	{
		return _lockService.GetLockedUsers();
	}

	/// <summary>
	/// Obtiene el número total de usuarios en el sistema
	/// </summary>
	public int GetUserCount()
	{
		return _userDatabase.GetAllUsernames().Count;
	}
}
