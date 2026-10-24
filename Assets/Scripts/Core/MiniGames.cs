using System;
using System.Collections.Generic;

namespace Noted.Core
{
    /// <summary>Time a wrong answer costs, in seconds. Shared by all the mini-games.</summary>
    public static class MiniGameRules
    {
        public const float WrongAnswerPenaltySeconds = 2f;
    }

    /// <summary>Sort the mail: each item goes in the right bin. A wrong bin keeps the item where it is.</summary>
    public sealed class SortGame
    {
        public static readonly string[] BinNames = { "FILE", "SHRED", "OUTBOX" };

        public sealed class Item
        {
            public readonly string Label;
            public readonly int Bin;
            public Item(string label, int bin) { Label = label; Bin = bin; }
        }

        static readonly Item[] Pool =
        {
            new Item("Invoice", 0), new Item("Timesheet", 0), new Item("Receipt", 0), new Item("Meeting notes", 0),
            new Item("Coupon book", 1), new Item("Flyer", 1), new Item("Old catalogue", 1), new Item("Chain letter", 1),
            new Item("Return slip", 2), new Item("Outgoing form", 2), new Item("Courier note", 2),
        };

        /// <summary>One envelope addressed in handwriting to "Operator". It is filed; nothing else about it is said.</summary>
        public static readonly Item Strange = new Item("Handwritten: \"For the Operator\"", 0);

        readonly List<Item> items = new List<Item>();
        int index;

        public int Total => items.Count;
        public int Sorted => index;
        public bool IsComplete => index >= items.Count;
        public Item Current => IsComplete ? null : items[index];

        public SortGame(Random rng, int count = 5, bool includeStrange = false)
        {
            var bag = new List<Item>(Pool);
            for (int i = bag.Count - 1; i > 0; i--)
            {
                int j = rng.Next(i + 1);
                (bag[i], bag[j]) = (bag[j], bag[i]);
            }
            for (int i = 0; i < count && i < bag.Count; i++) items.Add(bag[i]);
            if (includeStrange && items.Count > 0) items[rng.Next(items.Count)] = Strange;
        }

        /// <summary>Puts the current item in a bin. Returns true if it was the right bin.</summary>
        public bool Choose(int bin)
        {
            if (IsComplete) return false;
            if (items[index].Bin != bin) return false;
            index++;
            return true;
        }
    }

    /// <summary>Plug each cable into the port with its colour. The ports are in a shuffled order.</summary>
    public sealed class CableGame
    {
        public static readonly string[] Colors = { "RED", "BLUE", "GREEN", "YELLOW", "ORANGE", "WHITE" };

        readonly int[] portColor;     // portColor[port] = colour index the port wants
        readonly bool[] connected;    // by colour index

        public int Count { get; }
        public int Connected { get; private set; }
        public bool IsComplete => Connected >= Count;

        public CableGame(Random rng, int count = 4)
        {
            Count = Math.Max(2, Math.Min(count, Colors.Length));
            portColor = new int[Count];
            connected = new bool[Count];
            for (int i = 0; i < Count; i++) portColor[i] = i;
            for (int i = Count - 1; i > 0; i--)
            {
                int j = rng.Next(i + 1);
                (portColor[i], portColor[j]) = (portColor[j], portColor[i]);
            }
        }

        /// <summary>The colour index the given port wants.</summary>
        public int ColorOfPort(int port) => portColor[port];

        public bool IsCableConnected(int color) => connected[color];

        public bool IsPortConnected(int port) => connected[portColor[port]];

        /// <summary>Plugs a cable (by colour) into a port. Returns true when the colours match.</summary>
        public bool Plug(int color, int port)
        {
            if (color < 0 || color >= Count || port < 0 || port >= Count) return false;
            if (connected[color]) return false;
            if (portColor[port] != color) return false;
            connected[color] = true;
            Connected++;
            return true;
        }
    }

    /// <summary>Watch the lights blink, then repeat the pattern. A wrong pad starts the pattern again.</summary>
    public sealed class PatternGame
    {
        readonly int[] sequence;
        int progress;

        public int Pads { get; }
        public int Length => sequence.Length;
        public int Progress => progress;
        public bool IsComplete => progress >= sequence.Length;

        public PatternGame(Random rng, int length = 4, int pads = 4)
        {
            Pads = pads;
            sequence = new int[length];
            for (int i = 0; i < length; i++)
            {
                // Avoid the same pad three times in a row so the pattern is readable.
                int next;
                do { next = rng.Next(pads); }
                while (i >= 2 && sequence[i - 1] == next && sequence[i - 2] == next);
                sequence[i] = next;
            }
        }

        public int PadAt(int step) => sequence[step];

        /// <summary>Presses a pad. Returns true if it was the next one in the pattern; a wrong pad resets progress.</summary>
        public bool Press(int pad)
        {
            if (IsComplete) return false;
            if (sequence[progress] == pad)
            {
                progress++;
                return true;
            }
            progress = 0;
            return false;
        }
    }
}
