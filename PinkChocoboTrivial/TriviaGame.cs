//  TriviaGame.cs - Core trivia game engine                        
//  Manages game state, question flow, scoring, timer,             
//  and player answer processing.                                  

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace PinkChocoboTrivial;

/// <summary>
/// Possible states the trivia game can be in.
/// </summary>
public enum GameState
{
    /// <summary>No game is running. Waiting for host to start.</summary>
    Idle,

    /// <summary>A question has been asked and we're waiting for answers.</summary>
    QuestionActive,

    /// <summary>The timer expired or someone answered. Showing results before next question.</summary>
    BetweenQuestions,

    /// <summary>A player has reached the target score. Game over!</summary>
    GameOver
}

/// <summary>
/// Core game engine. Tracks questions, players, scores, and the countdown timer.
/// The host controls the flow via the UI; answers are processed from chat events.
/// </summary>
public class TriviaGame
{
    // References 

    /// <summary>Reference to the main plugin for sending chat and accessing config.</summary>
    private readonly Plugin plugin;

    //Game state 

    /// <summary>Current state of the trivia game.</summary>
    public GameState State { get; private set; } = GameState.Idle;

    /// <summary>The full pool of questions loaded from the QuestionBank.</summary>
    private List<TriviaQuestion> allQuestions;

    /// <summary>The question currently being asked (null if none).</summary>
    public TriviaQuestion? CurrentQuestion { get; private set; }

    /// <summary>
    /// Dictionary of player names -> their current score.
    /// Every player who answers (right or wrong) gets tracked here.
    /// </summary>
    public Dictionary<string, int> Scoreboard { get; private set; } = new();

    /// <summary>
    /// Players who answered during the current question's window.
    /// Used to track participation per question.
    /// </summary>
    public List<string> CurrentQuestionParticipants { get; private set; } = new();

    /// <summary>Name of the player who answered correctly first (empty if no one did).</summary>
    public string CorrectAnswerPlayer { get; private set; } = string.Empty;

    /// <summary>Whether the current question has been answered correctly.</summary>
    public bool QuestionAnswered { get; private set; }

    /// <summary>Name of the player who won the game (empty if game not over).</summary>
    public string WinnerName { get; private set; } = string.Empty;

    /// <summary>Index of the last question that was asked (for display purposes).</summary>
    public int QuestionsAsked { get; private set; }

    /// <summary>Log of recent game events for the host to review.</summary>
    public List<string> GameLog { get; private set; } = new();

    //  Timer 

    /// <summary>Stopwatch used to track the countdown timer for each question.</summary>
    private readonly Stopwatch timer = new();

    /// <summary>How many seconds remain on the current question's timer.</summary>
    public int SecondsRemaining { get; private set; }

    //  Constructor 

    public TriviaGame(Plugin plugin)
    {
        this.plugin = plugin;

        // Load all questions from the bank
        allQuestions = QuestionBank.GetAllQuestions();
    }

    //  Game flow methods 

    /// <summary>
    /// Starts a new trivia game. Resets all scores and question flags.
    /// </summary>
    public void StartGame()
    {
        // Reset everything for a fresh game
        Scoreboard.Clear();
        CurrentQuestionParticipants.Clear();
        CorrectAnswerPlayer = string.Empty;
        QuestionAnswered = false;
        WinnerName = string.Empty;
        QuestionsAsked = 0;
        GameLog.Clear();
        CurrentQuestion = null;

        // Clear the "used questions" list so all questions are available
        plugin.Configuration.UsedQuestionIds.Clear();
        plugin.Configuration.Save();

        State = GameState.BetweenQuestions;

        // Announce the game start in chat
        var cmd = plugin.Configuration.UseYell ? "/yell" : "/shout";
        plugin.SendChat($"{cmd} --- Pink Chocobo Club, Trivial Night! ---");
        plugin.SendChat($"{cmd} The trivia game is starting! Answer in /yell or /shout!");
        plugin.SendChat($"{cmd} First to {plugin.Configuration.PointsToWin} points wins! Good luck!");

        AddLog("Game started! First to " + plugin.Configuration.PointsToWin + " points wins.");
    }

