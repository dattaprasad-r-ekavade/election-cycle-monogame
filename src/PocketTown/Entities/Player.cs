using Microsoft.Xna.Framework;
using PocketTown.Core;

namespace PocketTown.Entities;

/// <summary>
/// The player character. Reads input for grid movement with Pokemon-style
/// turn-in-place (tapping a new direction turns without stepping) and running.
/// </summary>
public class Player : Entity
{
    /// <summary>Collision query supplied by the scene (map + entity occupancy).</summary>
    public Func<Point, bool> IsBlocked { get; set; } = _ => true;

    /// <summary>Raised whenever a step finishes, with the tile arrived on (used for warps).</summary>
    public event Action<Point>? SteppedOnTile;

    private float _turnTimer;
    private float _sinceLastStep = 10f;
    private float _bumpCooldown;

    public Player()
    {
        SpriteId = "hero";
    }

    public override void Update(float dt)
    {
        Speed = InputManager.Down(GameAction.Run) ? Constants.RunSpeed : Constants.WalkSpeed;

        _turnTimer -= dt;
        _sinceLastStep += dt;
        _bumpCooldown -= dt;

        if (!IsMoving && InputManager.HeldDirection() is Direction dir)
        {
            // Tapping a new direction first turns in place; walking begins if it stays held.
            // While already walking (fluid), direction changes apply immediately.
            bool fluid = _sinceLastStep < 0.1f;
            if (dir != Facing && !fluid)
            {
                if (_turnTimer <= 0f)
                {
                    Facing = dir;
                    _turnTimer = Constants.TurnDelay;
                }
            }
            else if (_turnTimer <= 0f || fluid)
            {
                if (!TryStartMove(dir, IsBlocked) && _bumpCooldown <= 0f)
                {
                    AudioBank.Play(AudioBank.Bump);
                    _bumpCooldown = 0.35f;
                }
            }
        }

        base.Update(dt);
    }

    protected override void OnArrived()
    {
        _sinceLastStep = 0f;
        SteppedOnTile?.Invoke(Tile);
    }

    /// <summary>The tile directly in front of the player (interaction target).</summary>
    public Point FacingTile => Tile + Facing.ToPoint();
}
