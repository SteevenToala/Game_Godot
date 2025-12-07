using Godot;
using System;
using System.Collections.Generic;

/// <summary>
/// Servicio para gestionar el bloqueo de usuarios tras múltiples intentos fallidos
/// Implementa IUserLockService para permitir inyección de dependencias
/// Principio SOLID: SRP (Single Responsibility - gestiona solo bloqueos)
/// </summary>
public class UserLockService : IUserLockService
{
    private const int MAX_FAILED_ATTEMPTS = 3;
    private const string LockDataFile = "user://user_locks.json";
    private Dictionary<string, UserLockData> _lockedUsers = new Dictionary<string, UserLockData>();

    [System.Serializable]
    private class UserLockData
    {
        public string Username { get; set; }
        public int FailedAttempts { get; set; }
        public DateTime LockedAt { get; set; }
        public bool IsLocked { get; set; }
        public int TimesLocked { get; set; } // Contador de veces que ha sido bloqueado
        public bool IsPermanentlyLocked { get; set; } // Bloqueo permanente después de la segunda vez

        public UserLockData() { }

        public UserLockData(string username, int failedAttempts = 0)
        {
            Username = username;
            FailedAttempts = failedAttempts;
            LockedAt = DateTime.Now;
            IsLocked = failedAttempts >= MAX_FAILED_ATTEMPTS;
            TimesLocked = 0;
            IsPermanentlyLocked = false;
        }
    }

    public UserLockService()
    {
        LoadLockData();
    }

    public void RecordFailedAttempt(string username)
    {
        if (!_lockedUsers.ContainsKey(username))
        {
            _lockedUsers[username] = new UserLockData(username, 1);
        }
        else
        {
            _lockedUsers[username].FailedAttempts++;
        }

        // Bloquear si se alcanza el máximo
        if (_lockedUsers[username].FailedAttempts >= MAX_FAILED_ATTEMPTS)
        {
            _lockedUsers[username].IsLocked = true;
            _lockedUsers[username].LockedAt = DateTime.Now;
            _lockedUsers[username].TimesLocked++;
            
            if (_lockedUsers[username].TimesLocked >= 2)
            {
                _lockedUsers[username].IsPermanentlyLocked = true;
                GD.Print($"🔒🔒 Usuario '{username}' bloqueado PERMANENTEMENTE (Segundo bloqueo)");
            }
            else
            {
                GD.Print($"🔒 Usuario '{username}' bloqueado tras {MAX_FAILED_ATTEMPTS} intentos fallidos");
            }
        }

        SaveLockData();
    }

    public int GetFailedAttempts(string username)
    {
        return _lockedUsers.ContainsKey(username) ? _lockedUsers[username].FailedAttempts : 0;
    }

    public bool IsUserLocked(string username)
    {
        return _lockedUsers.ContainsKey(username) && _lockedUsers[username].IsLocked;
    }

    public void UnlockUser(string username)
    {
        if (_lockedUsers.ContainsKey(username))
        {
            // Si está bloqueado permanentemente, no se puede desbloquear
            if (_lockedUsers[username].IsPermanentlyLocked)
            {
                GD.Print($"❌ Usuario '{username}' está PERMANENTEMENTE bloqueado. No puede desbloquearse.");
                return;
            }

            _lockedUsers[username].IsLocked = false;
            _lockedUsers[username].FailedAttempts = 0;
            GD.Print($"🔓 Usuario '{username}' desbloqueado");
            SaveLockData();
        }
    }

    public void ClearFailedAttempts(string username)
    {
        if (_lockedUsers.ContainsKey(username))
        {
            // Si está bloqueado permanentemente, no se puede limpiar
            if (_lockedUsers[username].IsPermanentlyLocked)
            {
                GD.Print($"❌ No se pueden limpiar intentos para '{username}'. Está PERMANENTEMENTE bloqueado.");
                return;
            }

            _lockedUsers[username].FailedAttempts = 0;
            _lockedUsers[username].IsLocked = false;
            GD.Print($"✅ Intentos fallidos limpios para '{username}'");
            SaveLockData();
        }
    }

