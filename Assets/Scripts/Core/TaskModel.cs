using System;
using System.Collections.Generic;

namespace Noted.Core
{
    /// <summary>One thing to do at one station: a short hold, optionally taking or needing a carried item.</summary>
    public sealed class TaskStep
    {
        public readonly string StationId;
        public readonly float HoldSeconds;
        public readonly string Prompt;
        public readonly string RequiresCarry;
        public readonly string GivesCarry;

        public TaskStep(string stationId, float holdSeconds, string prompt, string requiresCarry = null, string givesCarry = null)
        {
            StationId = stationId;
            HoldSeconds = holdSeconds;
            Prompt = prompt;
            RequiresCarry = requiresCarry;
            GivesCarry = givesCarry;
        }
    }

    public sealed class TaskDefinition
    {
        public readonly string Id;
        public readonly string Name;
        public readonly int MinRank;
        public readonly string HowTo;
        public readonly IReadOnlyList<TaskStep> Steps;

        public TaskDefinition(string id, string name, int minRank, string howTo, params TaskStep[] steps)
        {
            Id = id;
            Name = name;
            MinRank = minRank;
            HowTo = howTo;
            Steps = steps;
        }
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
        /// The five starter tasks. Each is a short fetch quest: pick something up in one room (a second or two),
        /// carry it across the building, and use it somewhere else (two or three seconds). The walking is the work.
        /// </summary>
        public static TaskCatalog Default()
        {
            return new TaskCatalog(new[]
            {
                new TaskDefinition("coffee", "Restock the coffee", 0,
                    "Two steps. Take a sack of coffee beans from the storeroom at the far end of the corridor (the utility room), then carry it to the coffee machine in the break room.",
                    new TaskStep("bean_sack", 1f, "Take the bean sack", null, "beans"),
                    new TaskStep("coffee_machine", 2f, "Refill the coffee machine", "beans", null)),
                new TaskDefinition("printer", "Load the printer paper", 0,
                    "Two steps. Take a ream of paper from the supply stack in the office, then carry it to the printer in the print room.",
                    new TaskStep("paper_stack", 1f, "Take a ream of paper", null, "paper"),
                    new TaskStep("printer", 2.5f, "Load the paper and clear the jam", "paper", null)),
                new TaskDefinition("shredder", "Empty the shredder", 0,
                    "Two steps. Pull the full bag from the shredder in the print room, then carry it to the dumpster in the utility room at the far end of the corridor.",
                    new TaskStep("shredder", 1.5f, "Pull the full shred bag", null, "bag"),
                    new TaskStep("dumpster", 2f, "Throw the bag in the dumpster", "bag", null)),
                new TaskDefinition("filter", "Swap the air filter", 1,
                    "Two steps. Take a clean filter from the supply shelf in the break room, then carry it to the vent at the east end of the corridor and install it.",
                    new TaskStep("filter_shelf", 1f, "Take a clean filter", null, "filter"),
                    new TaskStep("vent", 3f, "Install the air filter", "filter", null)),
                new TaskDefinition("temperature", "Log the temperature", 1,
                    "Two steps. Read the thermostat by the server hall window, then carry the number to the clipboard in the office and write it down.",
                    new TaskStep("thermostat", 1.5f, "Read the thermostat", null, "reading"),
                    new TaskStep("clipboard", 2f, "Write the number on the clipboard", "reading", null)),
            });
        }
    }
}
