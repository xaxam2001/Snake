namespace snake_cmd_visual;
using System.Collections.Generic;
using System.Timers;
using System.Text;
using Core;

public class GameManager
{
    private readonly Dictionary<int, string> _elementToString;
    private const string BORDER_VISUAL_ELEMENT = "██";
    private readonly string _borderLine;
    
    // TEMPORARY ACTIONS action is URDL (up, right, down, left)
    private bool[] _action = [false, false, false, true];
    
    private readonly Timer _timer;
    private readonly int _interval;
    private Snake _snake;
    private int[]? _gameState;
    private bool _gameOver = false;
    private readonly Snake.Difficulty _difficulty;
    
    private const int GRID_SIZE = 16;
    
    // Size of the game information (game over, score) + (distance from each walls) + (coordinates of apple) + (snake size)
    private const int INFO_SIZE = 2 + 4 + 2 + 1;
    
    public GameManager(int difficulty = 1)
    {
        _elementToString = new Dictionary<int, string>()
        {
            {0, "  "},
            {1, "❎"},
            {2, "😮"},
            {3, "🍎"}
        };
        _borderLine = "";
        for(int i = 0; i < GRID_SIZE+2; i++) _borderLine += BORDER_VISUAL_ELEMENT;
        Console.OutputEncoding = Encoding.UTF8;
        
        
        _snake = new Snake();
        
        _gameState = _snake.Initialize(GRID_SIZE, difficulty);
        _difficulty = (Snake.Difficulty) difficulty;
        
        _interval = _snake.GetIntervalForDifficulty();
        
        // Create a timer with a two second interval.
        _timer = new Timer(_interval);
        // Hook up the Elapsed event for the timer. 
        _timer.Elapsed += OnTimedEvent;
        _timer.AutoReset = true;
    }
    
    public void Run()
    {
        Console.Clear();
        Console.CursorVisible = false;
        
        PrintRawGame();
        
        _timer.Enabled = true;
        
        while (true)
        {
            var key = Console.ReadKey(true);

            _action = [false, false, false, false];
            
            int actionIndex = key.Key switch
            {
                ConsoleKey.UpArrow    or ConsoleKey.W => 0,
                ConsoleKey.RightArrow or ConsoleKey.D => 1,
                ConsoleKey.DownArrow  or ConsoleKey.S => 2,
                ConsoleKey.LeftArrow  or ConsoleKey.A => 3,
                _ => -1 // Default case if no key matches
            };
            
            if (actionIndex >= 0)
            {
                _action[actionIndex] = true;
            }

            if(_gameOver) break;
        }
        
        Console.CursorVisible = true;
    }

    private Vector2Int ConvertToWorldCoord(Vector2Int coord, Vector2Int snakeHeadCoord)
    {
        return coord + snakeHeadCoord;
    }

    private int[,] GetGameStateGrid(int[] gameState)
    {
        int[,] gameGrid = new int[GRID_SIZE, GRID_SIZE];
        
        Vector2Int snakeHeadWorld = new Vector2Int(gameState[2], gameState[5]);
        
        gameGrid[snakeHeadWorld.X, snakeHeadWorld.Y] = 2;
        
        Vector2Int applePos = new Vector2Int(gameState[6], gameState[7]);
        applePos = ConvertToWorldCoord(applePos, snakeHeadWorld);
        
        gameGrid[applePos.X, applePos.Y] = 3;
        
        int snakeLength = gameState[8] - 1; // remove the head
        for (int i = 0; i < 2*snakeLength ; i+=2)
        {
            Vector2Int snakePiece = new Vector2Int(gameState[i+ INFO_SIZE], gameState[i+INFO_SIZE+1]);
            snakePiece = ConvertToWorldCoord(snakePiece, snakeHeadWorld);
        
            gameGrid[snakePiece.X, snakePiece.Y] = 1;
        }

        return gameGrid;
    }

    private void PrintRawGame()
    {
        Console.SetCursorPosition(0, 0);
        Console.WriteLine("======= SNAKE GAME =======");
        Console.WriteLine($"Difficulty: {_difficulty}");
        Console.WriteLine($"Score: {_gameState![1]}"); // the exclamation mark check if _gameState is not null
        
        int [,] gameStateGrid = GetGameStateGrid(_gameState);

        Console.WriteLine(_borderLine);
        for (int i = 0; i < GRID_SIZE; i++)
        {
            Console.Write(BORDER_VISUAL_ELEMENT);
            for (int j = 0; j < GRID_SIZE; j++)
            {
                int elem = gameStateGrid[i, j];
                Console.Write(_elementToString[elem]);
            }
            Console.Write(BORDER_VISUAL_ELEMENT);
            Console.WriteLine();
        }
        Console.WriteLine(_borderLine);
    }

    private void CheckGameOver()
    {
        if (_gameState![0] == 1)
        {
            Console.WriteLine($"Game over!\nYour score: {_gameState[1]}\nPress any key to quit");
            _timer.Stop();
            _timer.Dispose();
            _gameOver = true;
        }
    }

    private void OnTimedEvent(Object? source, ElapsedEventArgs e)
    {
        _gameState = _snake.DoAction(_action);
        PrintRawGame();
        CheckGameOver();
    }
    
}