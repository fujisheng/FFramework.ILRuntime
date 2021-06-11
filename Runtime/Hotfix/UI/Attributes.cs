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
        
        /// <summary>
        /// 将View绑定到某个ViewModel
        /// </summary>
        /// <param name="viewModelType">viewModel的类型</param>
        public BindingAttribute(Type viewModelType)
        {
            ViewModelType = viewModelType;
        }
    }

    /// <summary>
    /// View的配置
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class ConfigAttribute : Attribute
    {
        public string AssetName { get; }
        public int Layer { get; }
        public int Flag { get; }

        /// <summary>
        /// View的一些配置信息
        /// </summary>
        /// <param name="assetName">资源名</param>
        /// <param name="layer">层级</param>
        /// <param name="flag">一些标记</param>
        public ConfigAttribute(string assetName = null, int layer = UI.Layer.NORMAL, int flag = UI.Flag.NONE)
        {
            AssetName = assetName;
            Layer = layer;
            Flag = flag;
        }

        /// <summary>
        /// 转化成元组
        /// </summary>
        /// <returns></returns>
        public (string assetName, int layer, int flag) ToTuple()
        {
            return (AssetName, Layer, Flag);
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
