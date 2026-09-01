using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PocketTown.World;

namespace PocketTown.Core;

/// <summary>
/// All placeholder artwork is generated in code at startup (no content pipeline, no asset
/// files). Swap any texture here for real art later - the rest of the game only asks for
/// <see cref="TileFrames"/> and <see cref="CharacterSheet"/>.
///
/// Character sheets are 3 columns x 3 rows of 16x24 cells:
///   columns = walk frame (step A, idle, step B), rows = facing (down, up, left).
/// The right-facing sprite is the left row flipped horizontally at draw time.
/// </summary>
public static class Art
{
    public const int CharWidth = 16;
    public const int CharHeight = 24;

    public static Texture2D Pixel { get; private set; } = null!;
    public static Texture2D TallGrassOverlay { get; private set; } = null!;

    private static readonly Dictionary<TileKind, Texture2D[]> Tiles = new();
    private static readonly Dictionary<string, Texture2D> Characters = new();

    public static Texture2D[] TileFrames(TileKind kind) => Tiles[kind];

    public static Texture2D CharacterSheet(string id) =>
        Characters.TryGetValue(id, out var sheet) ? sheet : Characters["villager"];

    // --- Palette -------------------------------------------------------------

    private static readonly Color GrassBase = new(116, 180, 92);
    private static readonly Color GrassDark = new(100, 164, 78);
    private static readonly Color GrassLight = new(132, 196, 108);
    private static readonly Color BladeDark = new(64, 128, 60);
    private static readonly Color PathBase = new(214, 194, 150);
    private static readonly Color PathDark = new(194, 174, 130);
    private static readonly Color WaterBase = new(56, 128, 212);
    private static readonly Color WaterLight = new(124, 180, 240);
    private static readonly Color WoodMid = new(164, 118, 70);
    private static readonly Color WoodDark = new(126, 88, 50);
    private static readonly Color WallBase = new(224, 198, 150);
    private static readonly Color WallLine = new(202, 174, 126);
    private static readonly Color RoofBase = new(198, 84, 72);
    private static readonly Color RoofDark = new(158, 62, 54);
    private static readonly Color FloorBase = new(206, 168, 120);
    private static readonly Color FloorLine = new(182, 144, 98);
    private static readonly Color IntWallBase = new(196, 182, 162);
    private static readonly Color IntWallLine = new(174, 158, 136);
    private static readonly Color IntWallTrim = new(148, 128, 104);

    private record CharPalette(Color Skin, Color Hair, Color Shirt, Color Pants, Color Shoes);

    private static readonly Dictionary<string, CharPalette> Palettes = new()
    {
        ["hero"] = new(new(240, 200, 162), new(92, 58, 40), new(64, 106, 192), new(56, 64, 92), new(62, 42, 34)),
        ["mom"] = new(new(240, 200, 162), new(170, 90, 50), new(216, 122, 152), new(126, 82, 108), new(90, 56, 48)),
        ["elder"] = new(new(232, 192, 156), new(204, 204, 204), new(98, 130, 90), new(88, 88, 82), new(52, 44, 40)),
        ["girl"] = new(new(244, 206, 168), new(216, 170, 76), new(202, 74, 74), new(238, 238, 238), new(120, 60, 50)),
        ["boy"] = new(new(236, 196, 158), new(44, 44, 52), new(224, 170, 66), new(82, 112, 74), new(56, 48, 40)),
        ["villager"] = new(new(238, 198, 160), new(122, 82, 58), new(142, 142, 152), new(92, 92, 102), new(58, 50, 44)),
    };

    // --- Loading -------------------------------------------------------------

