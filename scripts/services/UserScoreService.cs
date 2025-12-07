using Godot;

/// <summary>
/// Servicio para gestionar puntajes de usuarios
/// Principio SOLID: SRP (Single Responsibility - solo gestiona puntajes)
/// </summary>
public class UserScoreService
{
	private readonly UserDatabaseService _userDatabase;

	public UserScoreService(UserDatabaseService userDatabase)
	{
		_userDatabase = userDatabase;
	}

	/// <summary>
	/// Actualiza el puntaje del usuario si es un nuevo récord
	/// </summary>
	/// <returns>True si es un nuevo récord, false si no</returns>
	public bool UpdateScore(User user, uint newScore)
	{
		if (user == null)
			return false;

		bool isNewRecord = user.UpdateHighScore(newScore);
		
		if (isNewRecord)
		{
			_userDatabase.UpdateUser(user);
			GD.Print($"🏆 Nuevo récord para {user.Username}: {newScore}");
		}

		return isNewRecord;
	}

	/// <summary>
	/// Obtiene el puntaje más alto de un usuario
	/// </summary>
	public uint GetHighScore(User user)
	{
		return user?.HighScore ?? 0;
	}
}
