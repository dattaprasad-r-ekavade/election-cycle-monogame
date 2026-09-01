namespace PocketTown.World;

/// <summary>JSON shape of a map file in <c>Data/Maps/*.json</c>.</summary>
public class MapData
{
    public string Name { get; set; } = "";
    public bool Outdoor { get; set; }
    /// <summary>ASCII tile layout, one string per row. See <see cref="TileCatalog"/> for the legend.</summary>
    public List<string> Rows { get; set; } = new();
    public SpawnData Spawn { get; set; } = new();
    public List<NpcData> Npcs { get; set; } = new();
    public List<WarpData> Warps { get; set; } = new();
    public List<InteractableData> Interactables { get; set; } = new();
}

public class SpawnData
{
    public int X { get; set; }
    public int Y { get; set; }
    public string Facing { get; set; } = "down";
}

public class NpcData
{
    public string Name { get; set; } = "???";
    /// <summary>Character sheet id, see <see cref="Core.Art"/> palettes (e.g. "mom", "elder", "girl", "boy").</summary>
    public string Sprite { get; set; } = "villager";
    public int X { get; set; }
    public int Y { get; set; }
    public string Facing { get; set; } = "down";
    /// <summary>When true the NPC wanders a couple of tiles around its start position.</summary>
    public bool Wander { get; set; }
    /// <summary>Dialogue pages shown when the player talks to this NPC.</summary>
    public List<string> Dialogue { get; set; } = new();
}

public class WarpData
{
    public int X { get; set; }
    public int Y { get; set; }
    public string ToMap { get; set; } = "";
    public int ToX { get; set; }
    public int ToY { get; set; }
    /// <summary>Direction the player faces after arriving.</summary>
    public string Facing { get; set; } = "down";
}

/// <summary>A tile the player can face and press Confirm on to read some text (signs, furniture...).</summary>
public class InteractableData
{
    public int X { get; set; }
    public int Y { get; set; }
    public List<string> Lines { get; set; } = new();
}
