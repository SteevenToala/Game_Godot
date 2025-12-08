using Godot;

public partial class Movement : Node, IMovable
{
	[Export] public float Speed { get; set; } = 100.0f;
	[Export] public Vector2 Direction { get; set; } = Vector2.Down;
	
	private IMovementStrategy _movementStrategy;
	private Node2D _owner;
	
	public void Initialize()
	{
		// Inicializar el owner antes de cualquier otra operación
		if (_owner == null)
		{
			_owner = GetParent<Node2D>();
		}
	}
	
	public override void _Ready()
	{
		// Asegurar que _owner esté inicializado
		Initialize();
		
		// Estrategia por defecto
		if (_movementStrategy == null)
		{
			SetMovementStrategy(new LinearMovementStrategy());
		}
	}
	
	public void Move(double delta)
	{
		_movementStrategy?.Move(_owner, delta, Speed, Direction);
	}
	
	public void SetMovementStrategy(IMovementStrategy strategy)
	{
		// Asegurar que _owner esté inicializado antes de configurar la estrategia
		Initialize();
		
		_movementStrategy = strategy;
		_movementStrategy?.Initialize(_owner);
	}
	
	public IMovementStrategy GetMovementStrategy()
	{
		return _movementStrategy;
	}
	
	public void SetMovementParameters(float speed, Vector2 direction)
	{
		Speed = speed;
		Direction = direction;
		_movementStrategy?.SetDirection(direction);
	}
}
