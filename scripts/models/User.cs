using Godot;
using System;
using System.Collections.Generic;

[System.Serializable]
public class User
{
    public string Username { get; set; } = "";
    public string PasswordHash { get; set; } = "";
    public uint HighScore { get; set; } = 0;
    public List<string> PreviousPasswordHashes { get; set; } = new List<string>();
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime LastLogin { get; set; } = DateTime.Now;

    public User() { }

    public User(string username, string passwordHash)
    {
        Username = username;
        PasswordHash = passwordHash;
        CreatedAt = DateTime.Now;
        LastLogin = DateTime.Now;
    }

    /// <summary>
    /// Valida si una contraseña no ha sido usada anteriormente
    /// </summary>
    public bool IsPasswordReused(string newPasswordHash)
    {
        // Verificar si es la misma contraseña actual
        if (PasswordHash == newPasswordHash)
            return true;

        // Verificar si está en el historial de contraseñas anteriores
        return PreviousPasswordHashes.Contains(newPasswordHash);
    }

    /// <summary>
    /// Actualiza la contraseña y guarda la anterior en el historial
    /// </summary>
    public void UpdatePassword(string newPasswordHash)
    {
        // Agregar la contraseña actual al historial si no está vacía
        if (!string.IsNullOrEmpty(PasswordHash))
        {
            PreviousPasswordHashes.Add(PasswordHash);

            // Mantener solo las últimas 5 contraseñas en el historial
            if (PreviousPasswordHashes.Count > 5)
            {
                PreviousPasswordHashes.RemoveAt(0);
            }
        }

        PasswordHash = newPasswordHash;
    }

    /// <summary>
    /// Actualiza el puntaje si es mayor al actual
    /// </summary>
    public bool UpdateHighScore(uint newScore)
    {
        if (newScore > HighScore)
        {
            HighScore = newScore;
            return true;
        }
        return false;
    }

    public void UpdateLastLogin()
    {
        LastLogin = DateTime.Now;
    }

    public override string ToString()
    {
        return $"User: {Username}, HighScore: {HighScore}, LastLogin: {LastLogin:yyyy-MM-dd HH:mm}";
    }
}
