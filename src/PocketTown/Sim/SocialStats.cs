namespace PocketTown.Sim;

/// <summary>S.O.C.I.A.L. sheet. Registration will become point-buy; defaults are a flat 4s (24 points).</summary>
public class SocialStats
{
    public int Strategy { get; set; } = 4;
    public int Outreach { get; set; } = 4;
    public int Charm { get; set; } = 4;
    public int Improvisation { get; set; } = 4;
    public int Authority { get; set; } = 4;
    public int Luck { get; set; } = 4;
}
