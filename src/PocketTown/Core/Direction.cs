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
        value?.ToLowerInvariant() switch
        {
            "up" => Direction.Up,
            "down" => Direction.Down,
            "left" => Direction.Left,
            "right" => Direction.Right,
            _ => fallback,
        };
}
