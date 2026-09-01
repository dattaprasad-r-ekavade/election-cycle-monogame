# Election Cycle

**Election Cycle** — a South Park season you can play. Each episode is a level. Built with [MonoGame](https://monogame.net/) (DesktopGL).

Season 1 is 10 short satire episodes (YouTube) that are also 10 playable campaign weeks (itch.io, donation / PWYW). Same stations every time: TV cold open, register, canvass, interview, speech, debate, then a 3rd-party underdog wins. The engine still ships a walkable town plus a 7-day stub (poll, WMAP-7 news, save). See `SEASON-PLAN.md`.

Everything (sprites, tiles, font, sound effects) is **generated procedurally at startup**, so the project builds and runs with zero asset files and no MonoGame content pipeline. Swap in real art later by editing one file.

## Running

Requires the [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0).

```bash
dotnet run --project src/PocketTown
```

To publish a self-contained Windows build:

```bash
dotnet publish src/PocketTown -c Release -r win-x64 --self-contained
```

## Controls

| Action | Keys | Gamepad |
| --- | --- | --- |
| Move | Arrow keys / WASD | D-pad / left stick (dominant axis) |
| Talk / interact / advance dialogue | Z, Enter, Space | A |
| Cancel / close dialogue | X | B |
| Run (hold) | Shift | Left/Right shoulder or trigger |
| Close dialogue, or back to title / quit | Escape | Start |
| Advance the campaign day (morning) | X | B |

## What's in the skeleton

- **Grid movement** — tile-to-tile walking with smooth interpolation, turn-in-place on a tap, running, and bump feedback, just like the classic handheld RPGs.
- **2.5D rendering** — the world draws with y-sorted depth (`SpriteSortMode.FrontToBack`): tall tiles (trees, house facades, beds, bookshelves, TVs) and characters overlap each other correctly, and tall grass blades cover your feet. Rendered at 320x180 and integer-upscaled with letterboxing for crisp pixels at any window size.
- **Maps as ASCII + JSON** — `src/PocketTown/Data/Maps/*.json` holds the tile layout (one character per tile), spawn point, NPCs, warps, and interactable text. Two maps included: **Maple Town** and **Your House**.
- **NPCs** — placed per map, optional wandering within a small radius, freeze during dialogue, and turn to face you when spoken to.
- **Dialogue** — typewriter reveal, word wrapping, multiple pages, speaker name tags, blinking continue arrow.
- **Warps** — step on a door / door mat to fade out, switch maps, and fade back in.
- **Interactables** — face a sign, bed, TV, bookshelf... and press Z to read flavor text.
- **7-day campaign stub** — HUD shows day and poll. Press **X** in the morning to play that day's placeholder event, then the **WMAP-7** news report (poll moves, headlines, tomorrow's modifier). Sleep auto-saves to `%APPDATA%\ElectionCycle\save.json`. Title screen: **Z** new campaign, **X** continue.

## Project layout

```
src/PocketTown/
  PocketTownGame.cs     Game loop, virtual resolution upscaling
  Core/                 Input, camera, scenes, pixel font, procedural art, synth audio
  Sim/                  RunState, DayManager, PollModel, SaveGame (the campaign spine)
  World/                Tile catalog, JSON map data, TileMap (collision/warps/drawing)
  Entities/             Entity (grid movement), Player, Npc
  UI/                   DialogueBox, CampaignHud, NewsReport
  Scenes/               TitleScene, WorldScene
  Data/Maps/            town.json, home.json
```

## Extending it

- **New map**: add `Data/Maps/mymap.json` (copy an existing one), then link it with a warp: `{ "x": 5, "y": 4, "toMap": "mymap", "toX": 3, "toY": 6, "facing": "down" }`. Row lengths are validated at load with helpful errors.
- **New tile**: add a value to `TileKind`, a character in `TileCatalog.FromChar`, its behaviour in `IsSolid` / `IsTall` / `IsAnimated`, and its artwork in `Core/Art.cs`.
- **New NPC look**: add a palette entry in `Art.Palettes` and reference its key from the NPC's `sprite` field.
- **Real artwork**: replace the generated textures in `Core/Art.cs` with loaded ones — the rest of the game only calls `Art.TileFrames(kind)` and `Art.CharacterSheet(id)` (3x3 sheet of 16x24 cells: columns = walk frames, rows = down/up/left).
- **Cutscenes, menus, battles...**: add a new `Scene` and switch to it via `Game.Scenes.Change(...)`.
