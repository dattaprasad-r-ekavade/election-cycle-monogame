using Microsoft.Xna.Framework.Audio;

namespace PocketTown.Core;

/// <summary>
/// Small chiptune-style sound effects synthesized at startup (square waves),
/// so no audio assets or content pipeline are needed. Silently disables itself
/// when no audio hardware is available (e.g. CI machines).
/// </summary>
public static class AudioBank
{
    private const int SampleRate = 22050;

    public static SoundEffect? TextBlip { get; private set; }
    public static SoundEffect? Confirm { get; private set; }
    public static SoundEffect? Bump { get; private set; }
    public static SoundEffect? Warp { get; private set; }

    private static bool _enabled;

    public static void Load()
    {
        try
        {
            TextBlip = Synth(0.035, t => 880, 0.12);
            Confirm = Synth(0.09, t => t < 0.045 ? 660 : 990, 0.15);
            Bump = Synth(0.06, t => 110, 0.2);
            Warp = Synth(0.25, t => 700 - 500 * (t / 0.25), 0.15);
            _enabled = true;
        }
        catch (NoAudioHardwareException)
        {
            _enabled = false;
        }
        catch (InvalidOperationException)
        {
            // OpenAL / device init failed on this machine.
            _enabled = false;
        }
    }

    public static void Dispose()
    {
        TextBlip?.Dispose();
        Confirm?.Dispose();
        Bump?.Dispose();
        Warp?.Dispose();
        TextBlip = Confirm = Bump = Warp = null;
        _enabled = false;
    }

    public static void Play(SoundEffect? effect)
    {
        if (_enabled && effect != null)
            effect.Play();
    }

    /// <summary>Build a square-wave sound. <paramref name="frequency"/> maps elapsed seconds to Hz.</summary>
    private static SoundEffect Synth(double seconds, Func<double, double> frequency, double volume)
    {
        int sampleCount = (int)(SampleRate * seconds);
        var buffer = new byte[sampleCount * 2];
        double phase = 0;

        for (int i = 0; i < sampleCount; i++)
        {
            double t = (double)i / SampleRate;
            phase += frequency(t) / SampleRate;
            // Short attack/release envelope to avoid clicks.
            double env = Math.Min(1.0, Math.Min(t / 0.005, (seconds - t) / 0.02));
            double sample = (phase % 1.0 < 0.5 ? 1.0 : -1.0) * volume * Math.Max(0, env);
            short value = (short)(sample * short.MaxValue);
            buffer[i * 2] = (byte)(value & 0xFF);
            buffer[i * 2 + 1] = (byte)((value >> 8) & 0xFF);
        }

        return new SoundEffect(buffer, SampleRate, AudioChannels.Mono);
    }
}