    public static void Load(GraphicsDevice device)
    {
        Pixel = Make(device, 1, 1, p => p.Px(0, 0, Color.White));

        Tiles[TileKind.Grass] = new[] { Make(device, 16, 16, DrawGrass) };
        Tiles[TileKind.TallGrass] = new[] { Make(device, 16, 16, DrawTallGrass) };
        Tiles[TileKind.Flowers] = new[]
        {
            Make(device, 16, 16, p => DrawFlowers(p, 0)),
            Make(device, 16, 16, p => DrawFlowers(p, 1)),
        };
        Tiles[TileKind.Path] = new[] { Make(device, 16, 16, DrawPath) };
        Tiles[TileKind.Water] = new[]
        {
            Make(device, 16, 16, p => DrawWater(p, 0)),
            Make(device, 16, 16, p => DrawWater(p, 1)),
        };
        Tiles[TileKind.Tree] = new[] { Make(device, 16, 32, DrawTree) };
        Tiles[TileKind.Fence] = new[] { Make(device, 16, 16, DrawFence) };
        Tiles[TileKind.Sign] = new[] { Make(device, 16, 16, DrawSign) };
        Tiles[TileKind.HouseWall] = new[] { Make(device, 16, 16, DrawHouseWall) };
        Tiles[TileKind.HouseRoof] = new[] { Make(device, 16, 16, DrawHouseRoof) };
        Tiles[TileKind.HouseDoor] = new[] { Make(device, 16, 16, p => DrawHouseDoor(p, false)) };
        Tiles[TileKind.HouseDoorLocked] = new[] { Make(device, 16, 16, p => DrawHouseDoor(p, true)) };
        Tiles[TileKind.HouseWindow] = new[] { Make(device, 16, 16, DrawHouseWindow) };
        Tiles[TileKind.Floor] = new[] { Make(device, 16, 16, DrawFloor) };
        Tiles[TileKind.Rug] = new[] { Make(device, 16, 16, DrawRug) };
        Tiles[TileKind.InteriorWall] = new[] { Make(device, 16, 16, DrawInteriorWall) };
        Tiles[TileKind.Bed] = new[] { Make(device, 16, 16, DrawBed) };
        Tiles[TileKind.Bookshelf] = new[] { Make(device, 16, 16, DrawBookshelf) };
        Tiles[TileKind.Table] = new[] { Make(device, 16, 16, DrawTable) };
        Tiles[TileKind.Chair] = new[] { Make(device, 16, 16, DrawChair) };
        Tiles[TileKind.Tv] = new[] { Make(device, 16, 16, DrawTv) };
        Tiles[TileKind.DoorMat] = new[] { Make(device, 16, 16, DrawDoorMat) };
        Tiles[TileKind.Void] = new[] { Make(device, 16, 16, p => p.Rect(0, 0, 16, 16, new Color(18, 18, 26))) };

        TallGrassOverlay = Make(device, 16, 16, DrawTallGrassOverlayTex);

        foreach (var (id, palette) in Palettes)
            Characters[id] = Make(device, CharWidth * 3, CharHeight * 3, p => DrawCharacterSheet(p, palette));
    }

    private static Texture2D Make(GraphicsDevice device, int w, int h, Action<Painter> draw)
    {
        var painter = new Painter(w, h);
        draw(painter);
        var tex = new Texture2D(device, w, h);
        tex.SetData(painter.Data);
        return tex;
    }

    // --- Tile painters -------------------------------------------------------

    private static void DrawGrass(Painter p)
    {
        p.Rect(0, 0, 16, 16, GrassBase);
        var rng = new Random(1234);
        for (int i = 0; i < 14; i++)
            p.Px(rng.Next(16), rng.Next(16), GrassDark);
        for (int i = 0; i < 8; i++)
            p.Px(rng.Next(16), rng.Next(16), GrassLight);
    }

    private static void DrawTallGrass(Painter p)
    {
        DrawGrass(p);
        // Clumps of tall blades.
        foreach (int x in new[] { 1, 4, 7, 10, 13 })
        {
            p.VLine(x, 4, 11, BladeDark);
            p.VLine(x + 1, 6, 9, new Color(84, 148, 72));
        }
    }

    private static void DrawTallGrassOverlayTex(Painter p)
    {
        // Transparent except lower blades; drawn over entities standing in tall grass.
        foreach (int x in new[] { 1, 4, 7, 10, 13 })
        {
            p.VLine(x, 9, 6, BladeDark);
            p.VLine(x + 1, 11, 4, new Color(84, 148, 72));
        }
    }

    private static void DrawFlowers(Painter p, int frame)
    {
        DrawGrass(p);
        DrawFlower(p, 3, 3, frame);
        DrawFlower(p, 10, 9, 1 - frame);
    }

