using System;
using System.Collections.Generic;

namespace Framework.IL.Hotfix.Module.UI
{
    public class BindableProperty<T>
    {
        T newValue;
        T oldValue;
        List<Action<T, T>> actions = new List<Action<T, T>>();

        public BindableProperty(T value = default)
        {
            this.newValue = value;
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
                this.oldValue = newValue;
                this.newValue = value;
                if (oldValue.Equals(newValue))
                {
                    return;
                }
                InvokeAll(oldValue, newValue);
            }
        }

        /// <summary>
        /// 强制触发改变
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
        }

        public void AddListener(System.Action<T, T> onValueChanged)
        {
            if (actions.Contains(onValueChanged))
            {
                return;
            }
            actions.Add(onValueChanged);
        }

        public void RemoveListener(System.Action<T, T> onValueChanged)
        {
            if (actions.Contains(onValueChanged))
            {
                actions.Remove(onValueChanged);
            }
        }

        public override string ToString()
        {
            return "Framework.IL.Hotfix.Module.UI.BindableProperty";
        }
    }
}

