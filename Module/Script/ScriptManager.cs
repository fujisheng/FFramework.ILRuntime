using Cysharp.Threading.Tasks;
using Framework.IL.Module.Script;
using Framework.Module.Resource;
using Framework.Utility;
using ILRuntime.CLR.Method;
using ILRuntime.CLR.TypeSystem;
using ILRuntime.Mono.Cecil.Pdb;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using AppDomain = ILRuntime.Runtime.Enviorment.AppDomain;

namespace Framework.Module.Script
{
    public class ScriptManager : IScriptManager
    {
        public AppDomain appdomain { get; private set; }
        IResourceLoader resourceLoader;
        (MemoryStream dll, MemoryStream pdb) gameStream;
        readonly (string dll, string pdb) gameDllNames = ("Game.Hotfix.dll", "Game.Hotfix.pdb");
        readonly Dictionary<string, IType> typeCache = new Dictionary<string, IType>();
        readonly Dictionary<(string typeName, string methodName, int paramCount), IMethod> methodCache = new Dictionary<(string typeName, string methodName, int paramCount), IMethod>();

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

        public async UniTask Load(string label)
        {
            appdomain = new AppDomain();
            resourceLoader = new ResourceLoader();
            await resourceLoader.PerloadAll<TextAsset>(label);
            gameStream = LoadDll(gameDllNames.dll, gameDllNames.pdb);
            InitializeILRuntime();
        }

        (MemoryStream dllStream, MemoryStream pdbStream) LoadDll(string dllName, string pdbName)
        {
            TextAsset dllAsset = resourceLoader.Get<TextAsset>(dllName);
            var dllStream = new MemoryStream(EncryptionUtility.AESDecrypt(dllAsset.bytes));
#if DEBUG || UNITY_EDITOR
            TextAsset pdbAsset = resourceLoader.Get<TextAsset>(pdbName);
            var pdbStream = new MemoryStream(EncryptionUtility.AESDecrypt(pdbAsset.bytes));

            appdomain.LoadAssembly(dllStream, pdbStream, new PdbReaderProvider());
            return (dllStream, pdbStream);
#else
            appdomain.LoadAssembly(dllStream, null, new PdbReaderProvider());
            return (dllStream, null);
#endif
        }

        void InitializeILRuntime()
        {
#if UNITY_EDITOR
            appdomain.DebugService.StartDebugService(56000);
#endif
#if DEBUG && (UNITY_EDITOR || UNITY_ANDROID || UNITY_IPHONE)
            //由于Unity的Profiler接口只允许在主线程使用，为了避免出异常，需要告诉ILRuntime主线程的线程ID才能正确将函数运行耗时报告给Profiler
            appdomain.UnityMainThreadID = System.Threading.Thread.CurrentThread.ManagedThreadId;
#endif
            adpaterReginster.Reginst(appdomain);
            valueTypeBinderReginster.Reginst(appdomain);
            delegateConvertor.Convert(appdomain);
            CLRBinderReginster.Reginst(appdomain);
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
            bool get = methodCache.TryGetValue((typeName, methodName, paramCount), out IMethod method);
            
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
            methodCache.Add((typeName, methodName, paramCount), method);
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

        List<(string, string, int)> removeKeys = new List<(string, string, int)>();
        public void Release(string typeName)
        {
            if (typeCache.ContainsKey(typeName))
            {
                typeCache.Remove(typeName);
            }

            removeKeys.Clear();
            foreach(var kv in methodCache)
            {
                var key = kv.Key;
                if(key.typeName == typeName)
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
                var key = kv.Key;
                if (key.typeName == typeName && key.methodName == methodName)
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

        public void ShutDown()
        {
            typeCache.Clear();
            methodCache.Clear();
            gameStream.dll?.Close();
            gameStream.pdb?.Close();
            gameStream = default;
        }
    }
}