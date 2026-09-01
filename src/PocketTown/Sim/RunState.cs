using PocketTown.Core;

namespace PocketTown.Sim;

/// <summary>
/// The spine of a campaign run. Serializable. Every scene should read and write this
/// rather than keeping its own copy of day, poll, or stats.
/// </summary>
public class RunState
{
    public string RunId { get; set; } = "";
    public int Seed { get; set; }
    public int Day { get; set; } = 1;
    public DayPhase Phase { get; set; } = DayPhase.Morning;
    public bool Finished { get; set; }
    public bool Won { get; set; }

    public string TownName { get; set; } = "MAPLE TOWN";
    public string OpponentName { get; set; } = "HAROLD QUINCE";
    public string OpponentTitle { get; set; } = "FORGETFUL INCUMBENT";
    public string PlayerName { get; set; } = "YOU";

    public SocialStats Stats { get; set; } = new();
    public List<BlocState> Blocs { get; set; } = new();
    public List<CampaignEvent> Log { get; set; } = new();
    public List<Headline> Headlines { get; set; } = new();
    public List<PollSnapshot> PollHistory { get; set; } = new();

    public string TomorrowModifier { get; set; } = "A QUIET NIGHT. NO SURPRISES... YET.";
    public string LastOpponentAction { get; set; } = "";

    public string MapId { get; set; } = "home";
    public int PlayerX { get; set; }
    public int PlayerY { get; set; }
    public string PlayerFacing { get; set; } = "down";

    public float Poll => PollModel.Compute(Blocs);

    public static RunState CreateNew(int? seed = null)
    {
        int s = seed ?? Random.Shared.Next();
        var run = new RunState
        {
            RunId = Guid.NewGuid().ToString("N")[..8],
            Seed = s,
            Day = 1,
            Phase = DayPhase.Morning,
            TownName = "MAPLE TOWN",
            OpponentName = "HAROLD QUINCE",
            OpponentTitle = "FORGETFUL INCUMBENT",
            MapId = Constants.StartMapId,
            PlayerX = 6,
            PlayerY = 5,
            PlayerFacing = "down",
            Blocs =
            {
                new BlocState { Name = "NEIGHBORS", Size = 0.40f, Turnout = 0.70f, Meter = -18f },
                new BlocState { Name = "SHOPKEEPERS", Size = 0.25f, Turnout = 0.80f, Meter = -8f },
                new BlocState { Name = "OLD GUARD", Size = 0.35f, Turnout = 0.90f, Meter = -36f },
            },
        };
        run.RecordPoll("start");
        return run;
    }

    public void SnapshotWorld(string mapId, Microsoft.Xna.Framework.Point tile, string facing)
    {
        MapId = mapId;
        PlayerX = tile.X;
        PlayerY = tile.Y;
        PlayerFacing = facing;
    }

    public void RecordPoll(string phase)
    {
        PollHistory.Add(new PollSnapshot
        {
            Day = Day,
            Phase = phase,
            Value = Poll,
        });
    }
}
