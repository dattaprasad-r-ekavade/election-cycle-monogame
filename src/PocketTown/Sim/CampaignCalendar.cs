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
    Registration = 1,
    Canvassing = 2,
    PublicAddress = 3,
    Interview = 4,
    Debate = 5,
    Voting = 6,
    Results = 7,
}

public static class CampaignCalendar
{
    public const int DayCount = 7;

    public static string Name(int day) => day switch
    {
        1 => "REGISTRATION",
        2 => "CANVASSING",
        3 => "PUBLIC ADDRESS",
        4 => "MEDIA INTERVIEW",
        5 => "DEBATE",
        6 => "VOTING DAY",
        7 => "RESULTS",
        _ => "CAMPAIGN",
    };

    public static string ShortName(int day) => day switch
    {
        1 => "FILE PAPERS",
        2 => "KNOCK DOORS",
        3 => "GIVE A SPEECH",
        4 => "FACE THE CAMERAS",
        5 => "DEBATE THE OPPONENT",
        6 => "WATCH THE POLLS",
        7 => "HEAR THE COUNT",
        _ => "CAMPAIGN",
    };
}
