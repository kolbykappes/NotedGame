using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Noted.Core.Tests
{
    /// <summary>Tests for the feel-test layer: the level table, start profiles, replaying a day, reliability, rollout, the reveal text and the second list.</summary>
    public class FeelTestTests
    {
        static GameSession NewSession(int seed = 7) => new GameSession(new GameConfig(), seed);

        static GameSession AtLevel(int level, int seed = 7)
        {
            var s = NewSession(seed);
            s.ApplyStartProfile(StartProfile.ForLevel(level));
            return s;
        }

        static void FinishTask(GameSession s, TaskInstance t)
        {
            while (!t.IsFinished)
            {
                // The badge reader job must wait between its two steps.
                var next = t.CurrentStep;
                if (next.MinSecondsAfterPrevious > 0f && t.StepIndex > 0) s.Tick(next.MinSecondsAfterPrevious + 0.1f);
                string station = next.StationId;
                var result = s.CompleteStep(station);
                Assert.IsTrue(result.Accepted, "step at " + station + " should be accepted");
            }
        }

        // ---- level table and config ----

        [Test]
        public void LevelTable_Default_HasFiveLevelsWithConsistentThresholds()
        {
            var table = LevelTable.Default();
            Assert.AreEqual(5, table.Count);
            Assert.AreEqual(4, table.MaxRank);
            CollectionAssert.AreEqual(new[] { 5, 12, 42, 90 }, table.Thresholds());
            Assert.AreEqual(110f, table.For(2).DaySeconds);
            Assert.AreEqual(5, table.For(2).TaskCount);
            Assert.AreEqual("Steward", table.For(99).Title, "an out-of-range rank clamps to the last level");
            Assert.AreEqual("Trainee", table.For(-3).Title);
        }

        [Test]
        public void LevelTable_RevealStageNeverGoesDown_AndFloorsRise()
        {
            var table = LevelTable.Default();
            for (int r = 1; r < table.Count; r++)
            {
                Assert.GreaterOrEqual(table.For(r).RevealStage, table.For(r - 1).RevealStage);
                Assert.Greater(table.For(r).RolloutFloor, table.For(r - 1).RolloutFloor);
                Assert.Greater(table.For(r).TasksToReach, table.For(r - 1).TasksToReach);
            }
        }

        [Test]
        public void GameConfig_KeepsTheOldLookupsWorking()
        {
            var c = new GameConfig();
            Assert.AreEqual("Specialist", c.TitleFor(2));
            Assert.AreEqual(2, c.TasksFor(0));
            Assert.AreEqual(60f, c.DayLengthFor(0));
            Assert.AreEqual(4, c.MaxRank);
        }

        [Test]
        public void EveryLevelAfterTheSecond_HasATapeText()
        {
            var table = LevelTable.Default();
            for (int r = 3; r < table.Count; r++) Assert.IsNotNull(table.For(r).TapeText, "level " + (r + 1));
            StringAssert.Contains("Technician", CardText.RankUp(new GameConfig(), 3));
        }

        // ---- start profile ----

        [Test]
        public void StartProfile_LevelThree_SetsRankDayCoinsToolsRoomsAndSeenJobs()
        {
            var s = AtLevel(3);
            Assert.AreEqual(2, s.CurrentRank);
            Assert.GreaterOrEqual(s.Rank.CompletedTasks, 12);
            Assert.AreEqual(150, s.Wallet.Balance);
            Assert.IsTrue(s.HasTool("hand_cart"));
            Assert.IsTrue(s.HasTool("staff_keycard"));
            Assert.IsTrue(s.HasTool("anti_static_strap"));
            Assert.IsFalse(s.HasTool("auto_coffee"), "service contracts are never granted");
            Assert.IsTrue(s.IsAreaUnlocked("hall"));
            Assert.AreEqual(11f, s.Rollout.Value, 1e-4f);
            Assert.AreEqual(4f, s.Rollout.Floor, 1e-4f);

            var list = s.BeginDay();
            Assert.AreEqual(11, s.Day);
            Assert.AreEqual(110f, s.Clock.Total, 1e-4f);
            Assert.AreEqual(5, list.Count);
            Assert.AreEqual(0, s.NewTaskTypesToday().Count, "no how-to cards on a debug start");
        }

        [Test]
        public void StartProfile_CustomDayAndRollout_AreUsed()
        {
            var s = NewSession();
            s.ApplyStartProfile(new StartProfile { Level = 4, Day = 40, Rollout = 55f, Reliability = 90f, Coins = 7 });
            s.BeginDay();
            Assert.AreEqual(40, s.Day);
            Assert.AreEqual(55f, s.Rollout.Value, 1e-4f);
            Assert.AreEqual(90f, s.Reliability.Value, 1e-4f);
            Assert.AreEqual(7, s.Wallet.Balance);
            Assert.AreEqual(130f, s.Clock.Total, 1e-4f);
        }

        [Test]
        public void StartProfile_LevelOne_IsAnOrdinaryNewGameApartFromTools()
        {
            var s = NewSession();
            s.ApplyStartProfile(new StartProfile { Level = 1, Loadout = ToolLoadout.None, AllRoomsOpen = false, MarkAllJobsSeen = false, Coins = 0 });
            s.BeginDay();
            Assert.AreEqual(0, s.CurrentRank);
            Assert.AreEqual(1, s.Day);
            Assert.AreEqual(60f, s.Clock.Total, 1e-4f);
            Assert.AreEqual(2, s.TodaysTasks.Count);
            Assert.IsFalse(s.IsAreaUnlocked("hall"));
        }

        [Test]
        public void Grant_AllGivesEveryBuyableToolAtMaxLevel_ButNotContractsOrLockedRanks()
        {
            var s = NewSession();
            s.ApplyStartProfile(new StartProfile { Level = 1, Loadout = ToolLoadout.All });
            Assert.AreEqual(3, s.Ledger.Level("faster_hands"));
            Assert.IsFalse(s.HasTool("staff_keycard"), "the keycard needs Specialist");
            Assert.IsFalse(s.HasTool("auto_printer"));
        }

        // ---- rank tracker ----

        [Test]
        public void RankTracker_HoldsAPromotionUntilItIsAllowed()
        {
            var r = new RankTracker(new[] { 5, 12 });
            Assert.IsFalse(r.Record(6, false));
            Assert.AreEqual(0, r.Rank);
            Assert.IsTrue(r.PromotionWaiting);
            Assert.IsTrue(r.Record(0, true));
            Assert.AreEqual(1, r.Rank);
            Assert.IsFalse(r.PromotionWaiting);
        }

        [Test]
        public void RankTracker_SetRank_RaisesTheCompletedCountToTheThreshold()
        {
            var r = new RankTracker(new[] { 5, 12 });
            r.SetRank(2);
            Assert.AreEqual(2, r.Rank);
            Assert.AreEqual(12, r.CompletedTasks);
            r.SetRank(99);
            Assert.AreEqual(2, r.Rank, "clamped to the top rank");
        }

        // ---- reliability and rollout ----

        [Test]
        public void Reliability_DropsPerUnfinishedTask_RecoversOnCleanDays_AndClamps()
        {
            var r = new Reliability();
            Assert.AreEqual(100f, r.Value);
            r.EndOfDay(2);
            Assert.AreEqual(84f, r.Value, 1e-4f);
            r.EndOfDay(0);
            Assert.AreEqual(86f, r.Value, 1e-4f);
            for (int i = 0; i < 20; i++) r.EndOfDay(0);
            Assert.AreEqual(100f, r.Value, 1e-4f);
            r.EndOfDay(50);
            Assert.AreEqual(0f, r.Value, 1e-4f);
        }

        [Test]
        public void Reliability_GateIsEighty()
        {
            var r = new Reliability();
            r.Set(79.9f);
            Assert.IsFalse(r.CanPromote);
            r.Set(80f);
            Assert.IsTrue(r.CanPromote);
        }

        [Test]
        public void Rollout_ParMovesItForward_ErrandsPushBack_FloorAndCapHold()
        {
            var r = new Rollout();
            Assert.AreEqual(1.06f, r.EndOfDay(1.06f, 0f), 1e-4f);
            r.RaiseFloor(10f);
            Assert.AreEqual(10f, r.Value, 1e-4f, "raising the floor lifts a value that is below it");
            r.EndOfDay(1.06f, -5f);
            Assert.AreEqual(10f, r.Value, 1e-4f, "cannot fall below the floor");
            r.Set(500f);
            Assert.AreEqual(Rollout.Cap, r.Value, 1e-4f);
            Assert.AreEqual(5, r.Locks);
            r.Set(0f);
            Assert.AreEqual(1, r.Locks, "at least one lock");
        }

        [Test]
        public void Rollout_PositiveErrandEffect_IsIgnored()
        {
            var r = new Rollout();
            r.EndOfDay(1f, +9f);
            Assert.AreEqual(1f, r.Value, 1e-4f);
        }

        [Test]
        public void Rollout_NinetyDaySimulation_MatchesThePaperNumbers()
        {
            float Simulate(float errandPerDay)
            {
                var r = new Rollout();
                for (int day = 1; day <= 88; day++)
                {
                    if (day % 10 == 0) r.RaiseFloor(r.Floor + 2f);
                    r.EndOfDay(1.06f, errandPerDay);
                }
                return r.Value;
            }

            Assert.That(Simulate(0f), Is.InRange(90f, 96f), "a pure worker is near the cap");
            Assert.That(Simulate(-0.5f), Is.InRange(44f, 54f), "a casual saboteur ends near half");
            Assert.That(Simulate(-1.3f), Is.InRange(15f, 20f), "a committed saboteur is pinned at the floor");
        }

        // ---- day stats ----

        [Test]
        public void DayStats_CsvRow_HasTheHeaderColumns_AndBlanksWhenUnfinished()
        {
            var stats = new DayStats
            {
                Level = 3, Day = 11, DaySeconds = 110f, TasksTotal = 5, TasksDone = 5, OfficialFinishedAt = 80f,
                ErrandsOffered = 1, ErrandsStarted = 1, ErrandsDone = 1, Reliability = 92f, Rollout = 12.5f, Coins = 60,
            };
            Assert.AreEqual(DayStats.CsvHeader.Split(',').Length, stats.ToCsv().Split(',').Length);
            Assert.AreEqual("3,11,110.0,5,5,80.0,30.0,1,1,1,92.0,12.5,60", stats.ToCsv());

            stats.OfficialFinishedAt = -1f;
            Assert.AreEqual(-1f, stats.Slack);
            Assert.AreEqual("3,11,110.0,5,5,,,1,1,1,92.0,12.5,60", stats.ToCsv());
        }

        [Test]
        public void EndDay_RecordsWhenTheListWasFinished_AndTheSlackLeft()
        {
            var s = NewSession();
            s.BeginDayWith(new[] { new TaskInstance(s.Tasks.Find("coffee")) });
            s.Tick(12f);
            FinishTask(s, s.TodaysTasks[0]);
            Assert.AreEqual(12f, s.OfficialFinishedAt, 1e-3f);
            s.Tick(10f);
            var summary = s.EndDay();
            Assert.AreEqual(1, summary.Stats.Level);
            Assert.AreEqual(1, summary.Stats.TasksDone);
            Assert.AreEqual(48f, summary.Stats.Slack, 1e-3f);
            Assert.AreSame(summary.Stats, s.LastStats);
        }

        [Test]
        public void EndDay_UnfinishedList_HasNoSlackAndCostsReliability()
        {
            var s = NewSession();
            s.BeginDay();
            var summary = s.EndDay();
            Assert.AreEqual(-1f, summary.Stats.Slack);
            Assert.AreEqual(100f - 2 * 8f, s.Reliability.Value, 1e-4f);
        }

        // ---- promotion held by reliability ----

        [Test]
        public void EndDay_HoldsThePromotionWhileReliabilityIsBelowTheGate()
        {
            var s = NewSession();
            s.Rank.Record(4);                 // one task short of Associate
            s.Reliability.Set(70f);
            s.BeginDay();
            foreach (var t in s.TodaysTasks) FinishTask(s, t);
            var held = s.EndDay();            // reliability recovers to 72, still below 80
            Assert.IsFalse(held.RankedUp);
            Assert.IsTrue(held.PromotionHeld);
            Assert.AreEqual(0, s.CurrentRank);

            s.Reliability.Set(90f);
            s.BeginDay();
            foreach (var t in s.TodaysTasks) FinishTask(s, t);
            var promoted = s.EndDay();
            Assert.IsTrue(promoted.RankedUp);
            Assert.IsFalse(promoted.PromotionHeld);
            Assert.AreEqual(1, s.CurrentRank);
            Assert.AreEqual(2f, s.Rollout.Floor, 1e-4f, "the floor rises with the promotion");
        }

        [Test]
        public void EndDay_MovesRolloutByParForTheCurrentLevel()
        {
            var s = NewSession();
            s.BeginDay();
            foreach (var t in s.TodaysTasks) FinishTask(s, t);
            var summary = s.EndDay();
            Assert.AreEqual(1.06f, summary.RolloutChange, 1e-4f);
            Assert.AreEqual(1.06f, s.Rollout.Value, 1e-4f);
        }

        // ---- replaying a day ----

        [Test]
        public void ReplayDay_RestoresTheStartOfTheDay_AndRegeneratesTheSameList()
        {
            var s = AtLevel(3);
            var first = s.BeginDay().Select(t => t.Definition.Id).ToList();
            int dayBefore = s.Day;
            FinishTask(s, s.TodaysTasks[0]);
            s.CollectAllOutstanding();
            s.Ledger.SetLevel("coin_magnet", 2);
            s.Rollout.Set(60f);
            s.Reliability.Set(10f);
            Assert.IsTrue(s.CanReplayDay);

            Assert.IsTrue(s.RestoreDayStart());
            Assert.AreEqual(dayBefore - 1, s.Day);
            Assert.AreEqual(150, s.Wallet.Balance);
            Assert.IsFalse(s.HasTool("coin_magnet"));
            Assert.AreEqual(11f, s.Rollout.Value, 1e-4f);
            Assert.AreEqual(100f, s.Reliability.Value, 1e-4f);
            Assert.AreEqual(0, s.JobCount(first[0]));

            var second = s.BeginDay().Select(t => t.Definition.Id).ToList();
            CollectionAssert.AreEqual(first, second);
            Assert.AreEqual(dayBefore, s.Day);
        }

        [Test]
        public void ReplayDay_WithoutAStartedDay_DoesNothing()
        {
            var s = NewSession();
            Assert.IsFalse(s.CanReplayDay);
            Assert.IsFalse(s.RestoreDayStart());
        }

        [Test]
        public void ReplayDay_AlsoRestoresRankAndErrandMemory()
        {
            var s = AtLevel(3);
            s.BeginDay();
            var errandIds = s.TodaysErrands.Select(e => e.Definition.Id).ToList();
            s.Rank.Record(40);
            s.RestoreDayStart();
            Assert.AreEqual(2, s.CurrentRank);
            Assert.AreEqual(12, s.Rank.CompletedTasks);
            s.BeginDay();
            CollectionAssert.AreEqual(errandIds, s.TodaysErrands.Select(e => e.Definition.Id).ToList());
        }

        // ---- the reveal text ----

        [Test]
        public void JobText_ReadsTheSameUntilTheReveal_ThenChanges()
        {
            Assert.IsNull(JobText.For("mail", 0));
            Assert.IsNull(JobText.For("mail", 1));
            Assert.AreEqual("Triage the appeals", JobText.For("mail", 2).Name);
            Assert.AreEqual("Triage the appeals: Zone 4", JobText.For("mail", 3).Name);
            Assert.AreEqual("Triage the appeals: Zone 4", JobText.For("mail", 9).Name, "beyond the last stage keeps the last wording");
            Assert.IsNull(JobText.For("no_such_job", 3));
        }

        [Test]
        public void JobText_EveryJobHasWordingForStagesTwoAndThree()
        {
            foreach (var def in TaskCatalog.Default().All)
            {
                Assert.IsNotNull(JobText.For(def.Id, 2), def.Id + " stage 2");
                Assert.IsNotNull(JobText.For(def.Id, 3), def.Id + " stage 3");
                Assert.AreNotEqual(JobText.For(def.Id, 2).DoneLine, JobText.For(def.Id, 3).DoneLine, def.Id + " stages differ");
            }
        }

        [Test]
        public void JobText_HowTo_AddsWhatTheJobReallyDoes_OnlyAfterTheReveal()
        {
            var mail = TaskCatalog.Default().Find("mail");
            Assert.AreEqual(mail.HowTo, JobText.HowToFor(mail, 0));
            StringAssert.StartsWith(mail.HowTo, JobText.HowToFor(mail, 2));
            StringAssert.Contains("appeals", JobText.HowToFor(mail, 2));
            Assert.AreEqual("Done: Sort the mail", JobText.DoneLineFor(mail, 0));
            StringAssert.Contains("Appeals triaged", JobText.DoneLineFor(mail, 2));
        }

        [Test]
        public void Procedures_AreReportedOncePerWordingChange()
        {
            var s = AtLevel(3);                         // stage 2
            s.BeginDay();
            var updates = s.ProcedureUpdatesToday();
            Assert.AreEqual(s.TodaysTasks.Select(t => t.Definition.Id).Distinct().Count(), updates.Count);
            s.MarkProceduresShown();
            Assert.AreEqual(0, s.ProcedureUpdatesToday().Count);

            var clean = AtLevel(1);
            clean.BeginDay();
            Assert.AreEqual(0, clean.ProcedureUpdatesToday().Count, "stage 0 has no updates");
        }

        [Test]
        public void Scraps_AreFoundOnceEach_InOrder_WhenTheirTriggerIsMet()
        {
            var found = new HashSet<string>();
            Assert.IsNull(ScrapBank.Next(0, _ => 99, found), "nothing before the reveal");
            Assert.AreEqual("not_first", ScrapBank.Next(1, _ => 0, found).Id, "the mail scrap needs three mail jobs");
            Assert.AreEqual("mail_not_junk", ScrapBank.Next(1, id => id == "mail" ? 3 : 0, found).Id);
            found.Add("mail_not_junk");
            Assert.AreEqual("not_first", ScrapBank.Next(1, id => id == "mail" ? 3 : 0, found).Id);
            found.Add("not_first");
            Assert.IsNull(ScrapBank.Next(1, id => id == "mail" ? 3 : 0, found));
        }

        [Test]
        public void Scraps_EveryScrapIdIsUnique_AndStageThreeScrapsExist()
        {
            Assert.AreEqual(ScrapBank.All.Length, ScrapBank.All.Select(s => s.Id).Distinct().Count());
            Assert.GreaterOrEqual(ScrapBank.All.Count(s => s.MinStage == 3), 3);
        }

        [Test]
        public void Session_FindsAndRemembersScraps()
        {
            var s = AtLevel(3);
            var first = s.NextScrap();
            Assert.IsNotNull(first);
            s.MarkScrapFound(first.Id);
            Assert.AreNotEqual(first.Id, s.NextScrap()?.Id);
            Assert.Contains(first.Id, s.ScrapsFound.ToList());
        }

        [Test]
        public void LedgerSlip_TiesTheNumbersToRollout_AndMentionsRestoredHouseholds()
        {
            string text = LedgerSlip.Text(2, 30f, 31);
            StringAssert.Contains("Households on supply: 340", text);
            StringAssert.Contains("31 households restored", text);
            StringAssert.DoesNotContain("restored", LedgerSlip.Text(2, 30f, 0));
            StringAssert.Contains("Appeals destroyed", LedgerSlip.Text(3, 30f, 0));
            StringAssert.DoesNotContain("Appeals destroyed", LedgerSlip.Text(2, 30f, 0));
        }

        [Test]
        public void LedgerText_OnlyFromTheRevealStage_AndNotOnTheFirstDay()
        {
            var clean = AtLevel(1);
            clean.BeginDay();
            Assert.IsNull(clean.LedgerText());

            var s = AtLevel(3);
            s.BeginDay();
            StringAssert.Contains("OVERNIGHT SUMMARY", s.LedgerText());

            var first = NewSession();
            first.ApplyStartProfile(new StartProfile { Level = 3, Day = 1 });
            first.BeginDay();
            Assert.IsNull(first.LedgerText(), "nothing has happened overnight on day 1");
        }

        [Test]
        public void ComplaintReplies_GoFlatAtTheLastStage_AndNeverRepeatBackToBack()
        {
            var bank = new ComplaintBank();
            var rng = new Random(5);
            string last = null;
            for (int i = 0; i < 60; i++)
            {
                string r = bank.PickReply(2, rng, ComplaintBank.FlatStage);
                StringAssert.DoesNotContain("frustrat", r);
                Assert.AreNotEqual(last, r);
                last = r;
            }
            Assert.GreaterOrEqual(ComplaintBank.FlatReplyCount, 5);
            StringAssert.Contains("frustrat", new ComplaintBank().PickReply(0, new Random(1), 0));
        }

        [Test]
        public void Wellness_ThreeTapesThenAnAbridgedOne()
        {
            var s = NewSession();
            var titles = new List<string>();
            var bodies = new HashSet<string>();
            for (int i = 0; i < 4; i++)
            {
                var (title, body) = s.NextWellness();
                titles.Add(title);
                bodies.Add(body);
            }
            Assert.AreEqual("MANDATORY WELLNESS MOMENT", titles[0]);
            Assert.AreEqual("WELLNESS MOMENT (ABRIDGED)", titles[3]);
            Assert.AreEqual(4, bodies.Count);
        }

        [Test]
        public void NewOddities_AreKeyedToStationsThatExist_AndFirstAppearAtSpecialist()
        {
            var areas = AreaCatalog.Default();
            foreach (var o in OddityBank.All) Assert.IsTrue(areas.StationIds.Contains(o.StationId), o.StationId);
            Assert.GreaterOrEqual(OddityBank.All.Count(o => o.MinRank >= 2), 10);
            Assert.AreEqual(0, OddityBank.Pick(0, new[] { "printer", "clipboard", "reader_power", "shredder" }, 3)
                .Count(kv => kv.Value.Contains("DENIED") || kv.Value.Contains("overnight")));
        }

        // ---- the second list ----

        [Test]
        public void Errands_Catalogue_IsConsistent()
        {
            var areas = AreaCatalog.Default();
            Assert.AreEqual(ErrandCatalog.All.Count, ErrandCatalog.All.Select(e => e.Id).Distinct().Count());
            foreach (var e in ErrandCatalog.All)
            {
                Assert.Less(e.RolloutEffect, 0f, e.Id);
                Assert.IsNotEmpty(e.Briefing, e.Id);
                foreach (var step in e.Steps) Assert.IsTrue(areas.StationIds.Contains(step.StationId), e.Id + " " + step.StationId);
                // a step that needs a carried item must be preceded by one that hands it over
                var held = new HashSet<string>();
                foreach (var step in e.Steps)
                {
                    if (step.RequiresCarry != null) Assert.IsTrue(held.Contains(step.RequiresCarry), e.Id + " needs " + step.RequiresCarry);
                    if (step.GivesCarry != null) held.Add(step.GivesCarry);
                }
            }
        }

        [Test]
        public void ErrandPlanner_OffersNoneBeforeSpecialist_OneAtSpecialist_TwoAfter_AndIsDeterministic()
        {
            Func<string, bool> open = _ => true;
            var none = new HashSet<string>();
            Assert.AreEqual(0, ErrandPlanner.Plan(0, 1, none, open).Count);
            Assert.AreEqual(0, ErrandPlanner.Plan(1, 1, none, open).Count);
            Assert.AreEqual(1, ErrandPlanner.Plan(2, 1, none, open).Count);
            Assert.AreEqual(2, ErrandPlanner.Plan(3, 1, none, open).Count);
            Assert.AreEqual(2, ErrandPlanner.Plan(4, 1, none, open).Count);

            var a = ErrandPlanner.Plan(4, 9, none, open).Select(e => e.Definition.Id).ToList();
            var b = ErrandPlanner.Plan(4, 9, none, open).Select(e => e.Definition.Id).ToList();
            CollectionAssert.AreEqual(a, b);
            Assert.AreEqual(0, ErrandPlanner.Plan(4, 9, none, _ => false).Count, "no errand whose rooms are closed");
        }

        [Test]
        public void ErrandPlanner_PrefersErrandsNotMetBefore()
        {
            Func<string, bool> open = _ => true;
            var seenAll = new HashSet<string>(ErrandCatalog.All.Where(e => e.Id != "copy_log").Select(e => e.Id));
            var plan = ErrandPlanner.Plan(4, 3, seenAll, open);
            Assert.AreEqual("copy_log", plan[0].Definition.Id);
        }

        [Test]
        public void Errand_RunsThroughItsStepsAtTheRightStations_WithoutTouchingTheOfficialJobs()
        {
            var s = AtLevel(3);
            s.BeginDay();
            var errand = s.TodaysErrands.Single();
            var official = s.TodaysTasks.Select(t => t.Status).ToList();

            while (!errand.IsDone)
            {
                var step = errand.CurrentStep;
                Assert.IsTrue(s.TryGetAlternateStep(step.StationId, out _, out _), step.Prompt);
                var result = s.CompleteAlternate(step.StationId);
                Assert.IsTrue(result.Accepted);
            }
            Assert.IsTrue(errand.IsDone);
            CollectionAssert.AreEqual(official, s.TodaysTasks.Select(t => t.Status).ToList());
            Assert.AreEqual(0, s.Carried.Count, "a carried errand item is used up by its last step");

            foreach (var t in s.TodaysTasks) FinishTask(s, t);
            var summary = s.EndDay();
            Assert.AreEqual(1, summary.ErrandsDone);
            Assert.AreEqual(1, summary.Stats.ErrandsStarted);
            Assert.AreEqual(1.06f + errand.Definition.RolloutEffect, summary.RolloutChange, 1e-3f);
        }

        [Test]
        public void Errand_NeedsItsCarriedItemFirst()
        {
            var s = AtLevel(3);
            s.BeginDay();
            s.OfferErrand("blank_notices");
            Assert.IsFalse(s.CompleteAlternate("printer").Accepted, "the blank stock has not been taken yet");
            Assert.IsTrue(s.CompleteAlternate("paper_stack").Accepted);
            CollectionAssert.Contains(s.Carried.ToList(), "blank");
            var done = s.CompleteAlternate("printer");
            Assert.IsTrue(done.Accepted);
            Assert.IsTrue(done.ErrandDone);
            CollectionAssert.DoesNotContain(s.Carried.ToList(), "blank");
        }

        [Test]
        public void Errand_WithAWindow_StartsAgainWhenTheWindowRunsOut()
        {
            var s = AtLevel(3);
            s.BeginDay();
            s.OfferErrand("jam_breakers");
            Assert.IsTrue(s.CompleteAlternate("breaker_corridor").Accepted);
            s.Tick(21f);
            var errand = s.TodaysErrands.First(e => e.Definition.Id == "jam_breakers");
            Assert.AreEqual(0, errand.StepIndex, "too slow, so it starts again");
            Assert.IsFalse(s.CompleteAlternate("breaker_admin").Accepted);

            Assert.IsTrue(s.CompleteAlternate("breaker_corridor").Accepted);
            s.Tick(10f);
            Assert.IsTrue(s.CompleteAlternate("breaker_admin").ErrandDone);
        }

        [Test]
        public void Errand_UnfinishedAtTheEndOfTheDay_CostsNothingAndDoesNotMoveRollout()
        {
            var s = AtLevel(3);
            s.BeginDay();
            s.OfferErrand("prop_vent");
            foreach (var t in s.TodaysTasks) FinishTask(s, t);
            float reliabilityBefore = s.Reliability.Value;
            var summary = s.EndDay();
            Assert.AreEqual(0, summary.ErrandsDone);
            Assert.AreEqual(1.06f, summary.RolloutChange, 1e-3f);
            Assert.GreaterOrEqual(s.Reliability.Value, reliabilityBefore);
        }

        [Test]
        public void Errand_CompletedOneDay_ShowsAsRestoredHouseholdsOnTheNextMorningSlip()
        {
            var s = AtLevel(3);
            s.BeginDay();
            s.OfferErrand("prop_vent");
            Assert.IsTrue(s.CompleteAlternate("vent").ErrandDone);
            foreach (var t in s.TodaysTasks) FinishTask(s, t);
            s.EndDay();
            s.BeginDay();
            StringAssert.Contains("households restored", s.LedgerText());
        }

        [Test]
        public void Alternate_AfterTheDayHasEnded_IsRejected()
        {
            var s = AtLevel(3);
            s.BeginDay();
            s.OfferErrand("prop_vent");
            s.EndDay();
            Assert.IsFalse(s.CompleteAlternate("vent").Accepted);
            Assert.IsFalse(s.CompleteStep("coffee_machine").Accepted);
        }
    }
}
