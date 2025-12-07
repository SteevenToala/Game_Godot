using Godot;
using System;

public partial class LoginScreen : Control, IInitializable
{
	[Signal] public delegate void LoginSuccessEventHandler(string username);
	[Signal] public delegate void UserCreatedEventHandler(string username);

	// Elementos UI para Login
	private LineEdit _usernameField;
	private LineEdit _passwordField;
	private Button _loginButton;
	private Button _createButton;
	private Label _statusLabel;
	private Button _togglePasswordButton; // Botón para mostrar/ocultar contraseña
	private Label _attemptsLabel; // Muestra intentos restantes

	// Elementos UI para cambio de contraseña
	private Control _changePasswordPanel;
	private LineEdit _currentPasswordField;
	private LineEdit _newPasswordField;
	private LineEdit _confirmPasswordField;
	private Button _changePasswordButton;
	private Button _cancelChangePasswordButton;
	private Label _changePasswordStatus;

	// Elementos UI adicionales
	private Button _changePasswordMenuButton;
	private Button _logoutButton;
	private Label _userInfoLabel;
	private Button _manageUsersButton; // Botón para gestionar usuarios bloqueados
	private Button _manageUsersLoginButton; // Botón para gestionar desde login (requiere credenciales)

	// Panel de gestión de usuarios
	private Control _manageUsersPanel;
	private ItemList _lockedUsersList;
	private Button _unlockUserButton;
	private Button _closeManageUsersButton;
	private Label _manageUsersStatus;
	public override void _Ready()
	{
		GD.Print("🔐 LoginScreen._Ready() - Inicializando pantalla de login");
		Initialize();
	}

	public void Initialize()
	{
		GD.Print("🔐 LoginScreen.Initialize() - Obteniendo nodos de la escena");
		GetNodesFromScene();
		ConnectSignals();
		UpdateUIState();
		
		// Asegurar que la pantalla sea visible
		Visible = true;
		GD.Print("✅ LoginScreen inicializado correctamente");
	}

	private void GetNodesFromScene()
	{
		try
		{
			// Obtener nodos existentes del árbol de escena - Sección Login
			_usernameField = GetNodeOrNull<LineEdit>("UILayer/MainPanel/MarginContainer/VBoxContainer/UsernameField");
			_passwordField = GetNodeOrNull<LineEdit>("UILayer/MainPanel/MarginContainer/VBoxContainer/PasswordContainer/PasswordField");
			_togglePasswordButton = GetNodeOrNull<Button>("UILayer/MainPanel/MarginContainer/VBoxContainer/PasswordContainer/TogglePasswordButton");
			_loginButton = GetNodeOrNull<Button>("UILayer/MainPanel/MarginContainer/VBoxContainer/ButtonContainer/LoginButton");
			_createButton = GetNodeOrNull<Button>("UILayer/MainPanel/MarginContainer/VBoxContainer/ButtonContainer/CreateButton");
			_attemptsLabel = GetNodeOrNull<Label>("UILayer/MainPanel/MarginContainer/VBoxContainer/AttemptsLabel");
			_statusLabel = GetNodeOrNull<Label>("UILayer/MainPanel/MarginContainer/VBoxContainer/StatusLabel");
			_manageUsersLoginButton = GetNodeOrNull<Button>("UILayer/MainPanel/MarginContainer/VBoxContainer/ManageUsersLoginButton");

			// Obtener nodos para usuario logueado
			_userInfoLabel = GetNodeOrNull<Label>("UILayer/MainPanel/MarginContainer/VBoxContainer/UserInfoLabel");
			_changePasswordMenuButton = GetNodeOrNull<Button>("UILayer/MainPanel/MarginContainer/VBoxContainer/ChangePasswordMenuButton");
			_manageUsersButton = GetNodeOrNull<Button>("UILayer/MainPanel/MarginContainer/VBoxContainer/ManageUsersButton");
			_logoutButton = GetNodeOrNull<Button>("UILayer/MainPanel/MarginContainer/VBoxContainer/LogoutButton");

			// Obtener nodos del panel de cambio de contraseña
			_changePasswordPanel = GetNodeOrNull<Control>("UILayer/ChangePasswordPanel");
			_currentPasswordField = GetNodeOrNull<LineEdit>("UILayer/ChangePasswordPanel/ChangePasswordContainer/CurrentPasswordField");
			_newPasswordField = GetNodeOrNull<LineEdit>("UILayer/ChangePasswordPanel/ChangePasswordContainer/NewPasswordField");
			_confirmPasswordField = GetNodeOrNull<LineEdit>("UILayer/ChangePasswordPanel/ChangePasswordContainer/ConfirmPasswordField");
			_changePasswordButton = GetNodeOrNull<Button>("UILayer/ChangePasswordPanel/ChangePasswordContainer/ButtonContainer/ChangePasswordButton");
			_cancelChangePasswordButton = GetNodeOrNull<Button>("UILayer/ChangePasswordPanel/ChangePasswordContainer/ButtonContainer/CancelChangePasswordButton");
			_changePasswordStatus = GetNodeOrNull<Label>("UILayer/ChangePasswordPanel/ChangePasswordContainer/ChangePasswordStatus");

			// Obtener nodos del panel de gestión de usuarios
			_manageUsersPanel = GetNodeOrNull<Control>("UILayer/ManageUsersPanel");
			_lockedUsersList = GetNodeOrNull<ItemList>("UILayer/ManageUsersPanel/ManageContainer/LockedUsersList");
			_unlockUserButton = GetNodeOrNull<Button>("UILayer/ManageUsersPanel/ManageContainer/ManageButtonContainer/UnlockUserButton");
			_closeManageUsersButton = GetNodeOrNull<Button>("UILayer/ManageUsersPanel/ManageContainer/ManageButtonContainer/CloseManageUsersButton");
			_manageUsersStatus = GetNodeOrNull<Label>("UILayer/ManageUsersPanel/ManageContainer/ManageUsersStatus");

			// Validar que al menos los nodos críticos existan
			if (_usernameField == null || _passwordField == null || _loginButton == null)
			{
				GD.PrintErr("❌ LoginScreen: Nodos críticos no encontrados");
				GD.PrintErr($"   UsernameField: {_usernameField != null}");
				GD.PrintErr($"   PasswordField: {_passwordField != null}");
				GD.PrintErr($"   LoginButton: {_loginButton != null}");
				return;
			}

			GD.Print("✅ LoginScreen: Todos los nodos obtenidos correctamente");
		}
		catch (Exception ex)
		{
			GD.PrintErr($"❌ Error al obtener nodos del LoginScreen: {ex.Message}");
			GD.PrintErr($"Stack trace: {ex.StackTrace}");
		}
	}

