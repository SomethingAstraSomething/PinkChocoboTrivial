// Configuration.cs - Persistent plugin settings
// Saved to disk automatically by Dalamud between sessions.

using Dalamud.Configuration;
using System;
using System.Collections.Generic;

namespace PinkChocoboTrivial;

/// <summary>
/// Stores all persistent settings for the trivia plugin.
/// Dalamud serializes this to JSON and saves it in the plugin config folder.
/// </summary>
[Serializable]
public class Configuration : IPluginConfiguration
{
    /// <summary>Config schema version (used by Dalamud for migration).</summary>
    public int Version { get; set; } = 0;

    /// <summary>
    /// How many points a player needs to win the game.
    /// The host can change this from the UI before starting a game.
    /// </summary>
    public int PointsToWin { get; set; } = 5;

    /// <summary>
    /// How many seconds players have to answer each question.
    /// Default is 60 seconds as requested.
    /// </summary>
    public int TimerSeconds { get; set; } = 60;

    /// <summary>
    /// Whether to use /yell (true) or /shout (false) for sending questions.
    /// Both are always accepted for answers regardless of this setting.
    /// </summary>
    public bool UseYell { get; set; } = true;

    /// <summary>
    /// List of question indices that have already been used in this session.
    /// Prevents the same question from appearing twice.
    /// </summary>
    public List<int> UsedQuestionIds { get; set; } = new();

    /// <summary>
    /// Saves the current configuration to disk via Dalamud.
    /// Call this after any setting change to persist it.
    /// </summary>
    public void Save()
    {
        Plugin.PluginInterface.SavePluginConfig(this);
    }
}
