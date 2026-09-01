using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PocketTown.Core;
using PocketTown.World;

namespace PocketTown.Entities;

/// <summary>
/// Base class for anything that walks the tile grid Pokemon-style: it occupies exactly
/// one tile, and moves smoothly between adjacent tiles. While moving, both the source
/// and target tile count as occupied.
/// </summary>
public abstract class Entity
{
    /// <summary>Tile currently occupied (source tile while a step is in progress).</summary>
    public Point Tile { get; protected set; }

    /// <summary>Tile being stepped onto; equals <see cref="Tile"/> when idle.</summary>
    public Point MoveTarget { get; protected set; }

    public bool IsMoving { get; private set; }
    public Direction Facing { get; set; } = Direction.Down;

    /// <summary>Pixel position of the top-left of the occupied tile (interpolated during steps).</summary>
    public Vector2 Position { get; protected set; }

    public string SpriteId { get; set; } = "villager";

    /// <summary>Movement speed in tiles per second.</summary>
    protected float Speed = Constants.WalkSpeed;

    private float _moveProgress;
    private float _walkCycle;
    private static readonly int[] WalkFrames = { 0, 1, 2, 1 };

    public void SnapTo(Point tile)
    {
        Tile = tile;
        MoveTarget = tile;
        Position = new Vector2(tile.X, tile.Y) * Constants.TileSize;
        IsMoving = false;
        _moveProgress = 0f;
        _walkCycle = 0f;
    }

    /// <summary>Occupies the given tile, either standing on it or stepping onto it.</summary>
    public bool Occupies(Point tile) => Tile == tile || MoveTarget == tile;

    /// <summary>Face <paramref name="direction"/> and begin a step if the target tile is free.</summary>
    protected bool TryStartMove(Direction direction, Func<Point, bool> isBlocked)
    {
        Facing = direction;
        var target = Tile + direction.ToPoint();
        if (isBlocked(target))
            return false;

        MoveTarget = target;
        IsMoving = true;
        _moveProgress = 0f;
        return true;
    }

    public virtual void Update(float dt)
    {
        if (!IsMoving)
        {
            _walkCycle = 0f;
            return;
        }

        _moveProgress += dt * Speed;
        _walkCycle += dt * Speed;

        if (_moveProgress >= 1f)
        {
            Tile = MoveTarget;
            Position = new Vector2(Tile.X, Tile.Y) * Constants.TileSize;
            IsMoving = false;
            _moveProgress = 0f;
            OnArrived();
        }
        else
        {
            Position = Vector2.Lerp(
                new Vector2(Tile.X, Tile.Y) * Constants.TileSize,
                new Vector2(MoveTarget.X, MoveTarget.Y) * Constants.TileSize,
                _moveProgress);
        }
    }

    /// <summary>Called when a step completes and the entity has arrived on <see cref="Tile"/>.</summary>
    protected virtual void OnArrived() { }

    public virtual void Draw(SpriteBatch sb, TileMap map)
    {
        var sheet = Art.CharacterSheet(SpriteId);

        int frame = IsMoving ? WalkFrames[(int)(_walkCycle * 2.5f) % 4] : 1;
        int row = Facing switch
        {
            Direction.Down => 0,
            Direction.Up => 1,
            _ => 2, // left sheet; flipped for right
        };
        var effects = Facing == Direction.Right ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
        var src = new Rectangle(frame * Art.CharWidth, row * Art.CharHeight, Art.CharWidth, Art.CharHeight);

        // The 16x24 sprite's feet sit on the bottom of the occupied tile.
        var drawPos = new Vector2(Position.X, Position.Y - (Art.CharHeight - Constants.TileSize));
        float feetY = Position.Y + Constants.TileSize;
        sb.Draw(sheet, drawPos, src, Color.White, 0f, Vector2.Zero, 1f, effects, map.DepthFor(feetY));
    }
}
