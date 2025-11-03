namespace Core;

/// <summary>
/// Represents the main functionality and game logic for the Snake game.
/// </summary>
public class Snake
{
    private Game? _game; // Contains the game state
    
    private const int BASE_DIFFICULTY_INTERVAL = 135; // speed in milliseconds/cell
    private const float EASY_DIFFICULTY_MULTIPLIER = 1.66f; // speed multiplier for easy mode
    private const float HARD_DIFFICULTY_MULTIPLIER = 0.66f; // speed multiplier for hard mode
    
    // Difficulty levels
    public enum Difficulty {Easy = 0, Normal = 1, Hard = 2}
    private Difficulty _difficulty; // Current difficulty level
    
    // recorder
    private Recorder? _recorder;

    /// <summary>
    /// Initializes the game state by setting up the game grid, placing the snake, and spawning the first apple.
    /// </summary>
    /// <param name="gameSize">The size of the game grid, which is a square with dimensions gameSize x gameSize.</param>
    /// <param name="difficulty">The difficulty level of the game, where 0 corresponds to Easy, 1 to Normal, and 2 to Hard. Defaults to 1 (Normal).</param>
    /// <returns>An array representing the initial state of the game grid.</returns>
    public int[] Initialize(int gameSize, int difficulty = 1)
    {
        _difficulty = (Difficulty)difficulty;

        _game = new Game(gameSize);
        
        _recorder = new Recorder();
        
        return _game.GetState();
    }

    /// <summary>
    /// Executes an action on the game state based on the input provided and retrieves the updated game state.
    /// </summary>
    /// <param name="action">An array of boolean values representing the player's directional input (e.g., up, down, left, right).</param>
    /// <returns>An integer array representing the updated state of the game grid and other game-related data.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the game is not initialized before calling this method.</exception>
    public int[] DoAction(bool[] action)
    {
        // check if the game is initialized
        if (_game == null)
            throw new InvalidOperationException("Snake not initialized.");

        _recorder?.RecordState(_game.GetState(), action);
        
        _game.DoAction(action); // perform the action
        
        return _game.GetState(); // return the updated state
    }

    /// <summary>
    /// Determines the movement interval for the snake based on the current difficulty level. Should be used to set the timer interval.
    /// </summary>
    /// <returns>The movement interval in `milliseconds`. The interval increases for easier difficulties and decreases for harder difficulties.</returns>
    public int GetIntervalForDifficulty()
    {
        return _difficulty switch
        {
            Difficulty.Easy => (int)(BASE_DIFFICULTY_INTERVAL * EASY_DIFFICULTY_MULTIPLIER),
            Difficulty.Normal => BASE_DIFFICULTY_INTERVAL,
            Difficulty.Hard => (int)(BASE_DIFFICULTY_INTERVAL * HARD_DIFFICULTY_MULTIPLIER),
            _ => BASE_DIFFICULTY_INTERVAL
        };
    }
}