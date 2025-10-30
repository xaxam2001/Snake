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
		_easyButton.ButtonDown += () => { StartGame(0); };

		_normalButton.ButtonDown += () => { StartGame(1); };

		_hardButton.ButtonDown += () => { StartGame(2); };
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
