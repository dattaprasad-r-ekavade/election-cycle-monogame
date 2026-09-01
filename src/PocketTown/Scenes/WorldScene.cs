using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PocketTown.Core;
using PocketTown.Entities;
using PocketTown.Sim;
using PocketTown.UI;
using PocketTown.World;

namespace PocketTown.Scenes;

/// <summary>
/// Overworld plus the campaign day clock. Morning is free-roam; X starts today's
/// placeholder event, which feeds the news TV and then sleep (auto-save).
/// </summary>
public class WorldScene : Scene
{
    private enum FadeState { FadeIn, None, FadeOutToWarp, FadeOutToSleep }

    private readonly RunState _run;
    private readonly Dictionary<string, TileMap> _mapCache = new();
    private readonly Camera2D _camera = new();
    private readonly DialogueBox _dialogue;
    private readonly NewsReport _news;
    private readonly Player _player = new();

    private TileMap _map = null!;
    private List<Npc> _npcs = new();

    private FadeState _fade = FadeState.FadeIn;
    private float _fadeProgress = 1f;
    private WarpData? _pendingWarp;
    private float _bannerTimer;

    public WorldScene(PocketTownGame game, RunState run) : base(game)
    {
        _run = run;
        _dialogue = new DialogueBox(game.Font);
        _news = new NewsReport(game.Font);

        _player.IsBlocked = IsBlockedForPlayer;
        _player.SteppedOnTile += OnPlayerSteppedOnTile;

        LoadMap(_run.MapId);
        _player.SnapTo(new Point(_run.PlayerX, _run.PlayerY));
        _player.Facing = DirectionExtensions.Parse(_run.PlayerFacing);

        if (_run.Log.Any(e => e.Day == _run.Day))
        {
            DayManager.BeginNews(_run);
            _news.Show(_run);
        }
        else
        {
            _run.Phase = DayPhase.Morning;
        }
    }

    private void LoadMap(string id)
    {
        if (!_mapCache.TryGetValue(id, out var map))
        {
            map = TileMap.Load(id);
            _mapCache[id] = map;
        }

        _map = map;
        _npcs = map.Data.Npcs.Select(data => new Npc(data)).ToList();
        foreach (var npc in _npcs)
        {
            var self = npc;
            self.IsBlocked = tile => IsBlockedForNpc(self, tile);
        }
        _bannerTimer = 2.5f;
        UpdateCamera();
    }

    public override void OnExit()
    {
        _player.SteppedOnTile -= OnPlayerSteppedOnTile;
        if (!_run.Finished)
        {
            CaptureWorld();
            SaveGame.Write(_run);
        }
    }

    private void CaptureWorld()
    {
        _run.SnapshotWorld(_map.Id, _player.Tile, _player.Facing.ToString().ToLowerInvariant());
    }

    // --- Collision -----------------------------------------------------------

    private bool IsBlockedForPlayer(Point tile) =>
        _map.IsBlocked(tile) || OccupiedByNpc(tile);

    private bool IsBlockedForNpc(Npc self, Point tile) =>
        _map.IsBlocked(tile)
        || _map.GetWarpAt(tile) != null
        || _player.Occupies(tile)
        || _npcs.Any(n => n != self && n.Occupies(tile));

    private bool OccupiedByNpc(Point tile) => _npcs.Any(n => n.Occupies(tile));

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
                UpdateCamera();
                return;

            case FadeState.FadeOutToWarp:
                _fadeProgress += dt / Constants.FadeDuration;
                if (_fadeProgress >= 1f)
                    CompleteWarp();
                UpdateCamera();
                return;

