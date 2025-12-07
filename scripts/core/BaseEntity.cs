using Godot;

public abstract partial class BaseEntity : Area2D, IGameEntity
{
	protected Health _healthComponent;
	public Movement _movementComponent;
	
	[Signal] public delegate void EntityDestroyedEventHandler(BaseEntity entity);
	
	// Implementación de IGameEntity
	public Health HealthComponent => _healthComponent;
	public Movement MovementComponent => _movementComponent;
	public bool IsActive => !IsQueuedForDeletion();
	
	// Implementación de IDamageable (a través de IGameEntity)
	public bool IsAlive => _healthComponent?.IsAlive ?? false;
	public int CurrentHealth => _healthComponent?.CurrentHealth ?? 0;
	public int MaxHealth => _healthComponent?.MaxHealth ?? 0;
	
	public virtual void Initialize()
	{
		_healthComponent = GetNode<Health>("Health");
		_movementComponent = GetNode<Movement>("Movement");
		
		if (_healthComponent != null)
		{
			_healthComponent.Died += OnDied;
		}
		
		// Configurar estrategia de movimiento por defecto
		ConfigureMovementStrategy();
	}
	
	protected virtual void ConfigureMovementStrategy()
	{
		// Las clases derivadas pueden sobrescribir esto para usar diferentes estrategias
		_movementComponent?.SetMovementStrategy(new LinearMovementStrategy());
	}
	
	public override void _Ready()
	{
		Initialize();
	}
	
	public virtual void TakeDamage(int damage)
	{
		_healthComponent?.TakeDamage(damage);
	}
	
	public virtual void OnDied()
	{
		EmitSignal(SignalName.EntityDestroyed, this);
		Destroy();
	}
	
	public virtual void Destroy()
	{
		QueueFree();
	}
	
	public virtual void OnVisibleOnScreenExited()
	{
		Destroy();
	}
}