	/// <summary>
	/// Conecta los eventos de los botones con sus respectivos handlers
	/// </summary>
	private void ConnectSignals()
	{
		// Conectar solo si los nodos existen
		if (_loginButton != null) _loginButton.Pressed += OnLoginButtonPressed;
		if (_createButton != null) _createButton.Pressed += OnCreateButtonPressed;
		if (_manageUsersLoginButton != null) _manageUsersLoginButton.Pressed += OnManageUsersLoginButtonPressed;
		if (_changePasswordMenuButton != null) _changePasswordMenuButton.Pressed += OnChangePasswordMenuPressed;
		if (_manageUsersButton != null) _manageUsersButton.Pressed += OnManageUsersButtonPressed;
		if (_logoutButton != null) _logoutButton.Pressed += OnLogoutButtonPressed;
		if (_changePasswordButton != null) _changePasswordButton.Pressed += OnChangePasswordButtonPressed;
		if (_cancelChangePasswordButton != null) _cancelChangePasswordButton.Pressed += OnCancelChangePasswordPressed;
		if (_unlockUserButton != null) _unlockUserButton.Pressed += OnUnlockUserButtonPressed;
		if (_closeManageUsersButton != null) _closeManageUsersButton.Pressed += OnCloseManageUsersPressed;
		if (_togglePasswordButton != null) _togglePasswordButton.Pressed += OnTogglePasswordVisibility;

		// Enter para hacer login
		if (_passwordField != null) _passwordField.TextSubmitted += (_) => OnLoginButtonPressed();

		GD.Print("✅ LoginScreen: Señales conectadas");
	}

	private void OnLoginButtonPressed()
	{
		var username = _usernameField.Text.Trim();
		var password = _passwordField.Text;

		if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
		{
			ShowStatus("Por favor ingresa usuario y contraseña", true);
			return;
		}

		var result = AuthService.Login(username, password);

		if (result.Success)
		{
			ShowStatus(result.Message, false);
			_attemptsLabel.Text = ""; // Limpiar label de intentos
			UpdateUIState();

			// Emitir señal según sea creación o login
			if (result.User != null && result.User.CreatedAt == result.User.LastLogin)
			{
				EmitSignal(SignalName.UserCreated, result.User.Username);
			}
			else
			{
				EmitSignal(SignalName.LoginSuccess, result.User.Username);
			}
		}
		else
		{
			ShowStatus(result.Message, true);
			UpdateAttemptsLabel(username);
		}

		// Limpiar campos
		_passwordField.Text = "";
	}

