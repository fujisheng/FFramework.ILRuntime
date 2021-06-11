using System;
using System.Collections.Generic;
using System.Reflection;
using Framework;
using Framework.ILR.Utility;

namespace Framework.ILR.Service.UI
{
    /// <summary>
    /// 可绑定属性标记接口
    /// </summary>
    public interface IBindableProperty
    {
        /// <summary>
        /// 释放
        /// </summary>
        void Release();
    }

    /// <summary>
    /// 可绑定的属性
    /// </summary>
    /// <typeparam name="T">值的类型</typeparam>
    public struct BindableProperty<T> : IBindableProperty
    {
        T newValue;
        T oldValue;

        List<Action<T, T>> actions;
        List<(object owner, MethodInfo methodInfo)> methodInfos;
        List<Delegate> delegates;

        Dictionary<int, Action<T, T>> actionCatch;
        Type actionType;

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="value">值</param>
        public BindableProperty(T value = default)
        {
            newValue = value;
            oldValue = default(T);

            actions = new List<Action<T, T>>();
            methodInfos = new List<(object owner, MethodInfo methodInfo)>();
            delegates = new List<Delegate>();

            actionCatch = new Dictionary<int, Action<T, T>>();
            actionType = typeof(Action<T, T>);
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

        /// <summary>
        /// 触发所有的listener
        /// </summary>
        /// <param name="oldValue">旧的值</param>
        /// <param name="newValue">新的值</param>
        void InvokeAll(T oldValue, T newValue)
        {
            for (int i = 0; i < actions.Count; i++)
            {
                actions[i].Invoke(oldValue, newValue);
            }

#if ILRUNTIME
            for (int i = 0; i < methodInfos.Count; i++)
            {
                var info = methodInfos[i];
                info.methodInfo.Invoke(info.owner, new object[] { oldValue, newValue });
            }

            for (int i = 0; i < delegates.Count; i++)
            {
                var @delegate = delegates[i];
                (@delegate as Action<T, T>).Invoke(oldValue, newValue);
            }
#endif
        }

        /// <summary>
        /// 添加一个listener
        /// </summary>
        /// <param name="onValueChanged"></param>
        public void AddListener(Action<T, T> onValueChanged)
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
#if ILRUNTIME
            if (methodInfos.ILRExists((i) => i.owner == owner && i.methodInfo == methodInfo))
            {
                return;
            }

            methodInfos.Add((owner, methodInfo));
#else

            var hash = Framework.Utility.Hash.CombineHash(owner.GetHashCode(), methodInfo.GetHashCode());
            if(actionCatch.TryGetValue(hash, out Action<T, T> action))
            {
                return;
            }

            action = methodInfo.CreateDelegate(actionType, owner) as Action<T, T>;
            actionCatch.Add(hash, action);
            AddListener(action);
#endif
        }

        /// <summary>
        /// 添加一个Delegate的Listener
        /// </summary>
        /// <param name="delegate"></param>
        public void AddListener(Delegate @delegate)
        {
#if ILRUNTIME
            if (delegates.Contains(@delegate))
            {
                return;
            }

            delegates.Add(@delegate);
#else

            var hash = @delegate.GetHashCode();
            if (actionCatch.TryGetValue(hash, out Action<T, T> action))
            {
                return;
            }

            action = @delegate as Action<T, T>;
            actionCatch.Add(hash, action);
            AddListener(action);
#endif
        }

        /// <summary>
        /// 移除一个listener
        /// </summary>
        /// <param name="onValueChanged"></param>
        public void RemoveListener(Action<T, T> onValueChanged)
        {
            actions.ILRRemoveAll((i) => i == onValueChanged);
        }

