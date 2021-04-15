using Cysharp.Threading.Tasks;
using Framework.Service.Resource;

namespace Framework.ILR.Service.Script
{
    public interface IScriptService
    {
        string[] Types { get; }
        void SetResourceLoader(IResourceLoader resourceLoader);
        UniTask Load(string label);
        object InvokeMethod(string typeName, string methodName, object owner = null, params object[] args);
        void Release(string typeName);
        void Release(string typeName, string methodName);
    }
}