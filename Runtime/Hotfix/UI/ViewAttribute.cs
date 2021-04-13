using System;

namespace Framework.ILR.Module.UI
{
    [AttributeUsage(AttributeTargets.Class)]
    public class Bind : Attribute
    {
        public Type ViewModelType { get; }
        public string AssetName { get; }
        public int Layer { get; }
        public int Flag { get; }

        public Bind(Type viewModelType, int layer = UI.Layer.NORMAL, int behaviour = UI.Flag.NONE, string assetName = null)
        {
            ViewModelType = viewModelType;
            AssetName = assetName;
            Layer = layer;
            Flag = behaviour;
        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class BindProperty : Attribute
    {
        public string PropertyName { get; }
        public BindProperty(string propertyName)
        {
            PropertyName = propertyName;
        }
    }
}
