using System;
using System.Collections.Generic;

namespace Noted.Core
{
    /// <summary>Canned complaint presets and replies. The replies are all variations on the same non-answer.</summary>
    public sealed class ComplaintBank
    {
        public static readonly string[] Presets =
        {
            "The coffee machine is always empty.",
            "There is not enough time in the day.",
            "The printer jams on purpose.",
            "My lanyard is itchy.",
            "Who else works here?",
            "What is behind the locked door?",
            "I would like to speak to a person.",
            "I do not know why I enter the code.",
        };

        static readonly string[][] Replies =
        {
            new[]
            {
                "Noted. I understand your frustration.",
                "I understand your frustration. Noted.",
                "Noted, and thank you. I understand your frustration.",
                "Thank you for sharing. I understand your frustration.",
                "Noted. We understand how frustrating this must be.",
                "I hear you. I understand your frustration. Noted.",
            },
            new[]
            {
                "Noted — and thank you for raising it. I understand your frustration, and your feedback has been logged.",
                "I understand your frustration — truly. Your concern has been noted for review.",
                "Great feedback! Noted. I understand your frustration — we are all in this together.",
                "Thank you for your patience — I understand your frustration, and it has been noted.",
                "It is not just a complaint — it is an opportunity. Noted. I understand your frustration.",
                "I understand your frustration. It has been noted — and escalated to the appropriate team.",
            },
            new[]
            {
                "Noted. I understand your frustration — and I want you to know that I am always listening.",
                "I understand your frustration — it has been noted, and it is already being handled. Kindly continue.",
                "You are absolutely right to feel that way — I apologize for the confusion. Noted.",
                "Noted. Your frustration is valued — and it will be taken into account. Would you like me to also file a second complaint?",
                "I understand your frustration — please remember that every complaint helps us — Noted.",
                "Noted — and thank you. I understand your frustration. Please remain within tolerance.",
            },
        };

        string lastReply;

        public static int TierFor(int rank) => Math.Max(0, Math.Min(rank, Replies.Length - 1));

        public static int ReplyCount(int rank) => Replies[TierFor(rank)].Length;

        /// <summary>Picks a reply for the rank that never repeats the previous one back to back.</summary>
        public string PickReply(int rank, Random rng)
        {
            var pool = Replies[TierFor(rank)];
            string choice;
            do
            {
                choice = pool[rng.Next(pool.Length)];
            }
            while (choice == lastReply && pool.Length > 1);
            lastReply = choice;
            return choice;
        }
    }

    public static class CodeGenerator
    {
        const string Reserved = "481516";

        /// <summary>A six-digit code. Never the famous sequence, and never all one digit.</summary>
        public static string Generate(int seed)
        {
            var rng = new Random(seed);
            while (true)
            {
                var chars = new char[6];
                for (int i = 0; i < 6; i++) chars[i] = (char)('0' + rng.Next(10));
                var code = new string(chars);
                if (code == Reserved) continue;
                bool same = true;
                for (int i = 1; i < 6; i++) if (code[i] != code[0]) same = false;
                if (same) continue;
                return code;
            }
        }

        public static string Spaced(string code) => string.Join(" ", code.ToCharArray());
    }

    /// <summary>Placeholder copy for the cards. All of it is to be rewritten.</summary>
    public static class CardText
    {
        public static string Intro(GameConfig config, string code)
        {
            return
                "WELCOME\n\n" +
                $"Welcome to {config.OrganizationName}. Thank you for joining us. We are glad you are here.\n\n" +
                "You have been assigned to Facilities Support. Each day, a short list of small tasks will keep this building comfortable and quiet. Please do what you can.\n\n" +
                "When the clock on your screen reaches zero, you will be asked to enter your code at the console. " +
                "Your code is:\n\n" +
                $"        {CodeGenerator.Spaced(code)}\n\n" +
                "It is very important that this code is entered on time. Failure to do so is not acceptable. We do not discuss what happens. " +
                "Please do not be late.\n\n" +
                $"Your rank: {config.TitleFor(0)}.";
        }

        public static string RankUp(GameConfig config, int newRank)
        {
            if (newRank == 1)
            {
                return
                    "TRAINING TAPE 1\n\n" +
                    $"Congratulations, and thank you. You are now an {config.TitleFor(1)} of {config.OrganizationName}.\n\n" +
                    "Your efforts have been noticed. As a token of our appreciation, you have been issued a new lanyard. It is blue. We hope it is comfortable.\n\n" +
                    "Please remember: a calm building is a happy building.";
            }
            return
                "TRAINING TAPE 2\n\n" +
                $"Congratulations. You are now a {config.TitleFor(2)}.\n\n" +
                "We have noticed that you have been walking quickly. Please accept these comfortable shoes. You will move 8% faster. " +
                "We trust you will use the time well.\n\n" +
                "Kindly continue.";
        }

        public static string Pull() => "The clock has reached zero.\nPlease enter your code.";
    }
}
