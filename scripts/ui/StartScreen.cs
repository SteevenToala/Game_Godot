using Godot;

public partial class StartScreen : Control, IInitializable
{
	[Signal] public delegate void PlayButtonPressedEventHandler();

	private Button _playButton;
	private Label _titleLabel;
	private Panel _background;

	public override void _Ready()
	{
		Initialize();
	}

	public void Initialize()
	{
		CreateUI();
	}

	private void CreateUI()
	{
		
		_background = new Panel();
		_background.Size = new Vector2(300, 200);
		_background.Position = new Vector2(150, 350);
		var bgStyle = new StyleBoxFlat();
		bgStyle.BgColor = new Color(0.10f, 0.12f, 0.20f);
		_background.AddThemeStyleboxOverride("panel", bgStyle);
		AddChild(_background);

		// Título
		_titleLabel = new Label();
		_titleLabel.Text = "VERTICAL SHOOTER";
		_titleLabel.Position = new Vector2(200, 350);
		AddChild(_titleLabel);

		// Botón de jugar feo
		_playButton = new Button();
		_playButton.Text = "JUGAR";
		_playButton.Position = new Vector2(200, 450);
		_playButton.Size = new Vector2(140, 40);
		_playButton.Modulate = new Color(0.15f, 0.55f, 0.60f);
		
		AddChild(_playButton);
		
		_playButton.Pressed += OnPlayButtonPressed;
	}

	private void OnPlayButtonPressed()
	{
		EmitSignal(SignalName.PlayButtonPressed);
		Hide();
	}

	public void ShowStartScreen()
	{
		Show();
		if (_playButton != null)
		{
			_playButton.GrabFocus();
		}
	}
}
