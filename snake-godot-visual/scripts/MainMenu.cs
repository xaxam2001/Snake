namespace Snake.scripts;

using Godot;

public partial class MainMenu : Node2D
{
	// UI elements
	private Button _easyButton;
	private Button _normalButton;
	private Button _hardButton;
	
	private CheckBox _mlCheckBox;
	private LineEdit _modelPath;
	
	private GameManager _manager; // reference to the GameManager
	private AIPlayerManager _aiManager;
	
	public override void _Ready()
	{
		// get the different nodes
		_easyButton = GetNode<Button>("EasyModeButton");
		_normalButton = GetNode<Button>("NormalModeButton");
		_hardButton = GetNode<Button>("HardModeButton");
		
		_mlCheckBox = GetNode<CheckBox>("MLCheckBox");
		_modelPath = GetNode<LineEdit>("ModelPath");
		
		_manager = GetNode<GameManager>("%GameManager");
		_aiManager = GetNode<AIPlayerManager>("%AIPlayerManager");

		// register the button click events
		_easyButton.ButtonDown += () => { StartGame(0); };

		_normalButton.ButtonDown += () => { StartGame(1); };

		_hardButton.ButtonDown += () => { StartGame(2); };
		
		_mlCheckBox.ButtonPressed = _manager.EnableAI;
		
		_modelPath.Text = _aiManager.modelPath;
		_modelPath.Visible = _mlCheckBox.ButtonPressed;
		
		// register the checkbox callback
		_mlCheckBox.Toggled += (enabled) =>
		{
			_modelPath.Visible = enabled;
			_aiManager.modelPath = _modelPath.Text;
			_manager.EnableAI = enabled;
		};
	}

	/// <summary>
	/// Starts the game with the specified difficulty level.
	/// </summary>
	/// <param name="difficulty">The difficulty level to initialize the game with. Can be Easy, Normal, or Hard.</param>
	private void StartGame(int difficulty)
	{
		_manager.SetDifficulty(difficulty);
		_manager.StartGame();
		Hide();
	}
}
