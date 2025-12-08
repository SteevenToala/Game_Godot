using Godot;

/// <summary>
/// Enemigo que dispara proyectiles al jugador
/// Patrón: Component (usa Health y Movement)
/// Principio SOLID: DIP - Depende de IAudioService (abstracción)
/// </summary>
public partial class ShooterEnemy : Enemy
{
	[Export] public PackedScene ProjectileScene { get; set; }
	[Export] public float ShootInterval { get; set; } = 2.0f;
	[Export] public float ProjectileSpeed { get; set; } = 200.0f;
	[Export] public int ProjectileDamage { get; set; } = 15;

	private Timer _shootTimer;
	private Node2D _muzzle;
	private bool _hasEnteredScreen = false;
	
	// Servicio de audio inyectado (DIP)
	private IAudioService _audioService;

	[Signal] public delegate void ProjectileFiredEventHandler(PackedScene projectileScene, Vector2 position, float speed, int damage);

	public override void Initialize()
	{
		base.Initialize();

		// Configuración específica del shooter
		Speed = 80.0f;
		Value = 150;
		Damage = 25;

		if (_healthComponent != null)
		{
			_healthComponent.SetMaxHealth(30);
		}

		if (_movementComponent != null)
		{
			_movementComponent.SetMovementParameters(Speed, Vector2.Down);
		}
	}

	private void SetupShooting()
	{
		// Configurar el timer de disparo
		_shootTimer = GetNode<Timer>("ShootTimer");
		if (_shootTimer != null)
		{
			_shootTimer.WaitTime = ShootInterval;
			_shootTimer.Timeout += OnShootTimerTimeout;
			_shootTimer.Start();
		}

		// Obtener el punto de disparo (muzzle)
		_muzzle = GetNodeOrNull<Node2D>("Muzzle");
		if (_muzzle == null)
		{
			GD.PrintErr("ShooterEnemy: Muzzle node not found!");
		}

		// Cargar la escena del proyectil si no está asignada
		if (ProjectileScene == null)
		{
			ProjectileScene = ResourceLoader.Load<PackedScene>("res://scenes/enemy_projectile.tscn");
		}
	}

	public override void _Ready()
	{
		base._Ready();
		
		// Inyectar AudioService desde AutoLoad o escena (nodo "SFX")
		// Esto debe hacerse en _Ready() cuando el nodo ya está en el árbol
		_audioService = GetNodeOrNull<AudioService>("/root/AudioService");
		if (_audioService == null)
		{
			_audioService = GetNodeOrNull<AudioService>("/root/Game/SFX");
		}
		
		// Configurar el sistema de disparo después de estar en el árbol
		SetupShooting();
	}
	
	protected override void ConfigureMovementStrategy()
	{
		// Los shooters usan movimiento lineal
		_movementComponent?.SetMovementStrategy(new LinearMovementStrategy());
	}

	private void OnShootTimerTimeout()
	{
		Shoot();
	}

	private void Shoot()
	{
		if (_muzzle == null || ProjectileScene == null) return;

		// Emitir señal para que el GameManager maneje la creación del proyectil
		EmitSignal(SignalName.ProjectileFired, ProjectileScene, _muzzle.GlobalPosition, ProjectileSpeed, ProjectileDamage);

		// Reproducir sonido de disparo enemigo
		_audioService?.PlayHit(); // Reutilizamos el sonido de hit
	}

	public override void _PhysicsProcess(double delta)
	{
		base._PhysicsProcess(delta);

		// Detectar cuando entra en pantalla por primera vez
		if (!_hasEnteredScreen && GlobalPosition.Y > 0)
		{
			_hasEnteredScreen = true;
			
			GetTree().CreateTimer(0.1f).Timeout += () => Shoot();
		}

		
		if (_shootTimer != null)
		{
			
			bool shouldShoot = GlobalPosition.Y > -20 && GlobalPosition.Y < 900;
			_shootTimer.Paused = !shouldShoot;
		}
	}

	protected override void OnDied()
	{
		// Detener el timer de disparo
		if (_shootTimer != null)
		{
			_shootTimer.Stop();
		}

		// Sonido específico para destrucción del shooter
		_audioService?.PlayExplosion();

		base.OnDied();
	}

	public override void OnVisibleOnScreenExited()
	{
		// Detener el timer antes de destruir
		if (_shootTimer != null)
		{
			_shootTimer.Stop();
		}

		base.OnVisibleOnScreenExited();
	}
}
