using System.Text.Json;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PocketTown.Core;

namespace PocketTown.World;

/// <summary>
/// A grid of tiles loaded from a JSON map file, with collision, warps, interactables
/// and depth-sorted drawing. Flat tiles are drawn at depth ~0; tall tiles (trees) are
/// drawn as 16x32 sprites y-sorted together with entities for the 2.5D effect.
/// </summary>
public class TileMap
{
    public string Id { get; }
    public string Name => Data.Name;
    public bool Outdoor => Data.Outdoor;
    public MapData Data { get; }

    public int Width { get; }
    public int Height { get; }
    public int PixelWidth => Width * Constants.TileSize;
    public int PixelHeight => Height * Constants.TileSize;

    private readonly TileKind[,] _tiles;
    private float _animTimer;
    private int _animFrame;

    private TileMap(string id, MapData data)
    {
        Id = id;
        Data = data;

        if (data.Rows.Count == 0)
            throw new InvalidDataException($"Map '{id}' has no rows.");

        Height = data.Rows.Count;
        Width = data.Rows[0].Length;
        _tiles = new TileKind[Width, Height];

        for (int y = 0; y < Height; y++)
        {
            string row = data.Rows[y];
            if (row.Length != Width)
                throw new InvalidDataException(
                    $"Map '{id}' row {y} has length {row.Length}, expected {Width}. All rows must match.");
            for (int x = 0; x < Width; x++)
                _tiles[x, y] = TileCatalog.FromChar(row[x]);
        }
    }

    public static TileMap Load(string id)
    {
        string path = Path.Combine(AppContext.BaseDirectory, "Data", "Maps", id + ".json");
        if (!File.Exists(path))
            throw new FileNotFoundException($"Map file not found: {path}");

        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true, ReadCommentHandling = JsonCommentHandling.Skip };
        var data = JsonSerializer.Deserialize<MapData>(File.ReadAllText(path), options)
            ?? throw new InvalidDataException($"Map '{id}' could not be parsed.");
        return new TileMap(id, data);
    }

    public TileKind GetTile(int x, int y) =>
        x < 0 || y < 0 || x >= Width || y >= Height ? TileKind.Void : _tiles[x, y];

    /// <summary>True when the tile blocks movement (out-of-bounds counts as blocked).</summary>
    public bool IsBlocked(Point tile) =>
        tile.X < 0 || tile.Y < 0 || tile.X >= Width || tile.Y >= Height
        || TileCatalog.IsSolid(_tiles[tile.X, tile.Y]);

    public WarpData? GetWarpAt(Point tile) =>
        Data.Warps.FirstOrDefault(w => w.X == tile.X && w.Y == tile.Y);

    public InteractableData? GetInteractableAt(Point tile) =>
        Data.Interactables.FirstOrDefault(i => i.X == tile.X && i.Y == tile.Y);

    public void Update(float dt)
    {
        _animTimer += dt;
        if (_animTimer >= 0.5f)
        {
            _animTimer -= 0.5f;
            _animFrame ^= 1;
        }
    }

    /// <summary>
    /// Depth value for y-sorting: things with lower "feet" (larger Y) draw in front.
    /// Used by tall tiles and entities alike so they interleave correctly.
    /// </summary>
    public float DepthFor(float feetY) =>
        0.1f + 0.8f * MathHelper.Clamp(feetY / (PixelHeight + 64f), 0f, 1f);

    /// <summary>Draw inside a SpriteBatch begun with SpriteSortMode.FrontToBack.</summary>
    public void Draw(SpriteBatch sb)
    {
        int ts = Constants.TileSize;
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                var kind = _tiles[x, y];
                var pos = new Vector2(x * ts, y * ts);

                if (TileCatalog.IsTall(kind))
                {
                    // Tall tiles stand on a grass base and get y-sorted with entities.
                    sb.Draw(Art.TileFrames(TileKind.Grass)[0], pos, null, Color.White,
                        0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                    var tex = Art.TileFrames(kind)[0];
                    float feetY = (y + 1) * ts;
                    sb.Draw(tex, new Vector2(pos.X, pos.Y - (tex.Height - ts)), null, Color.White,
                        0f, Vector2.Zero, 1f, SpriteEffects.None, DepthFor(feetY));
                    continue;
                }

                var frames = Art.TileFrames(kind);
                var frame = frames[TileCatalog.IsAnimated(kind) ? _animFrame % frames.Length : 0];
                sb.Draw(frame, pos, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);

                // Tall grass blades overlap whoever stands in them (drawn just above entity feet).
                if (kind == TileKind.TallGrass)
                {
                    float overlayDepth = DepthFor((y + 1) * ts + 2);
                    sb.Draw(Art.TallGrassOverlay, pos, null, Color.White,
                        0f, Vector2.Zero, 1f, SpriteEffects.None, overlayDepth);
                }
            }
        }
    }
}
