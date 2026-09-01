using Microsoft.Xna.Framework;

namespace PocketTown.Core;

public enum Direction
{
    Down,
    Up,
    Left,
    Right,
}

public static class DirectionExtensions
{
    public static Point ToPoint(this Direction d) => d switch
    {
        Direction.Up => new Point(0, -1),
        Direction.Down => new Point(0, 1),
        Direction.Left => new Point(-1, 0),
        _ => new Point(1, 0),
    };

    public static Direction Opposite(this Direction d) => d switch
    {
        Direction.Up => Direction.Down,
        Direction.Down => Direction.Up,
        Direction.Left => Direction.Right,
        _ => Direction.Left,
    };

    /// <summary>Direction that looks from <paramref name="from"/> towards <paramref name="to"/>.</summary>
    public static Direction Towards(Point from, Point to)
    {
        int dx = to.X - from.X;
        int dy = to.Y - from.Y;
        if (Math.Abs(dx) > Math.Abs(dy))
            return dx < 0 ? Direction.Left : Direction.Right;
        return dy < 0 ? Direction.Up : Direction.Down;
    }

    public static Direction Parse(string? value, Direction fallback = Direction.Down) =>
        TryParse(value, out var parsed) ? parsed : fallback;

    public static bool TryParse(string? value, out Direction direction)
    {
        switch (value?.Trim().ToLowerInvariant())
        {
            case "up": direction = Direction.Up; return true;
            case "down": direction = Direction.Down; return true;
            case "left": direction = Direction.Left; return true;
            case "right": direction = Direction.Right; return true;
            case null:
            case "":
                direction = Direction.Down;
                return false;
            default:
                direction = Direction.Down;
                return false;
        }
    }

    /// <summary>Parse a map JSON facing string, or throw with the map id and field name.</summary>
    public static Direction ParseRequired(string? value, string mapId, string field)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Direction.Down;
        if (TryParse(value, out var parsed))
            return parsed;
        throw new InvalidDataException($"Map '{mapId}' has invalid {field} '{value}'. Use up, down, left, or right.");
    }
}
