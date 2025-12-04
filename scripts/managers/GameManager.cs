using Godot;

public partial class GameManager : Node2D, IInitializable
{
	private Node2D _player;
	private Node2D _playerSpawnPosition;
	private Node2D _laserContainer;
	private Node2D _projectileContainer;
	private ParallaxBackground _parallaxBackground;
	private ScoreManager _scoreManager;
	private SpawnManager _spawnManager;
	private LevelManager _levelManager;
	private Hud _hud;
	private GameOverScreen _gameOverScreen;
	private AudioService _audioService;
	
	public override void _Ready()
	{
		Initialize();
	}
	
	public void Initialize()
	{
		InitializeNodes();
		InitializeManagers();
		SetupPlayer();
		LoadGameAsync();
	}
	
	public override void _Process(double delta)
	{
		HandleInput();
		AdvanceBackground((float)delta);
	}
	
	private void InitializeNodes()
	{
		_playerSpawnPosition = GetNode<Node2D>("PlayerSpawnPos");
		_laserContainer = GetNode<Node2D>("LaserContainer");
		
		// NUEVO: Contenedor para proyectiles enemigos
		_projectileContainer = GetNodeOrNull<Node2D>("ProjectileContainer");
		if (_projectileContainer == null)
		{
			_projectileContainer = new Node2D();
			_projectileContainer.Name = "ProjectileContainer";
			AddChild(_projectileContainer);
		}
		
		// Añadir al grupo para que los enemigos puedan encontrarlo
		_projectileContainer.AddToGroup("projectile_container");
		
		_parallaxBackground = GetNode<ParallaxBackground>("ParallaxBackground");
		_player = GetNode<Node2D>("Player");
		_hud = GetNode<Hud>("UILayer/HUD");
		_gameOverScreen = GetNode<GameOverScreen>("UILayer/GameOverScreen");
		
		// Crear AudioService como singleton
		_audioService = GetNode<AudioService>("SFX");
		
		// Añadir GameManager al grupo para que los enemigos puedan encontrarlo
		AddToGroup("game_manager");
	}
	
	private void InitializeManagers()
	{
		_scoreManager = GetNode<ScoreManager>("ScoreManager");
		_spawnManager = GetNode<SpawnManager>("SpawnManager");
		_levelManager = GetNodeOrNull<LevelManager>("LevelManager");
		
		// Crear LevelManager dinámicamente si no existe
		if (_levelManager == null)
		{
			_levelManager = new LevelManager();
			_levelManager.Name = "LevelManager";
			AddChild(_levelManager);
		}
		
		// ✅ CONECTAR TODOS LOS EVENTOS DEL SCOREMANAGER AL HUD
		if (_scoreManager != null)
		{
			_scoreManager.ScoreChanged += _hud.SetScore;
			_scoreManager.HighScoreChanged += _hud.SetHighScore;
			_scoreManager.ScoreChanged += OnScoreChanged;
		}
		
		// Conectar eventos del SpawnManager
		if (_spawnManager != null)
		{
			_spawnManager.EnemySpawned += OnEnemySpawned;
		}
		
		// Conexiones del LevelManager
		if (_levelManager != null)
		{
			_levelManager.LevelChanged += OnLevelChanged;
			_levelManager.DifficultyUpdated += OnDifficultyUpdated;
		}
		
		// ✅ FORZAR ACTUALIZACIÓN INICIAL DEL HUD CON VALORES ACTUALES
		if (_scoreManager != null && _hud != null)
		{
			_hud.SetScore(_scoreManager.CurrentScore);
			_hud.SetHighScore(_scoreManager.HighScore);
		}
		
		if (_levelManager != null && _hud != null)
		{
			_hud.SetLevel(_levelManager.CurrentLevel);
			_hud.SetNextLevelProgress(0, _levelManager.ScoreForNextLevel);
		}
	}
	
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
	
	private void HandleInput()
	{
		if (Input.IsActionJustPressed("quit"))
		{
			GetTree().Quit();
		}
		else if (Input.IsActionJustPressed("reset"))
		{
			ResetGame();
		}
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
		AudioService.Instance?.ResumeBackgroundMusic();
		GetTree().ReloadCurrentScene();
	}
	
	private void AdvanceBackground(float delta)
	{
		if (_parallaxBackground != null)
		{
			var newOffset = _parallaxBackground.ScrollOffset.Y <= 960 
				? _parallaxBackground.ScrollOffset.Y + delta * Constants.ScrollSpeed 
				: 0f;
			_parallaxBackground.ScrollOffset = new Vector2(
				_parallaxBackground.ScrollOffset.X,
				newOffset
			);
		}
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
			AudioService.Instance?.PlayExplosion();
		}
	}
	
	private void OnPlayerLaserShot(PackedScene laserScene, Vector2 location)
	{
		if (laserScene?.Instantiate() is Laser laser && _laserContainer != null)
		{
			laser.GlobalPosition = location;
			_laserContainer.AddChild(laser);
			AudioService.Instance?.PlayLaserShot();
		}
	}
	
	private async void OnPlayerKilled()
	{
		AudioService.Instance?.PlayExplosion();
		
		if (_gameOverScreen != null && _scoreManager != null)
		{
			_gameOverScreen.SetScore(_scoreManager.CurrentScore);
			_gameOverScreen.SetHighScore(_scoreManager.HighScore);
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
		GD.Print($"LEVEL UP! Nivel {newLevel}");
		_hud?.SetLevel(newLevel);
		_hud?.ShowLevelUpMessage(newLevel);
		AudioService.Instance?.PlayExplosion();
	}
	
	private void OnDifficultyUpdated(float speedMultiplier, float spawnRateMultiplier)
	{
		GD.Print($"Dificultad actualizada - Velocidad: {speedMultiplier:F1}x, Spawn Rate: {spawnRateMultiplier:F1}x");
	}
}
