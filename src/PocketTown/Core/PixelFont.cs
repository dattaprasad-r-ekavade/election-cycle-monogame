using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PocketTown.Core;

/// <summary>
/// A tiny 5x7 bitmap font generated entirely in code (no content pipeline required).
/// Lowercase input is rendered as uppercase, Game Boy style.
/// </summary>
public sealed class PixelFont
{
    public const int GlyphWidth = 5;
    public const int GlyphHeight = 7;
    public const int Advance = GlyphWidth + 1;
    public int LineHeight => 10;

    private readonly Texture2D _atlas;
    private readonly Dictionary<char, Rectangle> _glyphs;

    private PixelFont(Texture2D atlas, Dictionary<char, Rectangle> glyphs)
    {
        _atlas = atlas;
        _glyphs = glyphs;
    }

    public static PixelFont Create(GraphicsDevice device)
    {
        var patterns = GlyphPatterns();
        int width = patterns.Count * Advance;
        var data = new Color[width * GlyphHeight];
        var glyphs = new Dictionary<char, Rectangle>();

        int index = 0;
        foreach (var (ch, rows) in patterns)
        {
            int originX = index * Advance;
            for (int y = 0; y < GlyphHeight; y++)
            {
                for (int x = 0; x < GlyphWidth; x++)
                {
                    if (rows[y][x] == '#')
                        data[y * width + originX + x] = Color.White;
                }
            }
            glyphs[ch] = new Rectangle(originX, 0, GlyphWidth, GlyphHeight);
            index++;
        }

        var atlas = new Texture2D(device, width, GlyphHeight);
        atlas.SetData(data);
        return new PixelFont(atlas, glyphs);
    }

    public int MeasureWidth(string text, int scale = 1) =>
        text.Length == 0 ? 0 : (text.Length * Advance - 1) * scale;

    public void Draw(SpriteBatch sb, string text, Vector2 position, Color color, int scale = 1, float depth = 0f)
    {
        // Snap to whole pixels; point sampling at fractional positions garbles glyphs.
        float x = MathF.Round(position.X);
        float y = MathF.Round(position.Y);
        foreach (char raw in text)
        {
            char c = char.ToUpperInvariant(raw);
            if (c != ' ')
            {
                var src = _glyphs.TryGetValue(c, out var rect) ? rect : _glyphs['?'];
                sb.Draw(_atlas, new Vector2(x, y), src, color, 0f, Vector2.Zero, scale, SpriteEffects.None, depth);
            }
            x += Advance * scale;
        }
    }

    /// <summary>Greedy word wrap into lines no wider than <paramref name="maxWidth"/> pixels.</summary>
    public List<string> Wrap(string text, int maxWidth, int scale = 1)
    {
        var lines = new List<string>();
        foreach (var hardLine in text.Split('\n'))
        {
            string current = "";
            foreach (var word in hardLine.Split(' ', StringSplitOptions.RemoveEmptyEntries))
            {
                string candidate = current.Length == 0 ? word : current + " " + word;
                if (MeasureWidth(candidate, scale) <= maxWidth)
                {
                    current = candidate;
                }
                else
                {
                    if (current.Length > 0)
                        lines.Add(current);
                    current = word;
                }
            }
            lines.Add(current);
        }
        return lines;
    }

