using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PocketTown.Core;
using PocketTown.Sim;

namespace PocketTown.UI;

/// <summary>WMAP-7: morning cold open, evening recap, and results tag.</summary>
public class NewsReport
{
    public enum Mode { Evening, ColdOpen }

    public bool IsOpen { get; private set; }
    public Mode CurrentMode { get; private set; } = Mode.Evening;

    private readonly PixelFont _font;
    private float _time;
    private RunState? _run;

    private static readonly Color Screen = new(16, 22, 40);
    private static readonly Color Bezel = new(48, 36, 32);
    private static readonly Color Red = new(196, 64, 64);
    private static readonly Color Amber = new(248, 220, 120);
    private static readonly Color Tick = new(180, 196, 220);
    private static readonly Color Green = new(120, 200, 120);

    public NewsReport(PixelFont font) => _font = font;

    public void ShowColdOpen(RunState run)
    {
        IsOpen = true;
        CurrentMode = Mode.ColdOpen;
        _time = 0f;
        _run = run;
    }

    public void Show(RunState run)
    {
        IsOpen = true;
        CurrentMode = Mode.Evening;
        _time = 0f;
        _run = run;
    }

    public void Close() => IsOpen = false;

    public void Update(float dt) => _time += dt;

    public void Draw(SpriteBatch sb)
    {
        if (!IsOpen || _run == null)
            return;

        sb.Draw(Art.Pixel, new Rectangle(0, 0, Constants.VirtualWidth, Constants.VirtualHeight), Bezel);
        var screen = new Rectangle(8, 8, Constants.VirtualWidth - 16, Constants.VirtualHeight - 16);
        sb.Draw(Art.Pixel, screen, Screen);
        for (int y = screen.Y; y < screen.Bottom; y += 2)
            sb.Draw(Art.Pixel, new Rectangle(screen.X, y, screen.Width, 1), Color.Black * 0.18f);

        if (CurrentMode == Mode.ColdOpen)
            DrawColdOpen(sb);
        else
            DrawEvening(sb, screen);
    }

    private void DrawColdOpen(SpriteBatch sb)
    {
        var run = _run!;
        _font.Draw(sb, "WMAP-7  SPECIAL REPORT", new Vector2(14, 14), Red);
        _font.Draw(sb, run.TownName.ToUpperInvariant() + "  MAYORAL RACE IS ON", new Vector2(14, 26), Amber);
        _font.Draw(sb, "CANDIDATES", new Vector2(14, 44), Tick);

        DrawCandidate(sb, 14, 58, run.OpponentName, "INCUMBENT", run.OpponentPlatform);
        DrawCandidate(sb, 14, 88, run.PlayerName, "CHALLENGER", run.PlayerPlatform);
        DrawCandidate(sb, 14, 118, run.ThirdPartyName, "ALSO FILING", run.ThirdPartyPlatform);

        _font.Draw(sb, "THE CLERK WOULD LIKE TO NOTE THE THIRD FEE WAS IN QUARTERS.", new Vector2(14, 150), Tick);

        if (_time % 1.1f < 0.7f)
        {
            const string prompt = "Z: I'M GOING TO TOWN HALL";
            int w = _font.MeasureWidth(prompt);
            _font.Draw(sb, prompt, new Vector2(Constants.VirtualWidth - 14 - w, 162), Color.White);
        }
    }

    private void DrawCandidate(SpriteBatch sb, int x, int y, string name, string tag, string platform)
    {
        _font.Draw(sb, name, new Vector2(x, y), Color.White);
        _font.Draw(sb, tag, new Vector2(x, y + 10), Red);
        _font.Draw(sb, "\"" + platform + "\"", new Vector2(x + 110, y + 10), Amber);
    }

    private void DrawEvening(SpriteBatch sb, Rectangle screen)
    {
        var run = _run!;
        bool finale = run.Day >= CampaignCalendar.DayCount;
        _font.Draw(sb, finale ? "WMAP-7  ELECTION NIGHT" : "WMAP-7  EVENING NEWS", new Vector2(14, 14), Red);
        _font.Draw(sb, run.TownName, new Vector2(14, 26), Tick);
        _font.Draw(sb, "DAY " + run.Day + "  " + CampaignCalendar.Name(run.Day), new Vector2(14, 38), Amber);

        DrawShareBar(sb, 14, 56, run.PlayerName, run.PlayerShare, new Color(80, 140, 220));
        DrawShareBar(sb, 14, 72, run.OpponentName, run.OpponentShare, new Color(196, 84, 76));
        DrawShareBar(sb, 14, 88, run.ThirdPartyName, run.ThirdPartyShare, Green);

        int hy = 108;
        foreach (var h in run.Headlines.Where(x => x.Day == run.Day).Select(x => x.Text).Take(2))
        {
            foreach (var line in _font.Wrap("> " + h, screen.Width - 16))
            {
                if (hy > 136)
                    break;
                _font.Draw(sb, line, new Vector2(14, hy), Color.White);
                hy += 10;
            }
            hy += 2;
        }

        if (finale)
        {
            string tag = run.Leader == run.ThirdPartyName
                ? run.ThirdPartyName + " WINS. HUMANS CONCEDE."
                : run.Leader == run.PlayerName
                    ? "YOU WIN.  MAYOR, FOR NOW."
                    : run.OpponentName + " HOLDS THE OFFICE.";
            _font.Draw(sb, tag, new Vector2(14, 148), Amber);
        }
        else
        {
            _font.Draw(sb, run.LastOpponentAction, new Vector2(14, 148), Tick);
            _font.Draw(sb, run.TomorrowModifier, new Vector2(14, 160), Amber);
        }

        if (_time % 1.1f < 0.7f)
        {
            string prompt = finale ? "Z: RETURN TO TITLE" : "Z: GO TO SLEEP";
            int w = _font.MeasureWidth(prompt);
            _font.Draw(sb, prompt, new Vector2(Constants.VirtualWidth - 14 - w, 162), Color.White);
        }
    }

    private void DrawShareBar(SpriteBatch sb, int x, int y, string name, float share, Color color)
    {
        string label = name.Length > 14 ? name[..14] : name;
        _font.Draw(sb, label, new Vector2(x, y), Tick);
        int barX = x + 92;
        int barW = 140;
        sb.Draw(Art.Pixel, new Rectangle(barX, y + 2, barW, 6), new Color(32, 40, 56));
        int fill = (int)(barW * Math.Clamp(share / 100f, 0f, 1f));
        sb.Draw(Art.Pixel, new Rectangle(barX, y + 2, Math.Max(1, fill), 6), color);
        _font.Draw(sb, share.ToString("0") + "%", new Vector2(barX + barW + 6, y), Color.White);
    }
}