	private void OnCreateButtonPressed()
	{
		var username = _usernameField.Text.Trim();
		var password = _passwordField.Text;

		if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
		{
			ShowStatus("Por favor ingresa usuario y contraseña", true);
			return;
		}

		var result = AuthService.Register(username, password);
		if (result.Success)
		{
			ShowStatus(result.Message, false);
			UpdateUIState();
			EmitSignal(SignalName.UserCreated, result.User.Username);
		}
		else
		{
			ShowStatus(result.Message, true);
		}

		_passwordField.Text = "";
	}

	private void OnChangePasswordMenuPressed()
	{
		_changePasswordPanel.Visible = true;
		_currentPasswordField.Text = "";
		_newPasswordField.Text = "";
		_confirmPasswordField.Text = "";
		_changePasswordStatus.Text = "";
		_currentPasswordField.GrabFocus();
	}

	private void OnChangePasswordButtonPressed()
	{
		var currentPassword = _currentPasswordField.Text;
		var newPassword = _newPasswordField.Text;
		var confirmPassword = _confirmPasswordField.Text;

		if (string.IsNullOrEmpty(currentPassword) || string.IsNullOrEmpty(newPassword))
		{
			ShowChangePasswordStatus("Por favor completa todos los campos", true);
			return;
		}

		if (newPassword != confirmPassword)
		{
			ShowChangePasswordStatus("Las contraseñas no coinciden", true);
			return;
		}

		var result = AuthService.ChangePassword(currentPassword, newPassword);
		ShowChangePasswordStatus(result.Message, !result.Success);

		if (result.Success)
		{
			// Cerrar panel después de un breve delay
			GetTree().CreateTimer(1.5f).Timeout += () =>
			{
				_changePasswordPanel.Visible = false;
			};
		}
	}

	private void OnCancelChangePasswordPressed()
	{
		_changePasswordPanel.Visible = false;
	}

	private void OnLogoutButtonPressed()
	{
		AuthService.Logout();
		UpdateUIState();
		ShowStatus("Sesión cerrada", false);

		// Limpiar campos
		_usernameField.Text = "";
		_passwordField.Text = "";
	}

	private void UpdateUIState()
	{
		bool isLoggedIn = AuthService.IsLoggedIn;

		// Mostrar/ocultar secciones según estado de login
		_usernameField.Visible = !isLoggedIn;
		_passwordField.Visible = !isLoggedIn;
		_togglePasswordButton.Visible = !isLoggedIn;
		_loginButton.Visible = !isLoggedIn;
		_createButton.Visible = !isLoggedIn;
		_attemptsLabel.Visible = !isLoggedIn;
		_manageUsersLoginButton.Visible = !isLoggedIn;

		_userInfoLabel.Visible = isLoggedIn;
		_changePasswordMenuButton.Visible = isLoggedIn;
		_manageUsersButton.Visible = isLoggedIn;
		_logoutButton.Visible = isLoggedIn;

		if (isLoggedIn)
		{
			var user = AuthService.CurrentUser;
			_userInfoLabel.Text = $"👤 Usuario: {user.Username}\n🏆 Record: {user.HighScore}\n📅 Último acceso: {user.LastLogin:yyyy-MM-dd HH:mm}";
		}
	}

	private void ShowStatus(string message, bool isError)
	{
		_statusLabel.Text = message;
		_statusLabel.Modulate = isError ? Colors.Red : Colors.Green;
	}

	private void ShowChangePasswordStatus(string message, bool isError)
	{
		_changePasswordStatus.Text = message;
		_changePasswordStatus.Modulate = isError ? Colors.Red : Colors.Green;
	}

	public void RefreshUserInfo()
	{
		UpdateUIState();
	}

	/// <summary>
	/// Pre-llena el campo de usuario (útil cuando ya hay un usuario cargado)
	/// </summary>
	public void PreFillCredentials(string username)
	{
		if (_usernameField != null)
		{
			_usernameField.Text = username;
		}
		// Limpiar contraseña por seguridad
		if (_passwordField != null)
		{
			_passwordField.Text = "";
			_passwordField.GrabFocus(); // Poner foco en contraseña
		}
	}

