using Godot;

public partial class ScoreManager : Node, IInitializable
{
	private uint _currentScore;
	private uint _highScore;
	private UserManager _userManager;
	private bool _isInitialized = false;

	[Signal] public delegate void ScoreChangedEventHandler(uint score);
	[Signal] public delegate void HighScoreChangedEventHandler(uint highScore);

	public uint CurrentScore => _currentScore;
	public uint HighScore => _highScore;

	public override void _Ready()
	{
		// No inicializar aquí, esperar a que GameManager llame Initialize con UserManager
	}

	/// <summary>
	/// Inicializa el ScoreManager con el UserManager inyectado
	/// </summary>
	public void Initialize()
	{
		Initialize(null);
	}

	/// <summary>
	/// Inicializa el ScoreManager con el UserManager inyectado (DIP)
	/// </summary>
	public void Initialize(UserManager userManager)
	{
		if (_isInitialized) return;
		
		_userManager = userManager;
		_userManager = userManager;

		if (_userManager != null)
		{
			_userManager.HighScoreUpdated += OnUserHighScoreUpdated;
			GD.Print("✅ ScoreManager inicializado con UserManager.");
		}
		else
		{
			GD.Print("⚠️ ScoreManager inicializado sin UserManager (modo standalone).");
		}

		LoadHighScore();
		_isInitialized = true;
	}

	public void AddScore(uint points)
	{
		_currentScore += points;
		EmitSignal(SignalName.ScoreChanged, _currentScore);

		// Actualizar high score
		CheckAndUpdateHighScore();
	}

	public void ResetScore()
	{
		_currentScore = 0;
		EmitSignal(SignalName.ScoreChanged, _currentScore);
	}

	private void CheckAndUpdateHighScore()
	{
		bool newRecord = false;

		// Si hay un usuario logueado, usar su sistema
		if (_userManager != null && _userManager.IsUserLoggedIn())
		{
			newRecord = _userManager.UpdateUserScore(_currentScore);
			_highScore = _userManager.GetUserHighScore();
		}
		else
		{
			// Fallback al sistema anterior
			if (_currentScore > _highScore)
			{
				_highScore = _currentScore;
				newRecord = true;
				SaveHighScore();
			}
		}

		if (newRecord)
		{
			EmitSignal(SignalName.HighScoreChanged, _highScore);
			GD.Print($"🏆 ¡NUEVO RECORD! {_highScore}");
		}
	}

	private void LoadHighScore()
	{
		// Si hay usuario logueado, usar su high score
		if (_userManager != null && _userManager.IsUserLoggedIn())
		{
			_highScore = _userManager.GetUserHighScore();
		}
		else
		{
			// Fallback al sistema anterior
			_highScore = SaveService.LoadHighScore();
		}

		EmitSignal(SignalName.HighScoreChanged, _highScore);
	}

	private void SaveHighScore()
	{
		// Solo guardar en el sistema anterior si no hay usuario logueado
		if (_userManager == null || !_userManager.IsUserLoggedIn())
		{
			SaveService.SaveHighScore(_highScore);
		}
	}

	private void OnUserHighScoreUpdated(uint newScore)
	{
		_highScore = newScore;
		EmitSignal(SignalName.HighScoreChanged, _highScore);
	}

	/// <summary>
	/// Refrescar el high score cuando cambie el usuario
	/// </summary>
	public void RefreshHighScore()
	{
		LoadHighScore();
	}
}
