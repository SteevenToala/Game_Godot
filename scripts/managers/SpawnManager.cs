using Godot;
using Godot.Collections;

public partial class SpawnManager : Node, IUpdatable, IInitializable
{
	[Export] public Array<PackedScene> EnemyScenes { get; set; } = new();
	[Export] public float BaseMinSpawnTime { get; set; } = 1.0f; // Tiempo base mínimo
	[Export] public float SpawnReduction { get; set; } = 0.001f; //dificultad

	// PROPIEDADES PARA CONTROLAR METEOROS
	[Export] public float MeteoroSpawnChance { get; set; } = 0.30f;
	[Export] public float MinTimeBetweenMeteoros { get; set; } = 4.0f;

	// PROPIEDADES PARA CONTROLAR SHOOTER ENEMIES
	[Export] public float ShooterSpawnChance { get; set; } = 0.20f;
	[Export] public float MinTimeBetweenShooters { get; set; } = 7.0f;

	private Timer _spawnTimer;
	private Node2D _enemyContainer;
	private RandomNumberGenerator _rng = new();
	private float _lastMeteoroSpawnTime = 0.0f;
	private float _lastShooterSpawnTime = 0.0f;
	private float _gameTime = 0.0f;

	// NUEVAS VARIABLES PARA SISTEMA DE NIVELES
	private LevelManager _levelManager;
	private float _currentMinSpawnTime;

	[Signal] public delegate void EnemySpawnedEventHandler(Enemy enemy);

	public override void _Ready()
	{
		Initialize();
	}

	public void Initialize()
	{
		_spawnTimer = GetNode<Timer>("SpawnTimer");
		_enemyContainer = GetParent().GetNode<Node2D>("EnemyContainer");

		// Obtener referencia al LevelManager
		_levelManager = GetParent().GetNodeOrNull<LevelManager>("LevelManager");

		// Inicializar tiempo de spawn actual
		_currentMinSpawnTime = BaseMinSpawnTime;

		if (_spawnTimer != null)
		{
			_spawnTimer.Timeout += OnSpawnTimerTimeout;
		}

		// Conectar a cambios de dificultad
		if (_levelManager != null)
		{
			_levelManager.DifficultyUpdated += OnDifficultyUpdated;
		}
	}

	public override void _Process(double delta)
	{
		UpdateLogic(delta);
		_gameTime += (float)delta;
	}

	public void UpdateLogic(double delta)
	{
		// Aplicar reducción de spawn time con modificador de nivel
		if (_spawnTimer != null && _spawnTimer.WaitTime > _currentMinSpawnTime)
		{
			_spawnTimer.WaitTime -= delta * SpawnReduction;
		}
	}

	private void OnDifficultyUpdated(float speedMultiplier, float spawnRateMultiplier)
	{
		// Ajustar el tiempo mínimo de spawn basado en el multiplicador de nivel
		_currentMinSpawnTime = _levelManager?.GetAdjustedSpawnTime(BaseMinSpawnTime) ?? BaseMinSpawnTime;

		GD.Print($"🔄 SpawnManager: Nuevo tiempo mínimo de spawn: {_currentMinSpawnTime:F2}s (Multiplicador: {spawnRateMultiplier:F1}x)");

		// Si el timer actual es mayor que el nuevo mínimo, ajustarlo inmediatamente
		if (_spawnTimer != null && _spawnTimer.WaitTime > _currentMinSpawnTime)
		{
			_spawnTimer.WaitTime = _currentMinSpawnTime;
		}
	}

	private void OnSpawnTimerTimeout()
	{
		SpawnRandomEnemy();
	}

