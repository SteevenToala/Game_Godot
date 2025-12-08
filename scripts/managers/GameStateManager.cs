using Godot;

/// <summary>
/// Responsabilidad única: Gestionar la visibilidad y estado activo de los elementos del juego
/// Principio SOLID: SRP - Solo maneja mostrar/ocultar elementos y el estado del juego
/// </summary>
public partial class GameStateManager : Node
{
	private Node2D _player;
	private Hud _hud;
	private ParallaxBackground _parallaxBackground;
	private SpawnManager _spawnManager;
	private bool _gameActive = false;
	
	[Signal] public delegate void GameStateChangedEventHandler(bool isActive);
	
	public bool IsGameActive => _gameActive;
	
	/// <summary>
	/// Inicializa las referencias a los elementos del juego
	/// </summary>
	public void Initialize(Node2D player, Hud hud, ParallaxBackground parallaxBackground, SpawnManager spawnManager)
	{
		_player = player;
		_hud = hud;
		_parallaxBackground = parallaxBackground;
		_spawnManager = spawnManager;
	}
	
	/// <summary>
	/// Oculta todos los elementos del juego y pausa el spawning
	/// </summary>
	public void HideGameElements()
	{
		// Ocultar player
		if (_player != null)
		{
			_player.Visible = false;
		}

		// Ocultar HUD
		if (_hud != null)
		{
			_hud.Visible = false;
		}

		// Ocultar background (opcional)
		if (_parallaxBackground != null)
		{
			_parallaxBackground.Visible = false;
		}

		// Pausar spawning
		if (_spawnManager != null)
		{
			var spawnTimer = _spawnManager.GetNodeOrNull<Timer>("SpawnTimer");
			if (spawnTimer != null)
			{
				spawnTimer.Stop();
			}
		}

		_gameActive = false;
		EmitSignal(SignalName.GameStateChanged, false);
		
		GD.Print("🔒 Elementos del juego ocultos - Esperando login");
	}

	/// <summary>
	/// Muestra todos los elementos del juego y activa el spawning
	/// </summary>
	public void ShowGameElements()
	{
		// Mostrar player
		if (_player != null)
		{
			_player.Visible = true;
			GD.Print($"👾 Player mostrado en posición: {_player.GlobalPosition}");
		}

		// Mostrar HUD
		if (_hud != null)
		{
			_hud.Visible = true;
		}

		// Mostrar background
		if (_parallaxBackground != null)
		{
			_parallaxBackground.Visible = true;
		}

		// Iniciar spawning
		if (_spawnManager != null)
		{
			var spawnTimer = _spawnManager.GetNodeOrNull<Timer>("SpawnTimer");
			if (spawnTimer != null)
			{
				// Reiniciar el timer a su valor inicial y arrancarlo
				spawnTimer.WaitTime = 2.0f; // Tiempo inicial configurado en la escena
				spawnTimer.Stop();
				spawnTimer.Start();
			}
		}

		_gameActive = true;
		EmitSignal(SignalName.GameStateChanged, true);
		
		GD.Print("🔓 Elementos del juego mostrados - Juego activo");
	}
	
	/// <summary>
	/// Reposiciona el jugador a una posición específica
	/// </summary>
	public void SetPlayerPosition(Vector2 position)
	{
		if (_player != null)
		{
			_player.GlobalPosition = position;
		}
	}
}
