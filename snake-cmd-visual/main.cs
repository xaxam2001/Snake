using snake_cmd_visual;

int difficulty = 1; // Default: Normal

if (args.Length > 0 && int.TryParse(args[0], out int parsed))
{
    if (parsed is >= 0 and <= 2)
    {
        difficulty = parsed;
    }
    else
    {
        Console.WriteLine("Invalid difficulty. Use 0 (Easy), 1 (Normal), or 2 (Hard). Defaulting to Normal.");
    }
}

GameManager game = new(difficulty);
game.Run();