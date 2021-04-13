using System;
using System.Collections.Generic;
using System.Reflection;

namespace Framework.ILR.Utility
{
    public static class TypeUtility
    {
        static Dictionary<string, Type> typeCache = new Dictionary<string, Type>();

        /// <summary>
        /// 判断某个类型是否是某个类型的子类
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="type">类型</param>
        /// <returns></returns>
        public static bool Is<T>(this Type type)
        {
            try
            {
                return typeof(T).IsAssignableFrom(type);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 获取某种自定义特性
        /// </summary>
        /// <typeparam name="T">特性类型</typeparam>
        /// <param name="type">类型</param>
        /// <param name="inherit">是否获取继承的特性</param>
        /// <returns></returns>
        public static T GetCustomAttribute<T>(this MemberInfo memberInfo, bool inherit = true) where T : Attribute
        {
            var attributes = memberInfo.GetCustomAttributes(inherit);
            for (int j = 0; j < attributes.Length; j++)
            {
                var attribute = attributes[j];
                if (attribute is T)
                {
                    return attribute as T;
                }
            }
            return null;
        }

        /// <summary>
        /// 根据类型名获取类型
        /// </summary>
        /// <param name="typeName"></param>
        /// <returns></returns>
        public static Type GetType(string typeName)
        {
            bool get = typeCache.TryGetValue(typeName, out Type type);
            if (get)
            {
                return type;
            }
            type = Type.GetType(typeName);
            if (type == null)
            {
                return null;
            }
            typeCache.Add(typeName, type);
            return type;
        }
    }
}
