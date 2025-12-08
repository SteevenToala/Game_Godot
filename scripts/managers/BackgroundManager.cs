using Godot;

/// <summary>
/// Responsabilidad única: Gestionar el movimiento del fondo parallax
/// Principio SOLID: SRP - Solo maneja el scroll del background
/// </summary>
public partial class BackgroundManager : Node
{
	private ParallaxBackground _parallaxBackground;
	private bool _isActive = false;
	
	[Export] public float ScrollSpeed { get; set; } = Constants.ScrollSpeed;
	
	public override void _Ready()
	{
		// Intentar obtener el ParallaxBackground del padre
		_parallaxBackground = GetParent().GetNodeOrNull<ParallaxBackground>("ParallaxBackground");
		
		if (_parallaxBackground == null)
		{
			GD.PrintErr("BackgroundManager: No se encontró ParallaxBackground");
		}
	}
	
	public void SetParallaxBackground(ParallaxBackground background)
	{
		_parallaxBackground = background;
	}
	
	public void SetActive(bool active)
	{
		_isActive = active;
		
		if (_parallaxBackground != null)
		{
			_parallaxBackground.Visible = active;
		}
	}
	
	public bool IsActive => _isActive;
	
	public override void _Process(double delta)
	{
		if (_isActive)
		{
			AdvanceBackground((float)delta);
		}
	}
	
	private void AdvanceBackground(float delta)
	{
		if (_parallaxBackground != null)
		{
			var newOffset = _parallaxBackground.ScrollOffset.Y <= 960 
				? _parallaxBackground.ScrollOffset.Y + delta * ScrollSpeed 
				: 0f;
			_parallaxBackground.ScrollOffset = new Vector2(
				_parallaxBackground.ScrollOffset.X,
				newOffset
			);
		}
	}
}
