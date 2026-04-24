# 🐤 Pink Chocobo Club, Trivial Night!

A **Dalamud plugin** for Final Fantasy XIV that lets you host trivia game nights with your Free Company, friends, or community!

---

## ✨ Features

- **50 FFXIV-themed trivia questions** covering ARR, Heavensward, Stormblood, Shadowbringers, Endwalker, and general knowledge
- **Automatic chat integration** — Questions are sent via `/yell` or `/shout`, and player answers are detected automatically
- **Case-insensitive answers** — Players can type in any capitalization
- **60-second timer** per question (configurable)
- **Live scoreboard** with gold/silver/bronze rankings
- **Manual point adjustment** — Add or remove points if the system makes a mistake
- **Question tracking** — Used questions are flagged and won't repeat
- **Configurable win condition** — Set how many points are needed to win
- **Game log** — Timestamped record of all game events

## 🎮 How to Play

1. The **host** opens the plugin with `/pcctn`
2. Configure the **points to win** and **timer duration** in the Settings tab
3. Click **Start Game** — an announcement is sent in chat
4. Click **Send Next Question** — a trivia question is yelled in chat
5. **Players answer** via `/yell` or `/shout` within 60 seconds
6. The **first correct answer** earns 1 point
7. Continue until someone reaches the target score — **they win!**

## 📦 Installation

### Via Custom Repository (Recommended)

1. Open FFXIV and type `/xlsettings` to open Dalamud Settings
2. Go to the **Experimental** tab
3. Under **Custom Plugin Repositories**, paste:
   ```
   https://raw.githubusercontent.com/SomethingAstraSomething/PinkChocoboTrivial/main/repo.json
   ```
4. Click the **+** button, then **Save and Close**
5. Open the plugin installer with `/xlplugins`
6. Search for **"Pink Chocobo Club"** and install it

### Manual Build

1. Clone this repository
2. Open `PinkChocoboTrivial/PinkChocoboTrivial.csproj` in Visual Studio or Rider
3. Build the project (requires [Dalamud SDK](https://github.com/goatcorp/Dalamud))
4. Copy the output DLL and JSON to your Dalamud `devPlugins` folder

## 🛠️ Commands

| Command   | Description                          |
|-----------|--------------------------------------|
| `/pcctn`  | Toggle the main control panel        |

## 📁 Project Structure

```
PinkChocoboTrivial/
├── PinkChocoboTrivial.csproj    # Project file (Dalamud SDK 14)
├── PinkChocoboTrivial.json      # Plugin manifest
├── Plugin.cs                    # Main entry point & chat handling
├── Configuration.cs             # Persistent settings
├── TriviaGame.cs                # Core game engine
├── QuestionBank.cs              # 50 FFXIV trivia questions
└── Windows/
    ├── MainWindow.cs            # Host control panel (ImGui)
    └── ScoreboardWindow.cs      # Compact score overlay
```

## 🔧 Configuration

All settings are accessible from the **Settings** tab in the main window:

- **Points to Win** (1-50, default: 5)
- **Timer Duration** (10-300 seconds, default: 60)
- **Chat Channel** (toggle between `/yell` and `/shout` for questions)

## 📝 Adding Custom Questions

Edit `QuestionBank.cs` and add new entries following this pattern:

```csharp
new TriviaQuestion
{
    Id = 51, // Use the next available ID
    Category = "Your Category",
    Question = "Your question here?",
    Answers = new List<string> { "Answer1", "Alternate Answer" }
},
```

## 🤝 Credits

Made with 💖 by **Pink Chocobo Club**

## 📄 License

This project is licensed under the MIT License.
