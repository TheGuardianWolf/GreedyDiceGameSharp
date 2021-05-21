using System;
using System.Collections.Generic;
using System.Linq;

namespace GreedyDiceGameSharp
{
    public class GameConfiguration
    {
        public int PointsToWin { get; }

        public GameConfiguration(int pointsToWin)
        {
            PointsToWin = pointsToWin;
        }
    }


    public class Game
    {
        public GameConfiguration Configuration { get; private set; }
        public event EventHandler<IGameError> GameError;
        protected IDictionary<string, Player> Players { get; set; } = new Dictionary<string, Player>();
        protected Board Board { get; set; }
        protected int CurrentPlayer { get; set; }
        protected int ConsecutiveRolls { get; set; }

        public Game() : this(new GameConfiguration(10000)) { }

        public Game(GameConfiguration configuration)
        {
            Configuration = configuration;
            Board = new Board(new DefaultRuleset());
        }

        public bool HasPlayers()
        {
            return Players.Count >= 2;
        }

        public void AddPlayers(string player1, string player2)
        {
            Players.Add(player1, new Player(player1));
            Players.Add(player2, new Player(player2));
            CurrentPlayer = 1;
        }

        private bool HasScoringDiceGuard(GameAction action)
        {
            if (!Board.GetDice().Any(x => x.Selected))
            {
                // No selected dice
                GameError?.Invoke(this, new NoDiceSelected(action));
                return false;
            }

            if (!Board.HasScoringDiceSelected())
            {
                // No selected dice with scores
                GameError?.Invoke(this, new NoScoringDiceSelected(action));
                return false;
            }

            return true;
        }

        public bool PerformStop()
        {
            if (ConsecutiveRolls == 0)
            {
                // Haven't rolled
                GameError?.Invoke(this, new DiceNotRolled(GameAction.Stop));
                return false;
            }

            if (!HasScoringDiceGuard(GameAction.Stop))
            {
                return false;
            }

            Board.UpdateScoreFromSelected();
            var player = GetCurrentPlayer();
            player.IncreaseScore(Board.GetScore());

            NextPlayer();

            return true;
        }

        public bool PerformRoll()
        {
            if (ConsecutiveRolls > 0)
            {
                if (!HasScoringDiceGuard(GameAction.Roll))
                {
                    return false;
                }

                Board.UpdateScoreFromSelected();
            }

            Board.RollDice();

            if (!Board.HasScoringDiceSelected())
            {
                GameError?.Invoke(this, new NoScoringDiceSelected(GameAction.Roll));

                // Rolled no scoring dice
                NextPlayer();

                return false;
            }

            ConsecutiveRolls += 1;

            return true;
        }

        public void PerformSelect(IEnumerable<int> selection)
        {
            if (ConsecutiveRolls == 0)
            {
                GameError?.Invoke(this, new DiceNotRolled(GameAction.Select));
                return;
            }

            var dice = Board.GetDice();
            foreach (var select in selection)
            {
                if (dice[select].Selected)
                {
                    Board.DeselectDice(select);
                }
                else
                {
                    Board.SelectDice(select);
                }
            }
        }

        public Player GetCurrentPlayer()
        {
            return Players.Values.Skip(CurrentPlayer - 1).First();
        }

        public IList<DiceValue> GetDiceValues()
        {
            return Board.GetDice();
        }

        public int GetBoardScore()
        {
            return Board.GetScore();
        }

        private void NextPlayer()
        {
            ConsecutiveRolls = 0;
            GetCurrentPlayer().IncreaseScore(GetBoardScore());
            Board.Reset();
            CurrentPlayer = Math.Max(1, (CurrentPlayer + 1) % 3);
        }
    }

    public interface IGameError
    {
        GameAction LastGameAction { get; }
    }

    public abstract class BaseGameError
    {
        public GameAction LastGameAction { get; }

        public BaseGameError(GameAction gameAction)
        {
            LastGameAction = gameAction;
        }
    }

    public class NoDiceSelected : IGameError
    {
        public GameAction LastGameAction { get; }

        public NoDiceSelected(GameAction gameAction)
        {
            LastGameAction = gameAction;
        }
    }

    public class NoScoringDiceSelected : IGameError 
    {
        public GameAction LastGameAction { get; }

        public NoScoringDiceSelected(GameAction gameAction)
        {
            LastGameAction = gameAction;
        }
    }

    public class DiceNotRolled : IGameError
    {
        public GameAction LastGameAction { get; }

        public DiceNotRolled(GameAction gameAction)
        {
            LastGameAction = gameAction;
        }
    }

}
