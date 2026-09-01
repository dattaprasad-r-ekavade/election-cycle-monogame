# Pocket Town

A tiny **2.5D Pokemon-style RPG framework** built with [MonoGame](https://monogame.net/) (DesktopGL) for Windows desktop. It ships as a playable skeleton: a title screen, a town to walk around, NPCs to talk to, and a home interior — no combat, by design.

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
| Move | Arrow keys / WASD | D-pad / left stick |
| Talk / interact / advance dialogue | Z, Enter, Space | A |
| Run (hold) | Shift | B |
| Back to title / quit | Escape | Start |

## What's in the skeleton

- **Grid movement** — tile-to-tile walking with smooth interpolation, turn-in-place on a tap, running, and bump feedback, just like the classic handheld RPGs.
- **2.5D rendering** — the world draws with y-sorted depth (`SpriteSortMode.FrontToBack`): tall tiles (trees) and characters overlap each other correctly, and tall grass blades cover your feet. Rendered at 320x180 and integer-upscaled with letterboxing for crisp pixels at any window size.
- **Maps as ASCII + JSON** — `src/PocketTown/Data/Maps/*.json` holds the tile layout (one character per tile), spawn point, NPCs, warps, and interactable text. Two maps included: **Maple Town** and **Your House**.
- **NPCs** — placed per map, optional wandering within a small radius, freeze during dialogue, and turn to face you when spoken to.
- **Dialogue** — typewriter reveal, word wrapping, multiple pages, speaker name tags, blinking continue arrow.
- **Warps** — step on a door / door mat to fade out, switch maps, and fade back in.
- **Interactables** — face a sign, bed, TV, bookshelf... and press Z to read flavor text.

## Project layout

```
src/PocketTown/
  PocketTownGame.cs     Game loop, virtual resolution upscaling
  Core/                 Input, camera, scenes, pixel font, procedural art, synth audio
  World/                Tile catalog, JSON map data, TileMap (collision/warps/drawing)
  Entities/             Entity (grid movement), Player, Npc
  UI/                   DialogueBox
  Scenes/               TitleScene, WorldScene
  Data/Maps/            town.json, home.json
```

## Extending it

- **New map**: add `Data/Maps/mymap.json` (copy an existing one), then link it with a warp: `{ "x": 5, "y": 4, "toMap": "mymap", "toX": 3, "toY": 6, "facing": "down" }`. Row lengths are validated at load with helpful errors.
- **New tile**: add a value to `TileKind`, a character in `TileCatalog.FromChar`, its behaviour in `IsSolid` / `IsTall` / `IsAnimated`, and its artwork in `Core/Art.cs`.
- **New NPC look**: add a palette entry in `Art.Palettes` and reference its key from the NPC's `sprite` field.
- **Real artwork**: replace the generated textures in `Core/Art.cs` with loaded ones — the rest of the game only calls `Art.TileFrames(kind)` and `Art.CharacterSheet(id)` (3x3 sheet of 16x24 cells: columns = walk frames, rows = down/up/left).
- **Cutscenes, menus, battles...**: add a new `Scene` and switch to it via `Game.Scenes.Change(...)`.
