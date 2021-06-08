using System;

namespace Framework.ILR.Service.UI
{
    /// <summary>
    /// 将view绑定到某个ViewModel
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class BindingAttribute : Attribute
    {
        public Type ViewModelType { get; }
        public string AssetName { get; }
        public int Layer { get; }
        public int Flag { get; }

        /// <summary>
        /// 将View绑定到某个ViewModel
        /// </summary>
        /// <param name="viewModelType">viewModel的类型</param>
        /// <param name="layer">层级</param>
        /// <param name="behaviour">行为</param>
        /// <param name="assetName">资源名</param>
        public BindingAttribute(Type viewModelType, int layer = UI.Layer.NORMAL, int behaviour = UI.Flag.NONE, string assetName = null)
        {
            ViewModelType = viewModelType;
            AssetName = assetName;
            Layer = layer;
            Flag = behaviour;
        }
    }

    /// <summary>
    /// 绑定某个方法到BindableProperty
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class OnValueChangedAttribute : Attribute
    {
        public string PropertyName { get; }

        /// <summary>
        /// 绑定某个方法到BindableProperty
        /// </summary>
        /// <param name="propertyName">BindableProperty的名字</param>
        public OnValueChangedAttribute(string propertyName)
        {
            PropertyName = propertyName;
        }
    }

    /// <summary>
    /// 绑定某个方法到打开View事件
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class OnOpenAttribute : Attribute
    {
        public string ViewName { get; }

        /// <summary>
        /// 绑定某个方法到打开某个view事件
        /// </summary>
        /// <param name="viewName">打开的view的名字</param>
        public OnOpenAttribute(string viewName)
        {
            ViewName = viewName;
        }
    }

    /// <summary>
    /// 绑定某个方法到关闭View事件
    /// </summary>
    public class OnCloseAttribute : Attribute
    {
        public string ViewName { get; }

        /// <summary>
        /// 绑定某个方法到关闭View事件
        /// </summary>
        /// <param name="viewName">关闭的View的名字</param>
        public OnCloseAttribute(string viewName)
        {
            ViewName = viewName;
        }
    }
}
