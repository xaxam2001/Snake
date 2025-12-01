using System.Linq;

namespace Snake.scripts;

using Godot.Collections;
using Core;
using Godot;

public partial class GameManager : Node
{
	[Export] public Snake.Difficulty Difficulty; // used to set the difficulty of the game

	[Export] public bool EnableAI = false;
	[Export] public AIPlayerManager AIManager;
	
	private bool[] _action; // _actions represent the user's action
	private bool[] _lastAction;
	
	private Timer _timer; // timer used to control the game speed (calls the DoAction method every interval)
	private Snake _snake; // the game logic
	private int[] _gameState; // the current state of the game
	
	public int Score { get; private set; }
	public bool GameOver { get; private set; }
	
	public const int GridSize = 16; // size of the grid
	
	// Size of the game information (game over, score) + (distance from each walls) + (coordinates of apple) + (snake size)
	public const int InfoSize = 2 + 4 + 2 + 1;
	
	public override void _Ready()
	{ 
		Difficulty = Snake.Difficulty.Normal; // base difficulty is normal
		
		_snake = new Snake();
		
		// Create a Godot timer with an interval corresponding to the difficulty.
		_timer = new Timer();
		AddChild(_timer);
		_timer.Timeout += OnTimeOut;
		
		if (EnableAI && AIManager == null)
		{
			GD.Print("AIManager is not set, AI will not be enabled");
			EnableAI = false;
		}
	}

	/// <summary>
	/// Initializes the game state and starts the game loop.
	/// Resets the game score, sets the game state, and emits signals to update and start the game.
	/// Configures and starts the timer controlling game updates based on the selected difficulty.
	/// </summary>
	public void StartGame()
	{
		// reset game information
		GameOver = false;
		Score = 0;
		
		_action = new bool[4]; // set the default action to false
		_lastAction = _action;
		
		// Initialize the game state
		_gameState = _snake.Initialize(GridSize, (int) Difficulty);
		
		// Create a GameStateData object to pass to the renderer and UI
		var data = new GameStateData
		{
			State = _gameState,
			Actions = new Array<bool>(_action) 
		};
		
		// Emit a first signal to initialize the game
		EmitSignal(SignalName.GameUpdated, data);
		EmitSignal(SignalName.GameStarted); // notify the UI that the game has started
		
		_timer.WaitTime = _snake.GetIntervalForDifficulty() / 1000.0; // Godot timers are in seconds
		_timer.Start(); // start the timer
	}

	public override void _Process(double delta)
	{
		if (EnableAI) return;
		
		base._Process(delta);

		// Get the user's action from the Godot input system
		_action =
		[
			Input.IsActionPressed("move_up"),
			Input.IsActionPressed("move_right"),
			Input.IsActionPressed("move_down"),
			Input.IsActionPressed("move_left")
		];

		if (!_action.Any(b => b))
			_action = _lastAction;
		else
			_lastAction = _action;
		
	}

	/// <summary>
	/// Checks if the game is over by evaluating the current game state.
	/// If the game-over condition is met, stops the game timer and sets the GameOver flag to true.
	/// </summary>
	private void CheckGameOver()
	{
		if (_gameState![0] != 1) return; // the game is not over
		
		// game over: stop the timer and set the GameOver flag 
		_timer.Stop();
		GameOver = true;
	}

	public void SetDifficulty(int difficulty)
	{
		Difficulty = (Snake.Difficulty) difficulty;
	}

	/// <summary>
	/// Handles the timeout event fired by the game timer at regular intervals.
	/// Updates the game state, evaluates the game-ending condition, and updates the score.
	/// Emits the GameUpdated signal with the current game state and player actions.
	/// </summary>
	private void OnTimeOut()
	{
		if (GameOver) return; // the game is over
		
		// update the game state and check if the game is over
		_gameState = _snake.DoAction(_action);
		CheckGameOver();
		
		Score = _gameState[1]; // update the score
		
		var data = new GameStateData
		{
			State = _gameState,
			Actions = new Array<bool>(_action) 
		};

		// emit the signal to update the game
		EmitSignal(SignalName.GameUpdated, data);

		if (EnableAI) _action = AIManager.GetNextAction(_gameState);
	}
	
	// Signals
	[Signal]
	public delegate void GameUpdatedEventHandler(GameStateData data);
	
	[Signal]
	public delegate void GameStartedEventHandler();
}

/// <summary>
/// Represents the state of the game, encapsulating the current grid state and user actions.
/// This class is designed to provide a structure for passing game state data to other systems,
/// such as rendering or game UI components.
/// </summary>
public partial class GameStateData : RefCounted
{
	public int[] State { get; set; } = [];
	
	public Array<bool> Actions { get; set; } = []; 
}
