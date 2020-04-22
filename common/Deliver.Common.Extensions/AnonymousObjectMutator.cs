using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Pluto.Common.Extensions
{
    /// <summary>
    /// Change the values of Immutable anonymous types by changing their backing mutable fields.
    /// </summary>
    public static class AnonymousObjectMutator
    {
        private const BindingFlags FieldFlags = BindingFlags.NonPublic | BindingFlags.Instance;
        private const BindingFlags PropFlags = BindingFlags.Public | BindingFlags.Instance;
        private static readonly string[] BackingFieldFormats = { "<{0}>i__Field", "<{0}>" };
        private static ConcurrentDictionary<Type, IDictionary<string, Action<object, object>>> _map =
            new ConcurrentDictionary<Type, IDictionary<string, Action<object, object>>>();

        /// <summary>
        /// Change a value of the Anonymous Object's Property
        /// </summary>
        /// <typeparam name="T">Type of Anonymous Object</typeparam>
        /// <typeparam name="TProperty">Property type to be changed</typeparam>
        /// <param name="instance">The Anonymous Object in question</param>
        /// <param name="propExpression">The Property to be changed</param>
        /// <param name="newValue">Value to be assigned to the property</param>
        /// <returns>Mutated Object</returns>
        public static T Mutate<T, TProperty>(
            this T instance,
            Expression<Func<T, TProperty>> propExpression,
            TProperty newValue) where T : class
        {
            GetSetterFor(propExpression)(instance, newValue);
            return instance;
        }

        /// <summary>
        /// Change a value of the Anonymous Object's Property
        /// </summary>
        /// <typeparam name="TProperty">Property type to be changed</typeparam>
        /// <param name="instance">The Anonymous Object in question</param>
        /// <param name="propExpression">The Property to be changed</param>
        /// <param name="newValue">Value to be assigned to the property</param>
        /// <returns>Mutated Object</returns>
        public static object Mutate(
            this object instance,
            string propName,
            object newValue)
        {
            var anonType = instance.GetType();
            GetSetterFor(anonType, propName)(instance, newValue);
            return instance;
        }

        /// <summary>
        /// Get The Setter Field For a Property in an Anonymous Object
        /// </summary>
        /// <typeparam name="T">Type of Anonymous Object</typeparam>
        /// <typeparam name="TProperty">The type of the property of the field in question</typeparam>
        /// <param name="propExpression">The property in question</param>
        /// <returns></returns>
        private static Action<object, object> GetSetterFor<T, TProperty>(Expression<Func<T, TProperty>> propExpression)
        {
            var memberExpression = propExpression.Body as MemberExpression;
            if (memberExpression == null || memberExpression.Member.MemberType != MemberTypes.Property)
                throw new InvalidOperationException("Only property expressions are supported");
            Action<object, object> setter = null;
            GetPropMap<T>().TryGetValue(memberExpression.Member.Name, out setter);
            if (setter == null)
                throw new InvalidOperationException("No setter found");
            return setter;
        }


        /// <summary>
        /// Get The Setter Field For a Property in an Anonymous Object
        /// </summary>
        /// <typeparam name="T">Type of Anonymous Object</typeparam>
        /// <param name="anonType">The type of the property of the field in question</param>
        /// <param name="propName">The property in question</param>
        /// <returns></returns>
        private static Action<object, object> GetSetterFor(Type anonType, string propName)
        {
            Action<object, object> setter = null;
            GetPropMap(anonType).TryGetValue(propName, out setter);
            if (setter == null)
                throw new InvalidOperationException("No setter found");
            return setter;
        }

        /// <summary>
        /// Get the Property Map for a property in an Anonymous Object
        /// </summary>
        /// <typeparam name="T">Type of Anonymous Object</typeparam>
        /// <returns>Property Map</returns>
        private static IDictionary<string, Action<object, object>> GetPropMap<T>()
        {
            return _map.GetOrAdd(typeof(T), x => BuildPropMap<T>());
        }

        /// <summary>
        /// Get the Property Map for a property in an Anonymous Object
        /// </summary>
        /// <param name="anonType">Type of Anonymous Object</param>
        /// <returns>Property Map</returns>
        private static IDictionary<string, Action<dynamic, object>> GetPropMap(Type anonType)
        {
            return _map.GetOrAdd(anonType, x => BuildPropMap(anonType));
        }

        /// <summary>
        /// Dynamically Build a Property Map for an Anonymous type
        /// </summary>
        /// <typeparam name="T">Anonymous Type's property Type</typeparam>
        /// <returns></returns>
        private static IDictionary<string, Action<object, object>> BuildPropMap<T>()
        {
            var typeMap = new Dictionary<string, Action<object, object>>();
            var fields = typeof(T).GetFields(FieldFlags);
            foreach (var pi in typeof(T).GetProperties(PropFlags))
            {
                var backingFieldNames = BackingFieldFormats.Select(x => string.Format(x, pi.Name)).ToList();
                var fi = fields.FirstOrDefault(f => backingFieldNames.Contains(f.Name) && f.FieldType == pi.PropertyType);
                if (fi == null)
                    throw new NotSupportedException(string.Format("No backing field found for property {0}.", pi.Name));
                typeMap.Add(pi.Name, (inst, val) => fi.SetValue(inst, val));
            }
            return typeMap;
        }

        /// <summary>
        /// Dynamically Build a Property Map for an Anonymous type
        /// </summary>
        /// <param name="anonType">Anonymous Type's property Type</param>
        /// <returns></returns>
        private static IDictionary<string, Action<object, object>> BuildPropMap(Type anonType)
        {
            var typeMap = new Dictionary<string, Action<object, object>>();
            var fields = anonType.GetFields(FieldFlags);
            foreach (var pi in anonType.GetProperties(PropFlags))
            {
                var backingFieldNames = BackingFieldFormats.Select(x => string.Format(x, pi.Name)).ToList();
                var fi = fields.FirstOrDefault(f => backingFieldNames.Contains(f.Name) && f.FieldType == pi.PropertyType);
                if (fi == null)
                    throw new NotSupportedException(string.Format("No backing field found for property {0}.", pi.Name));
                typeMap.Add(pi.Name, (inst, val) => fi.SetValue(inst, val));
            }
            return typeMap;
        }
    }
}
