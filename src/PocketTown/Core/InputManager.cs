using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace PocketTown.Core;

public enum GameAction
{
    Up,
    Down,
    Left,
    Right,
    /// <summary>Interact / advance dialogue (Z, Enter, Space, gamepad A).</summary>
    Confirm,
    /// <summary>Cancel / back (X, gamepad B).</summary>
    Cancel,
    /// <summary>Hold to run (Left Shift, gamepad B).</summary>
    Run,
    /// <summary>Escape / gamepad Start.</summary>
    Menu,
}

/// <summary>
/// Polls keyboard + gamepad once per frame and exposes pressed/held queries plus a
/// "most recently pressed direction wins" helper used for grid movement.
/// </summary>
public static class InputManager
{
    private static KeyboardState _prevKeys;
    private static KeyboardState _keys;
    private static GamePadState _prevPad;
    private static GamePadState _pad;
    private static Direction? _lastDirection;

    private const float StickDeadZone = 0.5f;

    public static void Update()
    {
        _prevKeys = _keys;
        _prevPad = _pad;
        _keys = Keyboard.GetState();
        _pad = GamePad.GetState(PlayerIndex.One);

        // Track the most recently pressed direction so tapping a new arrow while
        // holding another turns the player immediately.
        foreach (var dir in new[] { Direction.Up, Direction.Down, Direction.Left, Direction.Right })
        {
            if (Pressed(ActionFor(dir)))
                _lastDirection = dir;
        }

        if (_lastDirection is Direction last && !Down(ActionFor(last)))
        {
            _lastDirection = null;
            foreach (var dir in new[] { Direction.Up, Direction.Down, Direction.Left, Direction.Right })
            {
                if (Down(ActionFor(dir)))
                {
                    _lastDirection = dir;
                    break;
                }
            }
        }
    }

    private static GameAction ActionFor(Direction d) => d switch
    {
        Direction.Up => GameAction.Up,
        Direction.Down => GameAction.Down,
        Direction.Left => GameAction.Left,
        _ => GameAction.Right,
    };

    /// <summary>The direction the player is currently holding, favouring the most recent press.</summary>
    public static Direction? HeldDirection()
    {
        if (_lastDirection is Direction d && Down(ActionFor(d)))
            return d;
        foreach (var dir in new[] { Direction.Up, Direction.Down, Direction.Left, Direction.Right })
        {
            if (Down(ActionFor(dir)))
                return dir;
        }
        return null;
    }

    public static bool Down(GameAction action) => IsDown(_keys, _pad, action);

    public static bool Pressed(GameAction action) =>
        IsDown(_keys, _pad, action) && !IsDown(_prevKeys, _prevPad, action);

    private static bool IsDown(KeyboardState k, GamePadState p, GameAction action) => action switch
    {
        GameAction.Up => k.IsKeyDown(Keys.Up) || k.IsKeyDown(Keys.W)
            || p.DPad.Up == ButtonState.Pressed || p.ThumbSticks.Left.Y > StickDeadZone,
        GameAction.Down => k.IsKeyDown(Keys.Down) || k.IsKeyDown(Keys.S)
            || p.DPad.Down == ButtonState.Pressed || p.ThumbSticks.Left.Y < -StickDeadZone,
        GameAction.Left => k.IsKeyDown(Keys.Left) || k.IsKeyDown(Keys.A)
            || p.DPad.Left == ButtonState.Pressed || p.ThumbSticks.Left.X < -StickDeadZone,
        GameAction.Right => k.IsKeyDown(Keys.Right) || k.IsKeyDown(Keys.D)
            || p.DPad.Right == ButtonState.Pressed || p.ThumbSticks.Left.X > StickDeadZone,
        GameAction.Confirm => k.IsKeyDown(Keys.Z) || k.IsKeyDown(Keys.Enter) || k.IsKeyDown(Keys.Space)
            || p.Buttons.A == ButtonState.Pressed,
        GameAction.Cancel => k.IsKeyDown(Keys.X) || p.Buttons.B == ButtonState.Pressed,
        GameAction.Run => k.IsKeyDown(Keys.LeftShift) || k.IsKeyDown(Keys.RightShift)
            || p.Buttons.B == ButtonState.Pressed,
        GameAction.Menu => k.IsKeyDown(Keys.Escape) || p.Buttons.Start == ButtonState.Pressed,
        _ => false,
    };
}