    /// <summary>
    /// Sends the next question from the pool.
    /// Skips questions that have already been used.
    /// </summary>
    public void NextQuestion()
    {
        // Find all unused questions
        var unusedQuestions = allQuestions
            .Where(q => !plugin.Configuration.UsedQuestionIds.Contains(q.Id))
            .ToList();

        // If we've run out of questions, notify the host
        if (unusedQuestions.Count == 0)
        {
            AddLog("ERROR: No more questions available! All 50 have been used.");
            plugin.SendChat(plugin.Configuration.UseYell
                ? "/yell We've run out of questions! The host needs to reset the question pool."
                : "/shout We've run out of questions! The host needs to reset the question pool.");
            return;
        }

        // Pick a random unused question
        var random = new Random();
        var index = random.Next(unusedQuestions.Count);
        CurrentQuestion = unusedQuestions[index];

        // Mark it as used so it won't appear again
        plugin.Configuration.UsedQuestionIds.Add(CurrentQuestion.Id);
        plugin.Configuration.Save();

        // Reset per-question tracking
        CurrentQuestionParticipants.Clear();
        CorrectAnswerPlayer = string.Empty;
        QuestionAnswered = false;
        QuestionsAsked++;

        // Start the countdown timer
        SecondsRemaining = plugin.Configuration.TimerSeconds;
        timer.Restart();

        // Set state to active
        State = GameState.QuestionActive;

        // Announce the question in chat
        var cmd = plugin.Configuration.UseYell ? "/yell" : "/shout";
        plugin.SendChat($"{cmd} -- Question #{QuestionsAsked} [{CurrentQuestion.Category}] --");
        plugin.SendChat($"{cmd} {CurrentQuestion.Question}");

        AddLog($"Q#{QuestionsAsked}: {CurrentQuestion.Question}");
    }

    /// <summary>
    /// Called every frame by the plugin's Framework.Update hook.
    /// Handles the countdown timer logic.
    /// </summary>
    public void Update()
    {
        // Only tick the timer when a question is active
        if (State != GameState.QuestionActive)
            return;

        // Calculate remaining seconds
        var elapsed = (int)timer.Elapsed.TotalSeconds;
        SecondsRemaining = Math.Max(0, plugin.Configuration.TimerSeconds - elapsed);

        // Check if time ran out
        if (SecondsRemaining <= 0)
        {
            timer.Stop();
            State = GameState.BetweenQuestions;

            if (!QuestionAnswered)
            {
                // Nobody answered correctly in time
                var cmd = plugin.Configuration.UseYell ? "/yell" : "/shout";
                var answerText = CurrentQuestion != null && CurrentQuestion.Answers.Count > 0
                    ? CurrentQuestion.Answers[0]
                    : "???";
                plugin.SendChat($"{cmd} Time's up! The answer was: {answerText}");
                AddLog("Time's up! Nobody answered correctly.");
            }
        }
    }

    /// <summary>
    /// Processes an answer attempt from a player.
    /// Called from the chat message handler in Plugin.cs.
    /// </summary>
    /// <param name="playerName">The name of the player who answered.</param>
    /// <param name="answer">The text of their answer.</param>
    public void ProcessAnswer(string playerName, string answer)
    {
        // Only accept answers when a question is active
        if (State != GameState.QuestionActive || CurrentQuestion == null)
            return;

        // Add the player to the participants list (tracks everyone who tried)
        if (!CurrentQuestionParticipants.Contains(playerName))
            CurrentQuestionParticipants.Add(playerName);

        // Make sure the player is in the scoreboard (with 0 points if new)
        if (!Scoreboard.ContainsKey(playerName))
            Scoreboard[playerName] = 0;

        // If someone already answered correctly, ignore further attempts
        if (QuestionAnswered)
            return;

        // Check if the answer is correct (case-insensitive comparison)
        var isCorrect = CurrentQuestion.Answers
            .Any(a => string.Equals(a.Trim(), answer.Trim(), StringComparison.OrdinalIgnoreCase));

        if (!isCorrect)
            return;

        //  Correct answer! 

        QuestionAnswered = true;
        CorrectAnswerPlayer = playerName;

        // Award 1 point to the first correct answerer
        Scoreboard[playerName]++;
        timer.Stop();

        // Announce the correct answer in chat
        var cmd = plugin.Configuration.UseYell ? "/yell" : "/shout";
        plugin.SendChat($"{cmd} Correct! {playerName} got it! The answer was: {CurrentQuestion.Answers[0]}");
        plugin.SendChat($"{cmd} {playerName} now has {Scoreboard[playerName]} point(s)!");

        AddLog($"{playerName} answered correctly! (+1 point, total: {Scoreboard[playerName]})");

        // Check if this player has won the game
        if (Scoreboard[playerName] >= plugin.Configuration.PointsToWin)
        {
            WinnerName = playerName;
            State = GameState.GameOver;

            plugin.SendChat($"{cmd} -----");
            plugin.SendChat($"{cmd} WINNER! {playerName} wins Trivial Night with {Scoreboard[playerName]} points!");
            plugin.SendChat($"{cmd} Congratulations from Pink Chocobo Club!");
            plugin.SendChat($"{cmd} -----");

            AddLog($"GAME OVER! {playerName} wins with {Scoreboard[playerName]} points!");
        }
        else
        {
            State = GameState.BetweenQuestions;
        }
    }

