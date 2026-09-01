namespace PocketTown.Sim;

/// <summary>
/// Phase machine for one campaign run. Placeholder events live here until each day
/// gets its own scene; they still write real poll deltas and headlines so the news TV works.
/// </summary>
public static class DayManager
{
    public readonly record struct EventScript(string Speaker, IReadOnlyList<string> Lines);

    public static EventScript BeginTodaysEvent(RunState run)
    {
        run.Phase = DayPhase.Event;
        float before = run.Poll;
        var (playerSwing, opponentLine, headlines, lines) = StubFor(run.Day);
        PollModel.ApplySwing(run.Blocs, playerSwing);
        // Incumbent bleeds a little every day just by existing — doing nothing still loses slowly.
        PollModel.ApplySwing(run.Blocs, -1.2f, favoredBloc: "OLD GUARD");

        float after = run.Poll;
        run.LastOpponentAction = opponentLine;
        run.TomorrowModifier = ModifierFor(run.Day);
        run.Log.Add(new CampaignEvent
        {
            Day = run.Day,
            Kind = CampaignCalendar.Name(run.Day),
            Text = lines[0],
            PollBefore = before,
            PollAfter = after,
        });
        foreach (var h in headlines)
            run.Headlines.Add(new Headline { Day = run.Day, Text = h });
        run.RecordPoll("event");
        return new EventScript("DAY " + run.Day, lines);
    }

    public static void BeginNews(RunState run)
    {
        run.Phase = DayPhase.News;
        run.RecordPoll("news");
    }

    /// <summary>End the day: save happens in the scene. Increments the calendar or marks the run finished.</summary>
    public static void Sleep(RunState run)
    {
        run.Phase = DayPhase.Sleep;
        if (run.Day >= CampaignCalendar.DayCount)
        {
            run.Finished = true;
            run.Won = run.Poll >= 50f;
            run.RecordPoll("final");
            return;
        }

        run.Day++;
        run.Phase = DayPhase.Morning;
        run.RecordPoll("morning");
    }

    private static (float swing, string opponent, string[] headlines, string[] lines) StubFor(int day) => day switch
    {
        1 => (
            6f,
            "QUINCE FILED HIS PAPERS, THEN ASKED WHERE HE WAS.",
            new[]
            {
                "DARK HORSE FILES FOR MAYOR, POLLS IN THE TEENS",
                "INCUMBENT QUINCE: 'I HAVE ALWAYS BEEN RUNNING. I THINK.'",
            },
            new[]
            {
                "TOWN HALL. The clerk slides you a candidacy form the size of a picnic blanket.",
                "You write your name. Somewhere in the lobby, HAROLD QUINCE files his own papers and forgets why he came.",
                "You are on the ballot. The first poll has you in the teens. Mom is already calling you Mayor.",
            }),
        2 => (
            8f,
            "QUINCE CANVASSED ONE PORCH, THEN NAPPED IN THE ROCKING CHAIR.",
            new[]
            {
                "CHALLENGER KNOCKS, TOWN NOTICES",
                "RIVERSIDE SAYS NOBODY ASKED THEM ANYTHING LAST CYCLE",
            },
            new[]
            {
                "You spend the day on porches. Some doors open. Some dogs vote with their teeth.",
                "A shopkeeper admits she has never met the incumbent. That is not an endorsement. It is a start.",
            }),
        3 => (
            5f,
            "QUINCE'S RALLY WAS ATTENDED BY THREE PIGEONS AND A COUSIN.",
            new[]
            {
                "CANDIDATE ADDRESSES TOWN SQUARE, MOSTLY IN THE CORRECT ORDER",
                "HECKLER SHOUTS ABOUT POTHHOLES, GETS A ROUND OF APPLAUSE",
            },
            new[]
            {
                "Town square. A wobbly mic. You promise to look at the potholes like they personally offended you.",
                "It is not a great speech. It is louder than silence, which is the local bar for oratory.",
            }),
        4 => (
            3f,
            "QUINCE'S INTERVIEW WAS MOSTLY HIM ASKING THE ANCHOR FOR THE QUESTION AGAIN.",
            new[]
            {
                "LIVE ON WMAP-7: CANDIDATE SURVIVES THE LIGHTS",
                "ANCHOR: 'HOW WILL YOU PAY FOR THAT?' CANDIDATE: 'NEXT QUESTION.'",
            },
            new[]
            {
                "The studio is smaller than it looks on TV. The anchor smiles with too many teeth.",
                "You dodge one trap, eat another, and walk out with a clip they will replay all week.",
            }),
        5 => (
            9f,
            "QUINCE BROUGHT NOTES TO THE DEBATE. THEY WERE A GROCERY LIST.",
            new[]
            {
                "DEBATE NIGHT: CHALLENGER LANDS A LINE, ROOM ACTUALLY LAUGHS",
                "QUINCE CALLS YOU 'THE OTHER FELLOW' FOR FORTY MINUTES",
            },
            new[]
            {
                "Podiums. A moderator who has given up. QUINCE reads a grocery list into the mic.",
                "You land one line the room laughs at. For a second the Sway meter is yours.",
            }),
        6 => (
            2f,
            "QUINCE VOTED, THEN ASKED IF HE HAD VOTED.",
            new[]
            {
                "CAMPAIGNING BANNED NEAR POLLS, TENSION IS THE WHOLE JOB NOW",
                "WEATHER: FINE. TURNOUT: ANYONE'S GUESS. DOG: STILL FOLLOWING YOU.",
            },
            new[]
            {
                "No more knocking. No more speeches. You walk the town and people look at you like a weather report.",
                "A dog you befriended on Tuesday follows you to the diner. That might be the whole strategy.",
            }),
        _ => (
            0f,
            "QUINCE BROUGHT A CONCESSION SPEECH AND A VICTORY SPEECH IN THE SAME ENVELOPE.",
            new[]
            {
                "THE COUNT IS IN — SORT OF",
                "TOWN HOLDS ITS BREATH, ALSO ITS CASSEROLES",
            },
            new[]
            {
                "Town Hall. Everyone you met this week is in the room. The machines hum like nervous bees.",
                "Bloc by bloc, the numbers come in. This is the part where you cannot knock on any more doors.",
            }),
    };

    private static string ModifierFor(int today) => today switch
    {
        1 => "TOMORROW: A STIFF BREEZE. FLYERS WILL GO WHERE THEY WANT.",
        2 => "TOMORROW: HEAT WAVE. SHORTER TEMPERS ON THE SQUARE.",
        3 => "TOMORROW: THE ANCHOR IS IN A MOOD. BRING NOTES.",
        4 => "TOMORROW: QUINCE FOUND HIS DEBATE GLASSES. THIS CHANGES NOTHING.",
        5 => "TOMORROW: CAMPAIGNING NEAR THE POLLS IS ILLEGAL. WALK SOFTLY.",
        6 => "TOMORROW: THE MACHINES ARE WARM. BRING YOUR CASSEROLE AND YOUR SPEECH.",
        _ => "THE BROADCAST SIGNING OFF. GO HOME.",
    };
}
