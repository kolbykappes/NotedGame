using System.Globalization;

namespace Noted.Core
{
    /// <summary>What happened in one day, for the day log and the debug overlay.</summary>
    public sealed class DayStats
    {
        public int Level;
        public int Day;
        public float DaySeconds;
        public int TasksTotal;
        public int TasksDone;

        /// <summary>Seconds into the day when the last official task finished, or -1 if the list was not finished.</summary>
        public float OfficialFinishedAt = -1f;

        public int ErrandsOffered;
        public int ErrandsStarted;
        public int ErrandsDone;
        public float Reliability;
        public float Rollout;
        public int Coins;

        /// <summary>Seconds left when the official list was finished (the room for sabotage), or -1 if it was not finished.</summary>
        public float Slack => OfficialFinishedAt < 0f ? -1f : DaySeconds - OfficialFinishedAt;

        public const string CsvHeader =
            "level,day,day_seconds,tasks_total,tasks_done,official_finished_at,slack,errands_offered,errands_started,errands_done,reliability,rollout,coins";

        static string F(float v) => v.ToString("0.0", CultureInfo.InvariantCulture);
        static string I(int v) => v.ToString(CultureInfo.InvariantCulture);

        public string ToCsv()
        {
            return string.Join(",",
                I(Level), I(Day), F(DaySeconds), I(TasksTotal), I(TasksDone),
                OfficialFinishedAt < 0f ? "" : F(OfficialFinishedAt),
                Slack < 0f ? "" : F(Slack),
                I(ErrandsOffered), I(ErrandsStarted), I(ErrandsDone),
                F(Reliability), F(Rollout), I(Coins));
        }
    }
}
