using Godot;

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

	public override void _Ready()
	{
		Initialize();
	}

	public void Initialize()
	{
		CreateUI();
		ConnectSignals();
		UpdateUIState();
	}

	private void CreateUI()
	{
		// Panel principal
		var mainPanel = new Panel();
		mainPanel.Size = new Vector2(400, 700);
		mainPanel.Position = new Vector2(50, 100);
		mainPanel.AddThemeStyleboxOverride("panel", new StyleBoxFlat() { BgColor = ColorPalette.PanelBackground });
		AddChild(mainPanel);

		// Título
		var titleLabel = new Label();
		titleLabel.Text = "AUTENTICACION";
		titleLabel.Position = new Vector2(150, 20);
		titleLabel.AddThemeColorOverride("font_color", ColorPalette.Text);
		mainPanel.AddChild(titleLabel);

		// --- SECCIÓN DE LOGIN ---
		var loginContainer = new VBoxContainer();
		loginContainer.Position = new Vector2(20, 50);
		loginContainer.Size = new Vector2(360, 200);
		mainPanel.AddChild(loginContainer);

		// Campo de usuario
		var usernameLabel = new Label();
		usernameLabel.Text = "Usuario:";
		usernameLabel.AddThemeColorOverride("font_color", ColorPalette.Text);
		loginContainer.AddChild(usernameLabel);

		_usernameField = new LineEdit();
		_usernameField.PlaceholderText = "Ingresa tu nombre de usuario";
		_usernameField.Size = new Vector2(340, 30);
		_usernameField.AddThemeColorOverride("font_color", ColorPalette.Text);
		_usernameField.AddThemeStyleboxOverride("normal", new StyleBoxFlat() { BgColor = ColorPalette.InputBackground });
		loginContainer.AddChild(_usernameField);

		// Campo de contraseña
		var passwordLabel = new Label();
		passwordLabel.Text = "Contraseña:";
		passwordLabel.AddThemeColorOverride("font_color", ColorPalette.Text);
		loginContainer.AddChild(passwordLabel);

		_passwordField = new LineEdit();
		_passwordField.PlaceholderText = "Ingresa tu contraseña";
		_passwordField.Secret = true;
		_passwordField.Size = new Vector2(340, 30);
		_passwordField.AddThemeColorOverride("font_color", ColorPalette.Text);
		_passwordField.AddThemeStyleboxOverride("normal", new StyleBoxFlat() { BgColor = ColorPalette.InputBackground });
		loginContainer.AddChild(_passwordField);

		// Botones: Login + Crear cuenta
		var buttons = new HBoxContainer();
		buttons.Size = new Vector2(360, 40);
		loginContainer.AddChild(buttons);

		_loginButton = new Button();
		_loginButton.Text = "INICIAR SESIÓN";
		_loginButton.Size = new Vector2(100, 40);
		_loginButton.AddThemeStyleboxOverride("normal", new StyleBoxFlat() { BgColor = ColorPalette.Button });
		_loginButton.AddThemeColorOverride("font_color", ColorPalette.Text);
		buttons.AddChild(_loginButton);

		_createButton = new Button();
		_createButton.Text = "CREAR CUENTA";
		_createButton.Size = new Vector2(170, 40);
		_createButton.AddThemeStyleboxOverride("normal", new StyleBoxFlat() { BgColor = ColorPalette.Button });
		_createButton.AddThemeColorOverride("font_color", ColorPalette.Text);
		buttons.AddChild(_createButton);

		// Label de estado
		_statusLabel = new Label();
		_statusLabel.Text = "";
		_statusLabel.AutowrapMode = TextServer.AutowrapMode.WordSmart;
		_statusLabel.Size = new Vector2(340, 60);
		_statusLabel.AddThemeColorOverride("font_color", ColorPalette.Text);
		loginContainer.AddChild(_statusLabel);

		// --- SECCIÓN DE USUARIO LOGUEADO ---
		var userContainer = new VBoxContainer();
		userContainer.Position = new Vector2(20, 280);
		userContainer.Size = new Vector2(360, 200);
		mainPanel.AddChild(userContainer);

		_userInfoLabel = new Label();
		_userInfoLabel.Text = "";
		_userInfoLabel.AutowrapMode = TextServer.AutowrapMode.WordSmart;
		_userInfoLabel.AddThemeColorOverride("font_color", ColorPalette.Text);
		userContainer.AddChild(_userInfoLabel);

		_changePasswordMenuButton = new Button();
		_changePasswordMenuButton.Text = "CAMBIAR CONTRASEÑA";
		_changePasswordMenuButton.Size = new Vector2(340, 30);
		_changePasswordMenuButton.AddThemeStyleboxOverride("normal", new StyleBoxFlat() { BgColor = ColorPalette.Button });
		_changePasswordMenuButton.AddThemeColorOverride("font_color", ColorPalette.Text);
		userContainer.AddChild(_changePasswordMenuButton);

		_logoutButton = new Button();
		_logoutButton.Text = "CERRAR SESIÓN";
		_logoutButton.Size = new Vector2(340, 30);
		_logoutButton.AddThemeStyleboxOverride("normal", new StyleBoxFlat() { BgColor = ColorPalette.Button });
		_logoutButton.AddThemeColorOverride("font_color", ColorPalette.Text);
		userContainer.AddChild(_logoutButton);

		// --- PANEL DE CAMBIO DE CONTRASEÑA ---
		_changePasswordPanel = new Panel();
		_changePasswordPanel.Position = new Vector2(300, 200);
		_changePasswordPanel.Size = new Vector2(350, 300);
		_changePasswordPanel.Visible = false;
		AddChild(_changePasswordPanel);

		var changePassTitle = new Label();
		changePassTitle.Text = "CAMBIAR CONTRASEÑA";
		changePassTitle.Position = new Vector2(20, 20);
		_changePasswordPanel.AddChild(changePassTitle);

		var changePassContainer = new VBoxContainer();
		changePassContainer.Position = new Vector2(20, 50);
		changePassContainer.Size = new Vector2(310, 240);
		_changePasswordPanel.AddChild(changePassContainer);

		// Contraseña actual
		var currentPassLabel = new Label();
		currentPassLabel.Text = "Contraseña actual:";
		currentPassLabel.AddThemeColorOverride("font_color", ColorPalette.Text);
		changePassContainer.AddChild(currentPassLabel);

		_currentPasswordField = new LineEdit();
		_currentPasswordField.Secret = true;
		_currentPasswordField.Size = new Vector2(300, 30);
		_currentPasswordField.AddThemeColorOverride("font_color", ColorPalette.Text);
		_currentPasswordField.AddThemeStyleboxOverride("normal", new StyleBoxFlat() { BgColor = ColorPalette.InputBackground });
		changePassContainer.AddChild(_currentPasswordField);

		// Nueva contraseña
		var newPassLabel = new Label();
		newPassLabel.Text = "Nueva contraseña:";
		newPassLabel.AddThemeColorOverride("font_color", ColorPalette.Text);
		changePassContainer.AddChild(newPassLabel);

		_newPasswordField = new LineEdit();
		_newPasswordField.Secret = true;
		_newPasswordField.Size = new Vector2(300, 30);
		_newPasswordField.AddThemeColorOverride("font_color", ColorPalette.Text);
		_newPasswordField.AddThemeStyleboxOverride("normal", new StyleBoxFlat() { BgColor = ColorPalette.InputBackground });
		changePassContainer.AddChild(_newPasswordField);

		// Confirmar contraseña
		var confirmPassLabel = new Label();
		confirmPassLabel.Text = "Confirmar nueva contraseña:";
		confirmPassLabel.AddThemeColorOverride("font_color", ColorPalette.Text);
		changePassContainer.AddChild(confirmPassLabel);

		_confirmPasswordField = new LineEdit();
		_confirmPasswordField.Secret = true;
		_confirmPasswordField.Size = new Vector2(300, 30);
		_confirmPasswordField.AddThemeColorOverride("font_color", ColorPalette.Text);
		_confirmPasswordField.AddThemeStyleboxOverride("normal", new StyleBoxFlat() { BgColor = ColorPalette.InputBackground });
		changePassContainer.AddChild(_confirmPasswordField);

		// Botones
		var buttonContainer = new HBoxContainer();
		buttonContainer.Size = new Vector2(300, 40);
		changePassContainer.AddChild(buttonContainer);

		_changePasswordButton = new Button();
		_changePasswordButton.Text = "CAMBIAR";
		_changePasswordButton.Size = new Vector2(140, 30);
		_changePasswordButton.AddThemeStyleboxOverride("normal", new StyleBoxFlat() { BgColor = ColorPalette.Button });
		_changePasswordButton.AddThemeColorOverride("font_color", ColorPalette.Text);
		buttonContainer.AddChild(_changePasswordButton);

		_cancelChangePasswordButton = new Button();
		_cancelChangePasswordButton.Text = "CANCELAR";
		_cancelChangePasswordButton.Size = new Vector2(140, 30);
		_cancelChangePasswordButton.AddThemeStyleboxOverride("normal", new StyleBoxFlat() { BgColor = ColorPalette.Button });
		_cancelChangePasswordButton.AddThemeColorOverride("font_color", ColorPalette.Text);
		buttonContainer.AddChild(_cancelChangePasswordButton);

		// Status del cambio de contraseña
		_changePasswordStatus = new Label();
		_changePasswordStatus.Text = "";
		_changePasswordStatus.AutowrapMode = TextServer.AutowrapMode.WordSmart;
		_changePasswordStatus.Size = new Vector2(300, 40);
		_changePasswordStatus.AddThemeColorOverride("font_color", ColorPalette.Text);
		changePassContainer.AddChild(_changePasswordStatus);
	}

	private void ConnectSignals()
	{
		_loginButton.Pressed += OnLoginButtonPressed;
		_createButton.Pressed += OnCreateButtonPressed;
		_changePasswordMenuButton.Pressed += OnChangePasswordMenuPressed;
		_logoutButton.Pressed += OnLogoutButtonPressed;
		_changePasswordButton.Pressed += OnChangePasswordButtonPressed;
		_cancelChangePasswordButton.Pressed += OnCancelChangePasswordPressed;

		// Enter para hacer login
		_passwordField.TextSubmitted += (_) => OnLoginButtonPressed();
		// Enter en username+password crea usuario si se presiona shift (comportamiento extra opcional)
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
		_loginButton.Visible = !isLoggedIn;
		_createButton.Visible = !isLoggedIn;

		_userInfoLabel.Visible = isLoggedIn;
		_changePasswordMenuButton.Visible = isLoggedIn;
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
}