    //  Manual point adjustment 

    /// <summary>
    /// Manually adds points to a player's score.
    /// Used by the host to correct scoring mistakes.
    /// </summary>
    /// <param name="playerName">The player to award points to.</param>
    /// <param name="points">Number of points to add (can be negative to subtract).</param>
    public void AdjustPoints(string playerName, int points)
    {
        if (!Scoreboard.ContainsKey(playerName))
            Scoreboard[playerName] = 0;

        Scoreboard[playerName] += points;

        // Don't allow negative scores
        if (Scoreboard[playerName] < 0)
            Scoreboard[playerName] = 0;

        AddLog($"Manual adjustment: {playerName} {(points >= 0 ? "+" : "")}{points} (total: {Scoreboard[playerName]})");
    }

    /// <summary>
    /// Adds a new player to the scoreboard manually with 0 points.
    /// </summary>
    public void AddPlayer(string playerName)
    {
        if (!Scoreboard.ContainsKey(playerName))
        {
            Scoreboard[playerName] = 0;
            AddLog($"Player added manually: {playerName}");
        }
    }

    /// <summary>
    /// Removes a player from the scoreboard entirely.
    /// </summary>
    public void RemovePlayer(string playerName)
    {
        if (Scoreboard.ContainsKey(playerName))
        {
            Scoreboard.Remove(playerName);
            AddLog($"Player removed: {playerName}");
        }
    }

    /// <summary>
    /// Resets the used-questions list, making all questions available again
    /// without resetting the game itself.
    /// </summary>
    public void ResetQuestionPool()
    {
        plugin.Configuration.UsedQuestionIds.Clear();
        plugin.Configuration.Save();
        AddLog("Question pool reset. All questions are available again.");
    }

    /// <summary>
    /// Stops the current game entirely and returns to idle state.
    /// </summary>
    public void StopGame()
    {
        timer.Stop();
        State = GameState.Idle;
        CurrentQuestion = null;

        var cmd = plugin.Configuration.UseYell ? "/yell" : "/shout";
        plugin.SendChat($"{cmd} Trivial Night has ended. Thanks for playing!");

        AddLog("Game stopped by host.");
    }

    /// <summary>
    /// Skips the current question without awarding any points.
    /// </summary>
    public void SkipQuestion()
    {
        if (State != GameState.QuestionActive || CurrentQuestion == null)
            return;

        timer.Stop();
        State = GameState.BetweenQuestions;

        var cmd = plugin.Configuration.UseYell ? "/yell" : "/shout";
        plugin.SendChat($"{cmd} Question skipped! The answer was: {CurrentQuestion.Answers[0]}");

        AddLog($"Question skipped. Answer was: {CurrentQuestion.Answers[0]}");
    }

    //  Helper methods 

    /// <summary>
    /// Returns the number of unused questions remaining in the pool.
    /// </summary>
    public int RemainingQuestions()
    {
        return allQuestions.Count - plugin.Configuration.UsedQuestionIds.Count;
    }

    /// <summary>
    /// Returns the total number of questions in the bank.
    /// </summary>
    public int TotalQuestions()
    {
        return allQuestions.Count;
    }

    /// <summary>
    /// Adds a timestamped entry to the game log.
    /// </summary>
    private void AddLog(string message)
    {
        var timestamp = DateTime.Now.ToString("HH:mm:ss");
        GameLog.Add($"[{timestamp}] {message}");

        // Keep the log from growing too large (last 100 entries)
        if (GameLog.Count > 100)
            GameLog.RemoveAt(0);
    }
}
