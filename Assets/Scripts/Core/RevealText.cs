using System;
using System.Collections.Generic;

namespace Noted.Core
{
    /// <summary>
    /// How a job reads at each stage of the reveal. Stage 0 and 1 use the original wording; stage 2 names what the job really does;
    /// stage 3 puts numbers on the harm. The voice is flat and procedural: the wrong thing is stated in the same tone as the right thing.
    /// All copy is placeholder.
    /// </summary>
    public sealed class JobTextEntry
    {
        public readonly string Name;
        public readonly string DoneLine;
        public readonly string Procedure;

        public JobTextEntry(string name, string doneLine, string procedure)
        {
            Name = name;
            DoneLine = doneLine;
            Procedure = procedure;
        }
    }

    public static class JobText
    {
        // Index 0..3 is the stage. A null entry inherits from the stage below it.
        static readonly Dictionary<string, JobTextEntry[]> Table = new Dictionary<string, JobTextEntry[]>
        {
            { "coffee", Stages(
                null, null,
                E("Restock the coffee (Intake 2)", "Intake 2 restocked. Coffee not affected.", "Coffee is now referred to as Intake. This does not change the task."),
                E("Load Intake 2: complaints, Zone 4", "Intake 2 loaded. 212 complaints queued.", "Intake 2 holds complaints received from Zone 4. Please do not open the sacks.")) },
            { "printer", Stages(
                null, null,
                E("Load the notice stock", "Notice stock loaded. Printing resumes.", "The printer prints notices. Notices are sent to residents."),
                E("Load the notice stock: denials", "Notice stock loaded. 214 denials scheduled for 09:00.", "Denial notices are printed here. Please keep the tray clear.")) },
            { "breakers", Stages(
                null, null,
                E("Reset the breakers: Grid 14", "Grid 14 reset. Priority C, residential.", "Breaker resets are logged against a grid zone. This does not change the task."),
                E("Shed the load: Grid 14, residential, priority C", "Load shed. 31 households without supply.", "Load shedding is applied in order of priority. Priority C is shed first.")) },
            { "mail", Stages(
                null, null,
                E("Triage the appeals", "Appeals triaged. Responses will be issued.", "The mail is appeals. FILE defers, SHRED denies, OUTBOX approves."),
                E("Triage the appeals: Zone 4", "Triage complete. 31 approved, 214 denied.", "Appeals not approved are destroyed. The shredder is for this.")) },
            { "temperature", Stages(
                null, null,
                E("Log the cooling load", "Cooling load logged. Within tolerance.", "The temperature is used to schedule the rollout. Please record it accurately."),
                E("Log the cooling load: rollout schedule", "Logged. Schedule unchanged.", "A warmer room runs faster. Please record it accurately.")) },
            { "shredder", Stages(
                null, null,
                E("Clear the shredder", "Shredder cleared.", "The shredder holds records awaiting disposal."),
                E("Dispose of the appeals", "Disposed: 214 appeals.", "Disposal removes the record. Residents are not notified.")) },
            { "patch", Stages(
                null, null,
                E("Patch the outbound link", "Link patched.", "The panel connects this building to outside services."),
                E("Patch the link: Municipal Water", "Link patched. Municipal Water now follows the schedule.", "Municipal Water is controlled from this panel.")) },
            { "reader", Stages(
                null, null,
                E("Cycle the badge reader", "Reader cycled. Log gap: 8 s.", "The reader log has gaps when cycled. Gaps are normal."),
                E("Cycle the badge reader", "Reader cycled. Approvals continue under your badge.", "While the reader is off, approvals are signed under the last badge. This is permitted.")) },
            { "filter", Stages(
                null, null,
                E("Replace the east vent filter", "Filter replaced. The old one has been retained.", "Old filters are retained for analysis."),
                E("Replace the filter: residue sample", "Filter replaced. Sample logged.", "Residue is analysed. Please do not read the label.")) },
            { "drive", Stages(
                null, null,
                E("Replace a stored account", "Account replaced.", "Drives hold accounts. Failed drives are replaced, not restored."),
                E("Replace the account: A. Lindqvist, Zone 4", "Account replaced. Previous contents deleted.", "Replacement drives are blank. The previous account is not recoverable.")) },
            { "alarm", Stages(
                null, null,
                E("Acknowledge the alarm", "Alarm acknowledged.", "Alarms are raised from outside the building. Acknowledging one silences the source."),
                E("Silence the alarm: Zone 4, water", "Silenced. Source notified: no action.", "Silenced alarms are not forwarded.")) },
        };

        static JobTextEntry[] Stages(JobTextEntry s0, JobTextEntry s1, JobTextEntry s2, JobTextEntry s3) => new[] { s0, s1, s2, s3 };

        static JobTextEntry E(string name, string done, string procedure) => new JobTextEntry(name, done, procedure);

        /// <summary>The wording for a job at a stage, or null if the job reads as it always did.</summary>
        public static JobTextEntry For(string jobId, int stage)
        {
            if (!Table.TryGetValue(jobId, out var entries)) return null;
            for (int s = Math.Min(stage, 3); s >= 0; s--)
            {
                if (entries[s] != null) return entries[s];
            }
            return null;
        }

        public static string NameFor(TaskDefinition def, int stage) => For(def.Id, stage)?.Name ?? def.Name;

