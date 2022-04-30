using System;
using System.Collections.Generic;
using System.Linq;

namespace GreedyDiceGameSharp
{
    public enum GameAction
    {
        Stop,
        Roll,
        Select
    }

    public interface IPlayerInput
    {
        string PlayerName { get; }
    }

    public class StopPlayerInput : IPlayerInput
    {
        public string PlayerName { get; }

        public StopPlayerInput(string player)
        {
            PlayerName = player;
        }
    }

    public class RollPlayerInput : IPlayerInput
    {
        public string PlayerName { get; }

        public RollPlayerInput(string player)
        {
            PlayerName = player;
        }
    }

    public class SelectPlayerInput : IPlayerInput
    {
        public string PlayerName { get; }
        public IEnumerable<int> DiceNumbers { get; }

        public SelectPlayerInput(string player, IEnumerable<int> diceNumbers)
        {
            PlayerName = player;
            DiceNumbers = diceNumbers;
        }
    }

    public class GameManager
    {
        public Queue<IPlayerInput> PlayerInputQueue { get; } = new Queue<IPlayerInput>();
        //public event EventHandler<IPlayerInput> PlayerInput;
        public event EventHandler<IGameEvent> GameEvent;
        protected Game Game { get; set; }

        public GameManager()
        {
            Game = new Game();

            Game.GameEvent += Game_GameEvent;
        }

        private void Game_GameEvent(object sender, IGameEvent e)
        {
            GameEvent?.Invoke(this, e);
        }

        public void Configure(GameConfiguration configuration)
        {
            Game = new Game(configuration);
        }

        public void Loop()
        {
            for (var i = 0; i < PlayerInputQueue.Count; i++)
            {
                var input = PlayerInputQueue.Dequeue();
                if (Game.GetCurrentPlayer().Name == input.PlayerName)
                {
                    switch (input)
                    {
                        case RollPlayerInput t:
                            Game.PerformRoll();
                            break;
                        case SelectPlayerInput t:
                            Game.PerformSelect(t.DiceNumbers);
                            break;
                        case StopPlayerInput t:
                            Game.PerformStop();
                            break;
                    }
                }
            }
        }

        public void AddPlayers(string player1, string player2) => Game.AddPlayers(player1, player2);

        public void AddInput(IPlayerInput playerInput)
        {
            PlayerInputQueue.Enqueue(playerInput);
            //PlayerInput?.Invoke(this, playerInput);
        }

        public Player GetCurrentPlayer() => Game?.GetCurrentPlayer();

        public IList<DiceValue> GetDiceValues() => Game?.GetDiceValues();

        public int GetBoardScore() => Game?.GetBoardScore() ?? 0;
    }
}
