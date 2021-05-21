using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GreedyDiceGameSharp.Web.Data
{
    public abstract class BaseGameEvent : EventArgs
    {
        public Guid GameId { get; }
        public PlayerWrapper Player1 { get; }
        public PlayerWrapper Player2 { get; }

        public BaseGameEvent(Guid gameId, PlayerWrapper player1, PlayerWrapper player2)
        {
            GameId = gameId;
            Player1 = player1;
            Player2 = player2;
        }
    }

    public class GameCreatedEvent : BaseGameEvent
    {
        public GameCreatedEvent(Guid gameId, PlayerWrapper player1, PlayerWrapper player2) : base(gameId, player1, player2)
        {
        }
    }

    public class GameEndedEvent : BaseGameEvent
    {
        public GameEndedEvent(Guid gameId, PlayerWrapper player1, PlayerWrapper player2) : base(gameId, player1, player2)
        {
        }
    }
}
