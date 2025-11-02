namespace Core;

public class GameState
{
    /// <summary>
    /// Gets the current state of the game represented as an integer array.
    /// This array encapsulates essential data of the game state, including
    /// - Game over status (1 if game is over, 0 otherwise)
    /// - Current game score
    /// - Flattened grid information where each cell describes the element present (e.g., empty, snake body, snake head, or apple)
    /// </summary>
    public int[] State { get; }

    private int GridSize { get; }  // Size parameter for the grid

    // Size of the game information (game over, score) + (distance from each wall) + (coordinates of apple) + (snake size)
    private const int INFO_SIZE = 2 + 4 + 2 + 1; 
    private int Size => GridSize * GridSize * 2 + INFO_SIZE; // Total size of the vector

    public GameState(int n)
    {
        if (n <= 0) throw new ArgumentException("N must be positive.", nameof(n)); // n must be positive
        GridSize = n;
        State = new int[Size]; // Initialize an array with a fixed size
    }

    // updated game state:
    // - game over +1
    // - score +1
    // - coordinates of apple +2
    // - distance to top edge +1
    // - distance to right edge +1
    // - distance to bottom edge +1
    // - distance to left edge +1
    // - coordinates of snake body relative to snake head +2x16x16 (has to follow the order of the snake pieces
    // and, since the vector is of fixed size, we add coordinates of 0 for padding)
    public void UpdateGameState(bool isGameOver, int score, Vector2Int applePosition, Vector2Int snakeHeadPosition, Queue<Vector2Int> snakeBody)
    {
        State[0] = isGameOver ? 1 : 0;
        State[1] = score;

        Vector2Int applePosHeadRelative =
            FromWorldCoordinateToTargetRelative(applePosition, snakeHeadPosition);
        
        // distance of snake head from borders
        State[2] = snakeHeadPosition.X; // top
        State[3] = (GridSize - 1) - snakeHeadPosition.Y; // right 
        State[4] = (GridSize - 1) - snakeHeadPosition.X; // bottom 
        State[5] = snakeHeadPosition.Y; // left
        
        State[6] = applePosHeadRelative.X;
        State[7] = applePosHeadRelative.Y;
        
        // Store the number of snake body pieces
        State[8] = snakeBody.Count;
        
        // position of the snake body relative to the head at the end of the vector
        int count = INFO_SIZE; // start to count from info size
        // snake is upside down in the queue so first element in the loop is the tail and head will be placed at last with (0,0)
        foreach (Vector2Int snakeBodyPosition in snakeBody)
        {
            Vector2Int snakeBodyPositionHeadRelative =  FromWorldCoordinateToTargetRelative(snakeBodyPosition, snakeHeadPosition);
            State[count++] = snakeBodyPositionHeadRelative.X;
            State[count++] = snakeBodyPositionHeadRelative.Y;
        }
    }

    private Vector2Int FromWorldCoordinateToTargetRelative(Vector2Int worldCoordinate, Vector2Int targetCoordinateCenter)
    {
        return worldCoordinate - targetCoordinateCenter;
    }
    
    // /// <summary>
    // /// Updates the game state with the current game over status, score, and grid elements.
    // /// </summary>
    // /// <param name="isGameOver">Indicates whether the game has ended. True if the game is over; otherwise, false.</param>
    // /// <param name="score">The current score of the game.</param>
    // /// <param name="gridElement">The 2D grid representing the game board with its elements.</param>
    // public void UpdateGameState(bool isGameOver, int score, Game.Element[,] gridElement)
    // {
    //     State[0] = isGameOver ? 1 : 0;
    //     State[1] = score;
    //     
    //     // unwrap the grid in the remaining part of the state vector
    //     for (int i = 0; i < GridSize; i++)
    //     {
    //         for (int j = 0; j < GridSize; j++)
    //         {
    //             State[INFO_SIZE + i *GridSize + j] = (int)gridElement[i, j];
    //         }
    //     }
    // }
    
}