using System;

namespace Framework.ILR.Service.UI
{
    [AttributeUsage(AttributeTargets.Class)]
    public class BindingAttribute : Attribute
    {
        public Type ViewModelType { get; }
        public string AssetName { get; }
        public int Layer { get; }
        public int Flag { get; }

        public BindingAttribute(Type viewModelType, int layer = UI.Layer.NORMAL, int behaviour = UI.Flag.NONE, string assetName = null)
        {
            ViewModelType = viewModelType;
            AssetName = assetName;
            Layer = layer;
            Flag = behaviour;
        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class BindingPropertyAttribute : Attribute
    {
        public string PropertyName { get; }
        public BindingPropertyAttribute(string propertyName)
        {
            PropertyName = propertyName;
        }
    }
}
