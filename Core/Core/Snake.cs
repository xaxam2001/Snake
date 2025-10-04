namespace Core;

public class Snake
{
    private Game? _game;
    
    private const int BASE_DIFFICULTY_INTERVAL = 135; // speed in milliseconds/cell
    private const float EASY_DIFFICULTY_MULTIPLIER = 1.66f;
    private const float HARD_DIFFICULTY_MULTIPLIER = 0.66f;
    
    public enum Difficulty {Easy = 0, Normal = 1, Hard = 2}
    private Difficulty _difficulty;
    
    public int[] Initialize(int gameSize, int difficulty = 1)
    {
        _difficulty = (Difficulty)difficulty;
        
        _game = new Game(gameSize);
        
        return _game.GetState();
    }

    public int[] DoAction(bool[] action)
    {
        if (_game == null)
            throw new InvalidOperationException("Snake not initialized.");
        
        _game.DoAction(action);
        
        return _game.GetState();
    }
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