namespace Snake.scripts;

using Godot;

public partial class GameOverMenu : Node2D
{
	private GameManager _manager; // reference to the GameManager
	
	// UI elements
	private Button _restartButton;
	private Button _menuButton;
	
	// references to the main menu (to show it)
	private Node2D _mainMenu;
	
	private bool _isShowing; // used to show and hide the menu only once
	
	public override void _Ready()
	{
		// get the different nodes
		_restartButton = GetNode<Button>("RestartButton");
		_menuButton = GetNode<Button>("MenuButton");
		_manager = GetNode<GameManager>("%GameManager");
		_mainMenu = GetNode<Node2D>("%MainMenu");
		
		// register the restart button click event
		_restartButton.ButtonDown += _manager.StartGame;
		_menuButton.ButtonDown += OnMenuButtonDown;
		_manager.GameUpdated += Manager_GameUpdated;
	}
	
	private void OnMenuButtonDown()
	{
		// hide the game over menu and show the main menu
		Hide();
		_mainMenu.Show();
	}

	/// <summary>
	/// Handles updates to the game state when the game status changes,
	/// particularly dealing with game over conditions.
	/// </summary>
	private void Manager_GameUpdated(GameStateData _)
	{
		switch (_manager.GameOver)
		{
			case true when !_isShowing:
				_isShowing = true;
				Show();
				break;
			case false when _isShowing:
				_isShowing = false;
				Hide();
				break;
		}
	}
	

	
}
