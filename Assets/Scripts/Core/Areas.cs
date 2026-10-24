using System;
using System.Collections.Generic;

namespace Noted.Core
{
    public static class AreaIds
    {
        public const string Admin = "admin";
        public const string Corridor = "corridor";
        public const string Break = "break";
        public const string Print = "print";
        public const string Office = "office";
        public const string Utility = "utility";
        public const string Closet = "closet";
        public const string Hall = "hall";
    }

    /// <summary>How an area opens. Several conditions can apply; all must hold.</summary>
    public sealed class AreaRule
    {
        public readonly string AreaId;
        public readonly string Name;
        public readonly int MinRank;
        public readonly string UnlockJobId;
        public readonly int UnlockJobCount;
        public readonly string RequiresTool;

        public AreaRule(string areaId, string name, int minRank = 0, string unlockJobId = null, int unlockJobCount = 0, string requiresTool = null)
        {
            AreaId = areaId;
            Name = name;
            MinRank = minRank;
            UnlockJobId = unlockJobId;
            UnlockJobCount = unlockJobCount;
            RequiresTool = requiresTool;
        }

        public bool AlwaysOpen => MinRank == 0 && UnlockJobId == null && RequiresTool == null;
    }

    /// <summary>
    /// Which rooms exist, how each one opens, and which room each station is in. Rooms open as the player does the work:
    /// the print room after the coffee has been restocked twice, the office after the printer paper, the utility room after two breaker
    /// resets, the network closet at Associate, and the server hall at Specialist with a staff keycard.
    /// </summary>
    public sealed class AreaCatalog
    {
        public readonly IReadOnlyList<AreaRule> Rules;
        readonly Dictionary<string, string> stationAreas;

        public AreaCatalog(IReadOnlyList<AreaRule> rules, Dictionary<string, string> stations)
        {
            Rules = rules;
            stationAreas = stations;
        }

        public AreaRule Find(string areaId)
        {
            foreach (var r in Rules)
            {
                if (r.AreaId == areaId) return r;
            }
            return null;
        }

        public string AreaOfStation(string stationId)
        {
            return stationAreas.TryGetValue(stationId, out var area) ? area : AreaIds.Corridor;
        }

        public IEnumerable<string> StationIds => stationAreas.Keys;

        public bool IsUnlocked(string areaId, int rank, Func<string, int> jobCount, Func<string, bool> hasTool)
        {
            var rule = Find(areaId);
            if (rule == null || rule.AlwaysOpen) return true;
            if (rank < rule.MinRank) return false;
            if (rule.UnlockJobId != null && jobCount(rule.UnlockJobId) < rule.UnlockJobCount) return false;
            if (rule.RequiresTool != null && !hasTool(rule.RequiresTool)) return false;
            return true;
        }

        public static AreaCatalog Default()
        {
            var rules = new[]
            {
                new AreaRule(AreaIds.Admin, "the admin room"),
                new AreaRule(AreaIds.Corridor, "the corridor"),
                new AreaRule(AreaIds.Break, "the break room"),
                new AreaRule(AreaIds.Print, "the print room", 0, "coffee", 2),
                new AreaRule(AreaIds.Office, "the office", 0, "printer", 1),
                new AreaRule(AreaIds.Utility, "the utility room", 0, "breakers", 2),
                new AreaRule(AreaIds.Closet, "the network closet", 1),
                new AreaRule(AreaIds.Hall, "the server hall", 2, null, 0, "staff_keycard"),
            };

            var stations = new Dictionary<string, string>
            {
                { "bean_sack", AreaIds.Admin }, { "paper_stack", AreaIds.Admin }, { "mail_table", AreaIds.Admin }, { "breaker_admin", AreaIds.Admin },
                { "coffee_machine", AreaIds.Break }, { "filter_shelf", AreaIds.Break }, { "breaker_break", AreaIds.Break },
                { "breaker_corridor", AreaIds.Corridor }, { "thermostat", AreaIds.Corridor }, { "vent", AreaIds.Corridor }, { "reader_power", AreaIds.Corridor },
                { "printer", AreaIds.Print }, { "shredder", AreaIds.Print },
                { "clipboard", AreaIds.Office },
                { "dumpster", AreaIds.Utility },
                { "patch_panel", AreaIds.Closet },
                { "spares_cabinet", AreaIds.Hall }, { "rack_drive", AreaIds.Hall }, { "alarm_panel", AreaIds.Hall }, { "alarm_reset", AreaIds.Hall },
            };

            return new AreaCatalog(rules, stations);
        }
    }
}
