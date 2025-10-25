namespace Snake.scripts;

using Godot;

public partial class ScoreLabel : Label
{
	private GameManager _manager; // reference to the GameManager

	public override void _Ready()
	{
		_manager = GetNode<GameManager>("%GameManager"); // get the GameManager node
		_manager.GameUpdated += Manager_GameUpdated; // register the game updated event
	}

	private void Manager_GameUpdated(GameStateData _)
	{
		Text = _manager.Score.ToString();
	}
}
