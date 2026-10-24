using System;
using System.Collections.Generic;

namespace Noted.Core
{
    /// <summary>What the player does at a station to finish a step.</summary>
    public enum StepKind
    {
        /// <summary>Hold E for a short time (picking something up, using it, reading a gauge).</summary>
        Hold,
        /// <summary>One press of E: throw a switch or breaker.</summary>
        Flip,
        /// <summary>One press of E opens a short mini-game; winning it finishes the step.</summary>
        MiniGame,
    }

    public enum MiniGameKind
    {
        None,
        Sort,
        CableMatch,
        Pattern,
    }

    /// <summary>One thing to do at one station.</summary>
    public sealed class TaskStep
    {
        public readonly string StationId;
        public readonly float HoldSeconds;
        public readonly string Prompt;
        public readonly string RequiresCarry;
        public readonly string GivesCarry;
        public readonly StepKind Kind;
        public readonly MiniGameKind Game;

        /// <summary>The step cannot be done until this many seconds after the previous step of the same job (switch off, wait, switch on).</summary>
        public readonly float MinSecondsAfterPrevious;

        /// <summary>Finishing the step shows a readout (for example a temperature).</summary>
        public readonly bool IsReadout;

        public TaskStep(string stationId, float holdSeconds, string prompt, string requiresCarry = null, string givesCarry = null,
            StepKind kind = StepKind.Hold, MiniGameKind game = MiniGameKind.None, float minSecondsAfterPrevious = 0f, bool isReadout = false)
        {
            StationId = stationId;
            HoldSeconds = holdSeconds;
            Prompt = prompt;
            RequiresCarry = requiresCarry;
            GivesCarry = givesCarry;
            Kind = kind;
            Game = game;
            MinSecondsAfterPrevious = minSecondsAfterPrevious;
            IsReadout = isReadout;
        }

        public static TaskStep Flip(string stationId, string prompt, float minSecondsAfterPrevious = 0f)
            => new TaskStep(stationId, 0f, prompt, null, null, StepKind.Flip, MiniGameKind.None, minSecondsAfterPrevious);

        public static TaskStep Play(string stationId, MiniGameKind game, string prompt, string requiresCarry = null)
            => new TaskStep(stationId, 0f, prompt, requiresCarry, null, StepKind.MiniGame, game);
    }

    public sealed class TaskDefinition
    {
        public readonly string Id;
        public readonly string Name;
        public readonly int MinRank;
        public readonly string HowTo;
        public readonly IReadOnlyList<TaskStep> Steps;

        /// <summary>Uptime paid on completion; 0 means the config default.</summary>
        public int Coins { get; private set; }

        /// <summary>A tool the player must own before this job can be on their list.</summary>
        public string RequiresTool { get; private set; }

        public TaskDefinition(string id, string name, int minRank, string howTo, params TaskStep[] steps)
        {
            Id = id;
            Name = name;
            MinRank = minRank;
            HowTo = howTo;
            Steps = steps;
        }

        public TaskDefinition Paying(int coins) { Coins = coins; return this; }
        public TaskDefinition NeedingTool(string toolId) { RequiresTool = toolId; return this; }
    }

    public enum TaskStatus
    {
        Pending,
        Done,
        Automated,
    }

    public sealed class TaskInstance
    {
        public readonly TaskDefinition Definition;
        public int StepIndex;
        public TaskStatus Status;

        /// <summary>Day-clock seconds at which the previous step was finished (for steps that must wait).</summary>
        public float LastStepAt;

        public TaskInstance(TaskDefinition definition, TaskStatus status = TaskStatus.Pending)
        {
            Definition = definition;
            Status = status;
        }

        public bool IsFinished => Status != TaskStatus.Pending;

        public TaskStep CurrentStep => Status == TaskStatus.Pending ? Definition.Steps[StepIndex] : null;

        /// <summary>Moves to the next step. Returns true when that was the last step and the task is now done.</summary>
        public bool AdvanceStep()
        {
            StepIndex++;
            if (StepIndex >= Definition.Steps.Count)
            {
                Status = TaskStatus.Done;
                return true;
            }
            return false;
        }
    }

    public sealed class TaskCatalog
    {
        public readonly IReadOnlyList<TaskDefinition> All;

        public TaskCatalog(IReadOnlyList<TaskDefinition> all)
        {
            All = all;
        }

        public TaskDefinition Find(string id)
        {
            foreach (var t in All)
            {
                if (t.Id == id) return t;
            }
            return null;
        }

