using System;
using System.Security.Cryptography;
using System.Text;


public static class PasswordService
{
  
	public static string HashPassword(string password, string salt = "VerticalShooter2025")
	{
		if (string.IsNullOrEmpty(password))
			return "";

		var saltedPassword = password + salt;

		using (var sha256 = SHA256.Create())
		{
			var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(saltedPassword));
			return Convert.ToBase64String(hashedBytes);
		}
	}

	/// <summary>
	/// Verifica si una contraseña coincide con el hash almacenado
	/// </summary>
	public static bool VerifyPassword(string password, string storedHash, string salt = "VerticalShooter2025")
	{
		if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(storedHash))
			return false;

		var hashedPassword = HashPassword(password, salt);
		return hashedPassword == storedHash;
	}

	/// <summary>
	/// Valida que la contraseña cumpla con los requisitos mínimos
	/// </summary>
	public static bool IsValidPassword(string password)
	{
		if (string.IsNullOrEmpty(password))
			return false;

		// Mínimo 4 caracteres para simplicidad
		return password.Length >= 4;
	}

	/// <summary>
	/// Valida que el nombre de usuario sea válido
	/// </summary>
	public static bool IsValidUsername(string username)
	{
		if (string.IsNullOrEmpty(username))
			return false;

		// Entre 3 y 20 caracteres, solo letras, números y guiones bajos
		if (username.Length < 3 || username.Length > 20)
			return false;

		foreach (char c in username)
		{
			if (!char.IsLetterOrDigit(c) && c != '_')
				return false;
		}

		return true;
	}
}
