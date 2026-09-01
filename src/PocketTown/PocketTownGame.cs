using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PocketTown.Core;
using PocketTown.Scenes;

namespace PocketTown;

/// <summary>
/// Entry point game class. Renders every scene into a small 320x180 render target,
/// then upscales it (integer scale, letterboxed) to whatever size the window is,
/// which keeps the pixel art crisp at any resolution.
/// </summary>
public class PocketTownGame : Game
{
    private readonly GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch = null!;
    private RenderTarget2D _renderTarget = null!;

    public SceneManager Scenes { get; } = new();
    public PixelFont Font { get; private set; } = null!;

    public PocketTownGame()
    {
        _graphics = new GraphicsDeviceManager(this)
        {
            PreferredBackBufferWidth = Constants.WindowWidth,
            PreferredBackBufferHeight = Constants.WindowHeight,
        };
        IsMouseVisible = true;
        Window.AllowUserResizing = true;
        Window.Title = "Pocket Town";
        Content.RootDirectory = "Content";
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        _renderTarget = new RenderTarget2D(GraphicsDevice, Constants.VirtualWidth, Constants.VirtualHeight);

        Art.Load(GraphicsDevice);
        Font = PixelFont.Create(GraphicsDevice);
        AudioBank.Load();

        Scenes.Change(new TitleScene(this));
    }

    protected override void Update(GameTime gameTime)
    {
        InputManager.Update();
        Scenes.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        // 1) Scene draws at the virtual resolution.
        GraphicsDevice.SetRenderTarget(_renderTarget);
        Scenes.Draw(_spriteBatch);

        // 2) Upscale to the window with integer scaling + letterboxing.
        GraphicsDevice.SetRenderTarget(null);
        GraphicsDevice.Clear(Color.Black);

        var bounds = GraphicsDevice.PresentationParameters.Bounds;
        int scale = Math.Max(1, Math.Min(bounds.Width / Constants.VirtualWidth, bounds.Height / Constants.VirtualHeight));
        int width = Constants.VirtualWidth * scale;
        int height = Constants.VirtualHeight * scale;
        var destination = new Rectangle((bounds.Width - width) / 2, (bounds.Height - height) / 2, width, height);

        _spriteBatch.Begin(samplerState: SamplerState.PointClamp);
        _spriteBatch.Draw(_renderTarget, destination, Color.White);
        _spriteBatch.End();

        base.Draw(gameTime);
    }
}
