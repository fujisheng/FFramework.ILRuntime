using Framework.Service.Resource;
using System;

namespace Framework.ILR.Service.UI
{
    public interface IContext
    {
        IResourceLoader ResourceLoader { get; }
        void Binding<T>(string propertyName, Action<T, T> listener);
        void Unbind<T>(string propertyName, Action<T, T> listener);
        void BindingWithAttribute();
        void SetProperty<T>(string propertyName, T value, bool force = false);
        T GetProperty<T>(string propertyName);
    }
}