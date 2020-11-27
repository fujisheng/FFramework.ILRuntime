using System;

namespace Framework.IL.Hotfix.Module.UI
{
    public struct BindInfo
    {
        public Type ViewModelType { get; }
        public string AssetName { get; }
        public int Layer { get; }
        public int Behaviour { get; }

        public BindInfo(Type viewModelType, int layer = UI.Layer.NORMAL, int behaviour = B.NONE, string assetName = null)
        {
            ViewModelType = viewModelType;
            AssetName = assetName;
            Layer = layer;
            Behaviour = behaviour;
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class Bind : Attribute
    {
        public Type ViewModelType { get; }
        public string AssetName { get; }
        public int Layer { get; }
        public int Behaviour { get; }

        public Bind(Type viewModelType, int layer = UI.Layer.NORMAL, int behaviour = B.NONE, string assetName = null)
        {
            ViewModelType = viewModelType;
            AssetName = assetName;
            Layer = layer;
            Behaviour = behaviour;
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
