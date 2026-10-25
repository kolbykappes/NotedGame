using System;
using System.Collections.Generic;
using System.Linq;

namespace Noted.Core
{
    /// <summary>One thing to do at one station for an errand on the second list. Errand steps are side actions: they never touch the official jobs.</summary>
    public sealed class ErrandStep
    {
        public readonly string StationId;
        public readonly float HoldSeconds;
        public readonly string Prompt;
        public readonly string RequiresCarry;
        public readonly string GivesCarry;
        public readonly StepKind Kind;

        /// <summary>If above zero, this step must come within this many seconds of the previous step or the errand starts again from the first step.</summary>
        public readonly float MaxSecondsAfterPrevious;

        public ErrandStep(string stationId, float holdSeconds, string prompt, string requiresCarry = null, string givesCarry = null,
            StepKind kind = StepKind.Hold, float maxSecondsAfterPrevious = 0f)
        {
            StationId = stationId;
            HoldSeconds = holdSeconds;
            Prompt = prompt;
            RequiresCarry = requiresCarry;
            GivesCarry = givesCarry;
            Kind = kind;
            MaxSecondsAfterPrevious = maxSecondsAfterPrevious;
        }
    }

    public sealed class ErrandDefinition
    {
        public readonly string Id;
        public readonly string Name;
        public readonly int MinRank;
        public readonly string Briefing;
        public readonly IReadOnlyList<ErrandStep> Steps;

        /// <summary>How far completing the errand pushes the rollout back (negative).</summary>
        public readonly float RolloutEffect;

        public ErrandDefinition(string id, string name, int minRank, string briefing, float rolloutEffect, params ErrandStep[] steps)
        {
            Id = id;
            Name = name;
            MinRank = minRank;
            Briefing = briefing;
            RolloutEffect = rolloutEffect;
            Steps = steps;
        }

        /// <summary>Households shown as restored on the next morning's slip for a completed errand.</summary>
        public int Households => (int)Math.Round(Math.Abs(RolloutEffect) * 31f);
    }

    public enum ErrandStatus
    {
        Offered,
        InProgress,
        Done,
    }

    public sealed class ErrandInstance
    {
        public readonly ErrandDefinition Definition;
        public ErrandStatus Status;
        public int StepIndex;
        public float LastStepAt;

        public ErrandInstance(ErrandDefinition definition)
        {
            Definition = definition;
            Status = ErrandStatus.Offered;
        }

        public bool IsDone => Status == ErrandStatus.Done;

        public ErrandStep CurrentStep => IsDone ? null : Definition.Steps[StepIndex];

        /// <summary>Moves to the next step; returns true when that finished the errand.</summary>
        public bool Advance(float now)
        {
            StepIndex++;
            LastStepAt = now;
            if (StepIndex >= Definition.Steps.Count)
            {
                Status = ErrandStatus.Done;
                return true;
            }
            Status = ErrandStatus.InProgress;
            return false;
        }

        public void Restart()
        {
            StepIndex = 0;
            Status = ErrandStatus.Offered;
        }
    }

    public static class ErrandCatalog
    {
        public static readonly IReadOnlyList<ErrandDefinition> All = new[]
        {
            new ErrandDefinition("blank_notices", "Blank the notices", 2,
                "Take the blank stock from the bottom of the paper stack.\nLoad it in the printer instead of paper.\nThen do your job as usual.",
                -1.0f,
                new ErrandStep("paper_stack", 1f, "Take the blank stock", null, "blank"),
                new ErrandStep("printer", 2.5f, "Load the blank stock", "blank")),

            new ErrandDefinition("jam_breakers", "Jam the breakers", 2,
                "Jam the corridor breaker, then the admin breaker.\nWithin twenty seconds.\nGrid 14 stays lit.",
                -0.6f,
                new ErrandStep("breaker_corridor", 0f, "Jam the corridor breaker", null, null, StepKind.Flip),
                new ErrandStep("breaker_admin", 0f, "Jam the admin breaker", null, null, StepKind.Flip, 20f)),

            new ErrandDefinition("slip_appeal", "Slip the appeal out", 2,
                "There is always one that did not go through.\nTake it from the shredder.\nPut it in the outbox with the rest.",
                -0.7f,
                new ErrandStep("shredder", 1.5f, "Take an unshredded appeal", null, "appeal"),
                new ErrandStep("mail_table", 2f, "Put it in the outbox", "appeal")),

            new ErrandDefinition("prop_vent", "Prop the vent", 3,
                "Prop the damper open at the east end.\nLeave it.\nThe air from this building goes somewhere. Let it.",
                -0.8f,
                new ErrandStep("vent", 3f, "Prop the damper open")),

            new ErrandDefinition("copy_log", "Copy the log", 4,
                "Copy the last page of the log book.\nLeave it behind the patch panel.\nSomeone will collect it.",
                -1.2f,
                new ErrandStep("clipboard", 2f, "Copy the last page", null, "copy"),
                new ErrandStep("patch_panel", 3f, "Tuck the copy behind the panel", "copy")),
        };

        public static ErrandDefinition Find(string id) => All.FirstOrDefault(e => e.Id == id);
    }

    public static class ErrandPlanner
    {
        /// <summary>How many errands are offered at a rank: none before the reveal reaches Specialist, one there, two after.</summary>
        public static int CountFor(int rank) => rank < 2 ? 0 : rank == 2 ? 1 : 2;

        /// <summary>
        /// Chooses today's errands. Deterministic for a seed. Only errands whose rank is reached and whose stations are all in open
        /// rooms are eligible; errands the player has not met before come first.
        /// </summary>
        public static List<ErrandInstance> Plan(int rank, int seed, ISet<string> seenIds, Func<string, bool> stationOpen)
        {
            var rng = new Random(seed);
            var eligible = ErrandCatalog.All
                .Where(e => e.MinRank <= rank && e.Steps.All(s => stationOpen(s.StationId)))
                .ToList();

            var unseen = eligible.Where(e => !seenIds.Contains(e.Id)).OrderBy(_ => rng.Next()).ToList();
            var seen = eligible.Where(e => seenIds.Contains(e.Id)).OrderBy(_ => rng.Next()).ToList();

            int want = CountFor(rank);
            return unseen.Concat(seen).Take(want).Select(e => new ErrandInstance(e)).ToList();
        }
    }
}
