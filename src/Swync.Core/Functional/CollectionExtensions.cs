﻿using System.Collections.Generic;

namespace Swync.Core.Functional
{
    public static class CollectionExtensions
    {
        public static string Join(this IEnumerable<string> source, string separator = "")
        {
            return string.Join(separator, source);
        }
    }
}