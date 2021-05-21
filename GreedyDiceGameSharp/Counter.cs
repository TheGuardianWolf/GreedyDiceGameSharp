using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreedyDiceGameSharp
{
    public class Counter
    {
        IDictionary<dynamic, int> Store { get; set; } = new Dictionary<dynamic, int>();

        public Counter(IEnumerable<dynamic> o)
        {
            foreach (var obj in o)
            {
                Increment(obj);
            }
        }

        public int GetCount(dynamic o)
        {
            var hasKey = Store.TryGetValue(o, out int val);
            if (!hasKey)
            {
                return 0;
            }
            return val;
        }

        public void SetCount(dynamic o, int count)
        {
            if (Store.ContainsKey(o))
            {
                Store[o] = count;
            }
        }

        public IDictionary<dynamic, int> GetCounts()
        {
            return new Dictionary<dynamic, int>(Store);
        }

        public int Increment(dynamic o)
        {
            if (Store.ContainsKey(o))
            {
                Store[o] += 1;
            }
            else
            {
                Store[o] = 1;
            }

            return Store[o];
        }

        public int Decrement(dynamic o)
        {
            if (Store.ContainsKey(o))
            {
                Store[o] -= 1;
            }
            else
            {
                Store[o] = -1;
            }

            return Store[o];
        }
    }
}
