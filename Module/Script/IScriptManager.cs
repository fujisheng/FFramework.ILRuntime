using Cysharp.Threading.Tasks;
using Framework.Module.Resource;

namespace Framework.Module.Script
{
    public interface IScriptManager
    {
        string[] Types { get; }
        void SetResourceLoader(IResourceLoader resourceLoader);
        UniTask Load(string label);
        object InvokeMethod(string typeName, string methodName, object owner = null, params object[] args);
        void Release(string typeName);
        void Release(string typeName, string methodName);
    }
}