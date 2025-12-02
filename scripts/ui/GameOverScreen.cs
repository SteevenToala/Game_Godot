using Godot;

public partial class GameOverScreen : Control, IInitializable
{
	private Label _scoreLabel;
	private Label _highScoreLabel;
	private Label _userLabel; // NUEVO: Label para mostrar el usuario

	public override void _Ready()
	{
		Initialize();
	}

	public void Initialize()
	{
		_scoreLabel = GetNode<Label>("Panel/Score");
		_highScoreLabel = GetNode<Label>("Panel/HighScore");
		
		// Aplicar colores
		GetNode<Panel>("Panel").AddThemeStyleboxOverride("panel", new StyleBoxFlat() { BgColor = ColorPalette.PanelBackground });
		_scoreLabel?.AddThemeColorOverride("font_color", ColorPalette.Text);
		_highScoreLabel?.AddThemeColorOverride("font_color", ColorPalette.Text);
		
		// Intentar obtener el label de usuario, si no existe, crearlo
		_userLabel = GetNodeOrNull<Label>("Panel/User");
		if (_userLabel == null)
		{
			_userLabel = new Label();
			_userLabel.Name = "User";
			_userLabel.Text = "Usuario: ---";
			_userLabel.Position = new Vector2(20, 120); // Ajustar según el layout
			_userLabel.AddThemeColorOverride("font_color", ColorPalette.Accent);
			GetNode<Panel>("Panel").AddChild(_userLabel);
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
		GetTree().ReloadCurrentScene();
	}
}
