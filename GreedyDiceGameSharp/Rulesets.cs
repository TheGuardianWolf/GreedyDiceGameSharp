using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GreedyDiceGameSharp
{
    public interface IRuleset
    {
        int Score(IList<int> values);
    }

    public class DefaultRuleset : IRuleset
    {
        protected IList<Rule> Rules { get; }

        public DefaultRuleset()
        {
            Rules = new List<Rule>
            {
                new Rule("Straight", StraightRule),
                new Rule("Partial Straight", PartialStraightRule),
                new Rule("Three Pairs", ThreePairsRule),
                new Rule("Three or More of a Kind", ThreeOrMoreOfAKindRule),
                new Rule("Ones", OnesRule),
                new Rule("Fives", FivesRule)
            };
        }

        public int Score(IList<int> values)
        {
            var counter = new Counter<int>(values.Select(x => x));

            int score = 0;

            foreach (var rule in Rules)
            {
                score += rule.Score(counter);
            }

            return score;
        }

        protected int StraightRule(Counter<int> counter)
        {
            for (var i = 1; i <= 6; i++)
            {
                if (counter.GetCount(i) == 0)
                {
                    return 0;
                }
            }

            for (var i = 1; i <= 6; i++)
            {
                counter.SetCount(i, 0);
            }

            return 1800;
        }

        protected int PartialStraightRule(Counter<int> counter)
        {
            for (var k = 0; k < 2; k++)
            {
                bool matched = true;

                for (var i = 1 + k; i <= (5 + k); i++)
                {
                    if (counter.GetCount(i) == 0)
                    {
                        matched = false;
                        break;
                    }
                }

                if (matched)
                {
                    for (var i = 1 + k; i <= (5 + k); i++)
                    {
                        counter.SetCount(i, 0);
                    }

                    return 900;
                }
            }

            return 0;
        }

        protected int ThreePairsRule(Counter<int> counter)
        {
            List<int> pairs = new List<int>();
            var counts = counter.GetCounts();
            foreach (var kvp in counts)
            {
                var count = kvp.Value;
                if (count == 2)
                {
                    pairs.Add(kvp.Key);
                }
            }

            if (pairs.Count >= 3)
            {
                foreach (var key in pairs)
                {
                    counter.SetCount(key, 0);
                }

                return 1000;
            }

            return 0;
        }

        protected int ThreeOrMoreOfAKindRule(Counter<int> counter)
        {
            int score = 0;
            foreach (var kvp in counter.GetCounts())
            {
                var count = kvp.Value;
                if (count >= 3)
                {
                    var number = (int)kvp.Key;
                    var multiplier = (int)Math.Pow(2, count - 3);

                    if (number == 1)
                    {
                        score += number * 1000 * multiplier;
                    }
                    else
                    {
                        score += number * 100 * multiplier;
                    }

                    counter.SetCount(kvp.Key, 0);
                }
            }
            return score;
        }

        protected int OnesRule(Counter<int> counter)
        {
            var oneCount = counter.GetCount(1);

            counter.SetCount(1, 0);

            return oneCount * 100;
        }

        protected int FivesRule(Counter<int> counter)
        {
            var fiveCount = counter.GetCount(5);

            counter.SetCount(5, 0);

            return fiveCount * 50;
        }
    }

    public class RuleMatch
    {
        public int MatchCount { get; }
        public string Match { get; }

        public RuleMatch(int matchCount, string match)
        {
            MatchCount = matchCount;
            Match = match;
        }
    }

    public class Rule
    {
        public string Name { get; }
        protected Func<Counter<int>, int> Scorer { get; }

        public Rule(string name, Func<Counter<int>, int> scorer)
        {
            Name = name;
            Scorer = scorer;
        }

        public int Score(Counter<int> counter)
        {
            return Scorer(counter);
        }
    }
}
