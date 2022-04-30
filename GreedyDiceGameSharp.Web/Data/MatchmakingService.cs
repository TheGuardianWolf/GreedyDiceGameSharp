using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GreedyDiceGameSharp.Web.Data
{
    public interface IMatchmakingService
    {
        bool AddToQueue(Guid playerId);
        bool Contains(Guid playerId);
        bool RemoveFromQueue(Guid playerId);
    }

    /// <summary>
    /// Holds connections until there is a match
    /// </summary>
    public class MatchmakingService : IMatchmakingService
    {
        private event EventHandler PlayerAdded;
        private readonly IGameService _gameService;
        private PlayerWaitingQueue PlayerWaitingQueue = new PlayerWaitingQueue();

        public MatchmakingService(IGameService gameService)
        {
            _gameService = gameService;
            PlayerAdded += MatchmakingService_PlayerAdded;
        }

        private void MatchmakingService_PlayerAdded(object sender, EventArgs e)
        {
            if (PlayerWaitingQueue.Count > 1)
            {
                if (PlayerWaitingQueue.GetPlayers(out var player1Id, out var player2Id))
                {
                    _gameService.CreateNewGame(player1Id, player2Id);
                }
            }
        }

        public bool AddToQueue(Guid playerId)
        {
            var success = PlayerWaitingQueue.AddToQueue(playerId);

            if (success)
            {
                PlayerAdded?.Invoke(this, new EventArgs());
            }

            return success;
        }

        public bool Contains(Guid playerId) => PlayerWaitingQueue.Contains(playerId);

        public bool RemoveFromQueue(Guid playerId) => PlayerWaitingQueue.RemoveFromQueue(playerId);
    }
}
