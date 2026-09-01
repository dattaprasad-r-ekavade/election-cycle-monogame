using Microsoft.Xna.Framework;
using PocketTown.Core;
using PocketTown.World;

namespace PocketTown.Entities;

/// <summary>
/// A townsperson. Optionally wanders a small area around its start position,
/// freezes during dialogue, and turns to face the player when spoken to.
/// </summary>
public class Npc : Entity
{
    public string Name { get; }
    public IReadOnlyList<string> Dialogue { get; }
    public bool Wander { get; }

    /// <summary>Collision query supplied by the scene.</summary>
    public Func<Point, bool> IsBlocked { get; set; } = _ => true;

    /// <summary>Set while a dialogue is open; the NPC finishes its current step, then stands still.</summary>
    public bool Frozen { get; set; }

    private const int WanderRadius = 2;
    private readonly Point _origin;
    private readonly Random _rng;
    private float _decisionTimer;

    public Npc(NpcData data)
    {
        Name = data.Name;
        Dialogue = data.Dialogue.Count > 0 ? data.Dialogue : new List<string> { "..." };
        Wander = data.Wander;
        SpriteId = data.Sprite;
        Speed = Constants.NpcSpeed;

        _origin = new Point(data.X, data.Y);
        _rng = new Random(HashCode.Combine(data.Name, data.X, data.Y));
        _decisionTimer = 1f + (float)_rng.NextDouble() * 2f;

        SnapTo(_origin);
        Facing = DirectionExtensions.Parse(data.Facing);
    }

    public override void Update(float dt)
    {
        if (Frozen)
        {
            base.Update(dt); // finish an in-progress step, otherwise stand still
            return;
        }

        if (Wander && !IsMoving)
        {
            _decisionTimer -= dt;
            if (_decisionTimer <= 0f)
            {
                _decisionTimer = 1.5f + (float)_rng.NextDouble() * 2.5f;
                var direction = (Direction)_rng.Next(4);

                if (_rng.NextDouble() < 0.4)
                {
                    Facing = direction; // just look around
                }
                else
                {
                    var target = Tile + direction.ToPoint();
                    bool insideHome =
                        Math.Abs(target.X - _origin.X) <= WanderRadius &&
                        Math.Abs(target.Y - _origin.Y) <= WanderRadius;
                    if (insideHome)
                        TryStartMove(direction, IsBlocked);
                    else
                        Facing = direction;
                }
            }
        }

        base.Update(dt);
    }

    public void FacePlayer(Point playerTile) => Facing = DirectionExtensions.Towards(Tile, playerTile);
}
