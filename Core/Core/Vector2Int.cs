namespace Core;

/// <summary>
/// Represents a two-dimensional vector with integer components.
/// </summary>
public readonly struct Vector2Int
{
    public int X { get; }
    public int Y { get; }

    public Vector2Int(int x, int y)
    {
        X = x;
        Y = y;
    }

    public Vector2Int()
    {
        X = 0;
        Y = 0;
    }

    // Indexer: allows access via [0] or [1]
    public int this[int index]
    {
        get
        {
            return index switch
            {
                0 => X,
                1 => Y,
                _ => throw new IndexOutOfRangeException("Index must be 0 (X) or 1 (Y).")
            };
        }
    }


    // ToString override
    public override string ToString() => $"({X}, {Y})";

    // Equality members
    public override bool Equals(object? obj)
        => obj is Vector2Int other && X == other.X && Y == other.Y;
    
    // HashCode override, needed for HashSet
    public override int GetHashCode() => HashCode.Combine(X, Y);

    // Operator overloads
    public static bool operator ==(Vector2Int a, Vector2Int b) => a.Equals(b);
    public static bool operator !=(Vector2Int a, Vector2Int b) => !a.Equals(b);
    public static Vector2Int operator +(Vector2Int a, Vector2Int b) => new (a.X + b.X, a.Y + b.Y );
}
