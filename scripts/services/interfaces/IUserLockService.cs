/// <summary>
/// Interface para gestionar el bloqueo de usuarios
/// Permite registrar intentos fallidos y gestionar el bloqueo de cuentas
/// Principio SOLID: SRP y DIP
/// </summary>
public interface IUserLockService
{
	/// <summary>
	/// Registra un intento fallido de login
	/// </summary>
	void RecordFailedAttempt(string username);

	/// <summary>
	/// Obtiene el número de intentos fallidos para un usuario
	/// </summary>
	int GetFailedAttempts(string username);

	/// <summary>
	/// Verifica si un usuario está bloqueado
	/// </summary>
	bool IsUserLocked(string username);

	/// <summary>
	/// Desbloquea un usuario
	/// </summary>
	void UnlockUser(string username);

	/// <summary>
	/// Limpia los intentos fallidos cuando el login es exitoso
	/// </summary>
	void ClearFailedAttempts(string username);

	/// <summary>
	/// Obtiene la cantidad máxima de intentos permitidos antes de bloqueo
	/// </summary>
	int GetMaxFailedAttempts();

	/// <summary>
	/// Obtiene la lista de todos los usuarios bloqueados
	/// </summary>
	System.Collections.Generic.List<string> GetLockedUsers();

	/// <summary>
	/// Verifica si un usuario está bloqueado permanentemente (segunda vez)
	/// </summary>
	bool IsPermanentlyLocked(string username);
}
