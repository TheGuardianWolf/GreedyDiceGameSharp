using GreedyDiceGameSharp.Web.Data;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GreedyDiceGameSharp.Web.Pages
{
    public partial class Index
    {
        [Inject] 
        private IMatchmakingService MatchmakingService { get; set; }
        [Inject] 
        private IGameService GameService { get; set; }
        [Inject] 
        private IAuthService AuthService { get; set; }

        public Guid PlayerId { get; set; }

        // Available after game start event
        public GameWrapper Game { get; set; }
        public PlayerWrapper Player { get; set; }
        public string GameMessage { get; set; }
        public string GameLog { get; set; } = "";

        public int GameScore => Game.Game.GetBoardScore();
        public int PlayerScore => Player.Player.GetScore();

        public bool IsPlayerTurn()
        {
            return Game.Game.GetCurrentPlayer().Name == Player.Player.Name;
        }

        public IList<DiceValue> DiceValues() => Game.Game.GetDiceValues();

        public string GetPlayerName()
        {
            if (Game.Player1.Id == Player.Id)
            {
                return Game.Player1.Player.Name;
            }
            else if (Game.Player2.Id == Player.Id)
            {
                return Game.Player2.Player.Name;
            }
            else
            {
                return null;
            }
        }

        protected override void OnInitialized()
        {
            var playerId = AuthService.UserId;

            PlayerId = playerId;

            if (GameService.CheckPlayerInGame(playerId, out var game))
            {
                ConnectGame(game);
            }
            else
            {
                GameService.GameCreated += GameService_GameCreated;

                if (!MatchmakingService.Contains(playerId))
                {
                    MatchmakingService.AddToQueue(playerId);
                }
            }
        }

        private void ConnectGame(GameWrapper game)
        {
            Player = GameService.GetPlayer(PlayerId);
            game.Game.GameEvent += Game_GameEvent;
            Game = game;

            InvokeAsync(() =>
            {
                StateHasChanged();
            });
        }

        private void GameService_GameCreated(object sender, GameCreatedEvent e)
        {
            var game = GameService.GetGame(e.GameId);
            ConnectGame(game);
        }

        private void Game_GameEvent(object sender, IGameEvent e)
        {
            if (IsPlayerTurn())
            {
                switch (e.EventType)
                {
                    case GameEventType.NoDiceSelected:
                        GameMessage = "No dice selected";
                        break;
                    case GameEventType.NoScoringDiceSelected:
                        GameMessage = "No scoring dice selected";
                        break;
                    case GameEventType.DiceNotRolled:
                        GameMessage = "Dice not rolled";
                        break;
                }
            }

            switch (e.EventType)
            {
                case GameEventType.NextPlayer:
                    GameLog += $"It is now {e.PlayerName}'s turn\n";
                    break;
                case GameEventType.DiceRolled:
                    GameLog += $"Player {e.PlayerName} has rolled the dice\n";
                    break;
                case GameEventType.DiceSelectChanged:
                    GameLog += $"Player {e.PlayerName} has changed dice selection\n";
                    break;
                case GameEventType.DiceStopped:
                    GameLog += $"Player {e.PlayerName} has stopped\n";
                    break;
            }

            InvokeAsync(() =>
            {
                StateHasChanged();
            });
        }

        private void Button_OnRoll()
        {
            var game = Game.Game;

            if (IsPlayerTurn())
            {
                game.PerformRoll();
            }
        }

        private void Button_OnStop()
        {
            var game = Game.Game;

            if (IsPlayerTurn())
            {
                game.PerformStop();
            }
        }

        private void Dice_OnClick(int diceNumber)
        {
            if (IsPlayerTurn())
            {
                Game.Game.PerformSelect(new List<int> { diceNumber });
            }
        }
    }
}