	private void SpawnRandomEnemy()
	{
		if (EnemyScenes == null || EnemyScenes.Count == 0 || _enemyContainer == null)
			return;

		var spawnX = _rng.RandfRange(0, 540);
		var spawnPosition = new Vector2(spawnX, -10);

		// Determinar si spawear un meteoro o enemigo normal
		PackedScene selectedScene = SelectEnemyScene();

		if (selectedScene != null)
		{
			var enemy = EnemyFactory.CreateEnemy(selectedScene, spawnPosition);

			if (enemy != null)
			{
				// APLICAR MULTIPLICADOR DE VELOCIDAD POR NIVEL
				ApplyLevelDifficultyToEnemy(enemy);

				_enemyContainer.AddChild(enemy);
				EmitSignal(SignalName.EnemySpawned, enemy);

				// Actualizar tiempos según el tipo de enemigo spawneado
				string scenePath = selectedScene.ResourcePath.ToLower();

				if (enemy is MeteoroEnemy || scenePath.Contains("meteoro"))
				{
					_lastMeteoroSpawnTime = _gameTime;
					GD.Print($"🌑 Meteoro spawneado (Level {_levelManager?.CurrentLevel ?? 1}). Velocidad: {enemy.Speed:F0}");
				}
				else if (scenePath.Contains("shooter"))
				{
					_lastShooterSpawnTime = _gameTime;
					GD.Print($"🔫 Shooter spawneado (Level {_levelManager?.CurrentLevel ?? 1}). Velocidad: {enemy.Speed:F0}");
				}
			}
		}
	}

	private void ApplyLevelDifficultyToEnemy(Enemy enemy)
	{
		if (_levelManager != null && enemy != null)
		{
			// Aplicar multiplicador de velocidad del nivel
			float adjustedSpeed = _levelManager.GetAdjustedSpeed(enemy.Speed);
			enemy.Speed = adjustedSpeed;

			// Actualizar el componente de movimiento del enemigo
			if (enemy._movementComponent != null)
			{
				enemy._movementComponent.SetMovementParameters(adjustedSpeed, enemy._movementComponent.Direction);
			}
		}
	}

	private PackedScene SelectEnemyScene()
	{
		if (EnemyScenes.Count == 0) return null;

		// Clasificar enemigos por tipo
		PackedScene meteoroScene = null;
		PackedScene shooterScene = null;
		Array<PackedScene> normalEnemyScenes = new();

		foreach (PackedScene scene in EnemyScenes)
		{
			string scenePath = scene.ResourcePath.ToLower();

			// Identificar meteoro por el path de la escena
			if (scenePath.Contains("meteoro"))
			{
				meteoroScene = scene;
			}
			// Identificar shooter por el path de la escena
			else if (scenePath.Contains("shooter"))
			{
				shooterScene = scene;
			}
			else
			{
				normalEnemyScenes.Add(scene);
			}
		}

		// Decidir si spawear meteoro (primera prioridad)
		bool canSpawnMeteoro = meteoroScene != null &&
							  (_gameTime - _lastMeteoroSpawnTime) >= MinTimeBetweenMeteoros;
		bool shouldSpawnMeteoro = canSpawnMeteoro && _rng.Randf() < MeteoroSpawnChance;

		if (shouldSpawnMeteoro)
		{
			return meteoroScene;
		}

		// Decidir si spawear shooter (segunda prioridad)
		bool canSpawnShooter = shooterScene != null &&
							  (_gameTime - _lastShooterSpawnTime) >= MinTimeBetweenShooters;
		bool shouldSpawnShooter = canSpawnShooter && _rng.Randf() < ShooterSpawnChance;

		if (shouldSpawnShooter)
		{
			return shooterScene;
		}

		// Spawear enemigo normal
		if (normalEnemyScenes.Count > 0)
		{
			var randomIndex = _rng.RandiRange(0, normalEnemyScenes.Count - 1);
			return normalEnemyScenes[randomIndex];
		}
		else
		{
			// Si no hay enemigos normales, usar cualquiera (excepto shooter si no puede spawear)
			Array<PackedScene> availableScenes = new();

			if (meteoroScene != null && canSpawnMeteoro)
				availableScenes.Add(meteoroScene);
			if (shooterScene != null && canSpawnShooter)
				availableScenes.Add(shooterScene);

			if (availableScenes.Count > 0)
			{
				var randomIndex = _rng.RandiRange(0, availableScenes.Count - 1);
				return availableScenes[randomIndex];
			}
		}

		return null;
	}

	// Método para reiniciar dificultad cuando se reinicia el juego
	public void ResetDifficulty()
	{
		_currentMinSpawnTime = BaseMinSpawnTime;
		_gameTime = 0.0f;
		_lastMeteoroSpawnTime = 0.0f;
		_lastShooterSpawnTime = 0.0f;

		if (_spawnTimer != null)
		{
			_spawnTimer.WaitTime = 1.0f; // Resetear a tiempo inicial
		}
	}
}
