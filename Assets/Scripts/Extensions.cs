using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts
{
    public static class Extensions
    {
        public static void ForEach<T>(this IEnumerable<T> items, Action<T> action)
        {
            foreach (var item in items)
                action(item);
        }

        public static T Random<T>(this IEnumerable<T> items)
        {
            return items.ElementAt(UnityEngine.Random.Range(0, items.Count()));
        }
    }
}
