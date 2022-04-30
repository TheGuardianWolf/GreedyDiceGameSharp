using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GreedyDiceGameSharp.Web.Data
{

    public class PlayerWaitingQueue
    {
        private object _monitorTarget = new object();
        private LinkedList<Guid> WaitingQueue { get; set; } = new LinkedList<Guid>();

        public int Count => WaitingQueue.Count;

        public bool AddToQueue(Guid id)
        {
            var entered = Monitor.TryEnter(_monitorTarget, 100);
            if (!entered)
            {
                return false;
            }

            try
            {
                WaitingQueue.AddLast(id);
            }
            finally
            {
                Monitor.Exit(_monitorTarget);
            }

            return true;
        }

        public bool Contains(Guid id)
        {
            var entered = Monitor.TryEnter(_monitorTarget, 100);
            if (!entered)
            {
                return false;
            }

            try
            {
                return WaitingQueue.Contains(id);
            }
            finally
            {
                Monitor.Exit(_monitorTarget);
            }
        }

        public bool RemoveFromQueue(Guid id)
        {
            var entered = Monitor.TryEnter(_monitorTarget, 100);
            if (!entered)
            {
                return false;
            }

            try
            {
                return WaitingQueue.Remove(id);
            }
            finally
            {
                Monitor.Exit(_monitorTarget);
            }
        }

        public bool GetPlayers(out Guid player1Id, out Guid player2Id)
        {
            player1Id = Guid.Empty;
            player2Id = Guid.Empty;

            var entered = Monitor.TryEnter(_monitorTarget, 500);
            if (!entered)
            {
                return false;
            }

            try
            {
                if (WaitingQueue.Count < 2)
                {
                    return false;
                }

                player1Id = WaitingQueue.First.Value;
                WaitingQueue.RemoveFirst();
                player2Id = WaitingQueue.First.Value;
                WaitingQueue.RemoveFirst();
            }
            catch (Exception)
            {
                if (player2Id != Guid.Empty) 
                {
                    WaitingQueue.AddFirst(player2Id);
                }
                if (player1Id != Guid.Empty)
                {
                    WaitingQueue.AddFirst(player1Id);
                }
                throw;
            }
            finally
            {
                Monitor.Exit(_monitorTarget);
            }

            return true;
        }
    }
}
