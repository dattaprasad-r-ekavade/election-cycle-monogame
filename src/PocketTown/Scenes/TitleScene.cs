using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PocketTown.Core;
using PocketTown.Sim;

namespace PocketTown.Scenes;

public class TitleScene : Scene
{
    private float _time;
    private bool _starting;
    private bool _continuing;
    private float _fade;
    private readonly bool _hasSave;

    public TitleScene(PocketTownGame game) : base(game)
    {
        _hasSave = SaveGame.Exists();
    }

    public override void Update(float dt)
    {
        _time += dt;

        if (_starting)
        {
            _fade += dt / Constants.FadeDuration;
            if (_fade >= 1f)
            {
                var run = _continuing
                    ? SaveGame.TryLoad() ?? RunState.CreateNew()
                    : RunState.CreateNew();
                Game.Scenes.Change(new WorldScene(Game, run));
            }
            return;
        }

        if (InputManager.Pressed(GameAction.Confirm))
        {
            _starting = true;
            _continuing = false;
            AudioBank.Play(AudioBank.Confirm);
        }
        else if (_hasSave && InputManager.Pressed(GameAction.Cancel))
        {
            _starting = true;
            _continuing = true;
            AudioBank.Play(AudioBank.Confirm);
        }

        if (InputManager.Pressed(GameAction.Menu))
            Game.Exit();
    }

    public override void Draw(SpriteBatch sb)
    {
        var font = Game.Font;
        sb.GraphicsDevice.Clear(new Color(24, 32, 56));
        sb.Begin(samplerState: SamplerState.PointClamp);

        sb.Draw(Art.Pixel, new Rectangle(0, 150, Constants.VirtualWidth, 30), new Color(52, 96, 60));
        var tree = Art.TileFrames(World.TileKind.Tree)[0];
        for (int x = -4; x < Constants.VirtualWidth; x += 24)
            sb.Draw(tree, new Vector2(x, 126), Color.White);

        const string title = "ELECTION CYCLE";
        int titleWidth = font.MeasureWidth(title, 2);
        float bounce = MathF.Sin(_time * 2f) * 2f;
        font.Draw(sb, title, new Vector2((Constants.VirtualWidth - titleWidth) / 2f, 28 + bounce), new Color(248, 220, 120), 2);

        const string subtitle = "7 DAYS. ONE TOWN. DON'T PROMISE THE MOON.";
        font.Draw(sb, subtitle, new Vector2((Constants.VirtualWidth - font.MeasureWidth(subtitle)) / 2f, 56), new Color(168, 184, 216));

        if (_time % 1.1f < 0.65f)
        {
            const string prompt = "Z: NEW CAMPAIGN";
            font.Draw(sb, prompt, new Vector2((Constants.VirtualWidth - font.MeasureWidth(prompt)) / 2f, 86), Color.White);
        }

        if (_hasSave)
        {
            const string cont = "X: CONTINUE";
            font.Draw(sb, cont, new Vector2((Constants.VirtualWidth - font.MeasureWidth(cont)) / 2f, 100), new Color(200, 220, 180));
        }

        const string controls = "MOVE: ARROWS  TALK: Z  DAY: X  RUN: SHIFT";
        font.Draw(sb, controls, new Vector2((Constants.VirtualWidth - font.MeasureWidth(controls)) / 2f, 166), new Color(120, 136, 168));

        if (_starting)
        {
            sb.Draw(Art.Pixel, new Rectangle(0, 0, Constants.VirtualWidth, Constants.VirtualHeight),
                Color.Black * MathHelper.Clamp(_fade, 0f, 1f));
        }

        sb.End();
    }
}
