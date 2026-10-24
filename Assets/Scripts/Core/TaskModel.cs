using System;
using System.Collections.Generic;

namespace Noted.Core
{
    /// <summary>One thing to do at one station: hold for a time, optionally needing or giving a carried item.</summary>
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

        /// <summary>The five starter tasks. Hold times are deliberately long so a 60 s day cannot hold a whole list at low rank.</summary>
        public static TaskCatalog Default()
        {
            return new TaskCatalog(new[]
            {
                new TaskDefinition("coffee", "Refill the coffee machine", 0,
                    "Hold E at the coffee machine in the break room until it is full. Everyone is counting on you, kindly.",
                    new TaskStep("coffee_machine", 9f, "Refill the coffee machine")),
                new TaskDefinition("printer", "Clear the printer jam", 0,
                    "Hold E at the printer in the print room until the jam is cleared. Please do not ask what it was printing.",
                    new TaskStep("printer", 10f, "Clear the printer jam")),
                new TaskDefinition("shredder", "Empty the shredder bin", 0,
                    "Hold E at the shredder bin in the print room until it is empty. The bin fills faster than expected.",
                    new TaskStep("shredder", 8f, "Empty the shredder bin")),
                new TaskDefinition("filter", "Swap the air filter", 1,
                    "Two steps. Hold E at the supply shelf to take a clean filter. Then hold E at the vent at the far end of the corridor to install it.",
                    new TaskStep("filter_shelf", 3f, "Take a clean filter", null, "filter"),
                    new TaskStep("vent", 9f, "Install the air filter", "filter", null)),
                new TaskDefinition("temperature", "Log the temperature", 1,
                    "Two steps. Hold E at the thermostat by the server hall window to read it. Then hold E at the clipboard in the office to write the number down.",
                    new TaskStep("thermostat", 4f, "Read the thermostat", null, "reading"),
                    new TaskStep("clipboard", 6f, "Write the number on the clipboard", "reading", null)),
            });
        }
    }
}
