using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Threading.Tasks
{
    public static class AsyncHelpers
    {
        public static Task<T[]> ToArrayAsync<T>(this IEnumerable<Task<T>> tasks)
            => Task.WhenAll(tasks);

        public static async Task<List<T>> ToListAsync<T>(this IEnumerable<Task<T>> tasks)
            => (await Task.WhenAll(tasks)).ToList();
    }
}
