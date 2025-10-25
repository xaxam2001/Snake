namespace Snake.scripts;

using Godot;

public partial class DifficultyLabel : Label
{
	private GameManager _manager; // reference to the GameManager
	
	public override void _Ready()
	{
		_manager = GetNode<GameManager>("%GameManager");
		_manager.GameStarted += GameManager_GameStarted; // register the game started event
		Hide(); // hide the label until the game starts
	}

	private void GameManager_GameStarted()
	{
		// show the label when the game starts
		Show();
		Text = $"{_manager.Difficulty} mode";
	}
}
