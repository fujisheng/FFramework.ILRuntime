using Cysharp.Threading.Tasks;
using Framework.Module.Resource;
using Framework.Utility;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Framework.Module.Script
{
    public class EditorScriptManager : IScriptManager
    {
        Assembly assembly;
        IResourceLoader resourceLoader;
        readonly (string dll, string pdb) gameDllNames = ("Game.Hotfix.dll", "Game.Hotfix.pdb");
        readonly Dictionary<string, Type> typeCache = new Dictionary<string, Type>();
        readonly Dictionary<(string typeName, string methodName, int paramCount), MethodInfo> methodCache = new Dictionary<(string typeName, string methodName, int paramCount), MethodInfo>();

        static EditorScriptManager instance;
        public static EditorScriptManager Instance
        {
            get { return instance ?? (instance = new EditorScriptManager()); }
        }

        public async UniTask Load(string label)
        {
            resourceLoader = ResourceLoader.Ctor();
            await resourceLoader.PerloadAll<TextAsset>(label);
            LoadDll(gameDllNames.dll, gameDllNames.pdb);
        }

        void LoadDll(string dllName, string pdbName)
        {
            TextAsset dllAsset = resourceLoader.Get<TextAsset>(dllName);
            var dllBytes = EncryptionUtility.AESDecrypt(dllAsset.bytes);
#if DEBUG || UNITY_EDITOR
            TextAsset pdbAsset = resourceLoader.Get<TextAsset>(pdbName);
            var pdbBytes = EncryptionUtility.AESDecrypt(pdbAsset.bytes);

            assembly = Assembly.Load(dllBytes, pdbBytes);
#else
            assembly = Assembly.Load(dllBytes, null);
#endif
        }

        Type GetOrCacheType(string typeName)
        {
            bool get = typeCache.TryGetValue(typeName, out Type type);
            if (get)
            {
                return type;
            }

            type = assembly.GetType(typeName);
            if (type == null)
            {
                Debug.LogError($"热更工程中没有找到这个类型 ：{typeName} 请检查!!!");
                return null;
            }

            typeCache.Add(typeName, type);
            return type;
        }


        public MethodInfo GetAndCacheMethod(string typeName, string methodName, int paramCount)
        {
            bool get = methodCache.TryGetValue((typeName, methodName, paramCount), out MethodInfo method);
            
            if (get)
            {
                return method;
            }

            Type type = GetOrCacheType(typeName);
            if(type == null)
            {
                return null;
            }

            method = type.GetMethod(methodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
            if(method == null)
            {
                return null;
            }
            methodCache.Add((typeName, methodName, paramCount), method);
            return method;
        }

        public object InvokeMethod(string className, string methodName, object owner = null, params object[] args)
        {
            int paramCount = args.Length;
            MethodInfo method = GetAndCacheMethod(className, methodName, paramCount);
            if(method == null)
            {
                return null;
            }

            return method.Invoke(owner, args);
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
            assembly = null;
        }
    }
}