        public static string DoneLineFor(TaskDefinition def, int stage) => For(def.Id, stage)?.DoneLine ?? "Done: " + def.Name;

        /// <summary>The how-to card text: the original, plus what the job actually does once the reveal has reached it.</summary>
        public static string HowToFor(TaskDefinition def, int stage)
        {
            var entry = For(def.Id, stage);
            return entry == null ? def.HowTo : def.HowTo + "\n\n" + entry.Procedure;
        }
    }

    /// <summary>A handwritten note from the resistance. Found once, in order, when its trigger is met.</summary>
    public sealed class Scrap
    {
        public readonly string Id;
        public readonly int MinStage;
        public readonly string JobId;
        public readonly int JobCount;
        public readonly string Text;

        public Scrap(string id, int minStage, string jobId, int jobCount, string text)
        {
            Id = id;
            MinStage = minStage;
            JobId = jobId;
            JobCount = jobCount;
            Text = text;
        }
    }

    public static class ScrapBank
    {
        public static readonly Scrap[] All =
        {
            new Scrap("mail_not_junk", 1, "mail", 3, "The mail is not junk.\nRead one before you shred it."),
            new Scrap("not_first", 1, null, 0, "You are not the first to sit at that desk.\nDo not look for the others."),
            new Scrap("printer_people", 2, "printer", 2, "What the printer prints goes to people.\nThey get it tomorrow."),
            new Scrap("grid_street", 2, "breakers", 2, "Grid 14 is a street.\nThere are no windows here. Ask yourself why."),
            new Scrap("alarm_not_ours", 2, "alarm", 1, "That alarm is not ours.\nSomebody out there set it off. Do not silence it."),
            new Scrap("reader_signs", 2, "reader", 2, "When you cycle the reader, someone signs for you.\nWatch the log."),
            new Scrap("drives_names", 2, "drive", 1, "Those are not drives.\nThey have names."),
            new Scrap("bags_answers", 2, "shredder", 2, "Every bag you carry is somebody's answer.\nKeep one."),
            new Scrap("queue", 3, null, 0, "We are the people in the queue.\nWe are still here."),
            new Scrap("rank_leash", 3, "mail", 8, "Your rank is not a reward.\nIt is how far in they will let you."),
            new Scrap("two_numbers", 3, null, 0, "Be reliable. They watch that number.\nWe watch the other one."),
            new Scrap("caught_tape", 3, null, 0, "If you are caught, listen to the tape.\nIt tells you what they are afraid of."),
        };

        /// <summary>The next scrap whose trigger is met and that has not been found, or null. At most one per call.</summary>
        public static Scrap Next(int stage, Func<string, int> jobCount, ISet<string> found)
        {
            foreach (var s in All)
            {
                if (found.Contains(s.Id) || s.MinStage > stage) continue;
                if (s.JobId != null && jobCount(s.JobId) < s.JobCount) continue;
                return s;
            }
            return null;
        }
    }

    /// <summary>The slip that arrives with the morning: numbers from the town, in the same flat voice as everything else.</summary>
    public static class LedgerSlip
    {
        public static string Text(int stage, float rollout, int restoredHouseholds)
        {
            int onSupply = Math.Max(0, 412 - (int)Math.Round(rollout * 2.4f));
            int decided = 88 + (int)Math.Round(rollout * 3.1f);
            var sb = new System.Text.StringBuilder();
            sb.Append("OVERNIGHT SUMMARY\n");
            sb.Append("Zone 4, residential\n\n");
            sb.Append("Households on supply: ").Append(onSupply).Append('\n');
            sb.Append("Appeals decided: ").Append(decided).Append('\n');
            if (stage >= 3) sb.Append("Appeals destroyed: ").Append((int)Math.Round(decided * 0.86f)).Append('\n');
            sb.Append("Satisfaction: 99.2%\n");
            if (restoredHouseholds > 0)
            {
                sb.Append("\nUnexplained variance: ").Append(restoredHouseholds).Append(" households restored. Under review.");
            }
            return sb.ToString();
        }
    }

    /// <summary>The mandatory training session shown when a sabotage is caught. Text only for now; the trigger comes with stealth.</summary>
    public static class WellnessText
    {
        const int Tapes = 3;

        public static string Title(int timesSeen) => timesSeen >= Tapes ? "WELLNESS MOMENT (ABRIDGED)" : "MANDATORY WELLNESS MOMENT";

        public static string Body(int timesSeen)
        {
            if (timesSeen >= Tapes)
            {
                return "You have heard this before.\n\nToday's pay has been held. Please remain within tolerance.";
            }
            switch (timesSeen % Tapes)
            {
                case 0:
                    return "Thank you for taking a moment. A moment spent is a moment saved.\n\n" +
                           "Today's pay has been held while we talk about reliability. Reliability is a form of kindness. It is kindness to the building.\n\n" +
                           "You may feel that this is unfair. That is normal. Noted.";
                case 1:
                    return "A building is only as calm as its quietest person. We have noticed that you were not quiet. Please sit with that.\n\n" +
                           "Today's pay has been held. The following has been adjusted for your comfort: procedure.\n\n" +
                           "Thank you for your patience. I understand your frustration.";
                default:
                    return "Some of our colleagues have not completed this module. They are no longer with us in the same way. " +
                           "We would like you to complete it.\n\n" +
                           "Today's pay has been held. Please remain within tolerance.";
            }
        }
    }
}
