using System.Text.Json;
using System.Text.Json.Serialization;

namespace PocketTown.Sim;

public static class SaveGame
{
    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNameCaseInsensitive = true,
        WriteIndented = true,
        Converters = { new JsonStringEnumConverter() },
    };

    public static string DirectoryPath =>
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ElectionCycle");

    public static string FilePath => Path.Combine(DirectoryPath, "save.json");

    public static bool Exists() => File.Exists(FilePath);

    public static void Write(RunState run)
    {
        Directory.CreateDirectory(DirectoryPath);
        File.WriteAllText(FilePath, JsonSerializer.Serialize(run, Options));
    }

    public static RunState? TryLoad()
    {
        if (!Exists())
            return null;
        try
        {
            var run = JsonSerializer.Deserialize<RunState>(File.ReadAllText(FilePath), Options);
            if (run == null || run.Finished || run.Day < 1 || run.Day > CampaignCalendar.DayCount)
                return null;
            if (run.Blocs.Count == 0)
                return null;
            return run;
        }
        catch (JsonException)
        {
            return null;
        }
        catch (IOException)
        {
            return null;
        }
    }

    public static void Delete()
    {
        if (Exists())
            File.Delete(FilePath);
    }
}
