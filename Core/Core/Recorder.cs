namespace Core;

using System.Globalization;
using System.IO;
using CsvHelper;

public class Recorder
{
    private readonly string _path;

    public Recorder()
    {
        // path is hardcoded for now, not an issue for a small project (could be changed later using a config file, for example)
        _path = $"player-data/game-data.csv";

        // Create a directory if missing
        Directory.CreateDirectory(Path.GetDirectoryName(_path)!);

        if (File.Exists(_path)) return;
        
        using var writer = new StreamWriter(_path);
        using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);

        // Write header once
        csv.WriteField("gameOver");
        csv.WriteField("score");
        csv.WriteField("UpWallDist");
        csv.WriteField("RightWallDist");
        csv.WriteField("BottWallDist");
        csv.WriteField("LeftWallDist");
        csv.WriteField("appleX");
        csv.WriteField("appleY");
        csv.WriteField("snakeSize");

        // Snake body positions: X0,Y0 ... X255,Y255
        for (int i = 0; i <= 255; i++)
        {
            csv.WriteField($"X{i}");
            csv.WriteField($"Y{i}");
        }

        // Actions
        csv.WriteField("up");
        csv.WriteField("right");
        csv.WriteField("down");
        csv.WriteField("left");
        
        csv.NextRecord();
    }

    public void RecordState(int[] state, bool[] action)
    {
        using var writer = new StreamWriter(_path, append: true);
        using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);

        foreach (var s in state) csv.WriteField(s);
        foreach (var a in action) csv.WriteField(a ? 1 : 0);

        csv.NextRecord();
    }
}
