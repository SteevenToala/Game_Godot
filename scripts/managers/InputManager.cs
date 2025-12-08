using Godot;
using System;

/// <summary>
/// Responsabilidad única: Gestionar la entrada del usuario a nivel de juego
/// (no confundir con el input del jugador individual)
/// Principio SOLID: SRP - Solo maneja input de control del juego (quit, reset)
/// </summary>
public partial class InputManager : Node
{
	[Signal] public delegate void QuitRequestedEventHandler();
	[Signal] public delegate void ResetRequestedEventHandler();
	
	public override void _Process(double delta)
	{
		HandleInput();
	}
	
	private void HandleInput()
	{
		if (Input.IsActionJustPressed("quit"))
		{
			EmitSignal(SignalName.QuitRequested);
		}
		else if (Input.IsActionJustPressed("reset"))
		{
			EmitSignal(SignalName.ResetRequested);
		}
	}
}
