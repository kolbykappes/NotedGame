using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace Noted.Core.Tests
{
    public class EconomyTests
    {
        [Test]
        public void Wallet_SpendsOnlyWhatItHas()
        {
            var w = new Wallet();
            w.Add(30);
            Assert.IsFalse(w.TrySpend(31));
            Assert.AreEqual(30, w.Balance);
            Assert.IsTrue(w.TrySpend(30));
            Assert.AreEqual(0, w.Balance);
            w.Add(-5);
            Assert.AreEqual(0, w.Balance);
        }

        [Test]
        public void RankTracker_RanksUpAtEachThresholdAndOnlyOnce()
        {
            var r = new RankTracker(new[] { 5, 12 });
            Assert.IsFalse(r.Record(4));
            Assert.AreEqual(0, r.Rank);
            Assert.IsTrue(r.Record(1));
            Assert.AreEqual(1, r.Rank);
            Assert.IsFalse(r.Record(2));
            Assert.AreEqual(12, r.NextThreshold);
            Assert.IsTrue(r.Record(5));
            Assert.AreEqual(2, r.Rank);
            Assert.AreEqual(-1, r.NextThreshold);
            Assert.IsFalse(r.Record(10));
        }

        [Test]
        public void RankTracker_CanSkipARankInOneDay()
        {
            var r = new RankTracker(new[] { 5, 12 });
            Assert.IsTrue(r.Record(13));
            Assert.AreEqual(2, r.Rank);
        }

        [Test]
        public void Upgrades_CostGrowsPerLevelAndStopsAtMax()
        {
            var cat = UpgradeCatalog.Default();
            var ledger = new UpgradeLedger(cat);
            var w = new Wallet();
            w.Add(10000);
            var def = cat.Find("longer_day");
            Assert.AreEqual(40, ledger.CostFor(def));
            Assert.IsTrue(ledger.TryBuy(w, def));
            Assert.AreEqual(64, ledger.CostFor(def));
            Assert.IsTrue(ledger.TryBuy(w, def));
            Assert.IsTrue(ledger.TryBuy(w, def));
            Assert.IsTrue(ledger.IsMaxed(def));
            Assert.AreEqual(-1, ledger.CostFor(def));
            Assert.IsFalse(ledger.TryBuy(w, def));
        }

        [Test]
        public void Upgrades_CannotBeBoughtWithoutTheCoins()
        {
            var cat = UpgradeCatalog.Default();
            var ledger = new UpgradeLedger(cat);
            var w = new Wallet();
            w.Add(10);
            Assert.IsFalse(ledger.TryBuy(w, cat.Find("faster_hands")));
            Assert.AreEqual(10, w.Balance);
            Assert.AreEqual(0, ledger.Level("faster_hands"));
        }

        [Test]
        public void Modifiers_AddUpAndRankTwoGivesTheShoes()
        {
            var cfg = new GameConfig();
            var cat = UpgradeCatalog.Default();
            var ledger = new UpgradeLedger(cat);
            var w = new Wallet();
            w.Add(10000);
            ledger.TryBuy(w, cat.Find("faster_hands"));
            ledger.TryBuy(w, cat.Find("long_reach"));
            ledger.TryBuy(w, cat.Find("coin_magnet"));
            ledger.TryBuy(w, cat.Find("task_marker"));

            var m0 = ledger.Compute(cfg, 0);
            Assert.AreEqual(0.85f, m0.HoldMultiplier, 1e-4f);
            Assert.AreEqual(1f, m0.ReachBonus, 1e-4f);
            Assert.AreEqual(2.5f, m0.MagnetRadius, 1e-4f);
            Assert.IsTrue(m0.MarkerEnabled);
            Assert.IsFalse(m0.BriefingEnabled);
            Assert.AreEqual(1f, m0.SpeedMultiplier, 1e-4f);

            var m2 = ledger.Compute(cfg, 2);
            Assert.AreEqual(1.08f, m2.SpeedMultiplier, 1e-4f);
        }

        [Test]
        public void ComplaintBank_NeverRepeatsTheSameReplyBackToBack()
        {
            var bank = new ComplaintBank();
            var rng = new Random(1);
            for (int rank = 0; rank <= 2; rank++)
            {
                string last = null;
                for (int i = 0; i < 200; i++)
                {
                    string r = bank.PickReply(rank, rng);
                    Assert.AreNotEqual(last, r);
                    Assert.IsTrue(r.Contains("Noted") || r.Contains("frustration"));
                    last = r;
                }
            }
        }

        [Test]
        public void ComplaintBank_EveryReplyIsAVariationOnTheNonAnswer()
        {
            var bank = new ComplaintBank();
            var rng = new Random(2);
            var seen = new HashSet<string>();
            for (int i = 0; i < 300; i++) seen.Add(bank.PickReply(0, rng));
            Assert.AreEqual(ComplaintBank.ReplyCount(0), seen.Count);
            foreach (var r in seen) StringAssert.Contains("frustrat", r);
        }

        [Test]
        public void CodeGenerator_MakesSixDigitsAndAvoidsTheFamousSequence()
        {
            for (int seed = 0; seed < 500; seed++)
            {
                string code = CodeGenerator.Generate(seed);
                Assert.AreEqual(6, code.Length);
                Assert.IsTrue(code.TrimStart('0', '1', '2', '3', '4', '5', '6', '7', '8', '9').Length == 0);
                Assert.AreNotEqual("481516", code);
                Assert.AreEqual(code, CodeGenerator.Generate(seed), "deterministic");
            }
            Assert.AreEqual("1 2 3 4 5 6", CodeGenerator.Spaced("123456"));
        }

        [Test]
        public void CardText_IntroNeverSaysWhatHappensOnFailure()
        {
            var cfg = new GameConfig();
            string text = CardText.Intro(cfg, "123456");
            StringAssert.Contains("1 2 3 4 5 6", text);
            StringAssert.Contains("The Solace Initiative", text);
            StringAssert.Contains("Trainee", text);
            StringAssert.Contains("We do not discuss what happens", text);
        }
    }
}