    private static Dictionary<char, string[]> GlyphPatterns() => new()
    {
        ['A'] = new[] { ".###.", "#...#", "#...#", "#####", "#...#", "#...#", "#...#" },
        ['B'] = new[] { "####.", "#...#", "#...#", "####.", "#...#", "#...#", "####." },
        ['C'] = new[] { ".###.", "#...#", "#....", "#....", "#....", "#...#", ".###." },
        ['D'] = new[] { "####.", "#...#", "#...#", "#...#", "#...#", "#...#", "####." },
        ['E'] = new[] { "#####", "#....", "#....", "####.", "#....", "#....", "#####" },
        ['F'] = new[] { "#####", "#....", "#....", "####.", "#....", "#....", "#...." },
        ['G'] = new[] { ".###.", "#...#", "#....", "#.###", "#...#", "#...#", ".###." },
        ['H'] = new[] { "#...#", "#...#", "#...#", "#####", "#...#", "#...#", "#...#" },
        ['I'] = new[] { "#####", "..#..", "..#..", "..#..", "..#..", "..#..", "#####" },
        ['J'] = new[] { "..###", "....#", "....#", "....#", "....#", "#...#", ".###." },
        ['K'] = new[] { "#...#", "#..#.", "#.#..", "##...", "#.#..", "#..#.", "#...#" },
        ['L'] = new[] { "#....", "#....", "#....", "#....", "#....", "#....", "#####" },
        ['M'] = new[] { "#...#", "##.##", "#.#.#", "#.#.#", "#...#", "#...#", "#...#" },
        ['N'] = new[] { "#...#", "##..#", "#.#.#", "#..##", "#...#", "#...#", "#...#" },
        ['O'] = new[] { ".###.", "#...#", "#...#", "#...#", "#...#", "#...#", ".###." },
        ['P'] = new[] { "####.", "#...#", "#...#", "####.", "#....", "#....", "#...." },
        ['Q'] = new[] { ".###.", "#...#", "#...#", "#...#", "#.#.#", "#..#.", ".##.#" },
        ['R'] = new[] { "####.", "#...#", "#...#", "####.", "#.#..", "#..#.", "#...#" },
        ['S'] = new[] { ".####", "#....", "#....", ".###.", "....#", "....#", "####." },
        ['T'] = new[] { "#####", "..#..", "..#..", "..#..", "..#..", "..#..", "..#.." },
        ['U'] = new[] { "#...#", "#...#", "#...#", "#...#", "#...#", "#...#", ".###." },
        ['V'] = new[] { "#...#", "#...#", "#...#", "#...#", ".#.#.", ".#.#.", "..#.." },
        ['W'] = new[] { "#...#", "#...#", "#...#", "#.#.#", "#.#.#", "##.##", "#...#" },
        ['X'] = new[] { "#...#", "#...#", ".#.#.", "..#..", ".#.#.", "#...#", "#...#" },
        ['Y'] = new[] { "#...#", "#...#", ".#.#.", "..#..", "..#..", "..#..", "..#.." },
        ['Z'] = new[] { "#####", "....#", "...#.", "..#..", ".#...", "#....", "#####" },
        ['0'] = new[] { ".###.", "#..##", "#.#.#", "#.#.#", "##..#", "#...#", ".###." },
        ['1'] = new[] { "..#..", ".##..", "..#..", "..#..", "..#..", "..#..", "#####" },
        ['2'] = new[] { ".###.", "#...#", "....#", "..##.", ".#...", "#....", "#####" },
        ['3'] = new[] { ".###.", "#...#", "....#", "..##.", "....#", "#...#", ".###." },
        ['4'] = new[] { "...#.", "..##.", ".#.#.", "#..#.", "#####", "...#.", "...#." },
        ['5'] = new[] { "#####", "#....", "####.", "....#", "....#", "#...#", ".###." },
        ['6'] = new[] { ".###.", "#....", "#....", "####.", "#...#", "#...#", ".###." },
        ['7'] = new[] { "#####", "....#", "...#.", "..#..", ".#...", ".#...", ".#..." },
        ['8'] = new[] { ".###.", "#...#", "#...#", ".###.", "#...#", "#...#", ".###." },
        ['9'] = new[] { ".###.", "#...#", "#...#", ".####", "....#", "....#", ".###." },
        ['.'] = new[] { ".....", ".....", ".....", ".....", ".....", ".##..", ".##.." },
        [','] = new[] { ".....", ".....", ".....", ".....", ".....", "..#..", ".#..." },
        ['!'] = new[] { "..#..", "..#..", "..#..", "..#..", "..#..", ".....", "..#.." },
        ['?'] = new[] { ".###.", "#...#", "....#", "..##.", "..#..", ".....", "..#.." },
        ['\''] = new[] { "..#..", "..#..", ".....", ".....", ".....", ".....", "....." },
        ['"'] = new[] { ".#.#.", ".#.#.", ".....", ".....", ".....", ".....", "....." },
        [':'] = new[] { ".....", "..#..", ".....", ".....", ".....", "..#..", "....." },
        [';'] = new[] { ".....", "..#..", ".....", ".....", "..#..", ".#...", "....." },
        ['-'] = new[] { ".....", ".....", ".....", ".###.", ".....", ".....", "....." },
        ['+'] = new[] { ".....", ".....", "..#..", ".###.", "..#..", ".....", "....." },
        ['('] = new[] { "...#.", "..#..", ".#...", ".#...", ".#...", "..#..", "...#." },
        [')'] = new[] { ".#...", "..#..", "...#.", "...#.", "...#.", "..#..", ".#..." },
        ['/'] = new[] { "....#", "...#.", "...#.", "..#..", ".#...", ".#...", "#...." },
        ['>'] = new[] { "#....", ".#...", "..#..", "...#.", "..#..", ".#...", "#...." },
    };
}
