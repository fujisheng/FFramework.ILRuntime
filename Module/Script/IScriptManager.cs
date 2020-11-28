using Cysharp.Threading.Tasks;

namespace Framework.Module.Script
{
    public interface IScriptManager
    {
        UniTask Load(string label);
        object InvokeMethod(string typeName, string methodName, object owner = null, params object[] args);
        void Release(string typeName);
        void Release(string typeName, string methodName);
    }
}