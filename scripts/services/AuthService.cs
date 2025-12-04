using Godot;
using System;
using System.Linq;

/// <summary>
/// Servicio de autenticación para manejar el usuario único del juego
/// </summary>
public static class AuthService
{
    private static User _currentUser = null;
    private const string UserDataFile = "user://user_data.json";
    // Nuevo: soportar múltiples usuarios (máximo 3)
    private static System.Collections.Generic.List<User> _users = new System.Collections.Generic.List<User>();

    /// <summary>
    /// Usuario actualmente autenticado
    /// </summary>
    public static User CurrentUser => _currentUser;

    /// <summary>
    /// Indica si hay un usuario autenticado
    /// </summary>
    public static bool IsLoggedIn => _currentUser != null;

    /// <summary>
    /// Intenta hacer login con las credenciales proporcionadas
    /// </summary>
    public static AuthResult Login(string username, string password)
    {
        if (!PasswordService.IsValidUsername(username))
        {
            return new AuthResult(false, "Nombre de usuario inválido. Debe tener entre 3-20 caracteres alfanuméricos.");
        }

        if (!PasswordService.IsValidPassword(password))
        {
            return new AuthResult(false, "Contraseña inválida. Debe tener al menos 4 caracteres.");
        }

        // Cargar lista de usuarios
        LoadUsers();
        var storedUser = _users.Find(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));

        // Si no existe usuario, crear uno nuevo
        if (storedUser == null)
        {
            // Si no existe en la lista, crear y agregar respetando máximo 3
            var createRes = CreateNewUser(username, password);
            return createRes;
        }

        // Verificar credenciales
        if (storedUser.Username != username)
        {
            return new AuthResult(false, "Nombre de usuario incorrecto.");
        }

        if (!PasswordService.VerifyPassword(password, storedUser.PasswordHash))
        {
            return new AuthResult(false, "Contraseña incorrecta.");
        }

        // Login exitoso
        storedUser.UpdateLastLogin();
        _currentUser = storedUser;
        SaveUsers();

