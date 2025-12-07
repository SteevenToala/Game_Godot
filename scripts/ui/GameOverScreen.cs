using Godot;

public partial class GameOverScreen : Control, IInitializable
{
	private Label _scoreLabel;
	private Label _highScoreLabel;
	private Label _userLabel; // Label para mostrar el usuario
	private Button _restartButton;
	private Button _logoutButton;

	public override void _Ready()
	{
		Initialize();
	}

	public void Initialize()
	{
		_scoreLabel = GetNode<Label>("Panel/Score");
		_highScoreLabel = GetNode<Label>("Panel/HighScore");
		_userLabel = GetNodeOrNull<Label>("Panel/User");
		_restartButton = GetNodeOrNull<Button>("Panel/ButtonContainer/RestartButton");
		_logoutButton = GetNodeOrNull<Button>("Panel/ButtonContainer/LogoutButton");
		
		// Aplicar colores
		GetNode<Panel>("Panel").AddThemeStyleboxOverride("panel", new StyleBoxFlat() { BgColor = ColorPalette.PanelBackground });
		_scoreLabel?.AddThemeColorOverride("font_color", ColorPalette.Text);
		_highScoreLabel?.AddThemeColorOverride("font_color", ColorPalette.Text);
		_userLabel?.AddThemeColorOverride("font_color", ColorPalette.Accent);

		// Conectar eventos
		if (_restartButton != null)
		{
			_restartButton.Pressed += OnRestartButtonPressed;
		}

		if (_logoutButton != null)
		{
			_logoutButton.Pressed += OnLogoutButtonPressed;
		}
	}

	public void SetScore(uint value)
	{
		if (_scoreLabel != null)
		{
			_scoreLabel.Text = $"Score: {value}";
		}
	}

	public void SetHighScore(uint value)
	{
		if (_highScoreLabel != null)
		{
			_highScoreLabel.Text = $"Hi-Score: {value}";
		}
	}

	public void SetUser(string username)
	{
		if (_userLabel != null)
		{
			_userLabel.Text = $"Usuario: {username}";
		}
	}

	public void OnRestartButtonPressed()
	{
		// Obtener el GameManager desde la raíz
		var root = GetTree().Root;
		var gameManager = root.GetNode<GameManager>("Game");
		
		if (gameManager != null)
		{
			gameManager.RestartGameWithoutReload();
		}
		else
		{
			// Fallback si no se encuentra GameManager
			GD.PrintErr("GameManager no encontrado");
			GetTree().ReloadCurrentScene();
		}
	}

	private void OnLogoutButtonPressed()
	{
		// Cerrar sesión
		AuthService.Logout();

		// Volver a la pantalla de login
		GetTree().ReloadCurrentScene();
	}
}
