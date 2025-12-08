using Godot;

/// <summary>
/// Proyectil disparado por enemigos
/// Patrón: Component (usa Movement)
/// Principio SOLID: DIP - Depende de IAudioService (abstracción)
/// </summary>
public partial class EnemyProjectile : Area2D
{
	[Export] public float Speed { get; set; } = 200.0f;
	[Export] public int Damage { get; set; } = 15;
	
	private Movement _movementComponent;
	private VisibleOnScreenNotifier2D _visibilityNotifier;
	private bool _isInitialized = false;
	
	// Servicio de audio inyectado (DIP)
	private IAudioService _audioService;
	
	public override void _Ready()
	{
		// Inicializar referencias y conectar señales solo una vez
		if (!_isInitialized)
		{
			InitializeComponents();
		}
	}
	
	/// <summary>
	/// Configura los parámetros del proyectil (velocidad y daño)
	/// Este método puede llamarse múltiples veces de forma segura
	/// </summary>
	public void Initialize(float speed = 200.0f, int damage = 15)
	{
		Speed = speed;
		Damage = damage;
		
		// Actualizar parámetros de movimiento si ya existe el componente
		if (_movementComponent != null)
		{
			_movementComponent.SetMovementParameters(Speed, Vector2.Down);
		}
	}
	
	/// <summary>
	/// Inicializa componentes y conecta señales (se ejecuta solo una vez)
	/// </summary>
	private void InitializeComponents()
	{
		// Inyectar AudioService desde AutoLoad o escena (nodo "SFX")
		_audioService = GetNodeOrNull<AudioService>("/root/AudioService");
		if (_audioService == null)
		{
			_audioService = GetNode<AudioService>("/root/Game/SFX");
		}
		
		// Configurar componente de movimiento
		_movementComponent = GetNode<Movement>("Movement");
		if (_movementComponent != null)
		{
			_movementComponent.SetMovementParameters(Speed, Vector2.Down);
			_movementComponent.SetMovementStrategy(new LinearMovementStrategy());
		}
		
		// Configurar notificador de visibilidad
		_visibilityNotifier = GetNode<VisibleOnScreenNotifier2D>("VisibleOnScreenNotifier2D");
		if (_visibilityNotifier != null)
		{
			_visibilityNotifier.ScreenExited += OnScreenExited;
		}
		
		// Conectar colisiones
		AreaEntered += OnAreaEntered;
		BodyEntered += OnBodyEntered;
		
		// Añadir al grupo de proyectiles enemigos
		AddToGroup("enemy_projectiles");
		
		_isInitialized = true;
	}
	
	public override void _PhysicsProcess(double delta)
	{
		_movementComponent?.Move(delta);
	}
	
	private void OnAreaEntered(Area2D area)
	{
		// No hacer nada si colisiona con otros proyectiles enemigos o enemigos
		if (area.IsInGroup("enemy_projectiles") || area.IsInGroup(Constants.EnemyGroup))
		{
			return;
		}
		
		// Si colisiona con láseres del jugador, destruir ambos
		if (area.IsInGroup("player_lasers"))
		{
			area.QueueFree();
			DestroyProjectile();
		}
	}
	
	private void OnBodyEntered(Node2D body)
	{
		// Dañar al jugador si colisiona con él
		if (body.IsInGroup(Constants.PlayerGroup))
		{
			if (body is Player player)
			{
				player.TakeDamage(Damage);
				_audioService?.PlayHit();
			}
			
			DestroyProjectile();
		}
	}
	
	private void OnScreenExited()
	{
		QueueFree();
	}
	
	private void DestroyProjectile()
	{
		// Efecto visual pequeño (opcional)
		CreateImpactEffect();
		
		QueueFree();
	}
	
	private void CreateImpactEffect()
	{
		// Crear un pequeño efecto de partículas o animación
		// Por ahora, solo reproducir un sonido suave
		// AudioService.Instance?.PlayHit(); // Opcional: sonido más suave
	}
}
