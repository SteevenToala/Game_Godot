using Godot;

/// <summary>
/// Enemigo meteoro con movimiento serpenteante
/// Patrón: Component (usa Health y Movement)
/// Principio SOLID: DIP - Depende de IAudioService (abstracción)
/// </summary>
public partial class MeteoroEnemy : Enemy
{
	// Servicio de audio inyectado (DIP)
	private IAudioService _audioService;
	
	public override void Initialize()
	{
		base.Initialize();
		
		// Configuración específica del meteoro
		Speed = 60.0f;
		Value = 200;
		Damage = 30;
		
		if (_healthComponent != null)
		{
			_healthComponent.SetMaxHealth(50);
		}
		
		if (_movementComponent != null)
		{
			_movementComponent.SetMovementParameters(Speed, Vector2.Down);
		}
		
		// Inyectar AudioService desde AutoLoad o escena (nodo "SFX")
		_audioService = GetNodeOrNull<AudioService>("/root/AudioService");
		if (_audioService == null)
		{
			_audioService = GetNode<AudioService>("/root/Game/SFX");
		}
	}
	
	protected override void ConfigureMovementStrategy()
	{
		// Los meteoros usan movimiento serpenteante
		_movementComponent?.SetMovementStrategy(new WobbleMovementStrategy());
	}
	
	public override void _PhysicsProcess(double delta)
	{
		_movementComponent?.Move(delta);
	}
	
	protected override void OnDied()
	{
		// Efecto de sonido específico para meteoro
		_audioService?.PlayExplosion();
		
		base.OnDied();
	}
}
