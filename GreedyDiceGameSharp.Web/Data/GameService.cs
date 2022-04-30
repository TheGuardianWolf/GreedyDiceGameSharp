using RandomNameGeneratorLibrary;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GreedyDiceGameSharp.Web.Data
{
    public interface IGameService
    {
        IDictionary<Guid, GameWrapper> GameStore { get; set; }
        IDictionary<Guid, PlayerWrapper> PlayerStore { get; set; }

        event EventHandler<GameCreatedEvent> GameCreated;
        event EventHandler<GameEndedEvent> GameEnded;

        GameWrapper GetGame(Guid gameId);
        PlayerWrapper GetPlayer(Guid playerId);

        Guid CreateNewGame(Guid player1Id, Guid player2Id);
        bool CheckPlayerInGame(Guid playerId, out GameWrapper Game);
        void EndGame(Guid gameId);
    }

    /// <summary>
    /// Holds game objects
    /// </summary>
    public class GameService : IGameService
    {
        public event EventHandler<GameCreatedEvent> GameCreated;
        public event EventHandler<GameEndedEvent> GameEnded;
        public IDictionary<Guid, GameWrapper> GameStore { get; set; } = new ConcurrentDictionary<Guid, GameWrapper>();
        public IDictionary<Guid, PlayerWrapper> PlayerStore { get; set; } = new ConcurrentDictionary<Guid, PlayerWrapper>();

        public Guid CreateNewGame(Guid player1Id, Guid player2Id)
        {
            Guid gameId;
            var retries = 0;
            do
            {
                if (retries < 3)
                {
                    gameId = Guid.NewGuid();
                    retries += 1;
                }
                else
                {
                    throw new InvalidOperationException("Could not generate valid Guid within 3 tries");
                }
            } while (GameStore.ContainsKey(gameId));

            var personGenerator = new PersonNameGenerator();
            var names = personGenerator.GenerateMultipleFirstAndLastNames(2);

            var player1 = new PlayerWrapper
            {
                Player = new Player(names.First()),
                Id = player1Id
            };

            var player2 = new PlayerWrapper
            {
                Player = new Player(names.Last()),
                Id = player2Id
            };

            var game = new GameWrapper
            {
                Id = gameId,
                Game = new Game(),
                Player1 = player1,
                Player2 = player2
            };

            GameStore.Add(gameId, game);
            PlayerStore.Add(player1.Id, player1);
            PlayerStore.Add(player2.Id, player2);

            game.Game.AddPlayers(player1.Player.Name, player2.Player.Name);

            GameCreated?.Invoke(this, new GameCreatedEvent(gameId, player1, player2));

            return gameId;
        }

        public GameWrapper GetGame(Guid gameId)
        {
            return GameStore[gameId];
        }

        public PlayerWrapper GetPlayer(Guid playerId)
        {
            return PlayerStore[playerId];
        }

        public void EndGame(Guid gameId)
        {
            var gameWrapper = GameStore[gameId];
            GameStore.Remove(gameId);

            GameEnded?.Invoke(this, new GameEndedEvent(gameId, gameWrapper.Player1, gameWrapper.Player2));
        }

        public bool CheckPlayerInGame(Guid playerId, out GameWrapper Game)
        {
            Game = null;
            var game = GameStore.Values.FirstOrDefault(x => x.Player1.Id == playerId || x.Player2.Id == playerId);

            if (game == null)
            {
                return false;
            }

            Game = game;

            return true;
        }
    }
}
