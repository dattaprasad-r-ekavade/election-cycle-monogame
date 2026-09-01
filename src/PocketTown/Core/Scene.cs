using Microsoft.Xna.Framework.Graphics;

namespace PocketTown.Core;

/// <summary>Base class for game screens (title, overworld, ...).</summary>
public abstract class Scene
{
    protected readonly PocketTownGame Game;

    protected Scene(PocketTownGame game) => Game = game;

    public abstract void Update(float dt);

    /// <summary>Draw to the 320x180 virtual render target. SpriteBatch Begin/End is owned by the scene.</summary>
    public abstract void Draw(SpriteBatch spriteBatch);

    /// <summary>Called when the scene is replaced. Unsubscribe events and drop per-scene state here.</summary>
    public virtual void OnExit() { }
}

public class SceneManager
{
    public Scene? Current { get; private set; }

    public void Change(Scene? scene)
    {
        Current?.OnExit();
        Current = scene;
    }

    public void Update(float dt) => Current?.Update(dt);

    public void Draw(SpriteBatch spriteBatch) => Current?.Draw(spriteBatch);
}
