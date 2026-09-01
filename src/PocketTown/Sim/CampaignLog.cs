namespace PocketTown.Sim;

public class CampaignEvent
{
    public int Day { get; set; }
    public string Kind { get; set; } = "";
    public string Text { get; set; } = "";
    public float PollBefore { get; set; }
    public float PollAfter { get; set; }
}

public class Headline
{
    public int Day { get; set; }
    public string Text { get; set; } = "";
}

public class PollSnapshot
{
    public int Day { get; set; }
    public string Phase { get; set; } = "";
    public float Value { get; set; }
}
