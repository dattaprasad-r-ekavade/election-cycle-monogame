namespace PocketTown.Sim;

public enum DayPhase
{
    Morning,
    Event,
    News,
    Sleep,
}

public enum CampaignDay
{
    Broadcast = 1,
    Canvassing = 2,
    Interview = 3,
    PublicAddress = 4,
    Debate = 5,
    Results = 6,
}

public static class CampaignCalendar
{
    public const int DayCount = 6;

    public static string Name(int day) => day switch
    {
        1 => "ON THE AIR",
        2 => "CANVASSING",
        3 => "MEDIA INTERVIEW",
        4 => "PUBLIC ADDRESS",
        5 => "DEBATE",
        6 => "RESULTS",
        _ => "CAMPAIGN",
    };

    public static string ShortName(int day) => day switch
    {
        1 => "FILE PAPERS",
        2 => "KNOCK DOORS",
        3 => "FACE THE CAMERAS",
        4 => "GIVE A SPEECH",
        5 => "DEBATE THE OPPONENT",
        6 => "HEAR THE COUNT",
        _ => "CAMPAIGN",
    };
}
