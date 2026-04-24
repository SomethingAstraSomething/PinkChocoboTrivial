// QuestionBank.cs - FFXIV-themed trivia questions
// Contains 50 questions covering ARR, HW, SB, ShB, EW and
// general FFXIV knowledge. Each question supports multiple
// accepted answer forms (case-insensitive).

using System.Collections.Generic;

namespace PinkChocoboTrivial;

/// <summary>
/// Represents a single trivia question with one or more accepted answers.
/// </summary>
public class TriviaQuestion
{
    /// <summary>Unique ID for tracking which questions have been used.</summary>
    public int Id { get; set; }

    /// <summary>The category/expansion the question belongs to.</summary>
    public string Category { get; set; } = string.Empty;

    /// <summary>The question text that will be yelled in chat.</summary>
    public string Question { get; set; } = string.Empty;

    /// <summary>
    /// All accepted answers for this question (case-insensitive).
    /// Multiple entries allow for abbreviations or alternate spellings.
    /// Example: ["Minfilia", "Minfilia Warde"] both count as correct.
    /// </summary>
    public List<string> Answers { get; set; } = new();
}

/// <summary>
/// Static class holding the full bank of FFXIV trivia questions.
/// Questions are flagged as "used" during a game session so they don't repeat.
/// </summary>
public static class QuestionBank
{
    /// <summary>
    /// Returns the complete list of all available trivia questions.
    /// Covers A Realm Reborn, Heavensward, Stormblood, Shadowbringers,
    /// Endwalker, Jobs/Classes, and General FFXIV knowledge.
    /// </summary>
    public static List<TriviaQuestion> GetAllQuestions()
    {
        return new List<TriviaQuestion>
        {

            new TriviaQuestion
            {
                Id = 1,
                Category = "A Realm Reborn",
                Question = "Who is the leader of the Scions of the Seventh Dawn in ARR?",
                Answers = new List<string> { "Minfilia", "Minfilia Warde" }
            },
            new TriviaQuestion
            {
                Id = 2,
                Category = "A Realm Reborn",
                Question = "What primal do the Kobolds summon?",
                Answers = new List<string> { "Titan" }
            },
            new TriviaQuestion
            {
                Id = 3,
                Category = "A Realm Reborn",
                Question = "Which city-state is known as the Jewel of the Desert?",
                Answers = new List<string> { "Ul'dah", "Uldah" }
            },
            new TriviaQuestion
            {
                Id = 4,
                Category = "A Realm Reborn",
                Question = "What is the name of the Garlean weapon used in the Praetorium?",
                Answers = new List<string> { "Ultima Weapon", "The Ultima Weapon" }
            },
            new TriviaQuestion
            {
                Id = 5,
                Category = "A Realm Reborn",
                Question = "Which beast tribe worships the primal Ramuh?",
                Answers = new List<string> { "Sylphs", "The Sylphs", "Sylph" }
            },
            new TriviaQuestion
            {
                Id = 6,
                Category = "A Realm Reborn",
                Question = "What is the name of the Maelstrom's home city-state?",
                Answers = new List<string> { "Limsa Lominsa", "Limsa" }
            },
            new TriviaQuestion
            {
                Id = 7,
                Category = "A Realm Reborn",
                Question = "Who is the leader of the Immortal Flames?",
                Answers = new List<string> { "Raubahn", "Raubahn Aldynn" }
            },
            new TriviaQuestion
            {
                Id = 8,
                Category = "A Realm Reborn",
                Question = "What is the name of the massive crystal structure in Mor Dhona?",
                Answers = new List<string> { "Crystal Tower", "The Crystal Tower", "Syrcus Tower" }
            },
            new TriviaQuestion
            {
                Id = 9,
                Category = "A Realm Reborn",
                Question = "Which Ascian is the primary antagonist at the end of ARR 2.0?",
                Answers = new List<string> { "Lahabrea" }
            },
            new TriviaQuestion
            {
                Id = 10,
                Category = "A Realm Reborn",
                Question = "What primal do the Sahagin summon?",
                Answers = new List<string> { "Leviathan" }
            },

            //  HEAVENSWARD (HW) QUESTIONS¡

            new TriviaQuestion
            {
                Id = 11,
                Category = "Heavensward",
                Question = "What is the name of the dragoon known as the Azure Dragoon?",
                Answers = new List<string> { "Estinien", "Estinien Wyrmblood" }
            },
            new TriviaQuestion
            {
                Id = 12,
                Category = "Heavensward",
                Question = "Which dragon is Nidhogg's brood-brother?",
                Answers = new List<string> { "Hraesvelgr" }
            },
            new TriviaQuestion
            {
                Id = 13,
                Category = "Heavensward",
                Question = "What is the name of the Heavensward alliance raid series?",
                Answers = new List<string> { "Shadow of Mhach", "Void Ark" }
            },
            new TriviaQuestion
            {
                Id = 14,
                Category = "Heavensward",
                Question = "Who is the Archbishop of Ishgard?",
                Answers = new List<string> { "Thordan", "Thordan VII", "Archbishop Thordan" }
            },
            new TriviaQuestion
            {
                Id = 15,
                Category = "Heavensward",
                Question = "What player hub was introduced in Heavensward?",
                Answers = new List<string> { "Idyllshire" }
            },
            new TriviaQuestion
            {
                Id = 16,
                Category = "Heavensward",
                Question = "What is the name of the floating continent above the Sea of Clouds?",
                Answers = new List<string> { "Azys Lla" }
            },
            new TriviaQuestion
            {
                Id = 17,
                Category = "Heavensward",
                Question = "Which Heavensward job uses a star globe as its weapon?",
                Answers = new List<string> { "Astrologian", "AST" }
            },
            new TriviaQuestion
            {
                Id = 18,
                Category = "Heavensward",
                Question = "Who is the young Elezen boy that accompanies you in Heavensward?",
                Answers = new List<string> { "Alphinaud", "Alphinaud Leveilleur" }
            },
            new TriviaQuestion
            {
                Id = 19,
                Category = "Heavensward",
                Question = "What is the name of Ysayle's primal form?",
                Answers = new List<string> { "Shiva", "Saint Shiva" }
            },
            new TriviaQuestion
            {
                Id = 20,
                Category = "Heavensward",
                Question = "What is the name of the Heavensward 8-man raid series?",
                Answers = new List<string> { "Alexander" }
            },
            //  STORMBLOOD (SB) QUESTIONS

            new TriviaQuestion
            {
                Id = 21,
                Category = "Stormblood",
                Question = "Who is the viceroy of Doma during Stormblood?",
                Answers = new List<string> { "Yotsuyu", "Yotsuyu goe Brutus" }
            },
            new TriviaQuestion
            {
                Id = 22,
                Category = "Stormblood",
                Question = "What is the name of the underwater city of the Kojin?",
                Answers = new List<string> { "Tamamizu" }
            },
            new TriviaQuestion
            {
                Id = 23,
                Category = "Stormblood",
                Question = "Which primal does Yotsuyu become?",
                Answers = new List<string> { "Tsukuyomi" }
            },
            new TriviaQuestion
            {
                Id = 24,
                Category = "Stormblood",
                Question = "What is the name of the Stormblood 8-man raid series?",
                Answers = new List<string> { "Omega", "Alphascape", "Deltascape", "Sigmascape" }
            },
            new TriviaQuestion
            {
                Id = 25,
                Category = "Stormblood",
                Question = "Who is the leader of the Ala Mhigan Resistance?",
                Answers = new List<string> { "Lyse", "Lyse Hext" }
            },
            new TriviaQuestion
            {
                Id = 26,
                Category = "Stormblood",
                Question = "What is the name of the Garlean legatus who rules Ala Mhigo?",
                Answers = new List<string> { "Zenos", "Zenos yae Galvus" }
            },
            new TriviaQuestion
            {
                Id = 27,
                Category = "Stormblood",
                Question = "What player hub was introduced in Stormblood?",
                Answers = new List<string> { "Rhalgr's Reach", "Rhalgrs Reach" }
            },

            //  SHADOWBRINGERS (ShB) QUESTIONS

            new TriviaQuestion
            {
                Id = 28,
                Category = "Shadowbringers",
                Question = "What is the real identity of the Crystal Exarch?",
                Answers = new List<string> { "G'raha Tia", "Graha Tia", "G'raha" }
            },
            new TriviaQuestion
            {
                Id = 29,
                Category = "Shadowbringers",
                Question = "What is the name of the player hub in the First?",
                Answers = new List<string> { "The Crystarium", "Crystarium" }
            },
            new TriviaQuestion
            {
                Id = 30,
                Category = "Shadowbringers",
                Question = "What are the main enemies in Shadowbringers called?",
                Answers = new List<string> { "Sin Eaters", "Sin Eater", "Sineaters"}
            },
            new TriviaQuestion
            {
                Id = 31,
                Category = "Shadowbringers",
                Question = "Who is the main antagonist Ascian in Shadowbringers?",
                Answers = new List<string> { "Emet-Selch", "Emet Selch", "Hades" }
            },
            new TriviaQuestion
            {
                Id = 32,
                Category = "Shadowbringers",
                Question = "In shadowbringers, what is the name of the fae folk's home?",
                Answers = new List<string> { "Il Mheg" }
            },
            new TriviaQuestion
            {
                Id = 33,
                Category = "Shadowbringers",
                Question = "What is the Shadowbringers 8-man raid series called?",
                Answers = new List<string> { "Eden", "Eden's Gate", "Eden's Verse", "Eden's Promise" }
            },
            new TriviaQuestion
            {
                Id = 34,
                Category = "Shadowbringers",
                Question = "What was the name of the ancient Ascian city?",
                Answers = new List<string> { "Amaurot" }
            },
            new TriviaQuestion
            {
                Id = 35,
                Category = "Shadowbringers",
                Question = "What title does the Warrior of Light gain on the First?",
                Answers = new List<string> { "Warrior of Darkness" }
            },

            //  ENDWALKER QUESTIONS

            new TriviaQuestion
            {
                Id = 36,
                Category = "Endwalker",
                Question = "What is the name of the final zone in Endwalker?",
                Answers = new List<string> { "Ultima Thule" }
            },
            new TriviaQuestion
            {
                Id = 37,
                Category = "Endwalker",
                Question = "Who is the true final antagonist of Endwalker?",
                Answers = new List<string> { "Meteion", "The Endsinger" }
            },
            new TriviaQuestion
            {
                Id = 38,
                Category = "Endwalker",
                Question = "What is the player hub introduced in Endwalker?",
                Answers = new List<string> { "Old Sharlayan", "Sharlayan" }
            },
            new TriviaQuestion
            {
                Id = 39,
                Category = "Endwalker",
                Question = "What is the moon called that you visit in Endwalker?",
                Answers = new List<string> { "Mare Lamentorum" }
            },
            new TriviaQuestion
            {
                Id = 40,
                Category = "Endwalker",
                Question = "What is the Endwalker 8-man raid series called?",
                Answers = new List<string> { "Pandaemonium", "Pandemonium" }
            },
            new TriviaQuestion
            {
                Id = 41,
                Category = "Endwalker",
                Question = "What job was introduced as a melee DPS in Endwalker?",
                Answers = new List<string> { "Reaper", "RPR" }
            },
            new TriviaQuestion
            {
                Id = 42,
                Category = "Endwalker",
                Question = "What job was introduced as a healer in Endwalker?",
                Answers = new List<string> { "Sage", "SGE" }
            },

            //  GENERAL FFXIV KNOWLEDGE

            new TriviaQuestion
            {
                Id = 43,
                Category = "General",
                Question = "How many Grand Companies are there in Eorzea?",
                Answers = new List<string> { "3", "Three" }
            },
            new TriviaQuestion
            {
                Id = 44,
                Category = "General",
                Question = "What is the name of the tank role action that reduces damage by 20%?",
                Answers = new List<string> { "Rampart" }
            },
            new TriviaQuestion
            {
                Id = 45,
                Category = "General",
                Question = "What is the maximum level cap in Endwalker?",
                Answers = new List<string> { "90" }
            },
            new TriviaQuestion
            {
                Id = 46,
                Category = "General",
                Question = "What cute creatures serve as mail carriers in FFXIV?",
                Answers = new List<string> { "Moogles", "Moogle" }
            },
            new TriviaQuestion
            {
                Id = 47,
                Category = "General",
                Question = "What is the name of the Gold Saucer's card game?",
                Answers = new List<string> { "Triple Triad" }
            },
            new TriviaQuestion
            {
                Id = 48,
                Category = "General",
                Question = "What race has very long ears and two sub-clans: Rava and Veena?",
                Answers = new List<string> { "Viera" }
            },
            new TriviaQuestion
            {
                Id = 49,
                Category = "General",
                Question = "What is the name of the FFXIV chocobo racing venue?",
                Answers = new List<string> { "Gold Saucer", "The Gold Saucer", "Manderville Gold Saucer" }
            },
            new TriviaQuestion
            {
                Id = 50,
                Category = "General",
                Question = "What crafting class creates weapons and tools from metal?",
                Answers = new List<string> { "Blacksmith", "BSM" }
            },
        };
    }
}
