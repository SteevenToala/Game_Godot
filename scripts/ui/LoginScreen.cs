using Godot;
using System.Linq;

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
	// Lista de usuarios guardados (Top 3)
	private VBoxContainer _savedUsersList;
	private Control _savedUsersSection;
	private Button _toggleScoresButton;

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
		// Fondo negro a pantalla completa
		var bg = new ColorRect();
		bg.Color = Colors.Black;
		bg.SetAnchorsPreset(LayoutPreset.FullRect);
		AddChild(bg);

		// Panel principal (centrado y con estilo)
		var mainPanel = new Panel();
		mainPanel.Size = new Vector2(460, 560);
		mainPanel.SetAnchorsPreset(LayoutPreset.Center);
		mainPanel.SetOffsetsPreset(LayoutPreset.Center);
		var panelStyle = new StyleBoxFlat() { BgColor = Colors.Black };
		panelStyle.CornerRadiusTopLeft = 12;
		panelStyle.CornerRadiusTopRight = 12;
		panelStyle.CornerRadiusBottomLeft = 12;
		panelStyle.CornerRadiusBottomRight = 12;
		panelStyle.BorderColor = ColorPalette.Accent;
		panelStyle.BorderWidthTop = 2;
		panelStyle.BorderWidthBottom = 2;
		panelStyle.BorderWidthLeft = 2;
		panelStyle.BorderWidthRight = 2;
		panelStyle.ShadowColor = new Color(0,0,0,0.6f);
		panelStyle.ShadowSize = 16;
		panelStyle.ShadowOffset = new Vector2(0, 6);
		mainPanel.AddThemeStyleboxOverride("panel", panelStyle);
		AddChild(mainPanel);

		// Contenedor vertical interno para evitar solapamientos
		var panelStack = new VBoxContainer();
		panelStack.Size = mainPanel.Size;
		panelStack.Position = Vector2.Zero;
		panelStack.AddThemeConstantOverride("separation", 16);
		panelStack.Alignment = BoxContainer.AlignmentMode.Center;
		mainPanel.AddChild(panelStack);

		// Título
		var titleLabel = new Label();
		titleLabel.Text = "VERTICAL SHOOTER - LOGIN";
		titleLabel.Size = new Vector2(mainPanel.Size.X, 40);
		titleLabel.HorizontalAlignment = HorizontalAlignment.Center;
		titleLabel.AddThemeColorOverride("font_color", ColorPalette.Text);
		titleLabel.AddThemeFontSizeOverride("font_size", 24);
		titleLabel.AddThemeColorOverride("font_outline_color", ColorPalette.Accent);
		panelStack.AddChild(titleLabel);

		// --- SECCIÓN DE LOGIN (debajo) ---
		var loginContainer = new VBoxContainer();
		loginContainer.Size = new Vector2(404, 230);
		loginContainer.Alignment = BoxContainer.AlignmentMode.Center;
		panelStack.AddChild(loginContainer);


		// --- Botón "Puntajes" y sección Top-3 (debajo del formulario) ---
		var scoresToggleContainer = new VBoxContainer();
		scoresToggleContainer.Size = new Vector2(404, 48);
		scoresToggleContainer.Alignment = BoxContainer.AlignmentMode.Center;
		panelStack.AddChild(scoresToggleContainer);

		_toggleScoresButton = new Button();
		_toggleScoresButton.Text = "Puntajes";
		_toggleScoresButton.Size = new Vector2(404, 40);
		var toggleNormal = new StyleBoxFlat(){ BgColor = ColorPalette.Button };
		toggleNormal.CornerRadiusTopLeft = 8;
		toggleNormal.CornerRadiusTopRight = 8;
		toggleNormal.CornerRadiusBottomLeft = 8;
		toggleNormal.CornerRadiusBottomRight = 8;
		_toggleScoresButton.AddThemeStyleboxOverride("normal", toggleNormal);
		_toggleScoresButton.AddThemeColorOverride("font_color", ColorPalette.Text);
		scoresToggleContainer.AddChild(_toggleScoresButton);

		_savedUsersSection = new VBoxContainer();
		(_savedUsersSection as VBoxContainer).Size = new Vector2(404, 0);
		(_savedUsersSection as VBoxContainer).Alignment = BoxContainer.AlignmentMode.Center;
		_savedUsersSection.Visible = false; // Oculto por defecto
		panelStack.AddChild(_savedUsersSection);

		var savedTitle = new Label();
		savedTitle.Text = "Usuarios guardados (Top 3)";
		savedTitle.HorizontalAlignment = HorizontalAlignment.Center;
		savedTitle.AddThemeColorOverride("font_color", ColorPalette.Text);
		savedTitle.AddThemeFontSizeOverride("font_size", 18);
		_savedUsersSection.AddChild(savedTitle);

		_savedUsersList = new VBoxContainer();
		_savedUsersList.AddThemeConstantOverride("separation", 8);
		_savedUsersSection.AddChild(_savedUsersList);

		// Campo de usuario
		var usernameLabel = new Label();
		usernameLabel.Text = "Usuario:";
		usernameLabel.AddThemeColorOverride("font_color", ColorPalette.Text);
		loginContainer.AddChild(usernameLabel);

		_usernameField = new LineEdit();
		_usernameField.PlaceholderText = "Ingresa tu nombre de usuario";
		_usernameField.Size = new Vector2(380, 38);
		_usernameField.AddThemeColorOverride("font_color", ColorPalette.Text);
		_usernameField.AddThemeColorOverride("placeholder_color", ColorPalette.Text.Darkened(0.35f));
		var inputStyle = new StyleBoxFlat() { BgColor = ColorPalette.InputBackground };
		inputStyle.CornerRadiusTopLeft = 8;
		inputStyle.CornerRadiusTopRight = 8;
		inputStyle.CornerRadiusBottomLeft = 8;
		inputStyle.CornerRadiusBottomRight = 8;
		inputStyle.BorderColor = ColorPalette.Accent;
		inputStyle.BorderWidthTop = 1;
		inputStyle.BorderWidthBottom = 1;
		inputStyle.BorderWidthLeft = 1;
		inputStyle.BorderWidthRight = 1;
		_usernameField.AddThemeStyleboxOverride("normal", inputStyle);
		var inputHover = inputStyle.Duplicate() as StyleBoxFlat;
		inputHover.BgColor = ColorPalette.InputBackground.Lightened(0.06f);
		_usernameField.AddThemeStyleboxOverride("hover", inputHover);
		var inputFocus = inputStyle.Duplicate() as StyleBoxFlat;
		inputFocus.BorderColor = ColorPalette.Accent.Lightened(0.2f);
		inputFocus.BorderWidthTop = 2;
		inputFocus.BorderWidthBottom = 2;
		inputFocus.BorderWidthLeft = 2;
		inputFocus.BorderWidthRight = 2;
		_usernameField.AddThemeStyleboxOverride("focus", inputFocus);
		_usernameField.AddThemeFontSizeOverride("font_size", 16);
		loginContainer.AddChild(_usernameField);

		// Campo de contraseña
		var passwordLabel = new Label();
		passwordLabel.Text = "Contraseña:";
		passwordLabel.AddThemeColorOverride("font_color", ColorPalette.Text);
		loginContainer.AddChild(passwordLabel);

		_passwordField = new LineEdit();
		_passwordField.PlaceholderText = "Ingresa tu contraseña";
		_passwordField.Secret = true;
		_passwordField.Size = new Vector2(380, 38);
		_passwordField.AddThemeColorOverride("font_color", ColorPalette.Text);
		_passwordField.AddThemeStyleboxOverride("normal", inputStyle);
		_passwordField.AddThemeStyleboxOverride("hover", inputHover);
		_passwordField.AddThemeStyleboxOverride("focus", inputFocus);
		_passwordField.AddThemeColorOverride("placeholder_color", ColorPalette.Text.Darkened(0.35f));
		_passwordField.AddThemeFontSizeOverride("font_size", 16);
		loginContainer.AddChild(_passwordField);

		// Botones: Login + Crear cuenta
		var buttons = new HBoxContainer();
		buttons.Size = new Vector2(404, 48);
		buttons.AddThemeConstantOverride("separation", 12);
		loginContainer.AddChild(buttons);

		_loginButton = new Button();
		_loginButton.Text = "INICIAR SESIÓN";
		_loginButton.Size = new Vector2(192, 48);
		var btnNormal = new StyleBoxFlat() { BgColor = ColorPalette.Button };
		btnNormal.CornerRadiusTopLeft = 8;
		btnNormal.CornerRadiusTopRight = 8;
		btnNormal.CornerRadiusBottomLeft = 8;
		btnNormal.CornerRadiusBottomRight = 8;
		btnNormal.BorderColor = ColorPalette.Accent;
		btnNormal.BorderWidthTop = 1;
		btnNormal.BorderWidthBottom = 1;
		btnNormal.BorderWidthLeft = 1;
		btnNormal.BorderWidthRight = 1;
		var btnHover = btnNormal.Duplicate() as StyleBoxFlat;
		btnHover.BgColor = ColorPalette.Button.Lightened(0.08f);
		var btnPressed = btnNormal.Duplicate() as StyleBoxFlat;
		btnPressed.BgColor = ColorPalette.Button.Darkened(0.08f);
		_loginButton.AddThemeStyleboxOverride("normal", btnNormal);
		_loginButton.AddThemeStyleboxOverride("hover", btnHover);
		_loginButton.AddThemeStyleboxOverride("pressed", btnPressed);
		_loginButton.AddThemeColorOverride("font_color", ColorPalette.Text);
		_loginButton.AddThemeFontSizeOverride("font_size", 16);
		buttons.AddChild(_loginButton);

		_createButton = new Button();
		_createButton.Text = "CREAR CUENTA";
		_createButton.Size = new Vector2(192, 48);
		_createButton.AddThemeStyleboxOverride("normal", btnNormal);
		_createButton.AddThemeStyleboxOverride("hover", btnHover);
		_createButton.AddThemeStyleboxOverride("pressed", btnPressed);
		_createButton.AddThemeColorOverride("font_color", ColorPalette.Text);
		_createButton.AddThemeFontSizeOverride("font_size", 16);
		buttons.AddChild(_createButton);

		// Label de estado
		_statusLabel = new Label();
		_statusLabel.Text = "";
		_statusLabel.AutowrapMode = TextServer.AutowrapMode.WordSmart;
		_statusLabel.Size = new Vector2(380, 64);
		_statusLabel.AddThemeColorOverride("font_color", ColorPalette.Text);
		_statusLabel.AddThemeFontSizeOverride("font_size", 16);
		loginContainer.AddChild(_statusLabel);


		// --- SECCIÓN DE USUARIO LOGUEADO ---
		var userContainer = new VBoxContainer();
		userContainer.Size = new Vector2(404, 200);
		userContainer.Alignment = BoxContainer.AlignmentMode.Center;
		panelStack.AddChild(userContainer);

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
		_toggleScoresButton.Pressed += OnToggleScoresPressed;
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

		// Mostrar botón de puntajes solo cuando NO está logueado
		_toggleScoresButton.Visible = !isLoggedIn;
		// La sección se renderiza si está oculto el login
		if (!isLoggedIn)
		{
			RenderSavedUsersTop3();
		}

		if (isLoggedIn)
		{
			var user = AuthService.CurrentUser;
			_userInfoLabel.Text = $"👤 Usuario: {user.Username}\n🏆 Record: {user.HighScore}\n📅 Último acceso: {user.LastLogin:yyyy-MM-dd HH:mm}";
		}
	}

	private void OnToggleScoresPressed()
	{
		_savedUsersSection.Visible = !_savedUsersSection.Visible;
		if (_savedUsersSection.Visible)
		{
			RenderSavedUsersTop3();
		}
	}

	private void RenderSavedUsersTop3()
	{
		// Limpiar lista actual
		foreach (Node child in _savedUsersList.GetChildren())
		{
			child.QueueFree();
		}

		var users = AuthService.GetSavedUsers();
		if (users == null || !users.Any())
		{
			var emptyLabel = new Label();
			emptyLabel.Text = "Sin usuarios guardados aún";
			emptyLabel.HorizontalAlignment = HorizontalAlignment.Center;
			emptyLabel.AddThemeColorOverride("font_color", ColorPalette.Text.Darkened(0.2f));
			_savedUsersList.AddChild(emptyLabel);
			return;
		}

		// Ordenar por puntaje descendente y tomar top 3
		foreach (var u in users
			.OrderByDescending(x => x.HighScore)
			.ThenByDescending(x => x.LastLogin)
			.Take(3))
		{
			var row = new HBoxContainer();
			row.AddThemeConstantOverride("separation", 12);
			_savedUsersList.AddChild(row);

			var nameLabel = new Label();
			nameLabel.Text = $"👤 {u.Username}";
			nameLabel.Size = new Vector2(150, 24);
			nameLabel.AddThemeColorOverride("font_color", ColorPalette.Text);
			row.AddChild(nameLabel);

			var scoreLabel = new Label();
			scoreLabel.Text = $"🏆 {u.HighScore}";
			scoreLabel.Size = new Vector2(80, 24);
			scoreLabel.AddThemeColorOverride("font_color", ColorPalette.Text);
			row.AddChild(scoreLabel);

			var lastLabel = new Label();
			lastLabel.Text = $"📅 {u.LastLogin:yyyy-MM-dd HH:mm}";
			lastLabel.Size = new Vector2(140, 24);
			lastLabel.AddThemeColorOverride("font_color", ColorPalette.Text.Darkened(0.1f));
			row.AddChild(lastLabel);

			var useButton = new Button();
			useButton.Text = "Usar";
			useButton.Size = new Vector2(60, 28);
			useButton.AddThemeStyleboxOverride("normal", new StyleBoxFlat(){ BgColor = ColorPalette.Button });
			useButton.AddThemeColorOverride("font_color", ColorPalette.Text);
			row.AddChild(useButton);

			useButton.Pressed += () => {
				PreFillCredentials(u.Username);
			};
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
