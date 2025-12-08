using Godot;

/// <summary>
/// Responsabilidad única: Coordinar y orquestar los diferentes managers del juego
/// Principio SOLID: SRP - Solo coordina, delega responsabilidades específicas a managers especializados
/// Principio SOLID: DIP - Depende de IAudioService (abstracción) no de AudioService (implementación)
/// Patrón: Facade - Proporciona una interfaz simplificada para los subsistemas del juego
/// Patrón: Dependency Injection - Recibe servicios inyectados
/// </summary>
public partial class GameManager : Node2D, IInitializable
{
	// Managers especializados (Patrón: Delegation)
	private NodeInitializer _nodeInitializer;
	private InputManager _inputManager;
	private BackgroundManager _backgroundManager;
	private GameStateManager _gameStateManager;
	
	// Servicio de audio inyectado (DIP - Dependency Inversion Principle)
	private IAudioService _audioService;
	
	// Referencias rápidas a elementos principales (obtenidas del NodeInitializer)
	private Node2D _player;
	private Node2D _playerSpawnPosition;
	private Node2D _laserContainer;
	private Node2D _projectileContainer;
	private ScoreManager _scoreManager;
	private SpawnManager _spawnManager;
	private LevelManager _levelManager;
	private UserManager _userManager;
	private Hud _hud;
	private GameOverScreen _gameOverScreen;
	
	public override void _Ready()
	{
		Initialize();
	}
	
	public void Initialize()
	{
		// Crear e inicializar managers especializados
		InitializeManagers();
		
		// Obtener referencias a nodos a través del NodeInitializer
		InitializeNodeReferences();
		
		// Configurar conexiones y eventos
		SetupConnections();
		SetupPlayer();
		
		// Ocultar elementos del juego inicialmente
		_gameStateManager?.HideGameElements();
		
		// Si ya hay un usuario logueado (después de recargar escena), iniciar juego automáticamente
		if (_userManager != null && _userManager.IsUserLoggedIn())
		{
			var user = _userManager.GetCurrentUser();
			GD.Print($"🔄 Reiniciando juego con usuario: {user.Username}");
			
			// Resetear puntuación y nivel para una partida nueva
			if (_scoreManager != null)
			{
				_scoreManager.ResetScore();
				_scoreManager.RefreshHighScore(); // Cargar el high score del usuario
			}
			
			_levelManager?.ResetLevel();
			_spawnManager?.ResetDifficulty();
			
			// Actualizar HUD con los datos del usuario
			if (_hud != null)
			{
				_hud.SetUser(user.Username);
				_hud.SetHighScore(user.HighScore);
			}
			
			_gameStateManager?.ShowGameElements();
			_backgroundManager?.SetActive(true);
			_audioService?.ResumeBackgroundMusic();
		}
		else
		{
			// Mostrar pantalla de login si no hay usuario logueado
			if (_userManager != null)
			{
				_userManager.ShowLoginScreen();
			}
		}
	}
	
	/// <summary>
	/// Crea e inicializa todos los managers especializados
	/// </summary>
	private void InitializeManagers()
	{
		// NodeInitializer - Obtiene todas las referencias de nodos
		_nodeInitializer = new NodeInitializer();
		_nodeInitializer.Name = "NodeInitializer";
		AddChild(_nodeInitializer);
		_nodeInitializer.Initialize(this);
		
		// AudioService - Inyección de dependencia (buscar en el árbol de nodos)
		// Intentar primero en AutoLoad, luego buscar en la escena
		_audioService = GetNodeOrNull<AudioService>("/root/AudioService");
		if (_audioService == null)
		{
			// Buscar en el árbol de la escena actual (el nodo se llama "SFX")
			_audioService = GetNodeOrNull<AudioService>("SFX");
			
			if (_audioService == null)
			{
				GD.PrintErr("⚠️ AudioService no encontrado en AutoLoad ni en la escena.");
			}
			else
			{
				GD.Print("✅ AudioService encontrado en la escena (nodo SFX).");
			}
		}
		else
		{
			GD.Print("✅ AudioService encontrado en AutoLoad.");
		}
		
		// InputManager - Maneja input del juego (quit, reset)
		_inputManager = new InputManager();
		_inputManager.Name = "InputManager";
		AddChild(_inputManager);
		
		// BackgroundManager - Maneja el scroll del parallax
		_backgroundManager = new BackgroundManager();
		_backgroundManager.Name = "BackgroundManager";
		AddChild(_backgroundManager);
		
		// GameStateManager - Maneja visibilidad y estado del juego
		_gameStateManager = new GameStateManager();
		_gameStateManager.Name = "GameStateManager";
		AddChild(_gameStateManager);
	}
	
