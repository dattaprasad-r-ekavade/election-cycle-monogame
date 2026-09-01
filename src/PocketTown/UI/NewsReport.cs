using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PocketTown.Core;
using PocketTown.Sim;

namespace PocketTown.UI;

/// <summary>Full-screen WMAP-7 evening broadcast. Stub of the signature news TV.</summary>
public class NewsReport
{
    public bool IsOpen { get; private set; }

    private readonly PixelFont _font;
    private float _time;
    private string _town = "";
    private string _dayName = "";
    private string _poll = "";
    private string _delta = "";
    private string _opponent = "";
    private string _modifier = "";
    private readonly List<string> _headlines = new();
    private bool _finale;
    private bool _won;

    private static readonly Color Screen = new(16, 22, 40);
    private static readonly Color Bezel = new(48, 36, 32);
    private static readonly Color Red = new(196, 64, 64);
    private static readonly Color Amber = new(248, 220, 120);
    private static readonly Color Tick = new(180, 196, 220);

    public NewsReport(PixelFont font) => _font = font;

    public void Show(RunState run)
    {
        IsOpen = true;
        _time = 0f;
        _finale = run.Day >= CampaignCalendar.DayCount;
        _won = run.Poll >= 50f;
        _town = run.TownName;
        _dayName = "DAY " + run.Day + "  " + CampaignCalendar.Name(run.Day);
        _poll = run.Poll.ToString("0.0") + "%";
        var last = run.Log.LastOrDefault();
        float d = last == null ? 0f : last.PollAfter - last.PollBefore;
        _delta = d >= 0 ? "+" + d.ToString("0.0") : d.ToString("0.0");
        _opponent = run.LastOpponentAction;
        _modifier = run.TomorrowModifier;
        _headlines.Clear();
        _headlines.AddRange(run.Headlines.Where(h => h.Day == run.Day).Select(h => h.Text).Take(3));
        if (_headlines.Count == 0)
            _headlines.Add("TOWN REMAINS POLITE, UNHINGED");
    }

    public void Close() => IsOpen = false;

    public void Update(float dt) => _time += dt;

    public void Draw(SpriteBatch sb)
    {
        if (!IsOpen)
            return;

        sb.Draw(Art.Pixel, new Rectangle(0, 0, Constants.VirtualWidth, Constants.VirtualHeight), Bezel);
        var screen = new Rectangle(8, 8, Constants.VirtualWidth - 16, Constants.VirtualHeight - 16);
        sb.Draw(Art.Pixel, screen, Screen);

        // Fake scanlines.
        for (int y = screen.Y; y < screen.Bottom; y += 2)
            sb.Draw(Art.Pixel, new Rectangle(screen.X, y, screen.Width, 1), Color.Black * 0.18f);

        _font.Draw(sb, "WMAP-7  EVENING NEWS", new Vector2(14, 14), Red);
        _font.Draw(sb, _town, new Vector2(14, 26), Tick);
        _font.Draw(sb, _dayName, new Vector2(14, 38), Amber);

        _font.Draw(sb, "YOUR POLL", new Vector2(14, 56), Tick);
        _font.Draw(sb, _poll, new Vector2(14, 68), Color.White, 2);
        _font.Draw(sb, _delta + " TODAY", new Vector2(14, 90), _delta.StartsWith('+') || _delta == "0.0" ? new Color(120, 200, 120) : Red);

        int hy = 108;
        foreach (var h in _headlines)
        {
            foreach (var line in _font.Wrap("> " + h, screen.Width - 16))
            {
                if (hy > 140)
                    break;
                _font.Draw(sb, line, new Vector2(14, hy), Color.White);
                hy += 10;
            }
            hy += 2;
            if (hy > 140)
                break;
        }

        _font.Draw(sb, _opponent, new Vector2(14, 148), Tick);
        if (_finale)
        {
            string result = _won ? "YOU WIN.  MAYOR, FOR NOW." : "YOU LOSE.  QUINCE REMAINS.";
            _font.Draw(sb, result, new Vector2(14, 160), Amber);
        }
        else
        {
            _font.Draw(sb, _modifier, new Vector2(14, 160), Amber);
        }

        if (_time % 1.1f < 0.7f)
        {
            string prompt = _finale ? "Z: RETURN TO TITLE" : "Z: GO TO SLEEP";
            int w = _font.MeasureWidth(prompt);
            _font.Draw(sb, prompt, new Vector2(Constants.VirtualWidth - 14 - w, 160), Color.White);
        }
    }
}