    public int GetMaxFailedAttempts()
    {
        return MAX_FAILED_ATTEMPTS;
    }

    public bool IsPermanentlyLocked(string username)
    {
        return _lockedUsers.ContainsKey(username) && _lockedUsers[username].IsPermanentlyLocked;
    }

    public List<string> GetLockedUsers()
    {
        var locked = new List<string>();
        
        try
        {
            // Primero asegurarse de que todos los usuarios del sistema estén en el diccionario
            var allUsers = AuthService.GetAllUsernames();
            
            foreach (var username in allUsers)
            {
                // Si el usuario no está en el diccionario, agregarlo con estado desbloqueado
                if (!_lockedUsers.ContainsKey(username))
                {
                    _lockedUsers[username] = new UserLockData(username, 0);
                }
            }
            
            // Ahora iterar y retornar los usuarios bloqueados
            foreach (var kvp in _lockedUsers)
            {
                if (kvp.Value.IsLocked)
                {
                    locked.Add(kvp.Key);
                }
            }
        }
        catch (Exception e)
        {
            GD.PrintErr($"[ERROR] GetLockedUsers: {e.Message}");
        }
        
        return locked;
    }

    private void SaveLockData()
    {
        try
        {
            using var file = FileAccess.Open(LockDataFile, FileAccess.ModeFlags.Write);
            if (file == null)
            {
                GD.PrintErr($"Error al crear archivo de bloqueos: {FileAccess.GetOpenError()}");
                return;
            }

            var locksArray = new Godot.Collections.Array();
            foreach (var kvp in _lockedUsers)
            {
                var lockDict = new Godot.Collections.Dictionary();
                lockDict["username"] = kvp.Value.Username;
                lockDict["failedAttempts"] = kvp.Value.FailedAttempts;
                lockDict["isLocked"] = kvp.Value.IsLocked;
                lockDict["lockedAt"] = kvp.Value.LockedAt.ToString("O");
                lockDict["timesLocked"] = kvp.Value.TimesLocked;
                lockDict["isPermanentlyLocked"] = kvp.Value.IsPermanentlyLocked;
                locksArray.Add(lockDict);
            }

            var json = Json.Stringify(locksArray);
            file.StoreString(json);
            file.Flush();
        }
        catch (Exception e)
        {
            GD.PrintErr($"Error guardando datos de bloqueo: {e.Message}");
        }
    }

    private void LoadLockData()
    {
        try
        {
            if (!FileAccess.FileExists(LockDataFile))
            {
                return;
            }

            using var file = FileAccess.Open(LockDataFile, FileAccess.ModeFlags.Read);
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

            var locksArray = json.AsGodotArray();
            _lockedUsers.Clear();

            foreach (var item in locksArray)
            {
                var lockDict = item.AsGodotDictionary();
                var username = lockDict["username"].AsString();
                var lockData = new UserLockData();
                lockData.Username = username;
                lockData.FailedAttempts = (int)lockDict["failedAttempts"].AsInt32();
                lockData.IsLocked = lockDict["isLocked"].AsBool();
                lockData.TimesLocked = lockDict.ContainsKey("timesLocked") ? (int)lockDict["timesLocked"].AsInt32() : 0;
                lockData.IsPermanentlyLocked = lockDict.ContainsKey("isPermanentlyLocked") ? lockDict["isPermanentlyLocked"].AsBool() : false;

                if (DateTime.TryParse(lockDict["lockedAt"].AsString(), out var lockedAt))
                {
                    lockData.LockedAt = lockedAt;
                }

                _lockedUsers[username] = lockData;
            }

            GD.Print($"✅ Datos de bloqueo cargados: {_lockedUsers.Count} registros");
        }
        catch (Exception e)
        {
            GD.PrintErr($"Error cargando datos de bloqueo: {e.Message}");
        }
    }
}
