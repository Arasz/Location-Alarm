using System;
using System.Collections.Generic;

namespace CoreLibrary.Extensions
{
    /// <summary>
    /// Enumerable class extension 
    /// </summary>
    public static class EnumerableExtension
    {
        /// <exception cref="Exception"> A delegate callback throws an exception. </exception>
        public static void ForEach<T>(this IEnumerable<T> collection, Action<T> action)
        {
            foreach (var element in collection)
                action?.Invoke(element);
        }
    }
}