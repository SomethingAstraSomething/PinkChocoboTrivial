// Plugin.cs - Main entry point for the Dalamud plugin
// Handles initialization, command registration, chat events,
// and coordinates all subsystems (UI, game logic, etc.)

using Dalamud.Game.Command;
using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using Dalamud.Interface.Windowing;
using PinkChocoboTrivial.Windows;
using System;

namespace PinkChocoboTrivial;

/// <summary>
/// Main plugin class. Implements IDalamudPlugin for Dalamud to discover and load it.
/// All Dalamud services are injected via the [PluginService] attribute.
/// </summary>
public sealed class Plugin : IDalamudPlugin
{
    // Dalamud services (injected automatically by the framework)

    /// <summary>Core plugin interface for config, UI builder, and manifest access.</summary>
    [PluginService] internal static IDalamudPluginInterface PluginInterface { get; private set; } = null!;

    /// <summary>Manages slash-command registration (e.g. /pcctn).</summary>
    [PluginService] internal static ICommandManager CommandManager { get; private set; } = null!;

    /// <summary>Provides access to the in-game chat log (read and send messages).</summary>
    [PluginService] internal static IChatGui ChatGui { get; private set; } = null!;

    /// <summary>Provides information about the local player character.</summary>
    [PluginService] internal static IClientState ClientState { get; private set; } = null!;

    /// <summary>Per-frame update hook used for timers and periodic logic.</summary>
    [PluginService] internal static IFramework Framework { get; private set; } = null!;

    /// <summary>Plugin logging service (visible via /xllog in-game).</summary>
    [PluginService] internal static IPluginLog Log { get; private set; } = null!;

    // Constants

    /// <summary>The slash command that opens the main trivia window.</summary>
    private const string CommandName = "/pcctn";

    // Plugin subsystems

    /// <summary>Persistent configuration (saved between sessions).</summary>
    public Configuration Configuration { get; init; }

    /// <summary>The core trivia game engine that tracks questions, players, and scores.</summary>
    public TriviaGame Game { get; init; }

    /// <summary>ImGui window system provided by Dalamud.</summary>
    public readonly WindowSystem WindowSystem = new("PinkChocoboTrivial");

    /// <summary>The main control panel window.</summary>
    private MainWindow MainWindow { get; init; }

    /// <summary>The live scoreboard window.</summary>
    private ScoreboardWindow ScoreboardWindow { get; init; }

    // Constructor (called once when Dalamud loads the plugin)

    public Plugin()
    {
        // Load saved configuration, or create a fresh one if none exists
        Configuration = PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();

        // Create the trivia game engine
        Game = new TriviaGame(this);

        // Create UI windows
        MainWindow = new MainWindow(this);
        ScoreboardWindow = new ScoreboardWindow(this);

        // Register windows with the Dalamud window system
        WindowSystem.AddWindow(MainWindow);
        WindowSystem.AddWindow(ScoreboardWindow);

        // Register the /pcctn slash command
        CommandManager.AddHandler(CommandName, new CommandInfo(OnCommand)
        {
            HelpMessage = "Open the Pink Chocobo Club Trivial Night control panel."
        });

        // Hook into Dalamud's draw loop so our ImGui windows get rendered
        PluginInterface.UiBuilder.Draw += WindowSystem.Draw;

        // Add a gear-icon button in the plugin installer to open our main UI
        PluginInterface.UiBuilder.OpenConfigUi += ToggleMainUi;
        PluginInterface.UiBuilder.OpenMainUi += ToggleMainUi;

        // Subscribe to incoming chat messages so we can detect player answers
        ChatGui.ChatMessage += OnChatMessage;

        // Subscribe to the per-frame update for timer logic
        Framework.Update += OnFrameworkUpdate;

        Log.Information("Pink Chocobo Club, Trivial Night! loaded successfully.");
    }

    // Disposal (called when the plugin is unloaded)

    public void Dispose()
    {
        // Unsubscribe from all events to prevent memory leaks
        Framework.Update -= OnFrameworkUpdate;
        ChatGui.ChatMessage -= OnChatMessage;
        PluginInterface.UiBuilder.Draw -= WindowSystem.Draw;
        PluginInterface.UiBuilder.OpenConfigUi -= ToggleMainUi;
        PluginInterface.UiBuilder.OpenMainUi -= ToggleMainUi;

        // Clean up windows
        WindowSystem.RemoveAllWindows();
        MainWindow.Dispose();
        ScoreboardWindow.Dispose();

        // Remove the slash command
        CommandManager.RemoveHandler(CommandName);
    }

    // Slash command handler

    /// <summary>
    /// Called when the player types /pcctn in chat.
    /// Toggles the main trivia control panel.
    /// </summary>
    private void OnCommand(string command, string args)
    {
        MainWindow.Toggle();
    }

    // Chat message handler

    /// <summary>
    /// Fired for every chat message the client receives.
    /// We filter for Yell and Shout messages and forward them to the game engine.
    /// </summary>
    private void OnChatMessage(
        XivChatType type,
        int senderId,
        ref SeString sender,
        ref SeString message,
        ref bool isHandled)
    {
        // We only care about Yell (30) and Shout (11) messages
        if (type != XivChatType.Yell && type != XivChatType.Shout)
            return;

        // Extract the sender's name and message text
        var senderName = sender.TextValue;
        var messageText = message.TextValue;

        // Ignore messages from the host (the player running the plugin)
        var localPlayer = ClientState.LocalPlayer;
        if (localPlayer != null)
        {
            var localName = localPlayer.Name.TextValue;
            // The sender name might include the world, e.g. "Name@World"
            if (senderName.Contains(localName, StringComparison.OrdinalIgnoreCase))
                return;
        }

        // Forward the answer attempt to the game engine
        Game.ProcessAnswer(senderName, messageText);
    }

    // Per-frame update

    /// <summary>
    /// Called every frame by the Dalamud framework.
    /// Used to tick the question timer down.
    /// </summary>
    private void OnFrameworkUpdate(IFramework framework)
    {
        Game.Update();
    }

    // UI toggles

    public void ToggleMainUi() => MainWindow.Toggle();
    public void ToggleScoreboardUi() => ScoreboardWindow.Toggle();

    /// <summary>
    /// Sends a chat message as the host player.
    /// Used to /yell questions and announcements.
    /// </summary>
    public void SendChat(string message)
    {
        try
        {
            ChatGui.SendMessage(message);
        }
        catch (Exception ex)
        {
            Log.Error($"Failed to send chat message: {ex.Message}");
        }
    }
}
