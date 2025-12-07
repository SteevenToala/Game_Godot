using Godot;

/// <summary>
/// Manager principal para el sistema de usuarios, integra autenticación con el juego
/// </summary>
public partial class UserManager : Node, IInitializable
{
	[Signal] public delegate void UserLoggedInEventHandler(string username);
	[Signal] public delegate void UserLoggedOutEventHandler();
	[Signal] public delegate void HighScoreUpdatedEventHandler(uint newScore);

	private LoginScreen _loginScreen;
	private bool _gameStarted = false;

	public override void _Ready()
	{
		Initialize();
	}
	
	public void Initialize()
	{
		// Inicializar el servicio de autenticación
		AuthService.Initialize();
		
		CreateLoginScreen();
		
		// CAMBIO: Si hay un usuario ya logueado (ej: después de recargar escena), conectarse automáticamente
		if (AuthService.IsLoggedIn)
		{
			GD.Print($"🔄 Usuario aún logueado después de recarga: {AuthService.CurrentUser.Username}");
			HideLoginScreen();
			EmitSignal(SignalName.UserLoggedIn, AuthService.CurrentUser.Username);
		}
		else
		{
			GD.Print("📋 Sistema de múltiples usuarios activo - Esperando credenciales del usuario");
		}
	}

	private void CreateLoginScreen()
	{
		// Cargar la escena completa de login_screen.tscn en lugar de crear instancia vacía
		var loginScene = GD.Load<PackedScene>("res://scenes/login_screen.tscn");
		if (loginScene == null)
		{
			GD.PrintErr("❌ No se pudo cargar la escena login_screen.tscn");
			return;
		}

		_loginScreen = loginScene.Instantiate<LoginScreen>();
		if (_loginScreen == null)
		{
			GD.PrintErr("❌ No se pudo instanciar LoginScreen");
			return;
		}

		_loginScreen.Name = "LoginScreen";
		AddChild(_loginScreen);
		GD.Print("✅ LoginScreen añadido al árbol de escena");

		// Conectar señales usando constantes generadas
		_loginScreen.Connect(LoginScreen.SignalName.LoginSuccess, new Callable(this, nameof(OnUserLoggedInByName)));
		_loginScreen.Connect(LoginScreen.SignalName.UserCreated, new Callable(this, nameof(OnUserCreatedByName)));
	}

	private void OnUserLoggedIn(User user)
	{
		GD.Print($"🎮 Usuario logueado: {user.Username}");

		// Ocultar pantalla de login
		if (_loginScreen != null)
		{
			_loginScreen.Visible = false;
		}

		// Emitir señal para que el GameManager sepa que puede empezar
		EmitSignal(SignalName.UserLoggedIn, user.Username);
		_gameStarted = true;
		
	}

	private void OnUserCreated(User user)
	{
		GD.Print($"🆕 Nuevo usuario creado: {user.Username}");
		OnUserLoggedIn(user);
	}

	private void OnUserLoggedInByName(string username)
	{
		var user = AuthService.CurrentUser;
		if (user != null)
		{
			OnUserLoggedIn(user);
		}
	}

	private void OnUserCreatedByName(string username)
	{
		var user = AuthService.CurrentUser;
		if (user != null)
		{
			OnUserCreated(user);
		}
	}

	/// <summary>
	/// Actualiza el puntaje del usuario actual
	/// </summary>
	public bool UpdateUserScore(uint newScore)
	{
		if (!AuthService.IsLoggedIn)
			return false;

		bool isNewRecord = AuthService.UpdateScore(newScore);

		if (isNewRecord)
		{
			EmitSignal(SignalName.HighScoreUpdated, newScore);

			// Actualizar la info en la pantalla de login si está visible
			if (_loginScreen != null && _loginScreen.Visible)
			{
				_loginScreen.RefreshUserInfo();
			}
		}

		return isNewRecord;
	}

	/// <summary>
	/// Obtiene el high score del usuario actual
	/// </summary>
	public uint GetUserHighScore()
	{
		return AuthService.GetHighScore();
	}

	/// <summary>
	/// Verifica si hay un usuario autenticado
	/// </summary>
	public bool IsUserLoggedIn()
	{
		return AuthService.IsLoggedIn;
	}

	/// <summary>
	/// Obtiene el usuario actual
	/// </summary>
	public User GetCurrentUser()
	{
		return AuthService.CurrentUser;
	}

	/// <summary>
	/// Cierra sesión y muestra la pantalla de login
	/// </summary>
	public void Logout()
	{
		AuthService.Logout();

		if (_loginScreen != null)
		{
			_loginScreen.Visible = true;
			_loginScreen.RefreshUserInfo();
		}

		EmitSignal(SignalName.UserLoggedOut);
		_gameStarted = false;
	}

	/// <summary>
	/// Muestra la pantalla de login
	/// </summary>
	public void ShowLoginScreen()
	{
		if (_loginScreen != null)
		{
			_loginScreen.Visible = true;
		}
	}

	/// <summary>
	/// Oculta la pantalla de login
	/// </summary>
	public void HideLoginScreen()
	{
		if (_loginScreen != null)
		{
			_loginScreen.Visible = false;
		}
	}

	/// <summary>
	/// Indica si el juego ya ha comenzado
	/// </summary>
	public bool HasGameStarted()
	{
		return _gameStarted && AuthService.IsLoggedIn;
	}

	public override void _Input(InputEvent @event)
	{
		// Permitir volver a la pantalla de login con una tecla especial (F1)
		if (@event is InputEventKey keyEvent && keyEvent.Pressed)
		{
			if (keyEvent.Keycode == Key.F1)
			{
				if (_loginScreen != null)
				{
					_loginScreen.Visible = !_loginScreen.Visible;
				}
			}
		}
	}
}