        /// <summary>
        /// 移除一个methodInfo的listener
        /// </summary>
        /// <param name="owner">methodInfo的拥有者</param>
        /// <param name="methodInfo"></param>
        public void RemoveListener(object owner, MethodInfo methodInfo)
        {
#if ILRUNTIME
            methodInfos.ILRRemoveAll((i) => i.owner == owner && i.methodInfo == methodInfo);
#else
            var hash = Framework.Utility.Hash.CombineHash(owner.GetHashCode(), methodInfo.GetHashCode());
            if (!actionCatch.TryGetValue(hash, out Action<T, T> action))
            {
                return;
            }
            RemoveListener(action);
#endif
        }

        /// <summary>
        /// 移除一个Delegate的Listener
        /// </summary>
        /// <param name="onValueChanged">onValueChanged</param>
        public void RemoveListener(Delegate onValueChanged)
        {
#if ILRUNTIME
            delegates.ILRRemoveAll((i) => i == onValueChanged);
#else
            var hash = onValueChanged.GetHashCode();
            if (!actionCatch.TryGetValue(hash, out Action<T, T> action))
            {
                return;
            }
            RemoveListener(action);
#endif
        }

        /// <summary>
        /// 释放
        /// </summary>
        public void Release()
        {
            actions.Clear();
            methodInfos.Clear();
            delegates.Clear();
            actionCatch.Clear();
        }

#region 各种运算符重写
        /// <summary>
        /// 将一个值隐式转换成一个BindableProperty
        /// </summary>
        /// <param name="value">值</param>
        public static implicit operator BindableProperty<T>(T value)
        {
            return new BindableProperty<T>(value);
        }

        /// <summary>
        /// 将一个BindableProperty隐式转换成一个值
        /// </summary>
        /// <param name="value">BindableProperty</param>
        public static implicit operator T(BindableProperty<T> property)
        {
            return property.Value;
        }

        /// <summary>
        /// 判断两个BindableProperty是否相等
        /// </summary>
        /// <param name="l">BinableProperty</param>
        /// <param name="r">BinableProperty</param>
        /// <returns>是否相等</returns>
        public static bool operator ==(BindableProperty<T> l, BindableProperty<T> r)
        {
            return l.Value.Equals(r.Value);
        }

        /// <summary>
        /// 判断两个BindableProperty是否不相等
        /// </summary>
        /// <param name="l">BinableProperty</param>
        /// <param name="r">BinableProperty</param>
        /// <returns>是否不相等</returns>
        public static bool operator !=(BindableProperty<T>l, BindableProperty<T> r)
        {
            return !l.Value.Equals(r.Value);
        }

        /// <summary>
        /// 判断一个BindableProperty和一个值是否相等
        /// </summary>
        /// <param name="l">BinableProperty</param>
        /// <param name="r">值</param>
        /// <returns>是否相等</returns>
        public static bool operator ==(BindableProperty<T> l, T r)
        {
            return l.Value.Equals(r);
        }

        /// <summary>
        /// 判断一个BindableProperty和一个值是否不相等
        /// </summary>
        /// <param name="l">BinableProperty</param>
        /// <param name="r">值</param>
        /// <returns>是否不相等</returns>
        public static bool operator !=(BindableProperty<T> l, T r)
        {
            return !l.Value.Equals(r);
        }

        /// <summary>
        /// 判断一个值和一个BindableProperty是否相等
        /// </summary>
        /// <param name="l">值</param>
        /// <param name="r">BinableProperty</param>
        /// <returns>是否相等</returns>
        public static bool operator ==(T l, BindableProperty<T> r)
        {
            return l.Equals(r.Value);
        }

        /// <summary>
        /// 判断一个值和一个BindableProperty是否不相等
        /// </summary>
        /// <param name="l">值</param>
        /// <param name="r">BinableProperty</param>
        /// <returns>是否不相等</returns>
        public static bool operator !=(T l, BindableProperty<T> r)
        {
            return !l.Equals(r.Value);
        }

        public override bool Equals(object obj)
        {
            return Value.Equals(obj);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public override string ToString()
        {
            return $"BindableProperty<{typeof(T)}>:{Value}";
        }
#endregion
    }
}

