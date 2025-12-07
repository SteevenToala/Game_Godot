using Godot;

/// <summary>
/// Servicio para gestionar la sesión del usuario actual
/// Principio SOLID: SRP (Single Responsibility - solo gestiona sesión)
/// </summary>
public static class SessionService
{
	private static User _currentUser = null;

	/// <summary>
	/// Usuario actualmente autenticado
	/// </summary>
	public static User CurrentUser => _currentUser;

	/// <summary>
	/// Indica si hay un usuario autenticado
	/// </summary>
	public static bool IsLoggedIn => _currentUser != null;

	/// <summary>
	/// Inicia sesión para un usuario
	/// </summary>
	public static void StartSession(User user)
	{
		if (user == null)
		{
			GD.PrintErr("❌ No se puede iniciar sesión con usuario nulo");
			return;
		}

		_currentUser = user;
		GD.Print($"✅ Sesión iniciada: {_currentUser.Username}");
	}

	/// <summary>
	/// Cierra la sesión actual
	/// </summary>
	public static void EndSession()
	{
		if (_currentUser != null)
		{
			GD.Print($"👋 Sesión cerrada: {_currentUser.Username}");
			_currentUser = null;
		}
	}

	/// <summary>
	/// Actualiza el usuario en sesión (después de cambios)
	/// </summary>
	public static void RefreshUser(User updatedUser)
	{
		if (_currentUser != null && updatedUser != null && _currentUser.Username == updatedUser.Username)
		{
			_currentUser = updatedUser;
		}
	}

	/// <summary>
	/// Obtiene el nombre del usuario actual
	/// </summary>
	public static string GetCurrentUsername()
	{
		return _currentUser?.Username ?? "";
	}
}
