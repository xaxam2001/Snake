namespace Snake.scripts;

using Godot;

public partial class MainMenu : Node2D
{
	// UI elements
	private Button _easyButton;
	private Button _normalButton;
	private Button _hardButton;
	
	private GameManager _manager; // reference to the GameManager
	
	public override void _Ready()
	{
		// get the different nodes
		_easyButton = GetNode<Button>("EasyModeButton");
		_normalButton = GetNode<Button>("NormalModeButton");
		_hardButton = GetNode<Button>("HardModeButton");
		
		_manager = GetNode<GameManager>("%GameManager");

		// register the button click events
		_easyButton.ButtonDown += () => { StartGame(Core.Snake.Difficulty.Easy); };

		_normalButton.ButtonDown += () => { StartGame(Core.Snake.Difficulty.Normal); };

		_hardButton.ButtonDown += () => { StartGame(Core.Snake.Difficulty.Hard); };
	}

	/// <summary>
	/// Starts the game with the specified difficulty level.
	/// </summary>
	/// <param name="difficulty">The difficulty level to initialize the game with. Can be Easy, Normal, or Hard.</param>
	private void StartGame(Core.Snake.Difficulty difficulty)
	{
		_manager.Difficulty = difficulty;
		_manager.StartGame();
		Hide();
	}
}
