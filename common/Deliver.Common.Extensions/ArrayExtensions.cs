using System;
using System.Collections.Generic;
using System.Text;

namespace System.Linq
{
    public static class ArrayExtensions
    {
        public static void ForEach<T>(this T[] array, Action<T> action)
        {
            foreach (var item in array)
                action(item);
        }

        public static void ForEach<T>(this T[] array, Action<T,int> action)
        {
            for(var i = 0; i<array.Length; i++)
                action(array[i],i);
        }
    }
}
