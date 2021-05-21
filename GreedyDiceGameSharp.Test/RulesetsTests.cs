using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Xunit;

namespace GreedyDiceGameSharp.Test
{
    public class RulesetsTests
    {
        [Fact]
        public void ScoreStraight()
        {
            var ruleset = new DefaultRuleset();

            var diceValues = new List<int> { 1, 2, 3, 4, 5, 6 };

            var score = ruleset.Score(diceValues);

            Assert.Equal(1800, score);
        }

        [Fact]
        public void ScorePartialStraight()
        {
            var ruleset = new DefaultRuleset();

            var diceValues1 = new List<int> { 4, 3, 1, 2, 5, 5 };
            var diceValues2 = new List<int> { 4, 3, 6, 2, 5, 5 };

            var score1 = ruleset.Score(diceValues1);
            var score2 = ruleset.Score(diceValues2);

            Assert.Equal(900, score1);
            Assert.Equal(900, score2);
        }

        [Fact]
        public void ScoreThreePairs()
        {
            var ruleset = new DefaultRuleset();

            var diceValues = new List<int> { 1, 5, 6, 1, 6, 5 };

            var score = ruleset.Score(diceValues);

            Assert.Equal(1000, score);
        }

        [Theory]
        [InlineData(new int[] { 3, 3, 3, 2, 2, 4 }, 300)]
        [InlineData(new int[] { 3, 3, 1, 1, 4, 1 }, 1000)]
        public void ScoreThreeOrMoreOfAKind(int[] diceValues, int expectedScore)
        {
            var ruleset = new DefaultRuleset();

            var score = ruleset.Score(diceValues);

            Assert.Equal(expectedScore, score);
        }

        [Fact]
        public void ScoreOnes()
        {
            var ruleset = new DefaultRuleset();

            var diceValues = new List<int> { 1, 1, 2, 3, 4, 6 };

            var score = ruleset.Score(diceValues);

            Assert.Equal(200, score);
        }

        [Fact]
        public void ScoreFives()
        {
            var ruleset = new DefaultRuleset();

            var diceValues = new List<int> { 4, 2, 2, 3, 4, 5 };

            var score = ruleset.Score(diceValues);

            Assert.Equal(50, score);
        }

        [Fact]
        public void ScoreNothing()
        {
            var ruleset = new DefaultRuleset();

            var diceValues = new List<int> { 2, 2, 3, 3, 4, 6 };

            var score = ruleset.Score(diceValues);

            Assert.Equal(0, score);
        }

        [Theory]
        [InlineData(new int[] { 3, 3, 3, 2, 2, 2 }, 500)]
        [InlineData(new int[] { 3, 5, 1, 1, 5, 1 }, 1100)]
        [InlineData(new int[] { 3, 5, 1, 1, 1, 1 }, 2050)]
        public void ScoreCombo(int[] diceValues, int expectedScore)
        {
            var ruleset = new DefaultRuleset();

            var score = ruleset.Score(diceValues);

            Assert.Equal(expectedScore, score);
        }
    }
}
