﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NoOrm.Extensions
{
    public static class NoOrmExtensions
    {
        public static IDictionary<string, object> ToDictionary(this IEnumerable<(string name, object value)> tuples) =>
            tuples.ToDictionary(t => t.name, t => t.value);

        public static IEnumerable<IDictionary<string, object>> ToDictionaries(this IEnumerable<IEnumerable<(string name, object value)>> tuples) => 
            tuples.Select(t => t.ToDictionary());


        public static async Task<IDictionary<string, object>> ToDictionaryAsync(this IAsyncEnumerable<(string name, object value)> tuples) => 
            await tuples.ToDictionaryAsync(t => t.name, t => t.value);

        public static async IAsyncEnumerable<IDictionary<string, object>> ToDictionariesAsync(
            this IAsyncEnumerable<IAsyncEnumerable<(string name, object value)>> tuples)
        {
            await foreach (var tuple in tuples)
            {
                yield return await tuple.ToDictionaryAsync();
            }
        }
    }
}