using System.Collections.Generic;
using System.Linq;

namespace GreedyDiceGameSharp
{
    public class ScoreTracker
    {
        protected Score Score { get; set; } = new Score(0);
        protected IRuleset Ruleset { get; set; }

        public ScoreTracker(IRuleset ruleset)
        {
            Ruleset = ruleset;
        }

        public bool HasScoringDice(IList<DiceValue> diceValues)
        {
            var values = diceValues.Select(x => x.Face).ToList();

            var score = Ruleset.Score(values);

            if (score > 0)
            {
                return true;
            }

            return false;
        }

        public bool AddScoringDice(IList<DiceValue> diceValues)
        {
            var values = diceValues.Select(x => x.Face).ToList();

            var score = Ruleset.Score(values);

            Score = new Score(Score.Value + score);

            if (score > 0)
            {
                return true;
            }

            return false;
        }

        public int GetScore()
        {
            return Score.Value;
        }

        public void Reset()
        {
            Score = new Score(0);
        }
    }
}
