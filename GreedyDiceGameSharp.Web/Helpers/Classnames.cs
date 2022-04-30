using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GreedyDiceGameSharp.Web.Helpers
{
    public static class Classames
    {
        public static string Join(dynamic obj)
        {
            var classes = new RouteValueDictionary(obj);
            var classnames = new List<string>();

            foreach (var _class in classes)
            {
                var hasClass = (_class.Value as bool?) ?? false;
                if (hasClass)
                {
                    classnames.Add(_class.Key);
                }
            }

            return string.Join(" ", classnames);
        }
    }
}