	/// <summary>
	/// Obtiene referencias a nodos desde el NodeInitializer
	/// </summary>
	private void InitializeNodeReferences()
	{
		_player = _nodeInitializer.Player;
		_playerSpawnPosition = _nodeInitializer.PlayerSpawnPosition;
		_laserContainer = _nodeInitializer.LaserContainer;
		_projectileContainer = _nodeInitializer.ProjectileContainer;
		_hud = _nodeInitializer.Hud;
		_gameOverScreen = _nodeInitializer.GameOverScreen;
		
		_scoreManager = _nodeInitializer.ScoreManager;
		_spawnManager = _nodeInitializer.SpawnManager;
		_levelManager = _nodeInitializer.LevelManager;
		_userManager = _nodeInitializer.UserManager;
		
		// Inicializar el BackgroundManager con el parallax
		_backgroundManager?.SetParallaxBackground(_nodeInitializer.ParallaxBackground);
		
		// Inicializar el GameStateManager con los elementos del juego
		_gameStateManager?.Initialize(_player, _hud, _nodeInitializer.ParallaxBackground, _spawnManager);
	}
	
	/// <summary>
	/// Conecta todos los eventos entre managers y componentes
	/// </summary>
	private void SetupConnections()
	{
		// Conexiones del InputManager
		if (_inputManager != null)
		{
			_inputManager.QuitRequested += OnQuitRequested;
			_inputManager.ResetRequested += OnResetRequested;
		}
		
		// Conexiones del ScoreManager
		if (_scoreManager != null && _hud != null)
		{
			_scoreManager.ScoreChanged += _hud.SetScore;
			_scoreManager.HighScoreChanged += _hud.SetHighScore;
			_scoreManager.ScoreChanged += OnScoreChanged;
			
			// Forzar actualización inicial del HUD
			_hud.SetScore(_scoreManager.CurrentScore);
			_hud.SetHighScore(_scoreManager.HighScore);
		}
		
		// Conexiones del SpawnManager
		if (_spawnManager != null)
		{
			_spawnManager.EnemySpawned += OnEnemySpawned;
		}
		
		// Conexiones del LevelManager
		if (_levelManager != null)
		{
			_levelManager.LevelChanged += OnLevelChanged;
			_levelManager.DifficultyUpdated += OnDifficultyUpdated;
			
			if (_hud != null)
			{
				_hud.SetLevel(_levelManager.CurrentLevel);
				_hud.SetNextLevelProgress(0, _levelManager.ScoreForNextLevel);
			}
		}
		
		// Conexiones del UserManager
		if (_userManager != null)
		{
			_userManager.Connect(UserManager.SignalName.UserLoggedIn, new Callable(this, nameof(OnUserLoggedIn)));
			_userManager.Connect(UserManager.SignalName.UserLoggedOut, new Callable(this, nameof(OnUserLoggedOut)));
		}
	}
	
	/// <summary>
	/// Configura el jugador y sus eventos
	/// </summary>
	private void SetupPlayer()
	{
		if (_player != null && _playerSpawnPosition != null)
		{
			_player.GlobalPosition = _playerSpawnPosition.GlobalPosition;
			
			if (_player is Player player)
			{
				player.LaserShot += OnPlayerLaserShot;
				player.Killed += OnPlayerKilled;
			}
		}
	}
	
	private async void LoadGameAsync()
	{
		await ToSignal(GetTree().CreateTimer(Constants.GameLoadTimeout), 
			SceneTreeTimer.SignalName.Timeout);
	}
	
	// Manejadores de input (delegados desde InputManager)
	private void OnQuitRequested()
	{
		GetTree().Quit();
	}
	
	private void OnResetRequested()
	{
		ResetGame();
	}
	
	private void ResetGame()
	{
		// Limpiar proyectiles enemigos
		if (_projectileContainer != null)
		{
			foreach (Node child in _projectileContainer.GetChildren())
			{
				child.QueueFree();
			}
		}
		
		// Reiniciar nivel cuando se reinicia el juego
		_levelManager?.ResetLevel();
		_spawnManager?.ResetDifficulty();
		_audioService?.ResumeBackgroundMusic();
		GetTree().ReloadCurrentScene();
	}

	/// <summary>
	/// Reinicia el juego recargando la escena completamente
	/// La sesión del usuario se mantiene a través de AuthService (es un singleton static)
	/// Los puntajes se guardan en la base de datos de usuarios
	/// </summary>
	public void RestartGameWithoutReload()
	{
		GD.Print("🔄 Recargando escena del juego...");
		GetTree().ReloadCurrentScene();
	}
	
