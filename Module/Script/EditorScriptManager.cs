﻿using Cysharp.Threading.Tasks;
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
        readonly (string dll, string pdb) frameworkDllNames = ("Framework.IL.Hotfix.dll", "Framework.IL.Hotfix.pdb");
        readonly (string dll, string pdb) gameDllNames = ("Game.Hotfix.dll", "Game.Hotfix.pdb");
        readonly Dictionary<string, Type> typeCache = new Dictionary<string, Type>();
        readonly Dictionary<(string typeName, string methodName, int paramCount), MethodInfo> methodCache = new Dictionary<(string typeName, string methodName, int paramCount), MethodInfo>();

        static EditorScriptManager instance;
        public static EditorScriptManager Instance
        {
            get { return instance ?? (instance = new EditorScriptManager()); }
        }

        string[] types;
        /// <summary>
        /// 所有加载的类型名
        /// </summary>
        public string[] Types
        {
            get
            {
                if(types != null)
                {
                    return types;
                }
                
                var allType = assembly.GetTypes();
                types = new string[allType.Length];
                for(int i = 0; i < allType.Length; i++)
                {
                    types[i] = allType[i].FullName;
                }

                return types;
            }
        }

        /// <summary>
        /// 加载dll
        /// </summary>
        /// <param name="label"></param>
        /// <returns></returns>
        public async UniTask Load(string label)
        {
            resourceLoader = new ResourceLoader();
            await resourceLoader.PerloadAll<TextAsset>(label);

            //LoadDll(frameworkDllNames);
            LoadDll(gameDllNames);
        }

        void LoadDll((string dll, string pdb) names)
        {
            TextAsset dllAsset = resourceLoader.Get<TextAsset>(names.dll);
            var dllBytes = EncryptionUtility.AESDecrypt(dllAsset.bytes);
#if DEBUG || UNITY_EDITOR
            TextAsset pdbAsset = resourceLoader.Get<TextAsset>(names.pdb);
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

        /// <summary>
        /// 执行一个方法
        /// </summary>
        /// <param name="className">类名全称</param>
        /// <param name="methodName">方法名</param>
        /// <param name="owner">类实例</param>
        /// <param name="args">参数</param>
        /// <returns></returns>
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
        /// <summary>
        /// 释放某个类型的方法缓存
        /// </summary>
        /// <param name="typeName"></param>
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

        /// <summary>
        /// 释放某个类型的某个方法的缓存
        /// </summary>
        /// <param name="typeName"></param>
        /// <param name="methodName"></param>
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
            types = null;
        }
    }
}