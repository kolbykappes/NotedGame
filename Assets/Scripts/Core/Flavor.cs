using System;
using System.Collections.Generic;
using System.Linq;

namespace Noted.Core
{
    /// <summary>Numbers a gauge can show. The odd ones are the first hint that someone else has been around.</summary>
    public static class Readouts
    {
        // string.GetHashCode is not stable between runtimes, so use a small fixed hash for seeds.
        static int StableHash(string s)
        {
            unchecked
            {
                int h = 17;
                foreach (char c in s) h = h * 31 + c;
                return h;
            }
        }

        /// <summary>The text to show when the player reads a station. Deterministic for a given seed and day.</summary>
        public static string ForStation(string stationId, int day, int rank, int seed)
        {
            var rng = new Random(unchecked(seed * 17 + day * 131 + StableHash(stationId) * 7));
            switch (stationId)
            {
                case "thermostat":
                    // Mostly a normal room temperature; occasionally a degree or two too warm, as if the room were lived in.
                    double t = 20.5 + rng.NextDouble() * 2.5;
                    if (rng.NextDouble() < 0.12 + 0.08 * rank) t += 3.5;
                    return t.ToString("0.0") + " C";
                default:
                    return null;
            }
        }
    }

    /// <summary>A small thing that suggests someone else was here. Text only; no one ever appears.</summary>
    public sealed class Oddity
    {
        public readonly string StationId;
        public readonly int MinRank;
        public readonly string Text;

        public Oddity(string stationId, int minRank, string text)
        {
            StationId = stationId;
            MinRank = minRank;
            Text = text;
        }
    }

    public static class OddityBank
    {
        public static readonly Oddity[] All =
        {
            // Trainee: mild
            new Oddity("coffee_machine", 0, "The pot is already warm. You have not touched it today."),
            new Oddity("breaker_corridor", 0, "The breaker is stiff, as if it had only just been thrown."),
            new Oddity("mail_table", 0, "One envelope is addressed in handwriting you do not recognise."),
            new Oddity("printer", 0, "There is a page in the output tray. It reads: Thank you for your patience."),
            new Oddity("paper_stack", 0, "The top ream has been opened and resealed."),
            new Oddity("clipboard", 0, "The last entry on the clipboard is in pen. Yesterday you wrote in pencil."),
            // Associate: slightly stranger
            new Oddity("reader_power", 1, "The badge log shows an entry at 03:12 for a badge that is not yours."),
            new Oddity("patch_panel", 1, "Port 14 is already patched. It is not on the diagram."),
            new Oddity("vent", 1, "The old filter has a smudge on it, like a hand."),
            // Specialist: modestly odd
            new Oddity("spares_cabinet", 2, "The failed drive's label has been written over by hand."),
            new Oddity("alarm_panel", 2, "The panel logged a door opening at 02:58. Nothing is on the camera."),
            new Oddity("rack_drive", 2, "The rack is warm to the touch. It is meant to be powered down."),
        };

        /// <summary>
        /// Picks today's oddities among the stations that today's jobs will actually visit. Trainee days get at most one, Associate days one,
        /// Specialist days one or two.
        /// </summary>
        public static Dictionary<string, string> Pick(int rank, IEnumerable<string> stationsToday, int seed)
        {
            var rng = new Random(seed);
            var today = new HashSet<string>(stationsToday);
            var candidates = All.Where(o => o.MinRank <= rank && today.Contains(o.StationId)).ToList();

            // At higher ranks prefer the stranger ones.
            candidates = candidates.OrderByDescending(o => o.MinRank == rank ? 1 : 0).ThenBy(_ => rng.Next()).ToList();

            int want = rank == 0 ? (rng.NextDouble() < 0.5 ? 1 : 0) : rank == 1 ? 1 : (rng.NextDouble() < 0.5 ? 2 : 1);
            var result = new Dictionary<string, string>();
            foreach (var o in candidates)
            {
                if (result.Count >= want) break;
                result[o.StationId] = o.Text;
            }
            return result;
        }
    }
}
