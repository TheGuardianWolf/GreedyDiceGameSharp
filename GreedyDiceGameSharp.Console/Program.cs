using Sharprompt;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GreedyDiceGameSharp.CommandLine
{
    enum DiceNumber
    {
        Dice1,
        Dice2,
        Dice3,
        Dice4,
        Dice5,
        Dice6
    }
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Greedy Dice Game!");

            Console.WriteLine("Press ENTER to start");

            Console.ReadLine();

            var game = new GameManager();

            var p1Name = Prompt.Input<string>("Please enter name for Player 1");
            var p2Name = Prompt.Input<string>("Please enter name for Player 2");

            game.AddPlayers(p1Name, p2Name);

            game.GameEvent += Game_GameEvent;

            var loopCount = 0;
            while(true)
            {
                loopCount += 1;
                var currentPlayer = game.GetCurrentPlayer();
                Console.WriteLine($"{loopCount} - Current Player: {currentPlayer.Name} - {currentPlayer.GetScore()} points");
                Console.WriteLine($"{loopCount} - Dice Values: {DisplayDice(game.GetDiceValues())}");

                var action = Prompt.Select<GameAction>(@"Select an action to perform");

                IPlayerInput input;
                switch (action)
                {
                    case GameAction.Roll:
                        input = new RollPlayerInput(currentPlayer.Name);
                        break;
                    case GameAction.Select:
                        var dice = (int)Prompt.Select<DiceNumber>("Select a dice number");
                        input = new SelectPlayerInput(currentPlayer.Name, new List<int> { dice });
                        break;
                    default:
                    case GameAction.Stop:
                        input = new StopPlayerInput(currentPlayer.Name);
                        break;
                }

                game.AddInput(input);
                game.Loop();


                Console.WriteLine($"{loopCount} - Points on board: {game.GetBoardScore()}");
            }
        }

        private static void Game_GameEvent(object sender, IGameEvent e)
        {
            Console.WriteLine($"Event encountered: {e.EventType} with last action {e.LastAction} from player {e.PlayerName}");
        }

        static string DisplayDice(IEnumerable<DiceValue> diceValue)
        {
            return string.Join(", ", diceValue.Select(x => {
                if (x.Locked) 
                {
                    return $"{x.Face} - Locked";
                }
                else if (x.Selected)
                {
                    return $"{x.Face} - Selected";
                }

                return x.Face.ToString();
            }));
        }
    }
}
