using Godot;

/// <summary>
/// Responsabilidad única: Manejar el input del jugador
/// Principio SOLID: SRP - Solo se encarga de capturar y procesar input
/// Patrón: Command - Convierte input en comandos ejecutables
/// </summary>
public partial class PlayerInputHandler : Node
{
	private Player _player;
	private CommandInvoker _commandInvoker;
	
	// Comandos de movimiento reutilizables
	private MoveUpCommand _moveUpCommand;
	private MoveDownCommand _moveDownCommand;
	private MoveLeftCommand _moveLeftCommand;
	private MoveRightCommand _moveRightCommand;
	private StopCommand _stopCommand;
	
	private Vector2 _currentMovementDirection = Vector2.Zero;
	
	[Signal] public delegate void ShootRequestedEventHandler();
	
	/// <summary>
	/// Inicializa el input handler con referencia al jugador
	/// </summary>
	public void Initialize(Player player, CommandInvoker commandInvoker)
	{
		_player = player;
		_commandInvoker = commandInvoker;
		
		// Crear comandos reutilizables
		_moveUpCommand = new MoveUpCommand(_player);
		_moveDownCommand = new MoveDownCommand(_player);
		_moveLeftCommand = new MoveLeftCommand(_player);
		_moveRightCommand = new MoveRightCommand(_player);
		_stopCommand = new StopCommand(_player);
	}
	
	public override void _Process(double delta)
	{
		HandleInput();
	}
	
	/// <summary>
	/// Maneja todo el input del jugador
	/// </summary>
	private void HandleInput()
	{
		// Input de disparo
		if (Input.IsActionPressed("shoot"))
		{
			EmitSignal(SignalName.ShootRequested);
		}
		
		// Input de movimiento usando comandos
		HandleMovementCommands();
	}
	
	/// <summary>
	/// Convierte input de movimiento en comandos
	/// </summary>
	private void HandleMovementCommands()
	{
		Vector2 inputDirection = Vector2.Zero;
		
		// Detectar input de movimiento
		if (Input.IsActionPressed("move_up"))
			inputDirection.Y -= 1;
		if (Input.IsActionPressed("move_down"))
			inputDirection.Y += 1;
		if (Input.IsActionPressed("move_left"))
			inputDirection.X -= 1;
		if (Input.IsActionPressed("move_right"))
			inputDirection.X += 1;
		
		// Normalizar dirección diagonal
		if (inputDirection.Length() > 1)
			inputDirection = inputDirection.Normalized();
		
		// Ejecutar comando apropiado solo si cambió la dirección
		if (inputDirection != _currentMovementDirection)
		{
			ICommand commandToExecute = null;
			
			if (inputDirection == Vector2.Zero)
			{
				commandToExecute = _stopCommand;
			}
			else if (inputDirection == Vector2.Up)
			{
				commandToExecute = _moveUpCommand;
			}
			else if (inputDirection == Vector2.Down)
			{
				commandToExecute = _moveDownCommand;
			}
			else if (inputDirection == Vector2.Left)
			{
				commandToExecute = _moveLeftCommand;
			}
			else if (inputDirection == Vector2.Right)
			{
				commandToExecute = _moveRightCommand;
			}
			else
			{
				// Para movimientos diagonales o complejos, usar comando genérico
				commandToExecute = new MoveCommand(_player, inputDirection);
			}
			
			if (commandToExecute != null && _commandInvoker != null)
			{
				_commandInvoker.ExecuteCommand(commandToExecute);
			}
			
			_currentMovementDirection = inputDirection;
		}
	}
	
	/// <summary>
	/// Limpia todos los comandos pendientes (útil al pausar)
	/// </summary>
	public void ClearCommands()
	{
		_commandInvoker?.ClearQueue();
		_commandInvoker?.ClearHistory();
		_currentMovementDirection = Vector2.Zero;
	}
	
	/// <summary>
	/// Deshace el último comando (útil para debug)
	/// </summary>
	public void UndoLastCommand()
	{
		_commandInvoker?.UndoLastCommand();
	}
}
