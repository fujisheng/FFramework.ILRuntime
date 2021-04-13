using Framework.ILR.Module.Script;

namespace Framework.ILR.Utility
{
    public class MethodInvoker
    {
        static IScriptManager scriptManager;
        static IScriptManager ScriptManager { get { return scriptManager; } }//?? (scriptManager = ModuleManager.GetModule<IScriptManager>()); } }

        public static object Invoke(string typeName, string methodName, object owner = null, params object[] args)
        {
            return ScriptManager.InvokeMethod(typeName, methodName, owner, args);
        }

        public static void Release(string typeName)
        {
            ScriptManager.Release(typeName);
        }

        public static void Release(string typeName, string methodName)
        {
            ScriptManager.Release(typeName, methodName);
        }
    }
}

