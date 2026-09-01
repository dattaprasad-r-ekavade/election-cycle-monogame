using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PocketTown.Core;

namespace PocketTown.UI;

/// <summary>
/// Pokemon-style message box: typewriter reveal, multiple pages, optional speaker
/// name tag, and a blinking "more" arrow. Confirm reveals the page instantly,
/// then advances to the next page or closes the box.
/// </summary>
public class DialogueBox
{
    private const int BoxHeight = 46;
    private const int Margin = 6;
    private const int Padding = 7;
    private const int LinesPerPage = 3;

    public bool IsOpen { get; private set; }

    private readonly PixelFont _font;
    private string? _speaker;
    private readonly List<List<string>> _pages = new();
    private int _pageIndex;
    private float _revealed;
    private float _blink;

    private static readonly Color BoxFill = new(28, 36, 72);
    private static readonly Color BoxBorder = new(236, 240, 248);
    private static readonly Color TextColor = new(240, 244, 252);
    private static readonly Color NameFill = new(196, 84, 76);

    public DialogueBox(PixelFont font) => _font = font;

    /// <summary>Open the box. Each entry in <paramref name="paragraphs"/> starts on a fresh page.</summary>
    public void Start(string? speaker, IEnumerable<string> paragraphs)
    {
        _speaker = speaker;
        _pages.Clear();
        _pageIndex = 0;
        _revealed = 0f;

        _blink = 0f;
        int textWidth = Constants.VirtualWidth - Margin * 2 - Padding * 2;
        foreach (var paragraph in paragraphs)
        {
            var lines = _font.Wrap(paragraph, textWidth);
            if (lines.Count == 0)
                continue;
            for (int i = 0; i < lines.Count; i += LinesPerPage)
                _pages.Add(lines.Skip(i).Take(LinesPerPage).ToList());
        }

        if (_pages.Count == 0)
            _pages.Add(new List<string> { "..." });

        IsOpen = true;
    }

    public void Close() => IsOpen = false;

    private int PageLength => _pages[_pageIndex].Sum(l => l.Length);
    private bool PageFullyRevealed => _revealed >= PageLength;

    public void Update(float dt)
    {
        if (!IsOpen)
            return;

        _blink += dt;

        if (!PageFullyRevealed)
        {
            float before = _revealed;
            _revealed = Math.Min(_revealed + dt * Constants.DialogueCharsPerSecond, PageLength);
            if ((int)(before / 3) != (int)(_revealed / 3))
                AudioBank.Play(AudioBank.TextBlip);
        }

        if (InputManager.Pressed(GameAction.Confirm) || InputManager.Pressed(GameAction.Cancel))
        {
            if (!PageFullyRevealed)
            {
                _revealed = PageLength;
            }
            else if (_pageIndex < _pages.Count - 1)
            {
                _pageIndex++;
                _revealed = 0f;
                AudioBank.Play(AudioBank.Confirm);
            }
            else
            {
                IsOpen = false;
                AudioBank.Play(AudioBank.Confirm);
            }
        }
    }

    /// <summary>Draw in screen space (no camera transform).</summary>
    public void Draw(SpriteBatch sb)
    {
        if (!IsOpen)
            return;

        int boxY = Constants.VirtualHeight - BoxHeight - Margin;
        var box = new Rectangle(Margin, boxY, Constants.VirtualWidth - Margin * 2, BoxHeight);

        DrawPanel(sb, box);

        if (!string.IsNullOrEmpty(_speaker))
        {
            int tagWidth = _font.MeasureWidth(_speaker) + 8;
            var tag = new Rectangle(box.X + 4, box.Y - 11, tagWidth, 13);
            FillRect(sb, tag, NameFill);
            DrawRectOutline(sb, tag, BoxBorder);
            _font.Draw(sb, _speaker, new Vector2(tag.X + 4, tag.Y + 3), TextColor);
        }

        // Typewriter text.
        var lines = _pages[_pageIndex];
        int remaining = (int)_revealed;
        for (int i = 0; i < lines.Count; i++)
        {
            if (remaining <= 0)
                break;
            string visible = lines[i].Length <= remaining ? lines[i] : lines[i][..remaining];
            remaining -= lines[i].Length;
            _font.Draw(sb, visible, new Vector2(box.X + Padding, box.Y + Padding + i * _font.LineHeight), TextColor);
        }

        // Blinking continue arrow.
        if (PageFullyRevealed && _blink % 0.8f < 0.5f)
        {
            int ax = box.Right - 10;
            int ay = box.Bottom - 8;
            FillRect(sb, new Rectangle(ax - 2, ay, 5, 1), TextColor);
            FillRect(sb, new Rectangle(ax - 1, ay + 1, 3, 1), TextColor);
            FillRect(sb, new Rectangle(ax, ay + 2, 1, 1), TextColor);
        }
    }

    private static void DrawPanel(SpriteBatch sb, Rectangle box)
    {
        FillRect(sb, box, BoxFill);
        DrawRectOutline(sb, box, BoxBorder);
        // Inner accent line for a chunky retro border.
        DrawRectOutline(sb, new Rectangle(box.X + 2, box.Y + 2, box.Width - 4, box.Height - 4), new Color(96, 112, 168));
    }

    private static void FillRect(SpriteBatch sb, Rectangle rect, Color color) =>
        sb.Draw(Art.Pixel, rect, color);

    private static void DrawRectOutline(SpriteBatch sb, Rectangle r, Color c)
    {
        FillRect(sb, new Rectangle(r.X, r.Y, r.Width, 1), c);
        FillRect(sb, new Rectangle(r.X, r.Bottom - 1, r.Width, 1), c);
        FillRect(sb, new Rectangle(r.X, r.Y, 1, r.Height), c);
        FillRect(sb, new Rectangle(r.Right - 1, r.Y, 1, r.Height), c);
    }
}
