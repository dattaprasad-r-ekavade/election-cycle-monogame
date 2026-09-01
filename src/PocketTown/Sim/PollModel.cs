namespace PocketTown.Sim;

/// <summary>
/// Intentionally simple: poll is a weighted blend of bloc meters.
/// Every gameplay hook should write an attributed delta so the news can say why it moved.
/// </summary>
public static class PollModel
{
    public static float Compute(IReadOnlyList<BlocState> blocs, float noise = 0f)
    {
        double sum = 0;
        double weight = 0;
        foreach (var bloc in blocs)
        {
            double support = Sigmoid(bloc.Meter / 35.0);
            double w = Math.Max(0.01, bloc.Size) * Math.Clamp(bloc.Turnout, 0.1, 1.5);
            sum += w * support;
            weight += w;
        }

        float poll = (float)(100.0 * sum / Math.Max(weight, 0.0001) + noise);
        return Math.Clamp(poll, 1f, 99f);
    }

    /// <summary>Push every bloc's meter by <paramref name="delta"/>, scaled by size so big blocs move the headline more.</summary>
    public static void ApplySwing(IList<BlocState> blocs, float delta, string? favoredBloc = null)
    {
        foreach (var bloc in blocs)
        {
            float scale = 1f;
            if (favoredBloc != null && string.Equals(bloc.Name, favoredBloc, StringComparison.OrdinalIgnoreCase))
                scale = 1.6f;
            bloc.Meter = Math.Clamp(bloc.Meter + delta * scale, -100f, 100f);
        }
    }

    private static double Sigmoid(double x) => 1.0 / (1.0 + Math.Exp(-x));
}
