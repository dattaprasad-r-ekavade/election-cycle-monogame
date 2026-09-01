using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PocketTown.Core;
using PocketTown.Entities;
using PocketTown.UI;
using PocketTown.World;

namespace PocketTown.Scenes;

/// <summary>
/// The overworld: owns the current map, the player, its NPCs, the camera, the dialogue
/// box, and the fade transitions used when warping between maps (doors, exits...).
/// </summary>
public class WorldScene : Scene
{
    private enum FadeState { FadeIn, None, FadeOutToWarp }

    private readonly Dictionary<string, TileMap> _mapCache = new();
    private readonly Camera2D _camera = new();
    private readonly DialogueBox _dialogue;
    private readonly Player _player = new();

    private TileMap _map = null!;
    private List<Npc> _npcs = new();

    private FadeState _fade = FadeState.FadeIn;
    private float _fadeProgress;
    private WarpData? _pendingWarp;

    private float _bannerTimer;

    public WorldScene(PocketTownGame game, string startMapId) : base(game)
    {
        _dialogue = new DialogueBox(game.Font);

        _player.IsBlocked = IsBlockedForPlayer;
        _player.SteppedOnTile += OnPlayerSteppedOnTile;

        LoadMap(startMapId);
        _player.SnapTo(new Point(_map.Data.Spawn.X, _map.Data.Spawn.Y));
        _player.Facing = DirectionExtensions.Parse(_map.Data.Spawn.Facing);
    }

    private void LoadMap(string id)
    {
        if (!_mapCache.TryGetValue(id, out var map))
        {
            map = TileMap.Load(id);
            _mapCache[id] = map;
        }

        _map = map;
        _npcs = map.Data.Npcs.Select(data => new Npc(data) { IsBlocked = IsBlockedForNpc }).ToList();
        _bannerTimer = 2.5f;
        UpdateCamera();
    }

    // --- Collision -----------------------------------------------------------

    private bool IsBlockedForPlayer(Point tile) =>
        _map.IsBlocked(tile) || _npcs.Any(n => n.Occupies(tile));

    private bool IsBlockedForNpc(Point tile) =>
        _map.IsBlocked(tile)
        || _map.GetWarpAt(tile) != null // NPCs never step onto doors/warps
        || _player.Occupies(tile)
        || _npcs.Any(n => n.Occupies(tile));

    // --- Update --------------------------------------------------------------

    public override void Update(float dt)
    {
        _map.Update(dt);
        _bannerTimer -= dt;

        switch (_fade)
        {
            case FadeState.FadeIn:
                _fadeProgress -= dt / Constants.FadeDuration;
                if (_fadeProgress <= 0f)
                {
                    _fadeProgress = 0f;
                    _fade = FadeState.None;
                }
                break;

            case FadeState.FadeOutToWarp:
                _fadeProgress += dt / Constants.FadeDuration;
                if (_fadeProgress >= 1f)
                    CompleteWarp();
                UpdateCamera();
                return; // world is frozen during the fade-out
        }

        if (_dialogue.IsOpen)
        {
            _dialogue.Update(dt);
            foreach (var npc in _npcs)
            {
                npc.Frozen = true;
                npc.Update(dt); // lets an in-progress step finish
            }
            if (!_dialogue.IsOpen)
                foreach (var npc in _npcs)
                    npc.Frozen = false;
        }
        else
        {
            _player.Update(dt);
            foreach (var npc in _npcs)
                npc.Update(dt);

            if (_fade == FadeState.None && !_player.IsMoving && InputManager.Pressed(GameAction.Confirm))
                TryInteract();

            if (InputManager.Pressed(GameAction.Menu))
                Game.Scenes.Change(new TitleScene(Game));
        }

        UpdateCamera();
    }

    private void UpdateCamera()
    {
        var center = _player.Position + new Vector2(Constants.TileSize / 2f, Constants.TileSize / 2f);
        _camera.Follow(center, _map.PixelWidth, _map.PixelHeight);
    }

    private void TryInteract()
    {
        var target = _player.FacingTile;

        var npc = _npcs.FirstOrDefault(n => n.Occupies(target));
        if (npc != null)
        {
            npc.FacePlayer(_player.Tile);
            _dialogue.Start(npc.Name, npc.Dialogue);
            return;
        }

        var interactable = _map.GetInteractableAt(target);
        if (interactable != null)
            _dialogue.Start(null, interactable.Lines);
    }

    private void OnPlayerSteppedOnTile(Point tile)
    {
        var warp = _map.GetWarpAt(tile);
        if (warp != null && _fade == FadeState.None)
        {
            _pendingWarp = warp;
            _fade = FadeState.FadeOutToWarp;
            _fadeProgress = 0f;
            AudioBank.Play(AudioBank.Warp);
        }
    }

    private void CompleteWarp()
    {
        var warp = _pendingWarp!;
        _pendingWarp = null;

        LoadMap(warp.ToMap);
        _player.SnapTo(new Point(warp.ToX, warp.ToY));
        _player.Facing = DirectionExtensions.Parse(warp.Facing);

        _fade = FadeState.FadeIn;
        _fadeProgress = 1f;
    }

    // --- Draw ----------------------------------------------------------------

    public override void Draw(SpriteBatch sb)
    {
        sb.GraphicsDevice.Clear(_map.Outdoor ? new Color(34, 48, 42) : new Color(12, 12, 18));

        // World pass: FrontToBack so tall tiles and entities y-sort against each other.
        sb.Begin(SpriteSortMode.FrontToBack, samplerState: SamplerState.PointClamp, transformMatrix: _camera.Transform);
        _map.Draw(sb);
        _player.Draw(sb, _map);
        foreach (var npc in _npcs)
            npc.Draw(sb, _map);
        sb.End();

        // UI pass in screen space.
        sb.Begin(samplerState: SamplerState.PointClamp);
        _dialogue.Draw(sb);
        DrawLocationBanner(sb);

        if (_fadeProgress > 0f)
        {
            sb.Draw(Art.Pixel, new Rectangle(0, 0, Constants.VirtualWidth, Constants.VirtualHeight),
                Color.Black * MathHelper.Clamp(_fadeProgress, 0f, 1f));
        }
        sb.End();
    }

    private void DrawLocationBanner(SpriteBatch sb)
    {
        if (_bannerTimer <= 0f || _dialogue.IsOpen)
            return;

        float alpha = MathHelper.Clamp(_bannerTimer / 0.4f, 0f, 1f);
        string name = _map.Name;
        int width = Game.Font.MeasureWidth(name) + 12;
        var rect = new Rectangle(6, 6, width, 15);

        sb.Draw(Art.Pixel, rect, new Color(28, 36, 72) * (0.85f * alpha));
        sb.Draw(Art.Pixel, new Rectangle(rect.X, rect.Bottom - 1, rect.Width, 1), new Color(236, 240, 248) * alpha);
        Game.Font.Draw(sb, name, new Vector2(rect.X + 6, rect.Y + 4), Color.White * alpha);
    }
}
