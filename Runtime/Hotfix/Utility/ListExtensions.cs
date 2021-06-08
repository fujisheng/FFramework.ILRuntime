using System;
using System.Collections.Generic;

namespace Framework.ILR.Utility
{
    /// <summary>
    /// 一些List在ILRuntime中因为委托不能用的方法 这个提供一个类似的
    /// </summary>
    public static class ListExtensions
    {
        /// <summary>
        /// 是否存在某个项
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="checker">检查方法</param>
        /// <returns>是否存在</returns>
        public static bool ILRExists<T>(this List<T> list, Func<T, bool> checker)
        {
            var result = false;
            for(int i = 0; i < list.Count; i++)
            {
                if (checker(list[i]))
                {
                    return true;
                }
            }

            return result;
        }

        /// <summary>
        /// 删除所有满足某个条件的项
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="checker"></param>
        public static void ILRRemoveAll<T>(this List<T> list, Func<T, bool> checker)
        {
            for (int i = list.Count - 1; i >= 0; i--)
            {
                var item = list[i];
                if (checker(item))
                {
                    list.Remove(item);
                }
            }
        }
    }
}
