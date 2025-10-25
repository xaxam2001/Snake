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

    private int N { get; }  // Size parameter for the grid

    
    private const int INFO_SIZE = 2; // Size of the game information (game over, score)
    private int Size => 2 * N * N + INFO_SIZE; // Total size of the vector

    public GameState(int n)
    {
        if (n <= 0) throw new ArgumentException("N must be positive.", nameof(n)); // n must be positive
        N = n;
        State = new int[Size]; // Initialize an array with a fixed size
    }

    /// <summary>
    /// Updates the game state with the current game over status, score, and grid elements.
    /// </summary>
    /// <param name="isGameOver">Indicates whether the game has ended. True if the game is over; otherwise, false.</param>
    /// <param name="score">The current score of the game.</param>
    /// <param name="gridElement">The 2D grid representing the game board with its elements.</param>
    public void UpdateGameState(bool isGameOver, int score, Game.Element[,] gridElement)
    {
        State[0] = isGameOver ? 1 : 0;
        State[1] = score;
        
        // unwrap the grid in the remaining part of the state vector
        for (int i = 0; i < N; i++)
        {
            for (int j = 0; j < N; j++)
            {
                State[INFO_SIZE + i * N + j] = (int)gridElement[i, j];
            }
        }
    }
    
}