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
        public event EventHandler<IGameEvent> GameEvent;
        public GameConfiguration Configuration { get; private set; }
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

            GameEvent?.Invoke(this, new GameEvent(player1, GameEventType.PlayersAdded, null));
            GameEvent?.Invoke(this, new GameEvent(player2, GameEventType.PlayersAdded, null));
        }
        private bool HasScoringDiceGuard(GameAction action)
        {
            var playerName = GetCurrentPlayer().Name;

            if (!Board.GetDice().Any(x => x.Selected))
            {
                // No selected dice
                GameEvent?.Invoke(this, new GameEvent(playerName, GameEventType.NoDiceSelected, action));
                return false;
            }

            if (!Board.HasScoringDiceSelected())
            {
                // No selected dice with scores
                GameEvent?.Invoke(this, new GameEvent(playerName, GameEventType.NoScoringDiceSelected, action));
                return false;
            }

            return true;
        }

        public bool PerformStop()
        {
            var player = GetCurrentPlayer();

            if (ConsecutiveRolls == 0)
            {
                // Haven't rolled
                GameEvent?.Invoke(this, new GameEvent(player.Name, GameEventType.DiceNotRolled, GameAction.Stop));
                return false;
            }

            if (!HasScoringDiceGuard(GameAction.Stop))
            {
                return false;
            }

            Board.UpdateScoreFromSelected();
            player.IncreaseScore(Board.GetScore());
            GameEvent?.Invoke(this, new GameEvent(player.Name, GameEventType.DiceStopped, GameAction.Stop));

            NextPlayer(GameAction.Stop);

            return true;
        }

        public bool PerformRoll()
        {
            var player = GetCurrentPlayer();

            if (ConsecutiveRolls > 0)
            {
                if (!HasScoringDiceGuard(GameAction.Roll))
                {
                    return false;
                }

                Board.UpdateScoreFromSelected();
            }

            Board.RollDice();
            GameEvent?.Invoke(this, new GameEvent(player.Name, GameEventType.DiceRolled, GameAction.Roll));

            if (ConsecutiveRolls > 0 && !Board.HasScoringDice())
            {
                GameEvent?.Invoke(this, new GameEvent(player.Name, GameEventType.NoScoringDiceSelected, GameAction.Roll));

                // Rolled no scoring dice
                NextPlayer(GameAction.Roll);

                return false;
            }

            ConsecutiveRolls += 1;

            return true;
        }

        public void PerformSelect(IEnumerable<int> selection)
        {
            var player = GetCurrentPlayer();

            if (ConsecutiveRolls == 0)
            {
                GameEvent?.Invoke(this, new GameEvent(player.Name, GameEventType.DiceNotRolled, GameAction.Select));
                return;
            }

            var dice = Board.GetDice();
            foreach (var select in selection)
            {
                if (dice[select].Selected)
                {
                    Board.DeselectDice(select);
                    GameEvent?.Invoke(this, new GameEvent(player.Name, GameEventType.DiceSelectChanged, GameAction.Select));
                }
                else
                {
                    Board.SelectDice(select);
                    GameEvent?.Invoke(this, new GameEvent(player.Name, GameEventType.DiceSelectChanged, GameAction.Select));
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

        private void NextPlayer(GameAction action)
        {
            var player = GetCurrentPlayer();
            ConsecutiveRolls = 0;
            player.IncreaseScore(GetBoardScore());
            Board.Reset();
            CurrentPlayer = Math.Max(1, (CurrentPlayer + 1) % 3);

            var newPlayer = GetCurrentPlayer();
            GameEvent?.Invoke(this, new GameEvent(newPlayer.Name, GameEventType.NextPlayer, action));
        }
    }

    public enum GameEventType
    {
        // Errors
        DiceNotRolled,
        NoScoringDiceSelected,
        NoDiceSelected,

        // Notices
        NextPlayer,
        PlayersAdded,
        DiceRolled,
        DiceSelectChanged,
        DiceStopped
    }

    public interface IGameEvent
    {
        public string PlayerName { get; }
        public GameEventType EventType { get; }
        public GameAction? LastAction { get; }
    }

    public class GameEvent : IGameEvent
    {
        public string PlayerName { get; }
        public GameEventType EventType { get; }
        public GameAction? LastAction { get; }

        public GameEvent(string player, GameEventType eventType, GameAction? action)
        {
            PlayerName = player;
            EventType = eventType;
            LastAction = action;
        }
    }
}
