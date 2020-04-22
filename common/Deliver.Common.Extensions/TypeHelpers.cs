using System;
using System.Collections.Generic;

namespace System.Reflection
{
    public static class TypeExtensions
    {
        /// <summary>
        /// Determines whether a type is an Enumerable type (ie. IEnumerable)
        /// </summary>
        /// <param name="type">The type to compare against</param>
        /// <returns>Whether the type is Enumerable</returns>
        public static bool IsMultiple(this Type type)
        {
            try
            {
                if (type.IsArray) return true;
                return typeof(IEnumerable<>).IsAssignableFrom(type);
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Follow a given path into a type and get the last child's Property Info
        /// </summary>
        /// <param name="type"></param>
        /// <param name="path"></param>
        /// <param name="followPath"></param>
        /// <returns></returns>
        public static PropertyInfo GetProperty(this Type type, string path, bool followPath)
        {
            PropertyInfo prop = null;
            foreach (var item in path.Split('.'))
            {
                if (prop == null)
                    prop = type.GetProperty(item);
                else
                    prop = prop.PropertyType.GetProperty(item);
            }

            return prop;
        }
    }
}
