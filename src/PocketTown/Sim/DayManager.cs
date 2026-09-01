namespace PocketTown.Sim;

/// <summary>
/// Episode 1 week: TV → register → canvass → interview → speech → debate → results.
/// The vending union climbs in the background and usually eats the count.
/// </summary>
public static class DayManager
{
    public readonly record struct EventScript(string Speaker, IReadOnlyList<string> Lines);

    public static EventScript BeginTodaysEvent(RunState run)
    {
        run.Phase = DayPhase.Event;
        float before = run.PlayerShare;
        var beat = BeatFor(run.Day);
        run.ApplyShares(beat.PlayerDelta, beat.OpponentDelta, beat.ThirdDelta);
        PollModel.ApplySwing(run.Blocs, beat.PlayerDelta * 0.6f);

        run.LastOpponentAction = beat.OpponentLine;
        run.TomorrowModifier = beat.Modifier;
        run.Log.Add(new CampaignEvent
        {
            Day = run.Day,
            Kind = CampaignCalendar.Name(run.Day),
            Text = beat.Lines[0],
            PollBefore = before,
            PollAfter = run.PlayerShare,
        });
        foreach (var h in beat.Headlines)
            run.Headlines.Add(new Headline { Day = run.Day, Text = h });
        run.RecordPoll("event");
        return new EventScript(beat.Speaker, beat.Lines);
    }

    public static void BeginNews(RunState run)
    {
        run.Phase = DayPhase.News;
        run.RecordPoll("news");
    }

    public static void Sleep(RunState run)
    {
        run.Phase = DayPhase.Sleep;
        if (run.Day >= CampaignCalendar.DayCount)
        {
            run.Finished = true;
            run.Won = run.Leader == run.PlayerName;
            run.RecordPoll("final");
            return;
        }

        run.Day++;
        run.Phase = DayPhase.Morning;
        run.RecordPoll("morning");
    }

    private readonly record struct Beat(
        string Speaker,
        float PlayerDelta,
        float OpponentDelta,
        float ThirdDelta,
        string OpponentLine,
        string Modifier,
        string[] Headlines,
        string[] Lines);

    private static Beat BeatFor(int day) => day switch
    {
        1 => new(
            "TOWN HALL",
            4f, -3f, 3f,
            "QUINCE FILED, THEN ASKED IF THE FORM WAS A WARRANTY.",
            "TOMORROW: PORCHES. BRING SHOES. MAYBE A TREAT FOR DOGS.",
            new[]
            {
                "CHALLENGER FILES FOR MAYOR AFTER WATCHING IT ON TV",
                "VENDING UNION ALSO FILES. CLERK: 'THEY HAD THE FEE IN QUARTERS.'",
            },
            new[]
            {
                "The clerk does not look up. The form is the size of a picnic blanket.",
                "You write: a clerk that never sleeps. A kiosk at Town Hall. Open at 2am. No line.",
                "In the lobby HAROLD QUINCE is arguing with a vending machine that has its own clipboard.",
                "You are on the ballot. So is the machine. Mom is already calling you Mayor.",
            }),
        2 => new(
            "PORCHES",
            6f, -4f, 5f,
            "QUINCE CANVASSED ONE PORCH AND NAPPED IN THE CHAIR.",
            "TOMORROW: THE ANCHOR HAS TEETH. BRING A SENTENCE.",
            new[]
            {
                "CHALLENGER KNOCKS. TOWN NOTICES. DOGS ALSO NOTICE.",
                "THREE HOUSEHOLDS ENDORSE 'WHATEVER IS IN THE SNACK SLOT.'",
            },
            new[]
            {
                "Door one: a mill family. They hear 'kiosk' and hear 'the mill again, but smaller.'",
                "Door two: a shopkeeper who wants Town Hall open after softball. She almost likes you.",
                "Door three: nobody home. A vending machine on the porch has a campaign sticker. It beeps.",
                "You promise nothing. You still feel like you promised something.",
            }),
        3 => new(
            "WMAP-7",
            2f, -1f, 6f,
            "QUINCE'S INTERVIEW WAS HIM ASKING FOR THE QUESTION AGAIN.",
            "TOMORROW: A MICROPHONE AND A SQUARE. DO NOT TRIP ON PURPOSE.",
            new[]
            {
                "LIVE: CANDIDATE CALLS THE KIOSK 'A VERY POLITE ROBOT UNCLE'",
                "CLIP ALREADY HAS A SOUND. THE SOUND IS NOT A GOOD SOUND.",
            },
            new[]
            {
                "The studio is smaller than on TV. The anchor smiles with too many teeth.",
                "'So the talking clerk replaces people?' You say it is more of a polite robot uncle.",
                "You try to walk it back. You say uncle in a larger sense. The red light stays on.",
                "In the green room a vending machine is giving a better interview than you.",
            }),
        4 => new(
            "TOWN SQUARE",
            5f, -3f, 4f,
            "QUINCE'S RALLY: THREE PIGEONS AND A COUSIN.",
            "TOMORROW: PODIUMS. QUINCE FOUND GLASSES. THIS CHANGES NOTHING.",
            new[]
            {
                "SPEECH INTERRUPTED BY A MACHINE THAT WANTS EQUAL TIME",
                "CROWD SPLITS: KIOSK / MILL / WHATEVER HAS CHIPS",
            },
            new[]
            {
                "A wobbly mic. You talk about a clerk that never sleeps. Someone yells about the mill.",
                "A vending machine rolls onstage and requests equal time. The crowd is not against this.",
                "You finish. It is louder than silence. Locally that is oratory.",
            }),
        5 => new(
            "DEBATE HALL",
            6f, -5f, 6f,
            "QUINCE BROUGHT NOTES. THEY WERE A GROCERY LIST.",
            "TOMORROW: THE MACHINES ARE WARM. BRING A CASSEROLE AND A SPEECH.",
            new[]
            {
                "DEBATE: THREE PODIUMS. ONE OF THEM HUMS.",
                "QUINCE CALLS YOU 'THE OTHER FELLOW' FOR FORTY MINUTES",
            },
            new[]
            {
                "Three podiums. The third one hums. The moderator has given up.",
                "QUINCE reads a grocery list. You land one line about 2am permits. The room laughs.",
                "The vending machine declines to take a side. It offers the moderator a soda. Applause.",
            }),
        _ => new(
            "THE COUNT",
            1f, -3f, 14f,
            "QUINCE BROUGHT A CONCESSION AND A VICTORY SPEECH IN ONE ENVELOPE.",
            "THE BROADCAST IS SIGNING OFF. GO HOME.",
            new[]
            {
                "THE COUNT IS IN. THE SNACKS ARE IN.",
                "MAPLE TOWN ELECTS A VENDING UNION. HUMANS CONCEDE.",
            },
            new[]
            {
                "Town Hall. Everyone you met is here. The machines hum like nervous bees.",
                "Your number is fine. QUINCE's number is fine. The third column keeps growing.",
                "Someone's dog barks. A coil drops. The clerk reads the winner like a warranty.",
                "You did not win. QUINCE did not win. The underdog did. Freeze frame.",
            }),
    };
}
