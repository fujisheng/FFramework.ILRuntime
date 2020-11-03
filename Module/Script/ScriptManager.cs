using Framework.IL.Module.Script;
using Framework.Module.Resource;
using ILRuntime.CLR.Method;
using ILRuntime.CLR.TypeSystem;
using ILRuntime.Mono.Cecil.Pdb;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using AppDomain = ILRuntime.Runtime.Enviorment.AppDomain;

namespace Framework.Module.Script
{
    public class ScriptManager : IScriptManager
    {
        public AppDomain appdomain { get; private set; }
        IResourceLoader resourceLoader;
        MemoryStream frameworkDllStream;
        MemoryStream frameworkPdbStream;
        MemoryStream gameDllStream;
        MemoryStream gamePdbStream;

        readonly string frameworkDllName = "Framework.IL.Hotfix.dll";
        readonly string frameworkPdbName = "Framework.IL.Hotfix.pdb";
        readonly string gameDllName = "Game.Hotfix.dll";
        readonly string gamePdbName = "Game.Hotfix.pdb";

        Dictionary<string, IType> typeCache = new Dictionary<string, IType>();
        Dictionary<Vector3Int, IMethod> methodCache = new Dictionary<Vector3Int, IMethod>();

        IAdpaterReginster adpaterReginster;
        ICLRBinderReginster CLRBinderReginster;
        IValueTypeBinderReginster valueTypeBinderReginster;
        IDelegateConvertor delegateConvertor;

        static ScriptManager instance;
        public static ScriptManager Instance
        {
            get { return instance ?? (instance = new ScriptManager()); }
        }

        public void SetReginster(IAdpaterReginster adpater, ICLRBinderReginster clr, IValueTypeBinderReginster valueType, IDelegateConvertor @delegate)
        {
            adpaterReginster = adpater;
            CLRBinderReginster = clr;
            valueTypeBinderReginster = valueType;
            delegateConvertor = @delegate;
        }

        public async Task Load(string name)
        {
            appdomain = new AppDomain();
            resourceLoader = ResourceLoader.Ctor();
            await resourceLoader.PerloadAll<TextAsset>(name);

            TextAsset frameworkDllAsset = resourceLoader.Get<TextAsset>(frameworkDllName);
            TextAsset gameAsset = resourceLoader.Get<TextAsset>(gameDllName);
            frameworkDllStream = new MemoryStream(frameworkDllAsset.bytes);
            gameDllStream = new MemoryStream(gameAsset.bytes);
#if DEBUG || UNITY_EDITOR
            TextAsset frameworkPdbAsset = resourceLoader.Get<TextAsset>(frameworkPdbName);
            TextAsset gamePdbAsset = resourceLoader.Get<TextAsset>(gamePdbName);
            frameworkPdbStream = new MemoryStream(frameworkPdbAsset.bytes);
            gamePdbStream = new MemoryStream(gamePdbAsset.bytes);

            appdomain.LoadAssembly(frameworkDllStream, frameworkPdbStream, new PdbReaderProvider());
            appdomain.LoadAssembly(gameDllStream, gamePdbStream, new PdbReaderProvider());
#else
            appdomain.LoadAssembly(frameworkDllStream, null, new PdbReaderProvider());
            appdomain.LoadAssembly(gameDllStream, null, new PdbReaderProvider());
#endif
            InitializeILRuntime();
        }

        void InitializeILRuntime()
        {
#if DEBUG && (UNITY_EDITOR || UNITY_ANDROID || UNITY_IPHONE)
            //由于Unity的Profiler接口只允许在主线程使用，为了避免出异常，需要告诉ILRuntime主线程的线程ID才能正确将函数运行耗时报告给Profiler
            appdomain.UnityMainThreadID = System.Threading.Thread.CurrentThread.ManagedThreadId;
#endif
            adpaterReginster.Reginst(appdomain);
            CLRBinderReginster.Reginst(appdomain);
            valueTypeBinderReginster.Reginst(appdomain);
            delegateConvertor.Convert(appdomain);
        }

        IType GetOrCacheType(string typeName)
        {
            bool get = typeCache.TryGetValue(typeName, out IType type);
            if (get)
            {
                return type;
            }

            bool getType = appdomain.LoadedTypes.TryGetValue(typeName, out type);
            if (!getType)
            {
                Debug.LogError($"热更工程中没有找到这个类型 ：{typeName} 请检查!!!");
                return null;
            }

            typeCache.Add(typeName, type);
            return type;
        }


        public IMethod GetAndCacheMethod(string typeName, string methodName, int paramCount)
        {
            int left = typeName.GetHashCode();
            int right = methodName.GetHashCode();
            Vector3Int key = new Vector3Int(left, right, paramCount);
            bool get = methodCache.TryGetValue(key, out IMethod method);
            
            if (get)
            {
                return method;
            }

            IType type = GetOrCacheType(typeName);
            if(type == null)
            {
                return null;
            }

            method = type.GetMethod(methodName, paramCount);
            if(method == null)
            {
                //Debug.LogWarning($"Type:{className} 中不包含这个方法 functionName:{methodName} paramCount:{paramCount}");
                return null;
            }
            methodCache.Add(key, method);
            return method;
        }

        public object InvokeMethod(string className, string methodName, object owner = null, params object[] args)
        {
            int paramCount = args.Length;
            IMethod method = GetAndCacheMethod(className, methodName, paramCount);
            if(method == null)
            {
                return null;
            }

            return appdomain.Invoke(method, owner, args);
        }

        List<Vector3Int> removeKeys = new List<Vector3Int>();
        public void Release(string typeName)
        {
            if (typeCache.ContainsKey(typeName))
            {
                typeCache.Remove(typeName);
            }

            removeKeys.Clear();
            foreach(var kv in methodCache)
            {
                Vector3Int key = kv.Key;
                if(key.x == typeName.GetHashCode())
                {
                    removeKeys.Add(key);
                }
            }

            foreach(var key in removeKeys)
            {
                methodCache.Remove(key);
            }
            removeKeys.Clear();
        }

        public void Release(string typeName, string methodName)
        {
            removeKeys.Clear();
            foreach (var kv in methodCache)
            {
                Vector3Int key = kv.Key;
                if (key.x == typeName.GetHashCode() && key.y == methodName.GetHashCode())
                {
                    removeKeys.Add(key);
                }
            }

            foreach (var key in removeKeys)
            {
                methodCache.Remove(key);
            }
            removeKeys.Clear();
        }

        public void OnUpdate()
        {
            InvokeMethod("Hotfix.Main", "OnUpdate");
        }

        public void OnFixedUpdate()
        {
            InvokeMethod("Hotfix.Main", "OnFixedUpdate");
        }

        public void OnLateUpdate()
        {
            InvokeMethod("Hotfix.Main", "OnLateUpdate");
        }

        public void OnTearDown()
        {
            InvokeMethod("Hotfix.Main", "OnTearDown");
            typeCache.Clear();
            methodCache.Clear();
            frameworkDllStream?.Close();
            frameworkPdbStream?.Close();
            gameDllStream?.Close();
            gamePdbStream?.Close();
            frameworkDllStream = null;
            frameworkPdbStream = null;
            gameDllStream = null;
            gamePdbStream = null;
        }
    }
}