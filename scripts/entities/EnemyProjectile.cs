using Godot;

public partial class EnemyProjectile : Area2D
{
	[Export] public float Speed { get; set; } = 200.0f;
	[Export] public int Damage { get; set; } = 15;
	
	private Movement _movementComponent;
	private VisibleOnScreenNotifier2D _visibilityNotifier;
	
	public override void _Ready()
	{
		Initialize();
	}
	
	public void Initialize(float speed = 200.0f, int damage = 15)
	{
		Speed = speed;
		Damage = damage;
		
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
				AudioService.Instance?.PlayHit();
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
