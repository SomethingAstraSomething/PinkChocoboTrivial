// ScoreboardWindow.cs - Standalone scoreboard overlay
// A compact, always-on-top scoreboard that can be shown
// separately from the main control panel. Useful for the host
// to keep track of scores while the main window is hidden.

using System;
using System.Linq;
using System.Numerics;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Windowing;

namespace PinkChocoboTrivial.Windows;

/// <summary>
/// A lightweight scoreboard window that shows player rankings.
/// Can be toggled independently from the main control panel.
/// </summary>
public class ScoreboardWindow : Window, IDisposable
{
    private readonly Plugin plugin;

    public ScoreboardWindow(Plugin plugin)
        : base("Trivial Night - Scoreboard###PCCTrivialScoreboard",
               ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.AlwaysAutoResize)
    {
        // Compact size for an overlay-style scoreboard
        SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(250, 100),
            MaximumSize = new Vector2(400, 500)
        };

        this.plugin = plugin;
    }

    public void Dispose() { }

    public override void Draw()
    {
        var game = plugin.Game;
        var config = plugin.Configuration;

        // Header with pink theme
        ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(1.0f, 0.4f, 0.7f, 1.0f));
        ImGui.Text("Trivial Night Scoreboard");
        ImGui.PopStyleColor();

        // Show target score
        ImGui.Text($"First to {config.PointsToWin} wins!");

        // Timer (if question is active)
        if (game.State == GameState.QuestionActive)
        {
            var timerColor = game.SecondsRemaining switch
            {
                > 30 => new Vector4(0.2f, 1.0f, 0.2f, 1.0f),
                > 10 => new Vector4(1.0f, 1.0f, 0.2f, 1.0f),
                _ => new Vector4(1.0f, 0.2f, 0.2f, 1.0f)
            };

            ImGui.PushStyleColor(ImGuiCol.Text, timerColor);
            ImGui.Text($"Time: {game.SecondsRemaining}s");
            ImGui.PopStyleColor();
        }

        ImGui.Separator();

        // Score list
        if (game.Scoreboard.Count == 0)
        {
            ImGui.TextWrapped("Waiting for players...");
            return;
        }

        // Sort players by score (highest first)
        var sorted = game.Scoreboard
            .OrderByDescending(kv => kv.Value)
            .ToList();

        for (var i = 0; i < sorted.Count; i++)
        {
            // Medal colors for top 3 players
            if (i == 0)
                ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(1.0f, 0.84f, 0.0f, 1.0f));  // Gold
            else if (i == 1)
                ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0.75f, 0.75f, 0.75f, 1.0f)); // Silver
            else if (i == 2)
                ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0.8f, 0.5f, 0.2f, 1.0f));    // Bronze

            // Display rank, name, and score
            ImGui.Text($"#{i + 1}  {sorted[i].Key}  —  {sorted[i].Value} pts");

            if (i < 3)
                ImGui.PopStyleColor();
        }

        // Winner announcement
        if (game.State == GameState.GameOver && !string.IsNullOrEmpty(game.WinnerName))
        {
            ImGui.Spacing();
            ImGui.Separator();
            ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(1.0f, 0.84f, 0.0f, 1.0f));
            ImGui.Text($"WINNER: {game.WinnerName}!");
            ImGui.PopStyleColor();
        }
    }
}
