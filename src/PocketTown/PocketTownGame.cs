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
    private bool _resizing;

    public SceneManager Scenes { get; } = new();
    public PixelFont Font { get; private set; } = null!;

    public PocketTownGame()
    {
        _graphics = new GraphicsDeviceManager(this)
        {
            PreferredBackBufferWidth = Constants.WindowWidth,
            PreferredBackBufferHeight = Constants.WindowHeight,
            HardwareModeSwitch = false,
        };
        IsMouseVisible = true;
        Window.AllowUserResizing = true;
        Window.Title = Constants.GameTitle;
        Content.RootDirectory = "Content";
        Window.ClientSizeChanged += OnClientSizeChanged;
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

    protected override void UnloadContent()
    {
        Scenes.Change(null);
        Font?.Dispose();
        Art.Dispose();
        AudioBank.Dispose();
        _renderTarget?.Dispose();
        _spriteBatch?.Dispose();
        base.UnloadContent();
    }

    protected override void Update(GameTime gameTime)
    {
        InputManager.Update();
        float dt = Math.Min((float)gameTime.ElapsedGameTime.TotalSeconds, Constants.MaxDeltaTime);
        Scenes.Update(dt);
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.SetRenderTarget(_renderTarget);
        Scenes.Draw(_spriteBatch);

        GraphicsDevice.SetRenderTarget(null);
        GraphicsDevice.Clear(Color.Black);

        var bounds = GraphicsDevice.Viewport.Bounds;
        if (bounds.Width <= 0 || bounds.Height <= 0)
            return;

        int scale = Math.Max(1, Math.Min(bounds.Width / Constants.VirtualWidth, bounds.Height / Constants.VirtualHeight));
        int width = Constants.VirtualWidth * scale;
        int height = Constants.VirtualHeight * scale;
        var destination = new Rectangle((bounds.Width - width) / 2, (bounds.Height - height) / 2, width, height);

        _spriteBatch.Begin(samplerState: SamplerState.PointClamp);
        _spriteBatch.Draw(_renderTarget, destination, Color.White);
        _spriteBatch.End();

        base.Draw(gameTime);
    }

    private void OnClientSizeChanged(object? sender, EventArgs e)
    {
        if (_resizing)
            return;

        int width = Math.Max(Constants.VirtualWidth, Window.ClientBounds.Width);
        int height = Math.Max(Constants.VirtualHeight, Window.ClientBounds.Height);
        if (width == _graphics.PreferredBackBufferWidth && height == _graphics.PreferredBackBufferHeight)
            return;

        _resizing = true;
        _graphics.PreferredBackBufferWidth = width;
        _graphics.PreferredBackBufferHeight = height;
        _graphics.ApplyChanges();
        _resizing = false;
    }
}