        GD.Print($"✅ Login exitoso: {_currentUser.Username}");
        return new AuthResult(true, "Login exitoso", _currentUser);
    }

    /// <summary>
    /// Registra (crea) un nuevo usuario. Si existe usuario previo lo sobrescribe.
    /// </summary>
    public static AuthResult Register(string username, string password)
    {
        if (!PasswordService.IsValidUsername(username))
        {
            return new AuthResult(false, "Nombre de usuario inválido. Debe tener entre 3-20 caracteres alfanuméricos.");
        }

        if (!PasswordService.IsValidPassword(password))
        {
            return new AuthResult(false, "Contraseña inválida. Debe tener al menos 4 caracteres.");
        }

        // Cargar usuarios existentes
        LoadUsers();

        // Si ya existe, impedir duplicado
        var existing = _users.Find(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
        if (existing != null)
        {
            return new AuthResult(false, "El usuario ya existe", existing);
        }

        var result = CreateNewUser(username, password);
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
        if (!IsLoggedIn)
        {
            return new AuthResult(false, "No hay usuario autenticado.");
        }

        // Verificar contraseña actual
        if (!PasswordService.VerifyPassword(currentPassword, _currentUser.PasswordHash))
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
        if (_currentUser.IsPasswordReused(newPasswordHash))
        {
            return new AuthResult(false, "No puedes usar una contraseña que ya has utilizado anteriormente.");
        }

        // Actualizar contraseña
        _currentUser.UpdatePassword(newPasswordHash);
        SaveUsers();

        GD.Print($"🔐 Contraseña cambiada exitosamente para: {_currentUser.Username}");
        return new AuthResult(true, "Contraseña cambiada exitosamente");
    }

    /// <summary>
    /// Cierra sesión del usuario actual
    /// </summary>
    public static void Logout()
    {
        if (_currentUser != null)
        {
            GD.Print($"👋 Logout: {_currentUser.Username}");
            _currentUser = null;
        }
    }

    /// <summary>
    /// Actualiza el puntaje del usuario actual
    /// </summary>
    public static bool UpdateScore(uint newScore)
    {
        if (!IsLoggedIn)
            return false;

        bool updated = _currentUser.UpdateHighScore(newScore);
        if (updated)
        {
            SaveUsers();
            GD.Print($"🏆 Nuevo record para {_currentUser.Username}: {newScore}");
        }
        return updated;
    }

    /// <summary>
    /// Obtiene el puntaje máximo del usuario actual
    /// </summary>
    public static uint GetHighScore()
    {
        return IsLoggedIn ? _currentUser.HighScore : 0;
    }

    /// <summary>
    /// Crea un nuevo usuario
    /// </summary>
    private static AuthResult CreateNewUser(string username, string password)
    {
        var passwordHash = PasswordService.HashPassword(password);
        var newUser = new User(username, passwordHash);

        // Añadir a la lista y mantener máximo 3 por último acceso
        _users.Add(newUser);
        _users = _users
            .OrderByDescending(u => u.LastLogin)
            .Take(3)
            .ToList();

        _currentUser = newUser;
        SaveUsers();

        GD.Print($"🆕 Nuevo usuario creado: {username}");
        return new AuthResult(true, "Usuario creado exitosamente", newUser);
    }

    /// <summary>
    /// Carga los datos del usuario desde archivo
    /// </summary>
    // Deprecated: mantener para compatibilidad, pero usar LoadUsers
    private static User LoadUser()
    {
        LoadUsers();
        return _users.FirstOrDefault();
    }

    /// <summary>
    /// Guarda los datos del usuario en archivo
    /// </summary>
    // Guardar lista de usuarios
    private static void SaveUsers()
    {
        try
        {
            using var file = FileAccess.Open(UserDataFile, FileAccess.ModeFlags.Write);
            if (file == null)
            {
                GD.PrintErr($"Error al crear archivo de usuarios: {FileAccess.GetOpenError()}");
                return;
            }

            var array = new Godot.Collections.Array();
            foreach (var u in _users)
            {
                array.Add(SerializeUser(u));
            }
            var jsonString = Json.Stringify(array);
            file.StoreString(jsonString);
            file.Flush();
        }
        catch (Exception e)
        {
            GD.PrintErr($"Error guardando usuarios: {e.Message}");
        }
    }

    /// <summary>
    /// Serializa un usuario a diccionario para guardar en JSON
    /// </summary>
    private static Godot.Collections.Dictionary SerializeUser(User user)
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
    /// Deserializa un usuario desde diccionario JSON
    /// </summary>
    private static User DeserializeUser(Godot.Collections.Dictionary dict)
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

    // Cargar lista de usuarios desde JSON (array)
    private static void LoadUsers()
    {
        try
        {
            _users.Clear();
            if (!FileAccess.FileExists(UserDataFile))
            {
                return;
            }

            using var file = FileAccess.Open(UserDataFile, FileAccess.ModeFlags.Read);
            if (file == null)
            {
                GD.PrintErr($"Error al abrir archivo de usuarios: {FileAccess.GetOpenError()}");
                return;
            }

            var jsonString = file.GetAsText();
            if (string.IsNullOrEmpty(jsonString))
            {
                return;
            }

            var json = Json.ParseString(jsonString);
            if (json.VariantType == Variant.Type.Array)
            {
                var arr = json.AsGodotArray();
                foreach (var item in arr)
                {
                    var dict = item.AsGodotDictionary();
                    var user = DeserializeUser(dict);
                    _users.Add(user);
                }
            }
            else if (json.VariantType == Variant.Type.Dictionary)
            {
                // Compatibilidad con formato antiguo de un solo usuario
                var dict = json.AsGodotDictionary();
                var user = DeserializeUser(dict);
                _users.Add(user);
            }

            // Mantener máximo 3
            _users = _users
                .OrderByDescending(u => u.LastLogin)
                .Take(3)
                .ToList();
        }
        catch (Exception e)
        {
            GD.PrintErr($"Error cargando usuarios: {e.Message}");
            _users.Clear();
        }
    }

    // Listar usuarios guardados (ordenados por último acceso)
    public static System.Collections.Generic.IEnumerable<User> GetSavedUsers()
    {
        LoadUsers();
        return _users.OrderByDescending(u => u.LastLogin);
    }

    /// <summary>
    /// Intenta cargar automáticamente el usuario al iniciar (pero sin mantener sesión activa)
    /// </summary>
    public static void Initialize()
    {
        // CAMBIO: Solo cargar datos pero no mantener sesión activa
        // El usuario siempre tendrá que hacer login manualmente
        GD.Print("🔧 AuthService inicializado - Requiere login manual");
    }

    /// <summary>
    /// Carga los datos del usuario sin activar la sesión (para pre-llenar login)
    /// </summary>
    public static User LoadUserData()
    {
        return LoadUser();
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
