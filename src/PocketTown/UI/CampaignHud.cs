using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PocketTown.Core;
using PocketTown.Sim;

namespace PocketTown.UI;

/// <summary>Always-on day / poll strip. Lives under the location banner.</summary>
public static class CampaignHud
{
    public static void Draw(SpriteBatch sb, PixelFont font, RunState run, bool showAdvanceHint)
    {
        string day = "DAY " + run.Day + "/" + CampaignCalendar.DayCount;
        string name = CampaignCalendar.Name(run.Day);
        string poll = "YOU " + run.PlayerShare.ToString("0") + "%";

        DrawChip(sb, font, 6, 6, day, new Color(28, 36, 72));
        int x = 6 + font.MeasureWidth(day) + 20;
        DrawChip(sb, font, x, 6, name, new Color(72, 40, 48));
        int pollX = Constants.VirtualWidth - 6 - (font.MeasureWidth(poll) + 12);
        DrawChip(sb, font, pollX, 6, poll, new Color(32, 56, 48));

        if (showAdvanceHint && run.Phase == DayPhase.Morning)
        {
            string hint = "X: " + CampaignCalendar.ShortName(run.Day);
            int hw = font.MeasureWidth(hint) + 12;
            DrawChip(sb, font, Constants.VirtualWidth - 6 - hw, Constants.VirtualHeight - 18, hint, new Color(28, 36, 72));
        }
    }

    private static void DrawChip(SpriteBatch sb, PixelFont font, int x, int y, string text, Color fill)
    {
        int w = font.MeasureWidth(text) + 12;
        var rect = new Rectangle(x, y, w, 15);
        sb.Draw(Art.Pixel, rect, fill * 0.9f);
        sb.Draw(Art.Pixel, new Rectangle(rect.X, rect.Bottom - 1, rect.Width, 1), new Color(236, 240, 248) * 0.8f);
        font.Draw(sb, text, new Vector2(rect.X + 6, rect.Y + 4), Color.White);
    }
}