    private static void DrawFlower(Painter p, int x, int y, int frame)
    {
        var petal = new Color(224, 84, 96);
        var center = new Color(248, 216, 96);
        int sway = frame == 0 ? 0 : 1;
        p.Px(x + sway, y, petal);
        p.Px(x + 2 + sway, y, petal);
        p.Px(x + 1 + sway, y - 1, petal);
        p.Px(x + 1 + sway, y + 1, petal);
        p.Px(x + 1 + sway, y, center);
    }

    private static void DrawPath(Painter p)
    {
        p.Rect(0, 0, 16, 16, PathBase);
        var rng = new Random(77);
        for (int i = 0; i < 12; i++)
            p.Px(rng.Next(16), rng.Next(16), PathDark);
    }

    private static void DrawWater(Painter p, int frame)
    {
        p.Rect(0, 0, 16, 16, WaterBase);
        int shift = frame * 3;
        p.HLine(2 + shift, 3, 4, WaterLight);
        p.HLine(9 - shift, 8, 4, WaterLight);
        p.HLine(4 + shift, 13, 4, WaterLight);
    }

    private static void DrawTree(Painter p)
    {
        var canopyDark = new Color(44, 112, 58);
        var canopyLight = new Color(74, 150, 82);
        // Trunk.
        p.Rect(6, 24, 4, 8, WoodDark);
        p.VLine(6, 24, 8, new Color(96, 64, 38));
        // Canopy (roughly round).
        p.Rect(5, 2, 6, 2, canopyDark);
        p.Rect(3, 4, 10, 2, canopyDark);
        p.Rect(1, 6, 14, 12, canopyDark);
        p.Rect(2, 18, 12, 4, canopyDark);
        p.Rect(4, 22, 8, 3, canopyDark);
        // Highlights.
        p.Rect(4, 5, 4, 3, canopyLight);
        p.Rect(3, 9, 3, 4, canopyLight);
        p.Rect(9, 8, 4, 3, canopyLight);
        p.Rect(6, 14, 4, 3, canopyLight);
    }

    private static void DrawFence(Painter p)
    {
        DrawGrass(p);
        p.Rect(0, 6, 16, 2, WoodMid);
        p.Rect(0, 10, 16, 2, WoodMid);
        p.Rect(2, 4, 2, 9, WoodDark);
        p.Rect(12, 4, 2, 9, WoodDark);
    }

    private static void DrawSign(Painter p)
    {
        DrawGrass(p);
        p.Rect(7, 8, 2, 6, WoodDark);
        p.Rect(2, 2, 12, 7, WoodMid);
        p.RectOutline(2, 2, 12, 7, WoodDark);
        p.HLine(4, 4, 8, WoodDark);
        p.HLine(4, 6, 6, WoodDark);
    }

    private static void DrawHouseWall(Painter p)
    {
        p.Rect(0, 0, 16, 16, WallBase);
        p.HLine(0, 4, 16, WallLine);
        p.HLine(0, 9, 16, WallLine);
        p.HLine(0, 14, 16, WallLine);
        p.HLine(0, 15, 16, new Color(184, 156, 108));
    }

    private static void DrawHouseRoof(Painter p)
    {
        p.Rect(0, 0, 16, 16, RoofBase);
        for (int y = 3; y < 16; y += 4)
            p.HLine(0, y, 16, RoofDark);
        // Staggered shingle seams.
        for (int y = 0; y < 16; y += 4)
        {
            int offset = (y / 4) % 2 == 0 ? 4 : 10;
            p.VLine(offset, y, 3, RoofDark);
        }
    }

    private static void DrawHouseDoor(Painter p, bool locked)
    {
        DrawHouseWall(p);
        var door = locked ? new Color(96, 62, 38) : new Color(122, 82, 48);
        p.Rect(3, 2, 10, 14, door);
        p.RectOutline(3, 2, 10, 14, new Color(74, 46, 26));
        p.Px(11, 9, new Color(232, 200, 96));
    }

    private static void DrawHouseWindow(Painter p)
    {
        DrawHouseWall(p);
        p.Rect(3, 4, 10, 8, new Color(240, 240, 236));
        p.Rect(4, 5, 8, 6, new Color(110, 156, 208));
        p.VLine(7, 5, 6, new Color(240, 240, 236));
        p.HLine(4, 7, 8, new Color(240, 240, 236));
    }

