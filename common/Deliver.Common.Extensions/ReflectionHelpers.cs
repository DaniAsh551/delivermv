using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace System.Reflection
{
    public static class ReflectionHelpers
    {
        public static int MapFrom<T>(this T target, object src, params string[] excludeList)
        {
            var srcType = src.GetType();
            var targetType = target.GetType();

            var targetMems = targetType.GetMembers().Where(x => !excludeList.Contains(x.Name) && (x.MemberType == MemberTypes.Property || x.MemberType == MemberTypes.Field)).ToArray();
            var correspondingMems = targetMems.ToDictionary(x => x, x => srcType.GetMember(x.Name).FirstOrDefault(y => y.MemberType == MemberTypes.Property || y.MemberType == MemberTypes.Field));

            var count = 0;

            foreach (var mem in correspondingMems)
            {
                try
                {
                    var srcVal = mem.Key.GetValue<object>(src);
                    mem.Value.SetValue(target, srcVal);
                    count++;
                }
                catch (Exception)
                {}
            }
            return count;
        }

        public static TModel GetValue<TModel>(this MemberInfo memInfo, object obj)
        {
            if (memInfo.MemberType == MemberTypes.Property)
                return (TModel)((PropertyInfo)memInfo).GetValue(obj, null);
            if (memInfo.MemberType == MemberTypes.Field)
                return (TModel)((FieldInfo)memInfo).GetValue(obj);
            return default(TModel);
        }

        public static void SetValue(this MemberInfo memInfo, object obj, object value)
        {
            if (memInfo.MemberType == MemberTypes.Property)
                ((PropertyInfo)memInfo).SetValue(obj, value);
            if (memInfo.MemberType == MemberTypes.Field)
                ((FieldInfo)memInfo).SetValue(obj, value);
        }

        public static T Replicate<T>(this T src, params string[] excludeList) where T : class
        {
            var type = typeof(T);
            var uninit = (T)FormatterServices.GetSafeUninitializedObject(type);
            MapFrom(uninit, src, excludeList);
            return uninit;
        }
    }
}
