using Godot;
using System;

/// <summary>
/// Servicio de autenticación - Solo maneja login, registro y cambio de contraseña
/// Principio SOLID: SRP (Single Responsibility - solo autenticación)
/// Principio SOLID: DIP (Dependency Inversion - usa ServiceLocator para obtener dependencias)
/// </summary>
public static class AuthService
{
	private static IUserLockService _lockService;
	private static IUserService _userService;
	private static IUserScoreService _scoreService;
	private static bool _initialized = false;

	/// <summary>
	/// Usuario actualmente autenticado (delegado a SessionService)
	/// </summary>
	public static User CurrentUser => SessionService.CurrentUser;

	/// <summary>
	/// Indica si hay un usuario autenticado (delegado a SessionService)
	/// </summary>
	public static bool IsLoggedIn => SessionService.IsLoggedIn;

	/// <summary>
	/// Inicializa AuthService y sus servicios dependientes mediante ServiceLocator
	/// </summary>
	public static void Initialize()
	{
		if (_initialized)
			return;

		// Asegurar que ServiceLocator esté inicializado
		if (!ServiceLocator.IsRegistered<IUserDatabaseService>())
		{
			ServiceLocator.Initialize();
		}

		// Obtener servicios desde ServiceLocator (Inyección de Dependencias)
		_lockService = ServiceLocator.Get<IUserLockService>();
		_userService = ServiceLocator.Get<IUserService>();
		_scoreService = ServiceLocator.Get<IUserScoreService>();
		
		_initialized = true;
		
		GD.Print("🔧 AuthService inicializado");
		GD.Print($"📊 Total de usuarios en base de datos: {_userService.GetUserCount()}");
	}

	/// <summary>
	/// Intenta hacer login con las credenciales proporcionadas
	/// Valida si el usuario está bloqueado por intentos fallidos
	/// </summary>
	public static AuthResult Login(string username, string password)
	{
		EnsureInitialized();
		
		if (!PasswordService.IsValidUsername(username))
		{
			return new AuthResult(false, "Nombre de usuario inválido. Debe tener entre 3-20 caracteres alfanuméricos.");
		}

		// Verificar si el usuario está bloqueado
		if (_lockService.IsUserLocked(username))
		{
			return new AuthResult(false, "❌ Usuario bloqueado. Demasiados intentos fallidos.");
		}

		if (!PasswordService.IsValidPassword(password))
		{
			return new AuthResult(false, "Contraseña inválida. Debe tener al menos 4 caracteres.");
		}

		// Buscar usuario en base de datos
		var storedUser = _userService.GetUser(username);

		// Si no existe usuario, crear uno nuevo (auto-registro)
		if (storedUser == null)
		{
			return CreateNewUserAndLogin(username, password);
		}

		// Verificar credenciales
		if (!PasswordService.VerifyPassword(password, storedUser.PasswordHash))
		{
			_lockService.RecordFailedAttempt(username);
			int attempts = _lockService.GetFailedAttempts(username);
			int remaining = _lockService.GetMaxFailedAttempts() - attempts;
			return new AuthResult(false, $"Contraseña incorrecta. ({remaining} intentos restantes)");
		}

		// Login exitoso - limpiar intentos fallidos
		_lockService.ClearFailedAttempts(username);
		storedUser.UpdateLastLogin();
		_userService.UpdateUser(storedUser);
		
		// Iniciar sesión
		SessionService.StartSession(storedUser);

		return new AuthResult(true, "Login exitoso", storedUser);
	}

	/// <summary>
	/// Registra (crea) un nuevo usuario en la base de datos
	/// </summary>
	public static AuthResult Register(string username, string password)
	{
		EnsureInitialized();
		
		if (!PasswordService.IsValidUsername(username))
		{
			return new AuthResult(false, "Nombre de usuario inválido. Debe tener entre 3-20 caracteres alfanuméricos.");
		}

		if (!PasswordService.IsValidPassword(password))
		{
			return new AuthResult(false, "Contraseña inválida. Debe tener al menos 4 caracteres.");
		}

		// Verificar si el usuario ya existe
		if (_userService.UserExists(username))
		{
			return new AuthResult(false, $"El usuario '{username}' ya existe. Usa otro nombre.");
		}

		var result = CreateNewUserAndLogin(username, password);
		if (result.Success)
		{
			GD.Print($"🆕 Usuario registrado: {username}");
		}
		return result;
	}

