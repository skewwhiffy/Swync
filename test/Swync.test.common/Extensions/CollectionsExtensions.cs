using System;
using System.Collections.Generic;
using System.Linq;

namespace Swync.test.common.Extensions
{
    public static class CollectionsExtensions
    {
        private static readonly Random Random = new Random();
        
        public static T TakeRandom<T>(this IEnumerable<T> collection)
        {
            var list = (collection as IList<T>) ?? collection.ToList();
            var index = Random.Next(list.Count);
            return list[index];
        }
    }
}