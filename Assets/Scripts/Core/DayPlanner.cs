using System;
using System.Collections.Generic;
using System.Linq;

namespace Noted.Core
{
    /// <summary>Chooses the day's task list for a rank. Deterministic for a given seed.</summary>
    public static class DayPlanner
    {
        public static List<TaskInstance> Plan(
            GameConfig config,
            TaskCatalog catalog,
            int rank,
            ISet<string> seenTaskIds,
            ISet<string> automatedTaskIds,
            int seed)
        {
            int count = config.TasksFor(rank);
            var rng = new Random(seed);
            var eligible = catalog.All.Where(t => t.MinRank <= rank).ToList();

            var picked = new List<TaskDefinition>();

            // Types the player has never seen come first, so each new type teaches itself as soon as it can.
            var unseen = eligible.Where(t => !seenTaskIds.Contains(t.Id)).ToList();
            Shuffle(unseen, rng);
            foreach (var t in unseen)
            {
                if (picked.Count >= count) break;
                picked.Add(t);
            }

            // Fill the rest without repeats where possible.
            var rest = eligible.Where(t => !picked.Contains(t)).ToList();
            Shuffle(rest, rng);
            foreach (var t in rest)
            {
                if (picked.Count >= count) break;
                picked.Add(t);
            }

            while (picked.Count < count && eligible.Count > 0)
            {
                picked.Add(eligible[rng.Next(eligible.Count)]);
            }

            Shuffle(picked, rng);

            return picked
                .Select(t => new TaskInstance(t, automatedTaskIds.Contains(t.Id) ? TaskStatus.Automated : TaskStatus.Pending))
                .ToList();
        }

        static void Shuffle<T>(IList<T> list, Random rng)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = rng.Next(i + 1);
                (list[i], list[j]) = (list[j], list[i]);
            }
        }
    }
}