    private static void DrawFloor(Painter p)
    {
        p.Rect(0, 0, 16, 16, FloorBase);
        p.HLine(0, 3, 16, FloorLine);
        p.HLine(0, 7, 16, FloorLine);
        p.HLine(0, 11, 16, FloorLine);
        p.HLine(0, 15, 16, FloorLine);
        p.VLine(4, 0, 4, FloorLine);
        p.VLine(11, 4, 4, FloorLine);
        p.VLine(6, 8, 4, FloorLine);
        p.VLine(13, 12, 4, FloorLine);
    }

    private static void DrawRug(Painter p)
    {
        p.Rect(0, 0, 16, 16, new Color(206, 98, 98));
        p.RectOutline(0, 0, 16, 16, new Color(166, 66, 66));
        p.RectOutline(2, 2, 12, 12, new Color(232, 148, 140));
    }

    private static void DrawInteriorWall(Painter p)
    {
        p.Rect(0, 0, 16, 16, IntWallBase);
        p.VLine(3, 0, 12, IntWallLine);
        p.VLine(8, 0, 12, IntWallLine);
        p.VLine(13, 0, 12, IntWallLine);
        p.Rect(0, 12, 16, 4, IntWallTrim);
        p.HLine(0, 12, 16, new Color(128, 108, 86));
    }

    private static void DrawBed(Painter p)
    {
        DrawFloor(p);
        p.Rect(1, 0, 14, 16, new Color(120, 88, 56));
        p.Rect(2, 1, 12, 14, new Color(232, 232, 240));
        p.Rect(3, 2, 10, 3, new Color(248, 248, 252)); // pillow
        p.Rect(2, 6, 12, 9, new Color(92, 116, 196));  // blanket
        p.HLine(2, 7, 12, new Color(130, 152, 220));
    }

    private static void DrawBookshelf(Painter p)
    {
        p.Rect(0, 0, 16, 16, new Color(134, 96, 60));
        p.RectOutline(0, 0, 16, 16, WoodDark);
        var bookColors = new[]
        {
            new Color(198, 82, 82), new Color(82, 118, 198), new Color(220, 180, 92),
            new Color(104, 168, 112), new Color(160, 104, 180),
        };
        foreach (int shelfY in new[] { 2, 9 })
        {
            p.HLine(1, shelfY + 5, 14, WoodDark);
            for (int i = 0; i < 6; i++)
                p.Rect(2 + i * 2, shelfY, 2, 5, bookColors[i % bookColors.Length]);
        }
    }

    private static void DrawTable(Painter p)
    {
        DrawFloor(p);
        p.Rect(1, 4, 14, 8, new Color(188, 142, 92));
        p.RectOutline(1, 4, 14, 8, WoodDark);
        p.Rect(2, 12, 2, 3, WoodDark);
        p.Rect(12, 12, 2, 3, WoodDark);
    }

    private static void DrawChair(Painter p)
    {
        DrawFloor(p);
        p.Rect(4, 2, 8, 4, WoodDark);       // backrest
        p.Rect(4, 6, 8, 6, new Color(172, 126, 78)); // seat
        p.Rect(4, 12, 2, 2, WoodDark);
        p.Rect(10, 12, 2, 2, WoodDark);
    }

    private static void DrawTv(Painter p)
    {
        DrawFloor(p);
        p.Rect(3, 12, 10, 3, WoodDark);              // stand
        p.Rect(2, 2, 12, 10, new Color(58, 58, 66)); // body
        p.Rect(4, 4, 8, 6, new Color(122, 198, 216)); // screen
        p.Px(5, 5, new Color(220, 244, 248));
    }

    private static void DrawDoorMat(Painter p)
    {
        DrawFloor(p);
        p.Rect(1, 2, 14, 12, new Color(172, 148, 100));
        p.RectOutline(1, 2, 14, 12, new Color(140, 116, 74));
        p.HLine(3, 6, 10, new Color(140, 116, 74));
        p.HLine(3, 9, 10, new Color(140, 116, 74));
    }

    // --- Character painter ---------------------------------------------------

