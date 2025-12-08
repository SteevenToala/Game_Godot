using Godot;

/// <summary>
/// Responsabilidad única: Inicializar y obtener referencias a nodos de la escena
/// Principio SOLID: SRP - Solo se encarga de la inicialización de nodos
/// </summary>
public partial class NodeInitializer : Node
{
	// Referencias a nodos del juego
	public Node2D Player { get; private set; }
	public Node2D PlayerSpawnPosition { get; private set; }
	public Node2D LaserContainer { get; private set; }
	public Node2D ProjectileContainer { get; private set; }
	public ParallaxBackground ParallaxBackground { get; private set; }
	public Hud Hud { get; private set; }
	public GameOverScreen GameOverScreen { get; private set; }
	public AudioService AudioService { get; private set; }
	
	// Referencias a managers
	public ScoreManager ScoreManager { get; private set; }
	public SpawnManager SpawnManager { get; private set; }
	public LevelManager LevelManager { get; private set; }
	public UserManager UserManager { get; private set; }
	
	private Node2D _parentNode;
	
	public void Initialize(Node2D parent)
	{
		_parentNode = parent;
		
		InitializeSceneNodes();
		InitializeContainers();
		InitializeManagers();
		
		// Añadir al grupo para que los enemigos puedan encontrarlo
		_parentNode.AddToGroup("game_manager");
	}
	
	private void InitializeSceneNodes()
	{
		PlayerSpawnPosition = _parentNode.GetNode<Node2D>("PlayerSpawnPos");
		LaserContainer = _parentNode.GetNode<Node2D>("LaserContainer");
		ParallaxBackground = _parentNode.GetNode<ParallaxBackground>("ParallaxBackground");
		Player = _parentNode.GetNode<Node2D>("Player");
		Hud = _parentNode.GetNode<Hud>("UILayer/HUD");
		GameOverScreen = _parentNode.GetNode<GameOverScreen>("UILayer/GameOverScreen");
		AudioService = _parentNode.GetNode<AudioService>("SFX");
	}
	
	private void InitializeContainers()
	{
		// Contenedor para proyectiles enemigos
		ProjectileContainer = _parentNode.GetNodeOrNull<Node2D>("ProjectileContainer");
		if (ProjectileContainer == null)
		{
			ProjectileContainer = new Node2D();
			ProjectileContainer.Name = "ProjectileContainer";
			_parentNode.AddChild(ProjectileContainer);
		}
		
		// Añadir al grupo para que los enemigos puedan encontrarlo
		ProjectileContainer.AddToGroup("projectile_container");
	}
	
	private void InitializeManagers()
	{
		ScoreManager = _parentNode.GetNode<ScoreManager>("ScoreManager");
		SpawnManager = _parentNode.GetNode<SpawnManager>("SpawnManager");
		LevelManager = _parentNode.GetNodeOrNull<LevelManager>("LevelManager");
		
		// Crear LevelManager dinámicamente si no existe
		if (LevelManager == null)
		{
			LevelManager = new LevelManager();
			LevelManager.Name = "LevelManager";
			_parentNode.AddChild(LevelManager);
		}
		
		// Crear UserManager si no existe
		UserManager = _parentNode.GetNodeOrNull<UserManager>("UserManager");
		if (UserManager == null)
		{
			UserManager = new UserManager();
			UserManager.Name = "UserManager";
			_parentNode.AddChild(UserManager);
		}
	}
}
