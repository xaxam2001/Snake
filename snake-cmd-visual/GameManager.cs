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
    private const int INFO_SIZE = 2;
    
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
            var key = Console.ReadKey();

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

    private void PrintRawGame()
    {
        Console.SetCursorPosition(0, 0);
        Console.WriteLine($"======= SNAKE GAME =======");
        Console.WriteLine($"Difficulty: {_difficulty}");
        Console.WriteLine($"Score: {_gameState![1]}"); // the exclamation mark check if _gameState is not null

        Console.WriteLine(_borderLine);
        for (int i = 0; i < GRID_SIZE; i++)
        {
            Console.Write(BORDER_VISUAL_ELEMENT);
            for (int j = 0; j < GRID_SIZE; j++)
            {
                int elem = _gameState[INFO_SIZE + i * GRID_SIZE + j];
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