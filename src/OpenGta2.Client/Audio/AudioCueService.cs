using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework.Audio;
using OpenGta2.Client.Diagnostics;

namespace OpenGta2.Client.Audio;

/// <summary>
/// Week-1 thin play-by-id cue player (Pulse / Vega).
/// Always logs + DiagnosticValues; plays a SoundEffect when a WAV exists under
/// Assets/Audio/{ui|stinger|sfx|amb|mus}/&lt;id&gt;.wav (placeholders OK — stub is default).
/// </summary>
public sealed class AudioCueService : IDisposable
{
    private readonly Dictionary<string, SoundEffect?> _cache = new(StringComparer.Ordinal);
    private readonly string _audioRoot;
    private bool _disposed;

    public AudioCueService(string? audioRoot = null)
    {
        // GtaGame Content.RootDirectory is "Assets"; cue sheet paths map under Audio/.
        _audioRoot = audioRoot
            ?? Path.Combine(AppContext.BaseDirectory, "Assets", "Audio");
    }

    /// <summary>Last cue id played this session (debug / smoke).</summary>
    public string? LastCueId { get; private set; }

    /// <summary>How many Play calls this session.</summary>
    public int PlayCount { get; private set; }

    /// <summary>
    /// Play cue by stable id. Missing assets → console + DiagnosticValues only (Week 1 stub).
    /// </summary>
    public void Play(string cueId)
    {
        if (string.IsNullOrWhiteSpace(cueId) || _disposed)
            return;

        LastCueId = cueId;
        PlayCount++;

        Console.WriteLine($"[Pulse] cue {cueId}");
        DiagnosticValues.Set("audio.cue", cueId);
        DiagnosticValues.Set($"audio.last", $"{cueId} #{PlayCount}");

        var effect = GetOrLoad(cueId);
        if (effect != null)
        {
            effect.Play();
            DiagnosticValues.Set($"audio.{cueId}", "wav");
        }
        else
        {
            DiagnosticValues.Set($"audio.{cueId}", "stub");
        }
    }

    private SoundEffect? GetOrLoad(string cueId)
    {
        if (_cache.TryGetValue(cueId, out var cached))
            return cached;

        SoundEffect? loaded = null;
        foreach (var path in CandidatePaths(cueId))
        {
            if (!File.Exists(path))
                continue;

            try
            {
                using var stream = File.OpenRead(path);
                loaded = SoundEffect.FromStream(stream);
                Console.WriteLine($"[Pulse] loaded {path}");
                break;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Pulse] failed to load {path}: {ex.Message}");
            }
        }

        _cache[cueId] = loaded;
        return loaded;
    }

    private IEnumerable<string> CandidatePaths(string cueId)
    {
        // Prefer category folder from cue prefix (ui_, stinger_, sfx_, amb_, mus_).
        var category = CategoryFor(cueId);
        yield return Path.Combine(_audioRoot, category, $"{cueId}.wav");
        yield return Path.Combine(_audioRoot, category, $"{cueId}.ogg");
        // Flat fallback
        yield return Path.Combine(_audioRoot, $"{cueId}.wav");
    }

    private static string CategoryFor(string cueId)
    {
        if (cueId.StartsWith("ui_", StringComparison.Ordinal))
            return "ui";
        if (cueId.StartsWith("stinger_", StringComparison.Ordinal))
            return "stinger";
        if (cueId.StartsWith("sfx_", StringComparison.Ordinal))
            return "sfx";
        if (cueId.StartsWith("amb_", StringComparison.Ordinal))
            return "amb";
        if (cueId.StartsWith("mus_", StringComparison.Ordinal))
            return "mus";
        return "ui";
    }

    public void Dispose()
    {
        if (_disposed)
            return;
        _disposed = true;
        foreach (var sfx in _cache.Values)
            sfx?.Dispose();
        _cache.Clear();
    }
}