            case FadeState.FadeOutToSleep:
                _fadeProgress += dt / Constants.FadeDuration;
                if (_fadeProgress >= 1f)
                    CompleteSleep();
                return;
        }

        if (_news.IsOpen)
        {
            _news.Update(dt);
            if (InputManager.Pressed(GameAction.Confirm) || InputManager.Pressed(GameAction.Cancel))
            {
                AudioBank.Play(AudioBank.Confirm);
                _fade = FadeState.FadeOutToSleep;
                _fadeProgress = 0f;
            }
            if (InputManager.Pressed(GameAction.Menu))
            {
                Game.Scenes.Change(new TitleScene(Game));
                return;
            }
            return;
        }

        if (InputManager.Pressed(GameAction.Menu))
        {
            if (_dialogue.IsOpen)
                _dialogue.Close();
            else
            {
                Game.Scenes.Change(new TitleScene(Game));
                return;
            }
        }

        bool dialogueWasOpen = _dialogue.IsOpen;
        if (_dialogue.IsOpen)
        {
            _dialogue.Update(dt);
            foreach (var npc in _npcs)
            {
                npc.Frozen = true;
                npc.Update(dt);
            }
            if (!_dialogue.IsOpen)
                foreach (var npc in _npcs)
                    npc.Frozen = false;
        }
        else if (_run.Phase == DayPhase.Morning)
        {
            _player.Update(dt);
            foreach (var npc in _npcs)
                npc.Update(dt);

            if (!_player.IsMoving && InputManager.Pressed(GameAction.Confirm))
                TryInteract();
            else if (!_player.IsMoving && InputManager.Pressed(GameAction.Cancel))
                StartTodaysEvent();
        }

        if (dialogueWasOpen && !_dialogue.IsOpen && _run.Phase == DayPhase.Event)
            OpenNews();

        UpdateCamera();
    }

    private void StartTodaysEvent()
    {
        if (_run.Log.Any(e => e.Day == _run.Day))
        {
            OpenNews();
            return;
        }

        var script = DayManager.BeginTodaysEvent(_run);
        _dialogue.Start(script.Speaker, script.Lines);
        AudioBank.Play(AudioBank.Confirm);
    }

    private void OpenNews()
    {
        DayManager.BeginNews(_run);
        _news.Show(_run);
        AudioBank.Play(AudioBank.Warp);
    }

    private void CompleteSleep()
    {
        _news.Close();
        CaptureWorld();
        DayManager.Sleep(_run);

        if (_run.Finished)
        {
            SaveGame.Delete();
            Game.Scenes.Change(new TitleScene(Game));
            return;
        }

        SaveGame.Write(_run);
        _bannerTimer = 2.5f;
        _fade = FadeState.FadeIn;
        _fadeProgress = 1f;
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
            AudioBank.Play(AudioBank.Confirm);
            return;
        }

        var interactable = _map.GetInteractableAt(target);
        if (interactable != null)
        {
            _dialogue.Start(null, interactable.Lines);
            AudioBank.Play(AudioBank.Confirm);
        }
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
        _player.SnapTo(ResolveLanding(new Point(warp.ToX, warp.ToY)));
        _player.Facing = DirectionExtensions.Parse(warp.Facing);
        CaptureWorld();

        _fade = FadeState.FadeIn;
        _fadeProgress = 1f;
    }

    private Point ResolveLanding(Point dest)
    {
        if (!IsLandingBlocked(dest))
            return dest;

        foreach (var dir in new[] { Direction.Up, Direction.Down, Direction.Left, Direction.Right })
        {
            var neighbor = dest + dir.ToPoint();
            if (!IsLandingBlocked(neighbor))
                return neighbor;
        }

        return dest;
    }

    private bool IsLandingBlocked(Point tile) =>
        _map.IsBlocked(tile) || OccupiedByNpc(tile);

    // --- Draw ----------------------------------------------------------------

    public override void Draw(SpriteBatch sb)
    {
        sb.GraphicsDevice.Clear(_map.Outdoor ? new Color(34, 48, 42) : new Color(12, 12, 18));

        sb.Begin(SpriteSortMode.FrontToBack, samplerState: SamplerState.PointClamp, transformMatrix: _camera.Transform);
        _map.Draw(sb);
        _player.Draw(sb, _map);
        foreach (var npc in _npcs)
            npc.Draw(sb, _map);
        sb.End();

        sb.Begin(samplerState: SamplerState.PointClamp);
        if (_news.IsOpen)
        {
            _news.Draw(sb);
        }
        else
        {
            CampaignHud.Draw(sb, Game.Font, _run, showAdvanceHint: !_dialogue.IsOpen && _run.Phase == DayPhase.Morning);
            _dialogue.Draw(sb);
            DrawLocationBanner(sb);
        }

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
        var rect = new Rectangle(6, 24, width, 15);

        sb.Draw(Art.Pixel, rect, new Color(28, 36, 72) * (0.85f * alpha));
        sb.Draw(Art.Pixel, new Rectangle(rect.X, rect.Bottom - 1, rect.Width, 1), new Color(236, 240, 248) * alpha);
        Game.Font.Draw(sb, name, new Vector2(rect.X + 6, rect.Y + 4), Color.White * alpha);
    }
}