	private void OnEnemySpawned(Enemy enemy)
	{
		if (enemy != null)
		{
			enemy.Killed += OnEnemyKilled;
			
			// NUEVO: Conectar eventos de proyectiles si es un ShooterEnemy
			if (enemy is ShooterEnemy shooterEnemy)
			{
				shooterEnemy.ProjectileFired += OnEnemyProjectileFired;
			}
		}
	}
	
	// NUEVO: Método para manejar proyectiles enemigos
	private void OnEnemyProjectileFired(PackedScene projectileScene, Vector2 position, float speed, int damage)
	{
		if (projectileScene?.Instantiate() is EnemyProjectile projectile && _projectileContainer != null)
		{
			projectile.GlobalPosition = position;
			projectile.Initialize(speed, damage);
			_projectileContainer.AddChild(projectile);
		}
	}
	
	private void OnEnemyKilled(Enemy enemy)
	{
		if (enemy != null && _scoreManager != null)
		{
			_scoreManager.AddScore(enemy.Value);
			_audioService?.PlayExplosion();
		}
	}
	
	private void OnPlayerLaserShot(PackedScene laserScene, Vector2 location)
	{
		if (laserScene?.Instantiate() is Laser laser && _laserContainer != null)
		{
			laser.GlobalPosition = location;
			_laserContainer.AddChild(laser);
			_audioService?.PlayLaserShot();
		}
	}
	
	private async void OnPlayerKilled()
	{
		_audioService?.PlayExplosion();
		
		if (_gameOverScreen != null && _scoreManager != null)
		{
			_gameOverScreen.SetScore(_scoreManager.CurrentScore);
			_gameOverScreen.SetHighScore(_scoreManager.HighScore);
			
			// Mostrar información del usuario en el game over
			if (_userManager != null && _userManager.IsUserLoggedIn())
			{
				var user = _userManager.GetCurrentUser();
				_gameOverScreen.SetUser(user.Username);
			}
		}
		
		await ToSignal(GetTree().CreateTimer(Constants.PlayerDeathTimeout), 
			SceneTreeTimer.SignalName.Timeout);
		
		if (_gameOverScreen != null)
		{
			_gameOverScreen.Visible = true;
		}
	}
	
	// Métodos para el sistema de niveles
	private void OnScoreChanged(uint newScore)
	{
		_levelManager?.CheckLevelUp(newScore);
		
		if (_levelManager != null)
		{
			_hud?.SetNextLevelProgress(newScore, _levelManager.ScoreForNextLevel);
		}
	}
	
	private void OnLevelChanged(uint newLevel)
	{
		GD.Print($"🎉 ¡LEVEL UP! Nivel {newLevel}");
		_hud?.SetLevel(newLevel);
		_hud?.ShowLevelUpMessage(newLevel);
		_audioService?.PlayExplosion();
	}
	
	private void OnDifficultyUpdated(float speedMultiplier, float spawnRateMultiplier)
	{
		GD.Print($"🔧 Dificultad actualizada - Velocidad: {speedMultiplier:F1}x, Spawn Rate: {spawnRateMultiplier:F1}x");
	}
	
	private void StartGame()
	{
		// Ocultar pantalla de login
		if (_userManager != null)
		{
			_userManager.HideLoginScreen();
		}

		// Repositionar y resetear al jugador
		if (_player != null && _playerSpawnPosition != null)
		{
			_gameStateManager?.SetPlayerPosition(_playerSpawnPosition.GlobalPosition);
			if (_player is Player player)
			{
				player.Respawn();
			}
		}

		// Mostrar elementos del juego y activar background
		_gameStateManager?.ShowGameElements();
		_backgroundManager?.SetActive(true);
		
		// Refrescar high score en el ScoreManager
		if (_scoreManager != null)
		{
			_scoreManager.RefreshHighScore();
		}
		
		// Actualizar HUD con información del usuario
		if (_hud != null && _userManager != null && _userManager.IsUserLoggedIn())
		{
			var user = _userManager.GetCurrentUser();
			_hud.SetUser(user.Username);
			_hud.SetHighScore(user.HighScore);
		}
		
		// Inicializar el juego
		LoadGameAsync();
		
		GD.Print("🎮 Juego iniciado correctamente");
	}
	
	private void OnUserLoggedIn(string username)
	{
		GD.Print($"🎯 Usuario logueado en GameManager: {username}");
		StartGame();
	}
	
	private void OnUserLoggedOut()
	{
		GD.Print("👋 Usuario deslogueado en GameManager");
		
		// Ocultar elementos del juego y desactivar background
		_gameStateManager?.HideGameElements();
		_backgroundManager?.SetActive(false);
		
		// Mostrar pantalla de login
		if (_userManager != null)
		{
			_userManager.ShowLoginScreen();
		}
	}
}