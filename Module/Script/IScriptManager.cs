using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;

namespace Framework.Module.Script
{
    public interface IScriptManager
    {
        AppDomain appdomain { get; }
        IMethod GetAndCacheMethod(string typeName, string methodName, int paramCount);
        object InvokeMethod(string typeName, string methodName, object owner = null, params object[] args);
        void Release(string typeName);
        void Release(string typeName, string methodName);
    }
}