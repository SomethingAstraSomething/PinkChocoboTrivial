//   MainWindow.cs - Host control panel for Trivial Night           
//   This is the main UI where the host controls the game:          
//   start/stop, send questions, adjust scores, configure settings. 
//   Uses ImGui for rendering (Dalamud's built-in UI framework).    

using System;
using System.Linq;
using System.Numerics;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Windowing;

namespace PinkChocoboTrivial.Windows;

/// <summary>
/// The main control panel window. Only the host (plugin user) sees this.
/// Provides buttons to start/stop the game, send questions, view scores,
/// and manually adjust points.
/// </summary>
public class MainWindow : Window, IDisposable
{
    private readonly Plugin plugin;

    // Input buffers for ImGui text fields
    // ImGui requires mutable buffers for text input fields

    /// <summary>Buffer for the "add player" text input.</summary>
    private string addPlayerName = string.Empty;

    /// <summary>Buffer for manual point adjustment amount.</summary>
    private int adjustAmount = 1;

    /// <summary>The player selected in the scoreboard for point adjustment.</summary>
    private string selectedPlayer = string.Empty;

    // Constructor

    public MainWindow(Plugin plugin)
        : base("Pink Chocobo Club, Trivial Night!###PCCTrivialMain",
               ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse)
    {
        // Set window size constraints
        SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(520, 600),
            MaximumSize = new Vector2(800, 900)
        };

