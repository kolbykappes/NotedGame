using System.IO;
using Noted.Core;
using UnityEngine;

namespace Noted.Game
{
    /// <summary>
    /// Appends one CSV row per finished day to daylog.csv in the persistent data path, so pacing can be read as numbers
    /// (how long the official list took, how much slack was left, what the second list achieved).
    /// </summary>
    public static class DayLogWriter
    {
        public static string Path => System.IO.Path.Combine(Application.persistentDataPath, "daylog.csv");

        public static void Append(DayStats stats)
        {
            if (stats == null) return;
            try
            {
                bool fresh = !File.Exists(Path);
                using (var writer = new StreamWriter(Path, true))
                {
                    if (fresh) writer.WriteLine(DayStats.CsvHeader);
                    writer.WriteLine(stats.ToCsv());
                }
            }
            catch (IOException e)
            {
                Debug.LogWarning("Could not write the day log: " + e.Message);
            }
            catch (System.UnauthorizedAccessException e)
            {
                Debug.LogWarning("Could not write the day log: " + e.Message);
            }
        }
    }
}
