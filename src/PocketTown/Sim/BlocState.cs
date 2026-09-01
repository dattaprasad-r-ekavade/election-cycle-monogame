namespace PocketTown.Sim;

/// <summary>One voter bloc. Meter is sentiment toward the player, roughly -100..+100.</summary>
public class BlocState
{
    public string Name { get; set; } = "Voters";
    public float Size { get; set; } = 1f;
    public float Turnout { get; set; } = 1f;
    public float Meter { get; set; }
}
