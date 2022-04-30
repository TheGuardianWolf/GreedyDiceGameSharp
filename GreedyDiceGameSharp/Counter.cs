using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreedyDiceGameSharp
{
    public class Counter<T>
    {
        IDictionary<T, int> Store { get; set; } = new Dictionary<T, int>();

        public Counter(IEnumerable<T> o)
        {
            foreach (var obj in o)
            {
                Increment(obj);
            }
        }

        public int GetCount(T o)
        {
            var hasKey = Store.TryGetValue(o, out int val);
            if (!hasKey)
            {
                return 0;
            }
            return val;
        }

        public void SetCount(T o, int count)
        {
            if (Store.ContainsKey(o))
            {
                Store[o] = count;
            }
        }

        public IDictionary<T, int> GetCounts()
        {
            return new Dictionary<T, int>(Store);
        }

        public int Increment(T o)
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

        public int Decrement(T o)
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
