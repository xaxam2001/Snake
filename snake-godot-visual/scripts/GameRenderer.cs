namespace Snake.scripts;

using Godot;

public partial class GameRenderer : TileMapLayer
{
	[Export] private TileSet _tileSet; // tileset for the game
	
	// different sprites for the snake head
	[Export] private Vector2I _snakeHeadTileUp;
	[Export] private Vector2I _snakeHeadTileRight;
	[Export] private Vector2I _snakeHeadTileDown;
	[Export] private Vector2I _snakeHeadTileLeft;
	
	// food and snake body sprites
	[Export] private Vector2I _snakeBodyTile;
	[Export] private Vector2I _foodTile;
	
	private GameManager _manager; // reference to the GameManager
	
	private int _atlasSourceId; // id of the atlas source (first source in the tileset)
	
	private Vector2I _lastHeadTileIndex; // last head used to keep the snake moving in the same direction
	
	public override void _Ready()
	{
		_manager = GetNode<GameManager>("%GameManager");
		_manager.GameUpdated += OnGameUpdated;
		_manager.GameStarted += OnGameStarted;
		
		// Get the first atlas source in the tileset
		if (_tileSet.GetSourceCount() > 0)
			_atlasSourceId = _tileSet.GetSourceId(0);
	}
	
	private void OnGameStarted()
	{
		_lastHeadTileIndex = _snakeHeadTileRight; // when spawning the snake, start moving right
	}
	
	private Vector2I ConvertToWorldCoord(Vector2I coord, Vector2I snakeHeadCoord)
	{
		return coord + snakeHeadCoord;
	}

	/// <summary>
	/// Handles the game state update event and updates the visual representation of the game grid.
	/// This method processes the game state data to determine and set appropriate tile representations
	/// based on the current state of the game grid and player actions.
	/// </summary>
	/// <param name="data">The current game state data, including grid state and player actions.</param>
	private void OnGameUpdated(GameStateData data)
	{
		// Clear or reuse tiles here
		ClearInnerGrid();
		
		// ======== Place the head ===========
		Vector2I snakeHeadWorld = new Vector2I(data.State[2], data.State[5]);
		
		Vector2I tileIndex = new Vector2I();
				
		// get the head tile index based on the last action performed by the player
		if (data.Actions[0]) tileIndex = _snakeHeadTileUp;
		else if (data.Actions[1]) tileIndex = _snakeHeadTileRight;
		else if (data.Actions[2]) tileIndex = _snakeHeadTileDown;
		else if (data.Actions[3]) tileIndex = _snakeHeadTileLeft;
		else tileIndex = _lastHeadTileIndex; // default to the last head
				
		_lastHeadTileIndex = tileIndex;
	
		// invert x and y because godot use a different coordinates system
		SetCell(new Vector2I(snakeHeadWorld.Y+1, snakeHeadWorld.X+1), _atlasSourceId, tileIndex);

		// ======== Place the apple ===========
		Vector2I applePos = new Vector2I(data.State[6], data.State[7]);
		applePos = ConvertToWorldCoord(applePos, snakeHeadWorld);
		
		SetCell(new Vector2I(applePos.Y+1, applePos.X+1), _atlasSourceId, _foodTile);

		// ======== Place the body ===========
		int snakeLength = data.State[8] - 1; // remove the head
		for (int i = 0; i < 2*snakeLength ; i+=2)
		{
			Vector2I snakePiece = new Vector2I(data.State[i + GameManager.InfoSize], data.State[i+GameManager.InfoSize+1]);
			snakePiece = ConvertToWorldCoord(snakePiece, snakeHeadWorld);
		
			// invert x and y because godot use a different coordinates system
			SetCell(new Vector2I(snakePiece.Y+1, snakePiece.X+1), _atlasSourceId, _snakeBodyTile);
		}
	}

	/// <summary>
	/// Clears all tiles within the inner grid of the game, resetting them to an empty state.
	/// This method iterates over the defined grid size, ensuring each cell is cleared.
	/// </summary>
	private void ClearInnerGrid()
	{
		for (int x = 1; x <= GameManager.GridSize; x++)
		{
			for (int y = 1; y <= GameManager.GridSize; y++)
			{
				SetCell(new Vector2I(x, y), -1); // -1 = empty cell
			}
		}
	}
	
}
