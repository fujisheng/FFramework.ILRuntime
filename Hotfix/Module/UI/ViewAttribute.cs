using System;

namespace Framework.IL.Hotfix.Module.UI
{
    public enum BindType
    {
        View,
        Function,
        Text,
        Sprite,
    }
    [AttributeUsage(AttributeTargets.Class)]
    public class Bind : Attribute
    {
        public string viewModelName { get; }
        public int layer { get; }
        public B behaviour { get; }

        public Bind(string viewModelName, int layer, B behaviour)
        {
            this.viewModelName = viewModelName;
            this.layer = layer;
            this.behaviour = behaviour;
        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class BindProperty : Attribute
    {
        public string propertyName { get; }
        public BindProperty(string propertyName)
        {
            this.propertyName = propertyName;
        }
    }
}
