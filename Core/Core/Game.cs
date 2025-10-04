namespace Core;

public class Game
{
    private Vector2Int _initialPosition = new (1, 3); // int the top left corner, with one cell gap from the border (facing right)
    
    public enum Element {Empty, SnakeBody, SnakeHead, Apple};
        

    private Element[,] _game;
    private int Size => _game.GetLength(0);
    
    private Queue<Vector2Int> _snake;
    private Vector2Int _snakeHead;
    private Vector2Int _apple;
    
    private GameState _state;
    private bool[] _lastAction = [false, true, false, false];
    
    private static readonly Random Random = new();
    
    private bool _isGameOver;
    
    public Game(int size, int difficulty = 1)
    {
        _game = new Element[size, size]; // initialize the game with empty cells (default value)
        _snake = new Queue<Vector2Int>();
        
        _snakeHead = _initialPosition;
        
        // add the tail to the snake
        _snake.Enqueue(new Vector2Int(1,2));
        _snake.Enqueue(_snakeHead);
        
        SpawnApple();
        
        _game[_snake.Peek().X, _snake.Peek().Y] = Element.SnakeBody;
        _game[_snakeHead.X, _snakeHead.Y] = Element.SnakeHead;
        _game[_apple.X, _apple.Y] = Element.Apple;

        _state = new GameState(size);
    }

    public void DoAction(bool[] action)
    {
        if (action.Length != 4) throw new ArgumentException("Action must be length 4");
  
        // if there are no actions or we can't perform the action, we use the last one, otherwise we use the new one and save it
        if (!(action[0] || action[1] || action[2] || action[3]) || !CanPerformAction(action)) action = _lastAction;
        else _lastAction = action;
        
        Vector2Int nextPosition = _snakeHead;
        
        // action is URDL (up, right, down, left)
        if (action[0]) nextPosition += new Vector2Int(-1, 0);
        else if (action[1]) nextPosition += new Vector2Int(0, 1);
        else if (action[2]) nextPosition += new Vector2Int(1, 0);
        else if (action[3]) nextPosition += new Vector2Int(0, -1);

        if (IsGameOver(nextPosition))
        {
            _isGameOver = true;
            return;
        }
        
        GrowSnake(nextPosition);
        
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

    private void GrowSnake(Vector2Int nextPosition)
    {
        _game[_snakeHead.X, _snakeHead.Y] = Element.SnakeBody;
        _snakeHead = nextPosition;
        _snake.Enqueue(_snakeHead);
        _game[_snakeHead.X, _snakeHead.Y] = Element.SnakeHead;
    }

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

    private bool IsGameOver(Vector2Int nextPosition)
    {
        if (nextPosition.X < 0 || nextPosition.X >= Size || nextPosition.Y < 0 || nextPosition.Y >= Size) return true;
        return IsSnakeOccupied(nextPosition);
    }

    private void SpawnApple()
    {
        List<Vector2Int> emptyCells = new();

        for (int y = 0; y < Size; y++)
        {
            for (int x = 0; x < Size; x++)
            {
                Vector2Int pos = new Vector2Int(x, y);

                if (!IsSnakeOccupied(pos))
                {
                    emptyCells.Add(pos);
                }
            }
        }

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

    private bool IsSnakeOccupied(Vector2Int pos)
    {
        return _snake.Contains(pos);
    }
    
    public int[] GetState()
    {
        _state.UpdateGameState(_isGameOver, _snake.Count-2, _game);
        return _state.State;
    }
}