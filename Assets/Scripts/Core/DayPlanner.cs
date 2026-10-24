using System;
using System.Collections.Generic;
using System.Linq;

namespace Noted.Core
{
    /// <summary>Chooses the day's task list for a rank. Deterministic for a given seed.</summary>
    public static class DayPlanner
    {
        /// <summary>Plans a day where every job in the catalog is allowed (used by tests and simple callers).</summary>
        public static List<TaskInstance> Plan(
            GameConfig config,
            TaskCatalog catalog,
            int rank,
            ISet<string> seenTaskIds,
            ISet<string> automatedTaskIds,
            int seed)
        {
            return Plan(config, catalog, rank, seenTaskIds, automatedTaskIds, seed, t => t.MinRank <= rank, null);
        }

        /// <param name="available">Whether a job may be on the list at all (rank, rooms that are open, tools owned).</param>
        /// <param name="priorityJobIds">Jobs that open the next room; they go on the list first so each early day unlocks something.</param>
        public static List<TaskInstance> Plan(
            GameConfig config,
            TaskCatalog catalog,
            int rank,
            ISet<string> seenTaskIds,
            ISet<string> automatedTaskIds,
            int seed,
            Func<TaskDefinition, bool> available,
            IReadOnlyList<string> priorityJobIds)
        {
            int count = config.TasksFor(rank);
            var rng = new Random(seed);
            var eligible = catalog.All.Where(available).ToList();

            var picked = new List<TaskDefinition>();

            // 1. Jobs that unlock the next room (but leave room for at least one other job).
            if (priorityJobIds != null)
            {
                int maxPriority = Math.Max(1, count - 1);
                foreach (var id in priorityJobIds)
                {
                    if (picked.Count >= maxPriority) break;
                    var def = eligible.FirstOrDefault(t => t.Id == id);
                    if (def != null && !picked.Contains(def)) picked.Add(def);
                }
            }

            // 2. Types the player has never seen, so each new type teaches itself as soon as it can.
            var unseen = eligible.Where(t => !seenTaskIds.Contains(t.Id) && !picked.Contains(t)).ToList();
            Shuffle(unseen, rng);
            foreach (var t in unseen)
            {
                if (picked.Count >= count) break;
                picked.Add(t);
            }

            // 3. Fill the rest without repeats where possible.
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
