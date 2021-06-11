using Framework.ILR.Service.UI;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Framework.ILR.Utility
{
    public static class BindablePropertyUtility
    {
        static readonly Dictionary<int, Dictionary<string, IBindableProperty>> propertyCache = new Dictionary<int, Dictionary<string, IBindableProperty>>();

        /// <summary>
        /// 通过MethodInfo来添加Listener的方法参数类型表
        /// </summary>
        public static readonly Type[] AddWithMethodInfoParamType = new Type[] { typeof(object), typeof(MethodInfo) };

        /// <summary>
        /// 获取某个实例下的某个名字的BindableProperty 会缓存
        /// </summary>
        /// <param name="instance">实例</param>
        /// <param name="propertyName">名字</param>
        /// <returns></returns>
        public static IBindableProperty GetBindableProperty(object instance, string propertyName)
        {
            var hash = instance.GetHashCode();
            if(!propertyCache.TryGetValue(hash, out var properties))
            {
                properties = new Dictionary<string, IBindableProperty>();
                var result = GetBindableProperty(instance, propertyName, properties);
                propertyCache.Add(hash, properties);
                return result;
            }

            return GetBindableProperty(instance, propertyName, properties);
        }

        static IBindableProperty GetBindableProperty(object instance, string propertyName, Dictionary<string, IBindableProperty> cache)
        {
            var type = instance.GetType();
            if(!cache.TryGetValue(propertyName, out IBindableProperty field))
            {
                var fieldInfo = type.GetField(propertyName, TypeUtility.allFlags);
                if (fieldInfo == null || !typeof(IBindableProperty).IsAssignableFrom(fieldInfo.FieldType))
                {
                    throw new Exception($"can not get property {propertyName} with {type.FullName}, please check property name is right");
                }

                field = fieldInfo.GetValue(instance) as IBindableProperty;
                if (field != null)
                {
                    cache.Add(propertyName, field);
                }
            }

            return field;
        }

        /// <summary>
        /// 释放某个实例的缓存
        /// </summary>
        /// <param name="instance"></param>
        public static void ReleasePropertyCache(object instance)
        {
            var hash = instance.GetHashCode();
            if (!propertyCache.TryGetValue(hash, out var properties))
            {
                return;
            }

            foreach(var property in properties.Values)
            {
                property.Release();
            }
            propertyCache.Remove(hash);
        }
    }
}
