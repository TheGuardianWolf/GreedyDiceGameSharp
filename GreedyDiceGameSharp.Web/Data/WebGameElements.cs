using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GreedyDiceGameSharp.Web.Data
{
    public class GameWrapper
    {
        public Guid Id { get; set; }
        public Game Game { get; set; }
        public PlayerWrapper Player1 { get; set; }
        public PlayerWrapper Player2 { get; set; }
    }

    public class PlayerWrapper
    {
        public Guid Id { get; set; }
        public Player Player { get; set; }
    }
}
