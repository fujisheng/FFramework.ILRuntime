using System;
using System.Collections.Generic;
using System.Reflection;

namespace Framework.IL.Hotfix.Module.UI
{
    //标记接口
    public interface IBindableProperty{}

    //可绑定的属性
    public struct BindableProperty<T> : IBindableProperty
    {
        T newValue;
        T oldValue;
        List<Action<T, T>> actions;
        List<(object owner, MethodInfo methodInfo)> methodInfos;

        public BindableProperty(T value = default)
        {
            actions = new List<Action<T, T>>();
            methodInfos = new List<(object owner, MethodInfo methodInfo)>();
            newValue = value;
            oldValue = default;
        }

        /// <summary>
        /// 当set的时候如果oldValue 和 newValue相等 那么不会触发
        /// </summary>
        /// <value>The value.</value>
        public T Value
        {
            get
            {
                return newValue;
            }
            set
            {
                oldValue = newValue;
                newValue = value;
                if (oldValue.Equals(newValue))
                {
                    return;
                }
                InvokeAll(oldValue, newValue);
            }
        }

        /// <summary>
        /// 设置值，可以指定是否强制触发listener
        /// </summary>
        /// <param name="value"></param>
        /// <param name="force"></param>
        public void SetValue(T value, bool force = false)
        {
            if (!force)
            {
                Value = value;
                return;
            }

            oldValue = newValue;
            newValue = value;
            InvokeAll(oldValue, newValue);
        }

        /// <summary>
        /// 强制触发listener
        /// </summary>
        public void Sync()
        {
            InvokeAll(oldValue, newValue);
        }

        void InvokeAll(T oldValue, T newValue)
        {
            for (int i = 0; i < actions.Count; i++)
            {
                actions[i].Invoke(oldValue, newValue);
            }

            for(int i = 0; i < methodInfos.Count; i++)
            {
                var info = methodInfos[i];
                info.methodInfo.Invoke(info.owner, new object[] {oldValue, newValue});
            }
        }

        /// <summary>
        /// 添加一个listener
        /// </summary>
        /// <param name="onValueChanged"></param>
        public void AddListener(System.Action<T, T> onValueChanged)
        {
            if (actions.Contains(onValueChanged))
            {
                return;
            }
            actions.Add(onValueChanged);
        }

        /// <summary>
        /// 添加一个methodInfo的listener
        /// </summary>
        /// <param name="owner">methodInfo的拥有者</param>
        /// <param name="methodInfo"></param>
        public void AddListener(object owner, MethodInfo methodInfo)
        {
            foreach(var info in methodInfos)
            {
                if(info.owner == owner && info.methodInfo == methodInfo)
                {
                    return;
                }
            }

            methodInfos.Add((owner, methodInfo));
        }

        /// <summary>
        /// 移除一个listener
        /// </summary>
        /// <param name="onValueChanged"></param>
        public void RemoveListener(System.Action<T, T> onValueChanged)
        {
            if (actions.Contains(onValueChanged))
            {
                actions.Remove(onValueChanged);
            }
        }

        /// <summary>
        /// 移除一个methodInfo的listener
        /// </summary>
        /// <param name="owner">methodInfo的拥有者</param>
        /// <param name="methodInfo"></param>
        public void RemoveListener(object owner, MethodInfo methodInfo)
        {
            for(int i = 0; i < methodInfos.Count; i++)
            {
                var info = methodInfos[i];
                if (info.owner == owner && info.methodInfo == methodInfo)
                {
                    methodInfos.RemoveAt(i);
                }
            }
        }

        public override string ToString()
        {
            return $"BindableProperty:{Value}";
        }
    }
}

