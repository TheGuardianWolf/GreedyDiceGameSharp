using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace GreedyDiceGameSharp.Test
{
    public class GameManagerTests
    {
        [Fact]
        public void CanTakeTurns()
        {
            var gameManager = new GameManager();

            gameManager.AddPlayers("P1", "P2");

            gameManager.AddInput(new RollPlayerInput("P1"));

            gameManager.Loop();
        }
    }
}