    private static void DrawCharacterSheet(Painter p, CharPalette pal)
    {
        for (int row = 0; row < 3; row++)       // 0 down, 1 up, 2 left
            for (int col = 0; col < 3; col++)   // 0 step A, 1 idle, 2 step B
                DrawCharacterCell(p, col * CharWidth, row * CharHeight, row, col, pal);
    }

    private static void DrawCharacterCell(Painter p, int ox, int oy, int facing, int frame, CharPalette pal)
    {
        bool stepping = frame != 1;
        int bob = stepping ? 1 : 0;
        var shadow = Color.FromNonPremultiplied(20, 40, 20, 70);
        var skinDark = new Color(
            (byte)Math.Max(0, pal.Skin.R - 40), (byte)Math.Max(0, pal.Skin.G - 40), (byte)Math.Max(0, pal.Skin.B - 40));

        // Ground shadow.
        p.Rect(ox + 4, oy + 22, 8, 2, shadow);

        int hy = oy + 2 + bob; // top of head

        // Head base (skin), then per-facing hair.
        p.Rect(ox + 4, hy, 8, 8, pal.Skin);
        switch (facing)
        {
            case 0: // down: hair cap + fringe corners, two eyes
                p.Rect(ox + 3, hy, 10, 3, pal.Hair);
                p.Px(ox + 3, hy + 3, pal.Hair);
                p.Px(ox + 4, hy + 3, pal.Hair);
                p.Px(ox + 11, hy + 3, pal.Hair);
                p.Px(ox + 12, hy + 3, pal.Hair);
                p.Px(ox + 6, hy + 5, Color.Black);
                p.Px(ox + 9, hy + 5, Color.Black);
                break;
            case 1: // up: back of head, all hair
                p.Rect(ox + 3, hy, 10, 7, pal.Hair);
                p.Rect(ox + 4, hy + 7, 8, 1, pal.Hair);
                break;
            default: // left: hair covers top and the back (right side), one eye
                p.Rect(ox + 3, hy, 10, 3, pal.Hair);
                p.Rect(ox + 9, hy + 3, 4, 4, pal.Hair);
                p.Px(ox + 5, hy + 5, Color.Black);
                break;
        }

        // Body / shirt with little arms.
        int by = oy + 10 + bob;
        p.Rect(ox + 4, by, 8, 6, pal.Shirt);
        p.Rect(ox + 3, by + 1, 1, 4, pal.Shirt);
        p.Rect(ox + 12, by + 1, 1, 4, pal.Shirt);
        p.Px(ox + 3, by + 5, skinDark);  // hands
        p.Px(ox + 12, by + 5, skinDark);

        // Legs + shoes. Step frames raise one leg for a simple stride.
        int ly = oy + 16 + bob;
        int leftLift = frame == 0 ? 1 : 0;
        int rightLift = frame == 2 ? 1 : 0;
        p.Rect(ox + 5, ly, 3, 4 - leftLift, pal.Pants);
        p.Rect(ox + 8, ly, 3, 4 - rightLift, pal.Pants);
        p.Rect(ox + 5, ly + 4 - leftLift, 3, 2, pal.Shoes);
        p.Rect(ox + 8, ly + 4 - rightLift, 3, 2, pal.Shoes);
    }

    // --- Pixel painting helper ----------------------------------------------

    private sealed class Painter
    {
        public readonly Color[] Data;
        private readonly int _w;
        private readonly int _h;

        public Painter(int w, int h)
        {
            _w = w;
            _h = h;
            Data = new Color[w * h];
        }

        public void Px(int x, int y, Color c)
        {
            if (x >= 0 && y >= 0 && x < _w && y < _h)
                Data[y * _w + x] = c;
        }

        public void Rect(int x, int y, int w, int h, Color c)
        {
            for (int yy = y; yy < y + h; yy++)
                for (int xx = x; xx < x + w; xx++)
                    Px(xx, yy, c);
        }

        public void RectOutline(int x, int y, int w, int h, Color c)
        {
            HLine(x, y, w, c);
            HLine(x, y + h - 1, w, c);
            VLine(x, y, h, c);
            VLine(x + w - 1, y, h, c);
        }

        public void HLine(int x, int y, int length, Color c) => Rect(x, y, length, 1, c);

        public void VLine(int x, int y, int length, Color c) => Rect(x, y, 1, length, c);
    }
}
