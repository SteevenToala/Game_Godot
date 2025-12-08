using Godot;

/// <summary>
/// Representa al jugador con responsabilidades enfocadas en coordinación de componentes
/// Principio SOLID: SRP - Coordina componentes, no maneja input directamente
/// Patrón: Composite - Compuesto de Health, Movement, PlayerInputHandler
/// </summary>
public partial class Player : CharacterBody2D, IDamageable, IInitializable
{
	private Node2D _muzzle;
	private PackedScene _laserScene;
	private Health _healthComponent;
	private Movement _movementComponent;
	private Timer _fireRateTimer;
	
	// Delegación de responsabilidades
	private PlayerInputHandler _inputHandler;
	private CommandInvoker _commandInvoker;
	private Vector2 _currentMovementDirection = Vector2.Zero;

	[Export(PropertyHint.Range, "100,1000,1,or_greater")]
	public int FireRate { get; set; } = Constants.DefaultFireRate;

	[Signal] public delegate void LaserShotEventHandler(PackedScene laserScene, Vector2 location);
	[Signal] public delegate void KilledEventHandler();

	// Implementación de IDamageable
	public bool IsAlive => _healthComponent?.IsAlive ?? false;
	public int CurrentHealth => _healthComponent?.CurrentHealth ?? 0;
	public int MaxHealth => _healthComponent?.MaxHealth ?? 0;

	public override void _Ready()
	{
		Initialize();
	}

	public virtual void Initialize()
	{
		AddToGroup(Constants.PlayerGroup);
		
		_muzzle = GetNode<Node2D>("Muzzle");
		_laserScene = ResourceLoader.Load<PackedScene>(Constants.LaserScenePath);
		
		_healthComponent = GetNode<Health>("Health");
		_movementComponent = GetNode<Movement>("Movement");
		_fireRateTimer = GetNode<Timer>("FireRateTimer");
		
		// Inicializar CommandInvoker
		InitializeCommandSystem();
		
		// Inicializar InputHandler como componente separado
		InitializeInputHandler();
		
		if (_healthComponent != null)
		{
			_healthComponent.SetMaxHealth(Constants.DefaultPlayerHealth);
			_healthComponent.Died += OnDied;
		}
		
		if (_movementComponent != null)
		{
			_movementComponent.SetMovementParameters(Constants.PlayerSpeed, Vector2.Zero);
			_movementComponent.SetMovementStrategy(new PlayerMovementStrategy());
		}
		
		if (_fireRateTimer != null)
		{
			_fireRateTimer.WaitTime = FireRate / 1000.0;
			_fireRateTimer.OneShot = true;
		}
	}
	
	/// <summary>
	/// Inicializa el sistema de comandos
	/// </summary>
	private void InitializeCommandSystem()
	{
		_commandInvoker = new CommandInvoker();
		_commandInvoker.Name = "CommandInvoker";
		AddChild(_commandInvoker);
	}
	
	/// <summary>
	/// Inicializa el componente de input handler (SRP)
	/// </summary>
	private void InitializeInputHandler()
	{
		_inputHandler = new PlayerInputHandler();
		_inputHandler.Name = "PlayerInputHandler";
		AddChild(_inputHandler);
		_inputHandler.Initialize(this, _commandInvoker);
		
		// Conectar señal de disparo
		_inputHandler.ShootRequested += OnShootRequested;
	}

	public override void _Process(double delta)
	{
		// El input ahora es manejado por PlayerInputHandler
		// Solo procesamos comandos encolados
		_commandInvoker?.ProcessQueuedCommands();
	}

	public override void _PhysicsProcess(double delta)
	{
		HandleMovement();
	}
	
	/// <summary>
	/// Maneja el movimiento físico del jugador
	/// </summary>
	private void HandleMovement()
	{
		if (_movementComponent != null)
		{
			_movementComponent.Direction = _currentMovementDirection;
			_movementComponent.Move(GetPhysicsProcessDeltaTime());
		}
	}
	
	/// <summary>
	/// Callback cuando el InputHandler solicita disparo
	/// </summary>
	private void OnShootRequested()
	{
		TryShoot();
	}
	
	/// <summary>
	/// Intenta disparar si el cooldown ha terminado
	/// </summary>
	private void TryShoot()
	{
		if (_fireRateTimer != null && _fireRateTimer.IsStopped())
		{
			EmitSignal(SignalName.LaserShot, _laserScene, _muzzle.GlobalPosition);
			_fireRateTimer.Start();
		}
	}

	public void TakeDamage(int damage)
	{
		_healthComponent?.TakeDamage(damage);
	}

	private void OnDied()
	{
		EmitSignal(SignalName.Killed);
		QueueFree();
	}
	
	/// <summary>
	/// Establece la dirección de movimiento (usado por comandos)
	/// </summary>
	public void SetMovementDirection(Vector2 direction)
	{
		_currentMovementDirection = direction;
	}
	
	/// <summary>
	/// Obtiene la dirección actual de movimiento
	/// </summary>
	public Vector2 GetCurrentDirection()
	{
		return _currentMovementDirection;
	}
	
	/// <summary>
	/// Limpia comandos de movimiento (útil al pausar)
	/// </summary>
	public void ClearMovementCommands()
	{
		_inputHandler?.ClearCommands();
	}
	
	/// <summary>
	/// Deshace el último movimiento (útil para debug)
	/// </summary>
	public void UndoLastMovement()
	{
		_inputHandler?.UndoLastCommand();
	}

	/// <summary>
	/// Reinicia el jugador a su estado inicial (para reiniciar juego sin recargar escena)
	/// </summary>
	public void Respawn()
	{
		if (_healthComponent != null)
		{
			_healthComponent.SetMaxHealth(Constants.DefaultPlayerHealth);
			_healthComponent.Heal(_healthComponent.MaxHealth);
		}

		_currentMovementDirection = Vector2.Zero;
		Velocity = Vector2.Zero;

		if (_movementComponent != null)
		{
			_movementComponent.SetMovementParameters(Constants.PlayerSpeed, Vector2.Zero);
		}

		GD.Print("🔄 Jugador reaparece");
	}
}
