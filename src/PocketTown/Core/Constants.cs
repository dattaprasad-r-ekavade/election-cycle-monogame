namespace PocketTown.Core;

/// <summary>Global tuning values for the game.</summary>
public static class Constants
{
    /// <summary>Size of one map tile in pixels.</summary>
    public const int TileSize = 16;

    /// <summary>Internal ("retro") resolution the game is rendered at before upscaling.</summary>
    public const int VirtualWidth = 320;
    public const int VirtualHeight = 180;

    /// <summary>Default window size (virtual resolution x 4).</summary>
    public const int WindowWidth = VirtualWidth * 4;
    public const int WindowHeight = VirtualHeight * 4;

    /// <summary>Movement speeds, in tiles per second.</summary>
    public const float WalkSpeed = 4.5f;
    public const float RunSpeed = 8.0f;
    public const float NpcSpeed = 2.25f;

    /// <summary>Seconds the player must hold a new direction before walking (turn-in-place).</summary>
    public const float TurnDelay = 0.09f;

    /// <summary>Dialogue typewriter speed in characters per second.</summary>
    public const float DialogueCharsPerSecond = 45f;

    /// <summary>Duration of the fade used for map transitions, in seconds.</summary>
    public const float FadeDuration = 0.35f;

    public const string GameTitle = "Election Cycle";

    /// <summary>Map loaded after the title screen (wake-up-at-home, Pokemon-style).</summary>
    public const string StartMapId = "home";

    /// <summary>Delta-time ceiling so a hitch cannot skip a whole tile or warp.</summary>
    public const float MaxDeltaTime = 1f / 20f;
}
