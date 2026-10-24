using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Noted.Core.Tests
{
    public class GameSessionTests
    {
        static GameSession NewSession(int seed = 7) => new GameSession(new GameConfig(), seed);

        // Finishes every step of a task the way the scene would, one station at a time.
        static void FinishTask(GameSession s, TaskInstance t)
        {
            while (!t.IsFinished)
            {
                var step = t.CurrentStep;
                var result = s.CompleteStep(step.StationId);
                Assert.IsTrue(result.Accepted, "step at " + step.StationId + " should be accepted");
            }
        }

        [Test]
        public void BeginDay_TraineeGetsTwoTasksFromTheStarterSet()
        {
            var s = NewSession();
            var list = s.BeginDay();
            Assert.AreEqual(2, list.Count);
            Assert.IsTrue(list.All(t => t.Definition.MinRank == 0));
            Assert.AreEqual(1, s.Day);
            Assert.AreEqual(60f, s.Clock.Total, 1e-4f);
        }

        [Test]
        public void BeginDay_IsDeterministicForASeed()
        {
            var a = NewSession(42);
            var b = NewSession(42);
            var la = a.BeginDay().Select(t => t.Definition.Id).ToList();
            var lb = b.BeginDay().Select(t => t.Definition.Id).ToList();
            CollectionAssert.AreEqual(la, lb);
        }

        [Test]
        public void NewTaskTypes_AreReportedBeforeTheyAreMarkedSeen()
        {
            var s = NewSession();
            s.BeginDay();
            Assert.AreEqual(2, s.NewTaskTypesToday().Count);
            s.MarkSeen();
            Assert.AreEqual(0, s.NewTaskTypesToday().Count);
        }

        [Test]
        public void CompleteStep_FinishingATaskPaysCoinsToTheFloorNotTheWallet()
        {
            var s = NewSession();
            var task = s.BeginDay()[0];
            FinishTask(s, task);
            Assert.AreEqual(TaskStatus.Done, task.Status);
            Assert.AreEqual(10, s.CoinsOutstanding);
            Assert.AreEqual(0, s.Wallet.Balance);
            s.CollectCoins(4);
            Assert.AreEqual(4, s.Wallet.Balance);
            Assert.AreEqual(6, s.CoinsOutstanding);
            Assert.AreEqual(6, s.CollectAllOutstanding());
            Assert.AreEqual(10, s.Wallet.Balance);
            Assert.AreEqual(0, s.CoinsOutstanding);
        }

        [Test]
        public void CompleteStep_AtTheWrongStationIsRejected()
        {
            var s = NewSession();
            s.BeginDay();
            var result = s.CompleteStep("vent");
            Assert.IsFalse(result.Accepted);
        }

        [Test]
        public void CompleteStep_AfterTheClockExpiresIsRejected()
        {
            var s = NewSession();
            var task = s.BeginDay()[0];
            Assert.IsTrue(s.Tick(61f));
            Assert.IsFalse(s.CompleteStep(task.CurrentStep.StationId).Accepted);
        }

        [Test]
        public void Tick_ReturnsTrueOnlyOnTheExpiringTick()
        {
            var s = NewSession();
            s.BeginDay();
            Assert.IsFalse(s.Tick(30f));
            Assert.IsTrue(s.Tick(30f));
            Assert.IsFalse(s.Tick(5f));
        }

        [Test]
        public void TwoStepTask_NeedsTheCarriedItemAndConsumesIt()
        {
            var cfg = new GameConfig();
            var s = new GameSession(cfg, 3);
            s.BeginDayWith(new[] { new TaskInstance(s.Tasks.Find("filter")) });

            Assert.IsFalse(s.TryGetAvailableStep("vent", out _, out _), "vent needs the filter first");
            Assert.IsTrue(s.TryGetAvailableStep("filter_shelf", out _, out _), "the shelf step is available first");

            Assert.IsTrue(s.CompleteStep("filter_shelf").Accepted);
            CollectionAssert.AreEqual(new[] { "filter" }, s.Carried);
            Assert.IsFalse(s.TryGetAvailableStep("filter_shelf", out _, out _), "the shelf step is done");
            Assert.IsTrue(s.TryGetAvailableStep("vent", out _, out _));

            var done = s.CompleteStep("vent");
            Assert.IsTrue(done.TaskDone);
            Assert.AreEqual(0, s.Carried.Count);
        }

        [Test]
        public void EndDay_CountsCompletedTasksAndRanksUpAtTheThreshold()
        {
            var s = NewSession();
            int completed = 0;
            bool rankedUp = false;
            for (int day = 0; day < 6 && !rankedUp; day++)
            {
                var list = s.BeginDay();
                foreach (var t in list.Where(x => !x.IsFinished)) { FinishTask(s, t); completed++; }
                var summary = s.EndDay();
                Assert.AreEqual(list.Count, summary.TasksCompleted);
                rankedUp = summary.RankedUp;
                s.CollectAllOutstanding();
                if (rankedUp) Assert.AreEqual(1, summary.NewRank);
            }
            Assert.IsTrue(rankedUp);
            Assert.GreaterOrEqual(completed, 5);
            Assert.AreEqual(1, s.CurrentRank);
        }

        [Test]
        public void AutomatedTasks_PayCoinsButDoNotCountTowardRank()
        {
            var s = NewSession();
            s.Wallet.Add(1000);
            s.Ledger.TryBuy(s.Wallet, s.Upgrades.Find("auto_coffee"));
            s.Ledger.TryBuy(s.Wallet, s.Upgrades.Find("auto_printer"));
            s.Ledger.TryBuy(s.Wallet, s.Upgrades.Find("auto_shredder"));
            int before = s.Wallet.Balance;
            var list = s.BeginDayWith(new[]
            {
                new TaskInstance(s.Tasks.Find("coffee"), TaskStatus.Automated),
                new TaskInstance(s.Tasks.Find("printer"), TaskStatus.Automated),
            });
            Assert.IsTrue(list.All(t => t.Status == TaskStatus.Automated));
            var summary = s.EndDay();
            Assert.AreEqual(2, summary.TasksAutomated);
            Assert.AreEqual(0, summary.TasksCompleted);
            Assert.AreEqual(0, s.Rank.CompletedTasks);
            Assert.AreEqual(before + 20, s.Wallet.Balance);
        }

        [Test]
        public void TidyHabits_AddsTimeForEachFinishedTask()
        {
            var s = NewSession();
            s.Wallet.Add(1000);
            Assert.IsTrue(s.Ledger.TryBuy(s.Wallet, s.Upgrades.Find("tidy_habits")));
            var task = s.BeginDay()[0];
            float before = s.Clock.Remaining;
            FinishTask(s, task);
            Assert.AreEqual(before + 3f, s.Clock.Remaining, 1e-4f);
        }

        [Test]
        public void LongerDay_AddsFifteenSecondsPerLevelToTheNextDay()
        {
            var s = NewSession();
            s.Wallet.Add(1000);
            s.Ledger.TryBuy(s.Wallet, s.Upgrades.Find("longer_day"));
            s.Ledger.TryBuy(s.Wallet, s.Upgrades.Find("longer_day"));
            s.BeginDay();
            Assert.AreEqual(90f, s.Clock.Total, 1e-4f);
        }

        [Test]
        public void PreviewNextDay_MatchesTheDayThatThenStarts()
        {
            var s = NewSession(11);
            s.BeginDay();
            s.MarkSeen();
            s.EndDay();
            var preview = s.PreviewNextDay().Select(t => t.Definition.Id).ToList();
            var actual = s.BeginDay().Select(t => t.Definition.Id).ToList();
            CollectionAssert.AreEqual(preview, actual);
        }

        [Test]
        public void SpecialistDay_HasFiveDifferentJobs()
        {
            var cfg = new GameConfig();
            var tasks = TaskCatalog.Default();
            var list = DayPlanner.Plan(cfg, tasks, 2, new HashSet<string>(), new HashSet<string>(), 5);
            Assert.AreEqual(5, list.Count);
            Assert.AreEqual(5, list.Select(t => t.Definition.Id).Distinct().Count());
        }

        [Test]
        public void UnseenJobTypes_ComeBeforeSeenOnes()
        {
            var cfg = new GameConfig();
            var tasks = TaskCatalog.Default();
            var seen = new HashSet<string> { "coffee", "printer", "shredder" };
            for (int seed = 0; seed < 25; seed++)
            {
                var list = DayPlanner.Plan(cfg, tasks, 1, seen, new HashSet<string>(), seed);
                Assert.IsTrue(list.All(t => !seen.Contains(t.Definition.Id)), "seed " + seed + ": " + string.Join(",", list.Select(t => t.Definition.Id)));
            }
        }

        [Test]
        public void EveryJob_HasShortHolds_AndEveryStationHasARoom()
        {
            var tasks = TaskCatalog.Default();
            var areas = AreaCatalog.Default();
            foreach (var t in tasks.All)
            {
                foreach (var step in t.Steps)
                {
                    Assert.LessOrEqual(step.HoldSeconds, 3f, t.Id + " hold is too long to stand and wait");
                    Assert.IsTrue(areas.StationIds.Contains(step.StationId), t.Id + ": station " + step.StationId + " has no room");
                    if (step.Kind != StepKind.Hold) Assert.AreEqual(0f, step.HoldSeconds, t.Id);
                    if (step.Kind == StepKind.MiniGame) Assert.AreNotEqual(MiniGameKind.None, step.Game, t.Id);
                }
            }
        }

        [Test]
        public void CarryCapacity_OfOne_MakesYouDeliverBeforeFetchingAgain()
        {
            var s = NewSession();
            s.BeginDayWith(new[] { new TaskInstance(s.Tasks.Find("coffee")), new TaskInstance(s.Tasks.Find("printer")) });
            Assert.AreEqual(1, s.CarryCapacity);

            Assert.IsTrue(s.CompleteStep("bean_sack").Accepted);
            Assert.IsFalse(s.TryGetAvailableStep("paper_stack", out _, out _), "hands are full");
            Assert.IsTrue(s.IsHandsFullAt("paper_stack"));
            Assert.IsFalse(s.CompleteStep("paper_stack").Accepted);

            Assert.IsTrue(s.CompleteStep("coffee_machine").TaskDone);
            Assert.IsFalse(s.IsHandsFullAt("paper_stack"));
            Assert.IsTrue(s.CompleteStep("paper_stack").Accepted);
        }

        [Test]
        public void HandCart_LetsYouCarryThreeThings()
        {
            var s = NewSession();
            s.Wallet.Add(500);
            Assert.IsTrue(s.Ledger.TryBuy(s.Wallet, s.Upgrades.Find("hand_cart")));
            Assert.AreEqual(3, s.CarryCapacity);
            s.BeginDayWith(new[]
            {
                new TaskInstance(s.Tasks.Find("coffee")),
                new TaskInstance(s.Tasks.Find("printer")),
                new TaskInstance(s.Tasks.Find("shredder")),
            });
            Assert.IsTrue(s.CompleteStep("bean_sack").Accepted);
            Assert.IsTrue(s.CompleteStep("paper_stack").Accepted);
            Assert.IsTrue(s.CompleteStep("shredder").Accepted);
            Assert.AreEqual(3, s.Carried.Count);
            Assert.IsTrue(s.IsCarrying("beans") && s.IsCarrying("paper") && s.IsCarrying("bag"));
            Assert.IsTrue(s.CompleteStep("dumpster").TaskDone);
            Assert.AreEqual(2, s.Carried.Count);
        }

        [Test]
        public void ToolsForLaterJobs_AreListedButCannotBeBought()
        {
            var s = NewSession();
            s.Wallet.Add(5000);
            foreach (var def in s.Upgrades.All)
            {
                if (def.Category != UpgradeCategory.Later) continue;
                Assert.IsFalse(def.Available, def.Id);
                Assert.IsFalse(s.Ledger.TryBuy(s.Wallet, def, 2), def.Id + " must not be buyable yet");
            }
            Assert.AreEqual(5000, s.Wallet.Balance);
        }

        [Test]
        public void RankGatedTools_NeedTheRank()
        {
            var cat = UpgradeCatalog.Default();
            var ledger = new UpgradeLedger(cat);
            var gated = new UpgradeDefinition("test_tool", "Test", "x", UpgradeKind.RunningShoes, 10, 1f, 1, null, UpgradeCategory.Tools, 2, true);
            var w = new Wallet();
            w.Add(100);
            Assert.IsFalse(ledger.TryBuy(w, gated, 1));
            Assert.IsTrue(ledger.TryBuy(w, gated, 2));
        }
    }
}