        /// <summary>
        /// The eleven jobs. Trainee jobs are office chores in five different verbs (carry, flip, sort, read, carry again); Associate jobs
        /// start touching the building's machines; Specialist jobs are in the server hall and need tools. Holds are short; the walking is the work.
        /// </summary>
        public static TaskCatalog Default()
        {
            return new TaskCatalog(new[]
            {
                // ---- Trainee: office mundane ----
                new TaskDefinition("coffee", "Restock the coffee", 0,
                    "Two steps. Take a sack of coffee beans from the supply cabinet in the admin room, then carry it to the coffee machine in the break room.",
                    new TaskStep("bean_sack", 1f, "Take the bean sack", null, "beans"),
                    new TaskStep("coffee_machine", 2f, "Refill the coffee machine", "beans", null)).Paying(10),

                new TaskDefinition("printer", "Load the printer paper", 0,
                    "Two steps. Take a ream of paper from the supply stack in the admin room, then carry it to the printer in the print room.",
                    new TaskStep("paper_stack", 1f, "Take a ream of paper", null, "paper"),
                    new TaskStep("printer", 2.5f, "Load the paper and clear the jam", "paper", null)).Paying(10),

                new TaskDefinition("breakers", "Reset the tripped breakers", 0,
                    "Three breaker boxes have tripped: one in the admin room, one in the break room and one in the corridor. Press E at each one to reset it.",
                    TaskStep.Flip("breaker_admin", "Reset the admin room breaker"),
                    TaskStep.Flip("breaker_break", "Reset the break room breaker"),
                    TaskStep.Flip("breaker_corridor", "Reset the corridor breaker")).Paying(10),

                new TaskDefinition("mail", "Sort the mail", 0,
                    "Press E at the mail table to sort today's mail. Press 1, 2 or 3 (or click) to put each item in the right bin. A wrong bin costs two seconds.",
                    TaskStep.Play("mail_table", MiniGameKind.Sort, "Sort the mail")).Paying(10),

                new TaskDefinition("temperature", "Log the temperature", 0,
                    "Two steps. Read the thermostat in the corridor, then carry the number to the clipboard in the office and write it down.",
                    new TaskStep("thermostat", 1.5f, "Read the thermostat", null, "reading", isReadout: true),
                    new TaskStep("clipboard", 2f, "Write the number on the clipboard", "reading", null)).Paying(10),

                new TaskDefinition("shredder", "Empty the shredder", 0,
                    "Two steps. Pull the full bag from the shredder in the print room, then carry it to the dumpster in the utility room.",
                    new TaskStep("shredder", 1.5f, "Pull the full shred bag", null, "bag"),
                    new TaskStep("dumpster", 2f, "Throw the bag in the dumpster", "bag", null)).Paying(10),

                // ---- Associate: the building's machines ----
                new TaskDefinition("patch", "Patch the cable", 1,
                    "Press E at the patch panel in the network closet. Plug each cable into the port that matches its colour. A wrong port costs two seconds.",
                    TaskStep.Play("patch_panel", MiniGameKind.CableMatch, "Patch the cables")).Paying(15),

                new TaskDefinition("reader", "Reboot the badge reader", 1,
                    "Two steps. Switch the badge reader off at its power box in the corridor, do something else for about eight seconds, then switch it back on.",
                    TaskStep.Flip("reader_power", "Switch the badge reader off"),
                    TaskStep.Flip("reader_power", "Switch the badge reader back on", 8f)).Paying(15),

                new TaskDefinition("filter", "Swap the air filter", 1,
                    "Two steps. Take a clean filter from the supply shelf in the break room, then carry it to the vent at the east end of the corridor and install it.",
                    new TaskStep("filter_shelf", 1f, "Take a clean filter", null, "filter"),
                    new TaskStep("vent", 3f, "Install the air filter", "filter", null)).Paying(15),

                // ---- Specialist: the server hall ----
                new TaskDefinition("drive", "Reseat a failed drive", 2,
                    "Two steps. Take a spare drive from the cabinet in the server hall, then press E at the lit rack and match each connector to its colour. You must own an anti-static strap.",
                    new TaskStep("spares_cabinet", 1f, "Take a spare drive", null, "drive"),
                    TaskStep.Play("rack_drive", MiniGameKind.CableMatch, "Seat the drive", "drive")).Paying(25).NeedingTool("anti_static_strap"),

                new TaskDefinition("alarm", "Clear the alarm panel", 2,
                    "Press E at the alarm panel in the server hall and repeat the pattern of lights. Then throw the reset switch beside it.",
                    TaskStep.Play("alarm_panel", MiniGameKind.Pattern, "Repeat the alarm pattern"),
                    TaskStep.Flip("alarm_reset", "Throw the reset switch")).Paying(25),
            });
        }
    }
}
