namespace Core;

public class GameState
{
    public int[] State { get; }

    private int N { get; }  // Size parameter for the grid

    
    private const int INFO_SIZE = 2;
    private int Size => 2 * N * N + INFO_SIZE; // Total size of the vector

    public GameState(int n)
    {
        if (n <= 0) throw new ArgumentException("N must be positive.", nameof(n));
        N = n;
        State = new int[Size]; // Initialize array with fixed size
    }
    
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
    
    // helper method to clear the state (reset to zero)
    public void Clear()
    {
        Array.Clear(State, 0, State.Length);
    }
}