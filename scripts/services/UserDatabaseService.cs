using Godot;
using System;
using System.Collections.Generic;

/// <summary>
/// Servicio para gestionar múltiples usuarios del sistema
/// Almacena información de todos los usuarios creados
/// Principio SOLID: SRP (Single Responsibility - gestiona BD de usuarios)
/// Principio SOLID: DIP (Dependency Inversion Principle) - Implementa interfaz
/// </summary>
public class UserDatabaseService : IUserDatabaseService
{
	private const string UsersDatabaseFile = "user://users_database.json";
	private Dictionary<string, User> _users = new Dictionary<string, User>();

	public UserDatabaseService()
	{
		LoadAllUsers();
		EnsureAdminExists();
	}

	/// <summary>
	/// Verifica si existe un usuario
	/// </summary>
	public bool UserExists(string username)
	{
		return _users.ContainsKey(username);
	}

	/// <summary>
	/// Obtiene un usuario por nombre
	/// </summary>
	public User GetUser(string username)
	{
		if (_users.ContainsKey(username))
		{
			return _users[username];
		}
		return null;
	}

	/// <summary>
	/// Crea/Registra un nuevo usuario
	/// </summary>
	public bool AddUser(User user)
	{
		if (user == null || string.IsNullOrEmpty(user.Username))
			return false;

		_users[user.Username] = user;
		SaveAllUsers();
		GD.Print($"✅ Usuario '{user.Username}' agregado a la base de datos");
		return true;
	}

	/// <summary>
	/// Actualiza datos de un usuario existente
	/// </summary>
	public bool UpdateUser(User user)
	{
		if (!_users.ContainsKey(user.Username))
			return false;

		_users[user.Username] = user;
		SaveAllUsers();
		return true;
	}

	/// <summary>
	/// Obtiene todos los usuarios
	/// </summary>
	public List<User> GetAllUsers()
	{
		return new List<User>(_users.Values);
	}

	/// <summary>
	/// Obtiene lista de nombres de usuarios
	/// </summary>
	public List<string> GetAllUsernames()
	{
		return new List<string>(_users.Keys);
	}

	/// <summary>
	/// Obtiene los 3 mejores usuarios por high score
	/// </summary>
	public List<User> GetTopScores(int count = 3)
	{
		var sortedUsers = new List<User>(_users.Values);
		sortedUsers.Sort((a, b) => b.HighScore.CompareTo(a.HighScore));
		
		// Retornar solo los primeros 'count' usuarios
		return sortedUsers.Count > count ? sortedUsers.GetRange(0, count) : sortedUsers;
	}

	/// <summary>
	/// Asegura que existe el usuario admin por defecto
	/// </summary>
	private void EnsureAdminExists()
	{
		if (!_users.ContainsKey("admin"))
		{
			var adminUser = new User("admin", PasswordService.HashPassword("admin"));
			_users["admin"] = adminUser;
			SaveAllUsers();
			GD.Print("✅ Usuario admin creado automáticamente");
		}
	}

	/// <summary>
	/// Guarda todos los usuarios en archivo JSON
	/// </summary>
	private void SaveAllUsers()
	{
		try
		{
			using var file = FileAccess.Open(UsersDatabaseFile, FileAccess.ModeFlags.Write);
			if (file == null)
			{
				GD.PrintErr($"Error al crear archivo de BD de usuarios: {FileAccess.GetOpenError()}");
				return;
			}

			var usersArray = new Godot.Collections.Array();
			foreach (var kvp in _users)
			{
				var userDict = SerializeUser(kvp.Value);
				usersArray.Add(userDict);
			}

			var json = Json.Stringify(usersArray);
			file.StoreString(json);
			file.Flush();
		}
		catch (Exception e)
		{
			GD.PrintErr($"Error guardando BD de usuarios: {e.Message}");
		}
	}

	/// <summary>
	/// Carga todos los usuarios desde archivo JSON
	/// </summary>
	private void LoadAllUsers()
	{
		try
		{
			if (!FileAccess.FileExists(UsersDatabaseFile))
			{
				GD.Print("📁 No existe base de datos de usuarios, se creará una nueva");
				return;
			}

			using var file = FileAccess.Open(UsersDatabaseFile, FileAccess.ModeFlags.Read);
			if (file == null)
			{
				return;
			}

			var jsonString = file.GetAsText();
			if (string.IsNullOrEmpty(jsonString))
			{
				return;
			}

			var json = Json.ParseString(jsonString);
			if (json.VariantType != Variant.Type.Array)
			{
				return;
			}

			var usersArray = json.AsGodotArray();
			_users.Clear();

			foreach (var item in usersArray)
			{
				var userDict = item.AsGodotDictionary();
				var user = DeserializeUser(userDict);
				if (user != null)
				{
					_users[user.Username] = user;
				}
			}

			GD.Print($"✅ Base de datos de usuarios cargada: {_users.Count} usuarios");
		}
		catch (Exception e)
		{
			GD.PrintErr($"Error cargando BD de usuarios: {e.Message}");
		}
	}

	/// <summary>
	/// Serializa un usuario a diccionario
	/// </summary>
	private Godot.Collections.Dictionary SerializeUser(User user)
	{
		var dict = new Godot.Collections.Dictionary();
		dict["username"] = user.Username;
		dict["passwordHash"] = user.PasswordHash;
		dict["highScore"] = user.HighScore;

		var hashArray = new Godot.Collections.Array();
		foreach (var hash in user.PreviousPasswordHashes)
		{
			hashArray.Add(hash);
		}
		dict["previousPasswordHashes"] = hashArray;

		dict["createdAt"] = user.CreatedAt.ToString("O");
		dict["lastLogin"] = user.LastLogin.ToString("O");
		return dict;
	}

	/// <summary>
	/// Deserializa un usuario desde diccionario
	/// </summary>
	private User DeserializeUser(Godot.Collections.Dictionary dict)
	{
		var user = new User();

		if (dict.ContainsKey("username"))
			user.Username = dict["username"].AsString();

		if (dict.ContainsKey("passwordHash"))
			user.PasswordHash = dict["passwordHash"].AsString();

		if (dict.ContainsKey("highScore"))
			user.HighScore = dict["highScore"].AsUInt32();

		if (dict.ContainsKey("previousPasswordHashes"))
		{
			var hashArray = dict["previousPasswordHashes"].AsGodotArray();
			foreach (var hash in hashArray)
			{
				user.PreviousPasswordHashes.Add(hash.AsString());
			}
		}

		if (dict.ContainsKey("createdAt"))
		{
			if (DateTime.TryParse(dict["createdAt"].AsString(), out var createdAt))
				user.CreatedAt = createdAt;
		}

		if (dict.ContainsKey("lastLogin"))
		{
			if (DateTime.TryParse(dict["lastLogin"].AsString(), out var lastLogin))
				user.LastLogin = lastLogin;
		}

		return user;
	}
}
