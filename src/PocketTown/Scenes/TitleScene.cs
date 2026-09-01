using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PocketTown.Core;

namespace PocketTown.Scenes;

public class TitleScene : Scene
{
    private float _time;
    private bool _starting;
    private float _fade;

    public TitleScene(PocketTownGame game) : base(game) { }

    public override void Update(float dt)
    {
        _time += dt;

        if (_starting)
        {
            _fade += dt / Constants.FadeDuration;
            if (_fade >= 1f)
                Game.Scenes.Change(new WorldScene(Game, "home"));
            return;
        }

        if (InputManager.Pressed(GameAction.Confirm))
        {
            _starting = true;
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

        // Decorative ground strip with a row of trees.
        sb.Draw(Art.Pixel, new Rectangle(0, 150, Constants.VirtualWidth, 30), new Color(52, 96, 60));
        var tree = Art.TileFrames(World.TileKind.Tree)[0];
        for (int x = -4; x < Constants.VirtualWidth; x += 24)
            sb.Draw(tree, new Vector2(x, 126), Color.White);

        const string title = "POCKET TOWN";
        int titleWidth = font.MeasureWidth(title, 3);
        float bounce = MathF.Sin(_time * 2f) * 2f;
        font.Draw(sb, title, new Vector2((Constants.VirtualWidth - titleWidth) / 2f, 38 + bounce), new Color(248, 220, 120), 3);

        const string subtitle = "A TINY 2.5D RPG FRAMEWORK";
        font.Draw(sb, subtitle, new Vector2((Constants.VirtualWidth - font.MeasureWidth(subtitle)) / 2f, 72), new Color(168, 184, 216));

        if (_time % 1.1f < 0.65f)
        {
            const string prompt = "PRESS Z OR ENTER";
            font.Draw(sb, prompt, new Vector2((Constants.VirtualWidth - font.MeasureWidth(prompt)) / 2f, 100), Color.White);
        }

        const string controls = "MOVE: ARROWS/WASD  TALK: Z  RUN: SHIFT";
        font.Draw(sb, controls, new Vector2((Constants.VirtualWidth - font.MeasureWidth(controls)) / 2f, 166), new Color(120, 136, 168));

        if (_starting)
        {
            sb.Draw(Art.Pixel, new Rectangle(0, 0, Constants.VirtualWidth, Constants.VirtualHeight),
                Color.Black * MathHelper.Clamp(_fade, 0f, 1f));
        }

        sb.End();
    }
}
