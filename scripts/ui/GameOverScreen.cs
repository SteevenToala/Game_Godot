using Godot;
using System.Collections.Generic;

public partial class GameOverScreen : Control, IInitializable
{
	private Label _scoreLabel;
	private Label _highScoreLabel;
	private Label _userLabel; // Label para mostrar el usuario
	private Label _topScoresLabel; // Label para mostrar los top 3
	private Button _restartButton;
	private Button _logoutButton;
	private UserDatabaseService _userDatabase;

	public override void _Ready()
	{
		Initialize();
	}

	public void Initialize()
	{
		_scoreLabel = GetNode<Label>("Panel/Score");
		_highScoreLabel = GetNode<Label>("Panel/HighScore");
		_userLabel = GetNodeOrNull<Label>("Panel/User");
		_topScoresLabel = GetNodeOrNull<Label>("Panel/TopScores");
		_restartButton = GetNodeOrNull<Button>("Panel/ButtonContainer/RestartButton");
		_logoutButton = GetNodeOrNull<Button>("Panel/ButtonContainer/LogoutButton");
		
		// Debug: verificar que se encontró TopScores
		if (_topScoresLabel == null)
		{
			GD.PrintErr("❌ TopScores label no encontrado en Panel/TopScores");
		}
		else
		{
			GD.Print("✅ TopScores label encontrado");
		}
		
		// Inicializar servicio de base de datos
		_userDatabase = new UserDatabaseService();
		if (_userDatabase == null)
		{
			GD.PrintErr("❌ UserDatabaseService no se inicializó");
		}
		else
		{
			GD.Print("✅ UserDatabaseService inicializado");
		}
		
		// Aplicar colores
		GetNode<Panel>("Panel").AddThemeStyleboxOverride("panel", new StyleBoxFlat() { BgColor = ColorPalette.PanelBackground });
		_scoreLabel?.AddThemeColorOverride("font_color", ColorPalette.Text);
		_highScoreLabel?.AddThemeColorOverride("font_color", ColorPalette.Text);
		_userLabel?.AddThemeColorOverride("font_color", ColorPalette.Accent);
		_topScoresLabel?.AddThemeColorOverride("font_color", ColorPalette.Text);

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
		
		// Actualizar top scores cuando se establece el usuario
		UpdateTopScores();
	}

	private void UpdateTopScores()
	{
		if (_topScoresLabel == null)
		{
			GD.PrintErr("❌ UpdateTopScores: _topScoresLabel es null");
			return;
		}
		
		if (_userDatabase == null)
		{
			GD.PrintErr("❌ UpdateTopScores: _userDatabase es null");
			return;
		}

		var topUsers = _userDatabase.GetTopScores(3);
		
		GD.Print($"📊 Actualizando top scores. Encontrados {topUsers.Count} usuarios");
		
		var topScoresText = "🏆 TOP 3:\n";
		
		if (topUsers.Count == 0)
		{
			topScoresText += "Sin registros";
		}
		else
		{
			for (int i = 0; i < topUsers.Count; i++)
			{
				var user = topUsers[i];
				var medal = i switch
				{
					0 => "🥇",
					1 => "🥈",
					2 => "🥉",
					_ => "  "
				};
				topScoresText += $"{medal} {user.Username}: {user.HighScore}\n";
				GD.Print($"  {i+1}. {user.Username}: {user.HighScore}");
			}
		}
		
		_topScoresLabel.Text = topScoresText;
		GD.Print("✅ Top scores actualizados");
	}

	public void OnRestartButtonPressed()
	{
		// Validar que el nodo esté en el árbol antes de usar GetTree()
		if (!IsInsideTree())
		{
			GD.PrintErr("GameOverScreen no está en el árbol al intentar reiniciar");
			return;
		}
		
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
		// Validar que el nodo esté en el árbol antes de cualquier operación
		if (!IsInsideTree())
		{
			GD.PrintErr("GameOverScreen no está en el árbol al intentar logout");
			return;
		}
		
		// Guardar referencia al SceneTree antes de hacer logout
		var sceneTree = GetTree();
		
		// Cerrar sesión
		AuthService.Logout();

		// Volver a la pantalla de login (si el nodo aún existe)
		if (IsInsideTree() && sceneTree != null)
		{
			sceneTree.ReloadCurrentScene();
		}
	}
}
