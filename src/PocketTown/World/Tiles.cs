namespace PocketTown.World;

public enum TileKind
{
    // Outdoor
    Grass,
    TallGrass,
    Flowers,
    Path,
    Water,
    Tree,
    Fence,
    Sign,
    // Buildings (exterior)
    HouseWall,
    HouseRoof,
    HouseDoor,       // walkable; usually paired with a warp
    HouseDoorLocked, // solid, purely decorative
    HouseWindow,
    // Interior
    Floor,
    Rug,
    InteriorWall,
    Bed,
    Bookshelf,
    Table,
    Chair,
    Tv,
    DoorMat,         // walkable; usually paired with a warp
    Void,
}

/// <summary>
/// Maps the ASCII characters used in map files to tiles, and defines per-tile behaviour.
/// Add a new tile by extending the enum, the legend below, and its artwork in <see cref="Core.Art"/>.
/// </summary>
public static class TileCatalog
{
    public static TileKind FromChar(char c) => c switch
    {
        '.' => TileKind.Grass,
        't' => TileKind.TallGrass,
        'f' => TileKind.Flowers,
        'p' => TileKind.Path,
        'w' => TileKind.Water,
        'T' => TileKind.Tree,
        'F' => TileKind.Fence,
        's' => TileKind.Sign,
        'W' => TileKind.HouseWall,
        'R' => TileKind.HouseRoof,
        'D' => TileKind.HouseDoor,
        'd' => TileKind.HouseDoorLocked,
        'o' => TileKind.HouseWindow,
        '_' => TileKind.Floor,
        'r' => TileKind.Rug,
        '#' => TileKind.InteriorWall,
        'b' => TileKind.Bed,
        'B' => TileKind.Bookshelf,
        'a' => TileKind.Table,
        'c' => TileKind.Chair,
        'v' => TileKind.Tv,
        'm' => TileKind.DoorMat,
        'x' => TileKind.Void,
        _ => throw new InvalidDataException($"Unknown tile character '{c}' in map file."),
    };

    /// <summary>Tiles that block movement.</summary>
    public static bool IsSolid(TileKind kind) => kind is
        TileKind.Water or TileKind.Tree or TileKind.Fence or TileKind.Sign or
        TileKind.HouseWall or TileKind.HouseRoof or TileKind.HouseDoorLocked or TileKind.HouseWindow or
        TileKind.InteriorWall or TileKind.Bed or TileKind.Bookshelf or TileKind.Table or
        TileKind.Chair or TileKind.Tv or TileKind.Void;

    /// <summary>
    /// Tall tiles are drawn as 16x32 sprites and y-sorted with entities,
    /// which is what produces the 2.5D depth effect (walk in front of / behind them).
    /// </summary>
    public static bool IsTall(TileKind kind) => kind is
        TileKind.Tree or TileKind.HouseWall or TileKind.HouseDoor or TileKind.HouseDoorLocked
        or TileKind.HouseWindow or TileKind.Bed or TileKind.Bookshelf or TileKind.Tv;

    /// <summary>Tiles whose artwork animates (frame flip every half second).</summary>
    public static bool IsAnimated(TileKind kind) => kind is TileKind.Water or TileKind.Flowers;
}