        this.plugin = plugin;
    }

    public void Dispose() { }

    // Main draw loop (called every frame when window is open)

    public override void Draw()
    {
        var game = plugin.Game;
        var config = plugin.Configuration;

        //
        //  HEADER - Game title and status
        //

        // Pink/magenta color for the title to match the club theme
        ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(1.0f, 0.4f, 0.7f, 1.0f));
        ImGui.Text("Pink Chocobo Club, Trivial Night!");
        ImGui.PopStyleColor();

        ImGui.Separator();

        // Show current game state
        var stateText = game.State switch
        {
            GameState.Idle => "Not Started",
            GameState.QuestionActive => "Question Active!",
            GameState.BetweenQuestions => "Ready for Next Question",
            GameState.GameOver => "Game Over!",
            _ => "Unknown"
        };

        // Color the state text based on game state
        var stateColor = game.State switch
        {
            GameState.Idle => new Vector4(0.5f, 0.5f, 0.5f, 1.0f),           // Grey
            GameState.QuestionActive => new Vector4(0.2f, 1.0f, 0.2f, 1.0f), // Green
            GameState.BetweenQuestions => new Vector4(1.0f, 1.0f, 0.2f, 1.0f),// Yellow
            GameState.GameOver => new Vector4(1.0f, 0.4f, 0.7f, 1.0f),       // Pink
            _ => new Vector4(1.0f, 1.0f, 1.0f, 1.0f)
        };

        ImGui.Text("Status: ");
        ImGui.SameLine();
        ImGui.PushStyleColor(ImGuiCol.Text, stateColor);
        ImGui.Text(stateText);
        ImGui.PopStyleColor();

        // Show remaining questions count
        ImGui.Text($"Questions remaining: {game.RemainingQuestions()} / {game.TotalQuestions()}");

        ImGui.Spacing();

        //
        //  TABS - Organize the UI into logical sections
        //

        if (ImGui.BeginTabBar("TrivialTabs"))
        {
            // TAB 1: Game Control
            if (ImGui.BeginTabItem("Game Control"))
            {
                DrawGameControlTab(game, config);
                ImGui.EndTabItem();
            }

            // TAB 2: Scoreboard
            if (ImGui.BeginTabItem("Scoreboard"))
            {
                DrawScoreboardTab(game, config);
                ImGui.EndTabItem();
            }

            // TAB 3: Settings
            if (ImGui.BeginTabItem("Settings"))
            {
                DrawSettingsTab(config);
                ImGui.EndTabItem();
            }

            // TAB 4: Game Log
            if (ImGui.BeginTabItem("Game Log"))
            {
                DrawLogTab(game);
                ImGui.EndTabItem();
            }

            ImGui.EndTabBar();
        }
    }

    //
    //  TAB DRAWING METHODS
    //

    /// <summary>
    /// Draws the Game Control tab with start/stop/next question buttons,
    /// current question display, and timer.
    /// </summary>
    private void DrawGameControlTab(TriviaGame game, Configuration config)
    {
        ImGui.Spacing();

        // Game control buttons
        if (game.State == GameState.Idle)
        {
            // Game hasn't started yet - show "Start Game" button
            ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0.2f, 0.7f, 0.2f, 1.0f));
            if (ImGui.Button("Start Game", new Vector2(200, 40)))
            {
                game.StartGame();
            }
            ImGui.PopStyleColor();
        }
        else if (game.State == GameState.GameOver)
        {
            // Game is over - show winner and restart option
            ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(1.0f, 0.84f, 0.0f, 1.0f)); // Gold
            ImGui.Text($"WINNER: {game.WinnerName}!");
            ImGui.PopStyleColor();

            ImGui.Spacing();

            ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0.2f, 0.7f, 0.2f, 1.0f));
            if (ImGui.Button("Start New Game", new Vector2(200, 40)))
            {
                game.StartGame();
            }
            ImGui.PopStyleColor();
        }
        else
        {
            // Game is running - show control buttons
            if (game.State == GameState.BetweenQuestions)
            {
                // Ready for next question
                ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0.2f, 0.5f, 0.9f, 1.0f));
                if (ImGui.Button("Send Next Question", new Vector2(200, 40)))
                {
                    game.NextQuestion();
                }
                ImGui.PopStyleColor();
            }
            else if (game.State == GameState.QuestionActive)
            {
                // Question is active - show skip button
                ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0.9f, 0.6f, 0.1f, 1.0f));
                if (ImGui.Button("Skip Question", new Vector2(200, 40)))
                {
                    game.SkipQuestion();
                }
                ImGui.PopStyleColor();
            }

            ImGui.SameLine();

            // Stop game button (always available when game is running)
            ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0.8f, 0.2f, 0.2f, 1.0f));
            if (ImGui.Button("Stop Game", new Vector2(200, 40)))
            {
                game.StopGame();
            }
            ImGui.PopStyleColor();
        }

        ImGui.Spacing();
        ImGui.Separator();
        ImGui.Spacing();

        // Current question display
        ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(1.0f, 0.4f, 0.7f, 1.0f));
        ImGui.Text("Current Question:");
        ImGui.PopStyleColor();

        if (game.CurrentQuestion != null)
        {
            ImGui.TextWrapped($"[{game.CurrentQuestion.Category}] {game.CurrentQuestion.Question}");

            // Show the accepted answers (host-only, not visible in chat)
            ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0.5f, 0.5f, 0.5f, 1.0f));
            ImGui.Text($"Answer(s): {string.Join(" / ", game.CurrentQuestion.Answers)}");
            ImGui.PopStyleColor();
        }
        else
        {
            ImGui.TextWrapped("No question active.");
        }

        ImGui.Spacing();

        // Timer display
        if (game.State == GameState.QuestionActive)
        {
            // Color the timer based on urgency
            var timerColor = game.SecondsRemaining switch
            {
                > 30 => new Vector4(0.2f, 1.0f, 0.2f, 1.0f),  // Green (plenty of time)
                > 10 => new Vector4(1.0f, 1.0f, 0.2f, 1.0f),  // Yellow (getting close)
                _ => new Vector4(1.0f, 0.2f, 0.2f, 1.0f)       // Red (almost out!)
            };

            ImGui.PushStyleColor(ImGuiCol.Text, timerColor);
            ImGui.Text($"Time Remaining: {game.SecondsRemaining}s");
            ImGui.PopStyleColor();

            // Show a progress bar for the timer
            var fraction = (float)game.SecondsRemaining / config.TimerSeconds;
            ImGui.ProgressBar(fraction, new Vector2(-1, 20), $"{game.SecondsRemaining}s");
        }

        // Answer status
        if (game.QuestionAnswered)
        {
            ImGui.Spacing();
            ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0.2f, 1.0f, 0.2f, 1.0f));
            ImGui.Text($"Answered correctly by: {game.CorrectAnswerPlayer}");
            ImGui.PopStyleColor();
        }

        // Participants this round
        if (game.CurrentQuestionParticipants.Count > 0)
        {
            ImGui.Spacing();
            ImGui.Text($"Players who answered this round ({game.CurrentQuestionParticipants.Count}):");
            foreach (var p in game.CurrentQuestionParticipants)
            {
                ImGui.BulletText(p);
            }
        }

        // Reset question pool button
        ImGui.Spacing();
        ImGui.Separator();
        ImGui.Spacing();

        if (ImGui.Button("Reset Question Pool"))
        {
            game.ResetQuestionPool();
        }
        ImGui.SameLine();
        ImGui.TextWrapped("(Makes all questions available again)");
    }

    /// <summary>
    /// Draws the Scoreboard tab with player scores and manual adjustment controls.
    /// </summary>
    private void DrawScoreboardTab(TriviaGame game, Configuration config)
    {
        ImGui.Spacing();

        ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(1.0f, 0.4f, 0.7f, 1.0f));
        ImGui.Text("Scoreboard");
        ImGui.PopStyleColor();

        ImGui.Text($"Points to win: {config.PointsToWin}");
        ImGui.Spacing();

        // Scoreboard table
        if (game.Scoreboard.Count > 0)
        {
            // Sort by score descending
            var sorted = game.Scoreboard.OrderByDescending(kv => kv.Value).ToList();

            if (ImGui.BeginTable("ScoreTable", 3,
                ImGuiTableFlags.Borders | ImGuiTableFlags.RowBg))
            {
                // Table headers
                ImGui.TableSetupColumn("Rank", ImGuiTableColumnFlags.WidthFixed, 50);
                ImGui.TableSetupColumn("Player", ImGuiTableColumnFlags.WidthStretch);
                ImGui.TableSetupColumn("Score", ImGuiTableColumnFlags.WidthFixed, 80);
                ImGui.TableHeadersRow();

                // Table rows
                for (var i = 0; i < sorted.Count; i++)
                {
                    ImGui.TableNextRow();

                    // Rank column
                    ImGui.TableSetColumnIndex(0);
                    // Gold/silver/bronze colors for top 3
                    if (i == 0)
                        ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(1.0f, 0.84f, 0.0f, 1.0f));
                    else if (i == 1)
                        ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0.75f, 0.75f, 0.75f, 1.0f));
                    else if (i == 2)
                        ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0.8f, 0.5f, 0.2f, 1.0f));

                    ImGui.Text($"#{i + 1}");

                    if (i < 3)
                        ImGui.PopStyleColor();

                    // Player name column
                    ImGui.TableSetColumnIndex(1);
                    var isSelected = selectedPlayer == sorted[i].Key;
                    if (ImGui.Selectable(sorted[i].Key, isSelected))
                    {
                        selectedPlayer = sorted[i].Key;
                    }

                    // Score column
                    ImGui.TableSetColumnIndex(2);
                    ImGui.Text($"{sorted[i].Value} / {config.PointsToWin}");
                }

                ImGui.EndTable();
            }
        }
        else
        {
            ImGui.TextWrapped("No players yet. Players are added automatically when they answer in chat.");
        }

        ImGui.Spacing();
        ImGui.Separator();
        ImGui.Spacing();

        // Manual point adjustment
        ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(1.0f, 0.4f, 0.7f, 1.0f));
        ImGui.Text("Manual Point Adjustment");
        ImGui.PopStyleColor();

        ImGui.TextWrapped("Select a player from the table above, then adjust their score.");

        if (!string.IsNullOrEmpty(selectedPlayer))
        {
            ImGui.Text($"Selected: {selectedPlayer}");

            ImGui.SetNextItemWidth(100);
            ImGui.InputInt("Points", ref adjustAmount);

            ImGui.SameLine();

            // Add points button (green)
            ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0.2f, 0.7f, 0.2f, 1.0f));
            if (ImGui.Button($"+{adjustAmount}"))
            {
                game.AdjustPoints(selectedPlayer, Math.Abs(adjustAmount));
            }
            ImGui.PopStyleColor();

            ImGui.SameLine();

            // Remove points button (red)
            ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0.8f, 0.2f, 0.2f, 1.0f));
            if (ImGui.Button($"-{adjustAmount}"))
            {
                game.AdjustPoints(selectedPlayer, -Math.Abs(adjustAmount));
            }
            ImGui.PopStyleColor();

            ImGui.SameLine();

            // Remove player button
            if (ImGui.Button("Remove Player"))
            {
                game.RemovePlayer(selectedPlayer);
                selectedPlayer = string.Empty;
            }
        }

        ImGui.Spacing();
        ImGui.Separator();
        ImGui.Spacing();

        // Add player manually
        ImGui.Text("Add Player Manually:");

        ImGui.SetNextItemWidth(200);
        ImGui.InputText("##AddPlayer", ref addPlayerName, 64);

        ImGui.SameLine();

        if (ImGui.Button("Add") && !string.IsNullOrWhiteSpace(addPlayerName))
        {
            game.AddPlayer(addPlayerName.Trim());
            addPlayerName = string.Empty;
        }
    }

    /// <summary>
    /// Draws the Settings tab for configuring game parameters.
    /// </summary>
    private void DrawSettingsTab(Configuration config)
    {
        ImGui.Spacing();

        ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(1.0f, 0.4f, 0.7f, 1.0f));
        ImGui.Text("Game Settings");
        ImGui.PopStyleColor();

        ImGui.Spacing();

        // Points to win setting
        var pointsToWin = config.PointsToWin;
        ImGui.SetNextItemWidth(150);
        if (ImGui.InputInt("Points to Win", ref pointsToWin))
        {
            // Clamp between 1 and 50
            config.PointsToWin = Math.Clamp(pointsToWin, 1, 50);
            config.Save();
        }
        ImGui.TextWrapped("How many points a player needs to win the game.");

        ImGui.Spacing();

        // Timer duration setting
        var timerSec = config.TimerSeconds;
        ImGui.SetNextItemWidth(150);
        if (ImGui.InputInt("Timer (seconds)", ref timerSec))
        {
            // Clamp between 10 and 300 seconds
            config.TimerSeconds = Math.Clamp(timerSec, 10, 300);
            config.Save();
        }
        ImGui.TextWrapped("How many seconds players have to answer each question.");

        ImGui.Spacing();

        // Yell vs Shout toggle
        var useYell = config.UseYell;
        if (ImGui.Checkbox("Use /yell for questions (unchecked = /shout)", ref useYell))
        {
            config.UseYell = useYell;
            config.Save();
        }
        ImGui.TextWrapped("Both /yell and /shout are always accepted for answers regardless.");

        ImGui.Spacing();
        ImGui.Separator();
        ImGui.Spacing();

        // Command help
        ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(1.0f, 0.4f, 0.7f, 1.0f));
        ImGui.Text("Commands:");
        ImGui.PopStyleColor();
        ImGui.BulletText("/pcctn - Toggle this window");

        ImGui.Spacing();

        ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0.5f, 0.5f, 0.5f, 1.0f));
        ImGui.TextWrapped("Pink Chocobo Club, Trivial Night! v1.0.0");
        ImGui.TextWrapped("A trivia game plugin for FFXIV community events.");
        ImGui.PopStyleColor();
    }

    /// <summary>
    /// Draws the Game Log tab showing timestamped event history.
    /// </summary>
    private void DrawLogTab(TriviaGame game)
    {
        ImGui.Spacing();

        ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(1.0f, 0.4f, 0.7f, 1.0f));
        ImGui.Text("Game Log");
        ImGui.PopStyleColor();

        ImGui.Spacing();

        if (ImGui.Button("Clear Log"))
        {
            game.GameLog.Clear();
        }

        ImGui.Spacing();

        // Scrollable log area
        if (ImGui.BeginChild("LogScroll", new Vector2(0, -1), true))
        {
            // Show log entries in reverse order (newest first)
            for (var i = game.GameLog.Count - 1; i >= 0; i--)
            {
                ImGui.TextWrapped(game.GameLog[i]);
            }

            ImGui.EndChild();
        }
    }
}
