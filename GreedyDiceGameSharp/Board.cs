using System.Collections.Generic;
using System.Linq;

namespace GreedyDiceGameSharp
{
    public class Board
    {
        protected IList<Dice> Dice { get; set; } = new List<Dice>();
        protected ScoreTracker ScoreTracker { get; set; }

        public Board(IRuleset ruleset = null)
        {
            for (int i = 1; i <= 6; i++)
            {
                Dice.Add(new Dice());
            }
            
            ScoreTracker = new ScoreTracker(ruleset ?? new DefaultRuleset());
        }

        public IList<DiceValue> GetDice()
        {
            return Dice.Select(x => new DiceValue(x.Face, x.Locked, x.Selected)).ToList();
        }

        public void SelectDice(int diceNumber)
        {
            var dice = Dice[diceNumber];

            if (!dice.Locked)
            {
                dice.Select();
            }
        }

        public void DeselectDice(int diceNumber)
        {
            var dice = Dice[diceNumber];

            if (!dice.Locked)
            {
                dice.Deselect();
            }
        }

        public void RollDice()
        {
            foreach (var dice in Dice)
            {
                if (dice.Selected)
                {
                    dice.Lock();
                    dice.Deselect();
                }
                else if (!dice.Locked)
                {
                    dice.Roll();
                }
            }
        }

        public void UpdateScoreFromSelected()
        {
            ScoreTracker.AddScoringDice(GetDice().Where(x => x.Selected).ToList());
        }

        public bool HasScoringDice()
        {
            return ScoreTracker.HasScoringDice(GetDice().Where(x => !x.Locked).ToList());
        }

        public bool HasScoringDiceSelected()
        {
            return ScoreTracker.HasScoringDice(GetDice().Where(x => x.Selected).ToList());
        }

        public int GetScore()
        {
            return ScoreTracker.GetScore();
        }

        public void Reset()
        {
            foreach (var dice in Dice)
            {
                dice.Unlock();
                dice.Deselect();
            }

            ScoreTracker.Reset();
        }
    }
}