	/// <summary>
	/// Cambia la contraseña del usuario actual
	/// </summary>
	public static AuthResult ChangePassword(string currentPassword, string newPassword)
	{
		EnsureInitialized();
		
		if (!SessionService.IsLoggedIn)
		{
			return new AuthResult(false, "No hay usuario autenticado.");
		}

		var currentUser = SessionService.CurrentUser;

		// Verificar contraseña actual
		if (!PasswordService.VerifyPassword(currentPassword, currentUser.PasswordHash))
		{
			return new AuthResult(false, "Contraseña actual incorrecta.");
		}

		// Validar nueva contraseña
		if (!PasswordService.IsValidPassword(newPassword))
		{
			return new AuthResult(false, "La nueva contraseña debe tener al menos 4 caracteres.");
		}

		var newPasswordHash = PasswordService.HashPassword(newPassword);

		// Verificar que no sea la misma contraseña
		if (currentUser.IsPasswordReused(newPasswordHash))
		{
			return new AuthResult(false, "No puedes usar una contraseña que ya has utilizado anteriormente.");
		}

		// Actualizar contraseña
		currentUser.UpdatePassword(newPasswordHash);
		_userService.UpdateUser(currentUser);
		SessionService.RefreshUser(currentUser);

		GD.Print($"🔐 Contraseña cambiada exitosamente para: {currentUser.Username}");
		return new AuthResult(true, "Contraseña cambiada exitosamente");
	}

	/// <summary>
	/// Cierra sesión del usuario actual
	/// </summary>
	public static void Logout()
	{
		SessionService.EndSession();
	}

	// ==================== DELEGACIÓN A SERVICIOS ESPECIALIZADOS ====================
	// Los siguientes métodos delegan a los servicios apropiados para mantener compatibilidad

	/// <summary>
	/// Actualiza el puntaje del usuario actual (delegado a UserScoreService)
	/// </summary>
	public static bool UpdateScore(uint newScore)
	{
		if (!SessionService.IsLoggedIn)
			return false;

		return _scoreService.UpdateScore(SessionService.CurrentUser, newScore);
	}

	/// <summary>
	/// Obtiene el puntaje máximo del usuario actual (delegado a UserScoreService)
	/// </summary>
	public static uint GetHighScore()
	{
		return SessionService.IsLoggedIn ? SessionService.CurrentUser.HighScore : 0;
	}

	/// <summary>
	/// Carga los datos de un usuario específico (delegado a UserService)
	/// </summary>
	public static User LoadUserData(string username)
	{
		EnsureInitialized();
		return _userService.GetUser(username);
	}

	/// <summary>
	/// Obtiene el servicio de bloqueos de usuarios
	/// </summary>
	public static IUserLockService GetLockService()
	{
		EnsureInitialized();
		return _lockService;
	}

	/// <summary>
	/// Desbloquea un usuario (delegado a UserService)
	/// </summary>
	public static bool UnlockUser(string username)
	{
		if (!SessionService.IsLoggedIn)
		{
			GD.Print("⚠️ Debe iniciar sesión para desbloquear usuarios");
			return false;
		}

		_userService.UnlockUser(username);
		return true;
	}

	/// <summary>
	/// Obtiene la lista de usuarios bloqueados (delegado a UserService)
	/// </summary>
	public static System.Collections.Generic.List<string> GetLockedUsers()
	{
		EnsureInitialized();
		return _userService.GetLockedUsers();
	}

	/// <summary>
	/// Obtiene todos los usuarios del sistema (delegado a UserService)
	/// </summary>
	public static System.Collections.Generic.List<User> GetAllUsers()
	{
		EnsureInitialized();
		return _userService.GetAllUsers();
	}

	/// <summary>
	/// Obtiene todos los nombres de usuarios del sistema (delegado a UserService)
	/// </summary>
	public static System.Collections.Generic.List<string> GetAllUsernames()
	{
		EnsureInitialized();
		return _userService.GetAllUsernames();
	}

	// ==================== MÉTODOS PRIVADOS ====================

	/// <summary>
	/// Crea un nuevo usuario y lo autentica automáticamente
	/// </summary>
	private static AuthResult CreateNewUserAndLogin(string username, string password)
	{
		var passwordHash = PasswordService.HashPassword(password);
		var newUser = _userService.CreateUser(username, passwordHash);

		// Iniciar sesión con el nuevo usuario
		SessionService.StartSession(newUser);

		return new AuthResult(true, "Usuario creado exitosamente", newUser);
	}

	/// <summary>
	/// Asegura que el servicio esté inicializado
	/// </summary>
	private static void EnsureInitialized()
	{
		if (!_initialized)
		{
			Initialize();
		}
	}
}

/// <summary>
/// Resultado de operaciones de autenticación
/// </summary>
public class AuthResult
{
	public bool Success { get; }
	public string Message { get; }
	public User User { get; }

	public AuthResult(bool success, string message, User user = null)
	{
		Success = success;
		Message = message;
		User = user;
	}
}