	// ========== NUEVOS MÉTODOS PARA GESTIÓN DE BLOQUEOS ==========

	/// <summary>
	/// Abre gestión de usuarios desde la pantalla de login (requiere credenciales admin)
	/// </summary>
	private void OnManageUsersLoginButtonPressed()
	{
		var username = _usernameField.Text.Trim();
		var password = _passwordField.Text;

		if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
		{
			ShowStatus("Por favor ingresa credenciales para acceder a gestión", true);
			return;
		}

		// Validar credenciales (sin autenticarse)
		var result = AuthService.Login(username, password);

		if (!result.Success)
		{
			ShowStatus("Credenciales inválidas. " + result.Message, true);
			UpdateAttemptsLabel(username);
			_passwordField.Text = "";
			return;
		}

		// Credenciales válidas - abrir panel de gestión
		ShowStatus("✅ Credenciales válidas", false);
		_manageUsersPanel.Visible = true;
		RefreshLockedUsersList();
		_manageUsersStatus.Text = "Sesión de gestión activa";

		// Limpiar campos para seguridad
		_usernameField.Text = "";
		_passwordField.Text = "";
	}

	/// <summary>
	/// Muestra/oculta la contraseña
	/// </summary>
	private void OnTogglePasswordVisibility()
	{
		_passwordField.Secret = !_passwordField.Secret;
		_togglePasswordButton.Text = _passwordField.Secret ? "👁️" : "👁️‍🗨️";
	}

	/// <summary>
	/// Actualiza el label de intentos restantes
	/// </summary>
	private void UpdateAttemptsLabel(string username)
	{
		var lockService = AuthService.GetLockService();
		int attempts = lockService.GetFailedAttempts(username);
		int maxAttempts = lockService.GetMaxFailedAttempts();
		int remaining = maxAttempts - attempts;

		if (lockService.IsUserLocked(username))
		{
			_attemptsLabel.Text = "❌ Usuario bloqueado tras 3 intentos fallidos";
			_attemptsLabel.Modulate = Colors.Red;
		}
		else if (remaining > 0)
		{
			_attemptsLabel.Text = $"⚠️ Intentos restantes: {remaining}";
			_attemptsLabel.Modulate = Colors.Yellow;
		}
		else
		{
			_attemptsLabel.Text = "";
		}
	}

	/// <summary>
	/// Abre el panel de gestión de usuarios bloqueados
	/// </summary>
	private void OnManageUsersButtonPressed()
	{
		_manageUsersPanel.Visible = true;
		RefreshLockedUsersList();
		_manageUsersStatus.Text = "";
	}

	/// <summary>
	/// Cierra el panel de gestión de usuarios
	/// </summary>
	private void OnCloseManageUsersPressed()
	{
		_manageUsersPanel.Visible = false;
	}

	/// <summary>
	/// Recarga la lista de usuarios bloqueados
	/// </summary>
	private void RefreshLockedUsersList()
	{
		_lockedUsersList.Clear();
		var lockedUsers = AuthService.GetLockedUsers();

		if (lockedUsers.Count == 0)
		{
			_lockedUsersList.AddItem("(No hay usuarios bloqueados)");
		}
		else
		{
			foreach (var username in lockedUsers)
			{
				_lockedUsersList.AddItem($"🔒 {username}");
			}
		}
	}

	/// <summary>
	/// Desbloquea el usuario seleccionado
	/// </summary>
	private void OnUnlockUserButtonPressed()
	{
		int[] selectedIndices = _lockedUsersList.GetSelectedItems();
		if (selectedIndices.Length == 0)
		{
			ShowManageUsersStatus("Por favor selecciona un usuario", true);
			return;
		}

		var lockedUsers = AuthService.GetLockedUsers();
		if (selectedIndices[0] >= lockedUsers.Count)
		{
			ShowManageUsersStatus("Selección inválida", true);
			return;
		}

		string username = lockedUsers[selectedIndices[0]];
		AuthService.UnlockUser(username);
		ShowManageUsersStatus($"✅ Usuario '{username}' desbloqueado", false);
		RefreshLockedUsersList();
	}

	/// <summary>
	/// Muestra estado en el panel de gestión de usuarios
	/// </summary>
	private void ShowManageUsersStatus(string message, bool isError)
	{
		_manageUsersStatus.Text = message;
		_manageUsersStatus.Modulate = isError ? Colors.Red : Colors.Green;
	}

	// ========== MÉTODOS PARA PANEL DE INFORMACIÓN POST-JUEGO ==========
}
