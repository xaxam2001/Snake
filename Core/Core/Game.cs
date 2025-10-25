namespace Core;

public class Game
{
    private readonly Vector2Int _initialSnakePosition = new (1, 3); // int the top left corner, with one cell gap from the border (facing right)
    
    public enum Element {Empty, SnakeBody, SnakeHead, Apple}; // different elements in the game
        

    private readonly Element[,] _game; // 2D array of elements representing the game
    private int Size => _game.GetLength(0);
    
    private Queue<Vector2Int> _snake; // snake is represented as a queue of positions
    private Vector2Int _snakeHead; 
    private Vector2Int _apple;
    
    private GameState _state;
    private bool[] _lastAction = [false, true, false, false]; // keep track of the last action performed by the player to keep the snake moving in the same direction
    
    private static readonly Random Random = new();
    
    private bool _isGameOver;
    
    public Game(int size, int difficulty = 1)
    {
        _isGameOver = false; // reinitialize the game
        
        _game = new Element[size, size]; // initialize the game with empty cells (default value)
        _snake = new Queue<Vector2Int>(); // initialize the snake with one cell
        
        _snakeHead = _initialSnakePosition; // add the head to the snake
        
        // add the tail to the snake
        _snake.Enqueue(new Vector2Int(1,2));
        _snake.Enqueue(_snakeHead);
        
        SpawnApple(); // spawn first apple
        
        // populate the game with the snake and apple
        _game[_snake.Peek().X, _snake.Peek().Y] = Element.SnakeBody;
        _game[_snakeHead.X, _snakeHead.Y] = Element.SnakeHead;
        _game[_apple.X, _apple.Y] = Element.Apple;

        // create the first state vector
        _state = new GameState(size);
    }

    /// <summary>
    /// Make a move in the game. Tries to move the snake in the direction of the action. If the action is invalid, the snake will not move.
    /// If the snake hits an obstacle, the game is over. If the snake eats an apple, a new apple is spawned.
    /// </summary>
    /// <param name="action"> `bool[]` of size 4 that represent the actions using URDL [up, right, down, left]</param>
    /// <exception cref="ArgumentException"></exception>
    public void DoAction(bool[] action)
    {
        // check if the action is valid
        if (action.Length != 4) throw new ArgumentException("Action must be length 4");
  
        // if there are no actions, or we can't perform the action, we use the last one, otherwise we use the new one and save it
        if (!(action[0] || action[1] || action[2] || action[3]) || !CanPerformAction(action)) action = _lastAction;
        else _lastAction = action;
        
        Vector2Int nextPosition = _snakeHead; // initialize the next position to the head
        
        // the action is URDL (up, right, down, left)
        // next position is the head position plus the action vector
        if (action[0]) nextPosition += new Vector2Int(-1, 0);
        else if (action[1]) nextPosition += new Vector2Int(0, 1);
        else if (action[2]) nextPosition += new Vector2Int(1, 0);
        else if (action[3]) nextPosition += new Vector2Int(0, -1);

        // check if the next position finishes the game
        if (IsGameOver(nextPosition))
        {
            _isGameOver = true;
            return;
        }
        
        // otherwise, grow the snake
        GrowSnake(nextPosition);
        
        // if it was an apple, spawn a new one, otherwise remove the tail (to make it move)
        if (nextPosition == _apple)
        {
            SpawnApple();
        }
        else
        {
            Vector2Int tail = _snake.Dequeue();
            _game[tail.X, tail.Y] = Element.Empty;
        }
    }

    /// <summary>
    /// Expands the snake's body by adding the specified position to its head and updates the game state accordingly.
    /// </summary>
    /// <param name="nextPosition">The new position to be added as the snake's head, which reflects the direction of movement.</param>
    private void GrowSnake(Vector2Int nextPosition)
    {
        _game[_snakeHead.X, _snakeHead.Y] = Element.SnakeBody;
        _snakeHead = nextPosition;
        _snake.Enqueue(_snakeHead);
        _game[_snakeHead.X, _snakeHead.Y] = Element.SnakeHead;
    }

    /// <summary>
    /// Determines whether the provided action can be performed based on the game's current state.
    /// Prevents the snake from moving in the opposite direction of its last move to avoid self-collision.
    /// </summary>
    /// <param name="action">A boolean array of size 4 representing desired movement directions in the order [up, right, down, left].</param>
    /// <returns>True if the action is not opposite to the last performed action, otherwise false.</returns>
    private bool CanPerformAction(bool[] action)
    {
        // check if the action is opposite to the last one (the snake can't move backwards)
        bool isOpposite = 
            (action[0] && _lastAction[2]) || // Up vs Down
            (action[1] && _lastAction[3]) || // Right vs Left
            (action[2] && _lastAction[0]) || // Down vs Up
            (action[3] && _lastAction[1]);   // Left vs Right

        return !isOpposite;
    }

    /// <summary>
    /// Determines if the game is over based on the given next position of the snake.
    /// The game ends if the snake moves out of bounds or collides with itself.
    /// </summary>
    /// <param name="nextPosition">The next position of the snake's head in the game grid.</param>
    /// <returns>Returns true if the game is over; otherwise, false.</returns>
    private bool IsGameOver(Vector2Int nextPosition)
    {
        if (nextPosition.X < 0 || nextPosition.X >= Size || nextPosition.Y < 0 || nextPosition.Y >= Size) return true;
        return IsSnakeOccupied(nextPosition); // if the snake hits itself, the game is over
    }

    /// <summary>
    /// Spawns an apple at a random empty position on the game board. If no empty positions are available,
    /// the game is marked as over due to the snake occupying all possible cells.
    /// </summary>
    private void SpawnApple()
    {
        List<Vector2Int> emptyCells = new(); // list of empty cells

        // get all empty cells
        for (int y = 0; y < Size; y++)
        {
            for (int x = 0; x < Size; x++)
            {
                Vector2Int pos = new Vector2Int(x, y);
                if (!IsSnakeOccupied(pos)) emptyCells.Add(pos); // only add empty cells to the list
            }
        }

        // if there are empty cells, spawn an apple at a random position
        if (emptyCells.Count > 0)
        {
            int index = Random.Next(emptyCells.Count);
            _apple = emptyCells[index];
            _game[_apple.X, _apple.Y] = Element.Apple;
        }
        else
        {
            // Game over: snake fills the game
            _isGameOver = true;
        }
    }

    /// <summary>
    /// Determines if the specified position is occupied by any part of the snake.
    /// </summary>
    /// <param name="pos">The position to check, represented as a <c>Vector2Int</c>.</param>
    /// <returns><c>true</c> if the snake occupies the position; otherwise, <c>false</c>.</returns>
    private bool IsSnakeOccupied(Vector2Int pos)
    {
        return _snake.Contains(pos);
    }

    /// <summary>
    /// Retrieves the current state of the game. The state includes whether the game is over, the player's score,
    /// and a representation of the game grid, unwrapped into a 1D array.
    /// </summary>
    /// <returns>An integer array representing the current game state, where the first element indicates if the game is over,
    /// the second element is the score, and the rest represents the game grid.</returns>
    public int[] GetState()
    {
        _state.UpdateGameState(_isGameOver, _snake.Count-2, _game);
        return _state.State;
    }
}